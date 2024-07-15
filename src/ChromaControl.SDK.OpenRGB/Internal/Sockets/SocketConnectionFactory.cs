// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Connections;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal.Sockets;

internal sealed class SocketConnectionFactory : IConnectionFactory
{
    public ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
    {
        return new SocketConnection(endpoint).StartAsync();
    }
}
