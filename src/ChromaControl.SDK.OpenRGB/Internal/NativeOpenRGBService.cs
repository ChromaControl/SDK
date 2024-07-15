// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using Microsoft.AspNetCore.Connections;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class NativeOpenRGBService
{
    private ConnectionContext? _connection;
    private ProtocolWriter? _writer;

    private readonly OpenRGBPProtocol _protocol = new();

    private readonly SocketConnectionFactory _connectionFactory = new();

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connection = await _connectionFactory.ConnectAsync(new IPEndPoint(IPAddress.Loopback, OpenRGBConstants.PortNumber), cancellationToken)
            .ConfigureAwait(false);

        _writer = _connection.CreateWriter();
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
}
