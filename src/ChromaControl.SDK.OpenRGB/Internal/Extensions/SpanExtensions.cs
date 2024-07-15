// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers.Binary;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class SpanExtensions
{
    public static uint ReadUInt32(this Span<byte> span, int start)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(span[start..]);
    }

    public static PacketId ReadPacketId(this Span<byte> span, int start)
    {
        return (PacketId)span.ReadUInt32(start);
    }
}
