// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct UpdateSingleLed : IOpenRGBPacket
{
    public readonly PacketId Id => PacketId.UpdateSingleLed;

    public uint DeviceIndex { get; private set; }

    public readonly uint Length => 8;

    public uint LedIndex { get; private set; }

    public Color Color { get; private set; }

    public UpdateSingleLed(uint deviceIndex, uint ledIndex, Color color)
    {
        DeviceIndex = deviceIndex;
        LedIndex = ledIndex;
        Color = color;
    }

    public bool TryParse(ref SequenceReader<byte> input, uint deviceIndex)
    {
        DeviceIndex = deviceIndex;

        return true;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output)
    {
        output.Write(LedIndex);
        output.Write(Color);
    }
}
