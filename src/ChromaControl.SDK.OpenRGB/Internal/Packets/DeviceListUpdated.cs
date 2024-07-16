// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct DeviceListUpdated : IOpenRGBPacket
{
    public readonly PacketId Id => PacketId.DeviceListUpdated;

    public uint DeviceIndex { get; private set; }

    public readonly uint Length => 0;

    public DeviceListUpdated()
    {
        DeviceIndex = 0;
    }

    public bool TryParse(ref SequenceReader<byte> input, uint deviceIndex)
    {
        DeviceIndex = deviceIndex;

        return true;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output) { }
}
