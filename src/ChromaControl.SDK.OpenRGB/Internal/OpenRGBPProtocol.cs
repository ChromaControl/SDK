// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using System.Buffers;
using System.Text;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class OpenRGBPProtocol : IPacketWriter<IOpenRGBPacket>
{
    private static ReadOnlySpan<byte> Magic => Encoding.ASCII.GetBytes("ORGB");

    public void WritePacket(IOpenRGBPacket packet, IBufferWriter<byte> output)
    {
        output.Write(Magic);
        output.Write(packet.DeviceIndex);
        output.Write(packet.PacketId);
        output.Write(packet.GetPacketLength());

        packet.WriteToBuffer(output);
    }
}
