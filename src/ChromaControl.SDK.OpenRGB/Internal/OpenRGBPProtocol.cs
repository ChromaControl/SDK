// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class OpenRGBPProtocol : IPacketReader<IOpenRGBPacket>, IPacketWriter<IOpenRGBPacket>, IAsyncDisposable
{
    private ConnectionContext? _connection;
    private ProtocolReader? _reader;
    private ProtocolWriter? _writer;
    private Task? _readingTask;

    private static ReadOnlySpan<byte> Magic => Encoding.ASCII.GetBytes("ORGB");

    private readonly Dictionary<PacketId, BlockingCollection<IOpenRGBPacket>> _pendingRequests;

    private const int HeaderLength = 16;

    public event EventHandler? DeviceListUpdated;

    public OpenRGBPProtocol()
    {
        _pendingRequests = Enum.GetValues<PacketId>()
            .ToDictionary(id => id, _ => new BlockingCollection<IOpenRGBPacket>());
    }

    public async Task StartAsync(SocketConnectionFactory connectionFactory, EndPoint endPoint, CancellationToken cancellationToken = default)
    {
        _connection = await connectionFactory.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);

        _reader = _connection.CreateReader();
        _writer = _connection.CreateWriter();

        _readingTask = ProcessReadAsync();
    }

    public bool TryParsePacket(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out IOpenRGBPacket packet)
    {
        packet = default!;

        if (input.Length < HeaderLength)
        {
            return false;
        }

        Span<byte> buffer = stackalloc byte[HeaderLength];
        input.Slice(0, HeaderLength).CopyTo(buffer);

        if (buffer[0] != Magic[0] ||
            buffer[1] != Magic[1] ||
            buffer[2] != Magic[2] ||
            buffer[3] != Magic[3])
        {
            throw new ProtocolViolationException("OpenRGB header is invalid.");
        }

        var deviceIndex = buffer.ReadUInt32(4);
        var packetId = buffer.ReadPacketId(8);
        var packetSize = buffer.ReadUInt32(12);

        if (input.Length >= (HeaderLength + packetSize))
        {
            var packetBody = input.Slice(HeaderLength, packetSize);

            packet = packetId switch
            {
                PacketId.RequestControllerCount => new RequestControllerCount() { DeviceIndex = deviceIndex },
                PacketId.RequestControllerData => new RequestControllerData() { DeviceIndex = deviceIndex },
                PacketId.RequestProtocolVersion => new RequestProtocolVersion() { DeviceIndex = deviceIndex },
                PacketId.SetClientName => new SetClientName() { DeviceIndex = deviceIndex },
                PacketId.DeviceListUpdated => new DeviceListUpdated() { DeviceIndex = deviceIndex },
                _ => throw new ProtocolViolationException("OpenRGB packet id invalid.")
            };

            var reader = new SequenceReader<byte>(packetBody);

            if (packet.TryParse(ref reader))
            {
                consumed = packetBody.End;
                examined = consumed;

                return true;
            }
        }

        return false;
    }

    public void WritePacket(IOpenRGBPacket packet, IBufferWriter<byte> output)
    {
        output.Write(Magic);
        output.Write(packet.DeviceIndex);
        output.Write(packet.Id);
        output.Write(packet.GetPacketLength());

        packet.WriteToBuffer(output);
    }

    public async Task SendPacketWithoutResponse(IOpenRGBPacket packet, CancellationToken cancellationToken = default)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("ConnectAsync must be called first.");
        }

        await _writer.WriteAsync(this, packet, cancellationToken);
    }

    public async Task<TPacket> SendPacketWithResponse<TPacket>(TPacket packet, CancellationToken cancellationToken = default) where TPacket : IOpenRGBPacket
    {
        await SendPacketWithoutResponse(packet, cancellationToken);

        var result = await Task.Run(() =>
        {
            if (!_pendingRequests[packet.Id].TryTake(out var result, 1000, cancellationToken))
            {
                throw new TimeoutException($"OpenRGB did not reply to {packet.Id} in the required amount of time.");
            }

            return result;
        });

        return (TPacket)result;
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

    private async Task ProcessReadAsync()
    {
        if (_reader is null)
        {
            return;
        }

        while (true)
        {
            var result = await _reader.ReadAsync(this);
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
                DeviceListUpdated?.Invoke(this, new());
            }
            else
            {
                _pendingRequests[packet.Id].Add(packet);
            }

            _reader.Advance();
        }
    }
}
