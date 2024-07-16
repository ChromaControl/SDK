// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using ChromaControl.SDK.OpenRGB.Structs;
using Microsoft.AspNetCore.Connections;
using System.Collections.Concurrent;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class NativeOpenRGBService : IAsyncDisposable
{
    public List<OpenRGBDevice> Devices { get; } = [];

    private ConnectionContext? _connection;
    private ProtocolReader? _reader;
    private ProtocolWriter? _writer;
    private Task? _readingTask;
    private CancellationTokenSource? _deviceListUpdatedCancellationTokenSource;

    private readonly SocketConnectionFactory _connectionFactory;
    private readonly OpenRGBPProtocol _protocol;
    private readonly Dictionary<PacketId, BlockingCollection<IOpenRGBPacket>> _pendingRequests;

    private event EventHandler DeviceListUpdated;

    public NativeOpenRGBService()
    {
        _protocol = new();
        _connectionFactory = new();
        _pendingRequests = Enum.GetValues<PacketId>()
            .ToDictionary(id => id, _ => new BlockingCollection<IOpenRGBPacket>());
        _deviceListUpdatedCancellationTokenSource = null;

        DeviceListUpdated += OnDeviceListUpdated;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connection = await _connectionFactory.ConnectAsync(new IPEndPoint(IPAddress.Loopback, OpenRGBConstants.PortNumber), cancellationToken)
            .ConfigureAwait(false);

        _reader = _connection.CreateReader();
        _writer = _connection.CreateWriter();

        _readingTask = ProcessReadAsync();
    }

    public async Task<RequestControllerCount> RequestControllerCountAsync()
    {
        return await SendPacketWithResponse(new RequestControllerCount());
    }

    public async Task<RequestControllerData> RequestControllerDataAsync(uint deviceIndex)
    {
        return await SendPacketWithResponse(new RequestControllerData()
        {
            DeviceIndex = deviceIndex,
            ProtocolVersion = OpenRGBConstants.ProtocolVersion
        });
    }

    public async Task<RequestProtocolVersion> RequestProtocolVersionAsync()
    {
        return await SendPacketWithResponse(new RequestProtocolVersion
        {
            ClientVersion = OpenRGBConstants.ProtocolVersion
        });
    }

    public async Task SetClientNameAsync(string name)
    {
        await SendPacketWithoutResponse(new SetClientName()
        {
            Name = name
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is null)
        {
            return;
        }

        if (_writer is not null)
        {
            await _writer.DisposeAsync().ConfigureAwait(false);
        }

        _connection.Transport.Input.CancelPendingRead();

        if (_readingTask is not null)
        {
            await _readingTask.ConfigureAwait(false);
        }

        if (_reader is not null)
        {
            await _reader.DisposeAsync().ConfigureAwait(false);
        }

        await _connection.DisposeAsync().ConfigureAwait(false);
    }

    private async void OnDeviceListUpdated(object? sender, EventArgs e)
    {
        Devices.Clear();

        var controllerCountResult = await RequestControllerCountAsync();

        for (uint i = 0; i < controllerCountResult.Count; i++)
        {
            var controllerDataResult = await RequestControllerDataAsync(i);

            Devices.Add(controllerDataResult.Device);
        }
    }

    private async Task ProcessReadAsync()
    {
        if (_reader is null)
        {
            return;
        }

        while (true)
        {
            var result = await _reader.ReadAsync(_protocol);
            var packet = result.Packet;

            if (result.IsCompleted || result.IsCanceled)
            {
                break;
            }

            if (packet is null)
            {
                break;
            }

            if (packet.Id == PacketId.DeviceListUpdated)
            {
                _deviceListUpdatedCancellationTokenSource?.Cancel();
                _deviceListUpdatedCancellationTokenSource = new();

                _ = Task.Delay(1000, _deviceListUpdatedCancellationTokenSource.Token)
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            DeviceListUpdated.Invoke(this, new());
                        }
                    });
            }
            else
            {
                _pendingRequests[packet.Id].Add(packet);
            }

            _reader.Advance();
        }
    }

    private async Task SendPacketWithoutResponse(IOpenRGBPacket packet)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("ConnectAsync must be called first.");
        }

        await _writer.WriteAsync(_protocol, packet);
    }

    private async Task<TPacket> SendPacketWithResponse<TPacket>(TPacket packet) where TPacket : IOpenRGBPacket
    {
        await SendPacketWithoutResponse(packet);

        var result = await Task.Run(() =>
        {
            if (!_pendingRequests[packet.Id].TryTake(out var result, 1000))
            {
                throw new TimeoutException($"OpenRGB did not reply to {packet.Id} in the required amount of time.");
            }

            return result;
        });

        return (TPacket)result;
    }
}
