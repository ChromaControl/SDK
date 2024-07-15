// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers;
using System.Text;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct SetClientName : IOpenRGBPacket
{
    public readonly PacketId PacketId => PacketId.SetClientName;

    public uint DeviceIndex { get; set; }

    public string Name { get; set; }

    public SetClientName(string name)
    {
        DeviceIndex = 0;
        Name = name;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output)
    {
        output.Write(Encoding.ASCII.GetBytes(Name + '\0'));
    }

    public readonly uint GetPacketLength()
    {
        return (uint)Encoding.ASCII.GetByteCount(Name) + 1;
    }
}
