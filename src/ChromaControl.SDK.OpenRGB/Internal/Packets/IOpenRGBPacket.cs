// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal interface IOpenRGBPacket
{
    PacketId PacketId { get; }

    uint DeviceIndex { get; set; }

    void WriteToBuffer(IBufferWriter<byte> output);

    uint GetPacketLength();
}
