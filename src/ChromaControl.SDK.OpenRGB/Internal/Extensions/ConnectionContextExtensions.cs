// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using Microsoft.AspNetCore.Connections;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class ConnectionContextExtensions
{
    public static ProtocolWriter CreateWriter(this ConnectionContext connection)
    {
        return new ProtocolWriter(connection.Transport.Output);
    }
}
