// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using Microsoft.AspNetCore.Connections;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class NativeOpenRGBService : IAsyncDisposable
{
    private ConnectionContext? _connection;
    private ProtocolReader? _reader;
    private ProtocolWriter? _writer;
    private Task? _readingTask;

    private readonly OpenRGBPProtocol _protocol = new();
    private readonly SocketConnectionFactory _connectionFactory = new();

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connection = await _connectionFactory.ConnectAsync(new IPEndPoint(IPAddress.Loopback, OpenRGBConstants.PortNumber), cancellationToken)
            .ConfigureAwait(false);

        _reader = _connection.CreateReader();
        _writer = _connection.CreateWriter();

        _readingTask = ProcessReadAsync();
    }

    public async Task SetClientNameAsync(string name)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("ConnectAsync must be called first.");
        }

        await _writer.WriteAsync(_protocol, new SetClientName()
        {
            Name = name
        });
    }

    public async Task ProcessReadAsync()
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

            switch (packet.PacketId)
            {
                case PacketId.SetClientName:
                    break;
                case PacketId.DeviceListUpdated:
                    break;
                default:
                    break;
            }

            _reader.Advance();
        }
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
}
