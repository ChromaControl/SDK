// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct UpdateZoneLeds : IOpenRGBPacket
{
    public readonly PacketId Id => PacketId.UpdateZoneLeds;

    public uint DeviceIndex { get; private set; }

    public readonly uint Length => (uint)(4 + 4 + 2 + (4 * Colors.Length));

    public uint ZoneIndex { get; private set; }

    public Color[] Colors { get; private set; }

    public UpdateZoneLeds(uint deviceIndex, uint zoneIndex, Color[] colors)
    {
        DeviceIndex = deviceIndex;
        ZoneIndex = zoneIndex;
        Colors = colors;
    }

    public bool TryParse(ref SequenceReader<byte> input, uint deviceIndex)
    {
        DeviceIndex = deviceIndex;

        return true;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output)
    {
        output.Write(Length);
        output.Write(ZoneIndex);
        output.Write((ushort)Colors.Length);

        for (ushort i = 0; i < Colors.Length; i++)
        {
            output.Write(Colors[i]);
        }
    }
}
