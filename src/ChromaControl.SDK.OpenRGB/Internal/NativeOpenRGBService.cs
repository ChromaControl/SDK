// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class NativeOpenRGBService : IAsyncDisposable
{
    private CancellationTokenSource? _deviceListUpdatedCancellationTokenSource;

    private readonly SocketConnectionFactory _connectionFactory;
    private readonly OpenRGBPProtocol _protocol;

    public event EventHandler? DeviceListUpdated;

    public NativeOpenRGBService()
    {
        _deviceListUpdatedCancellationTokenSource = null;
        _connectionFactory = new();
        _protocol = new();

        _protocol.DeviceListUpdated += OnDeviceListUpdated;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _protocol.StartAsync(_connectionFactory, new IPEndPoint(IPAddress.Loopback, OpenRGBConstants.PortNumber), cancellationToken);
    }

    public async Task<RequestControllerCount> RequestControllerCountAsync(CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestControllerCount(), cancellationToken);
    }

    public async Task<RequestControllerData> RequestControllerDataAsync(uint deviceIndex, CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestControllerData()
        {
            DeviceIndex = deviceIndex,
            ProtocolVersion = OpenRGBConstants.ProtocolVersion
        }, cancellationToken);
    }

    public async Task<RequestProtocolVersion> RequestProtocolVersionAsync(CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestProtocolVersion
        {
            ClientVersion = OpenRGBConstants.ProtocolVersion
        }, cancellationToken);
    }

    public async Task SetClientNameAsync(string name, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new SetClientName()
        {
            Name = name
        }, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _protocol.DisposeAsync();
    }

    internal void OnDeviceListUpdated(object? sender, EventArgs e)
    {
        _deviceListUpdatedCancellationTokenSource?.Cancel();
        _deviceListUpdatedCancellationTokenSource = new();

        _ = Task.Delay(1000, _deviceListUpdatedCancellationTokenSource.Token)
            .ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DeviceListUpdated?.Invoke(sender, e);
                }
            });
    }
}
