// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using System.Buffers;
using System.Buffers.Binary;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class BufferWriterExtensions
{
    public static void Write(this IBufferWriter<byte> writer, uint value)
    {
        var bytes = writer.GetSpan(4);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        writer.Advance(4);
    }

    public static void Write(this IBufferWriter<byte> writer, PacketId value)
    {
        var bytes = writer.GetSpan(4);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, (uint)value);
        writer.Advance(4);
    }
}
