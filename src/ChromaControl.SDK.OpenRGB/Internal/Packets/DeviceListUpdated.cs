// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct DeviceListUpdated : IOpenRGBPacket
{
    public readonly PacketId Id => PacketId.DeviceListUpdated;

    public uint DeviceIndex { get; set; }

    public DeviceListUpdated()
    {
        DeviceIndex = 0;
    }

    public readonly bool TryParse(in SequenceReader<byte> input)
    {
        return true;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output) { }

    public readonly uint GetPacketLength()
    {
        return 0;
    }
}
