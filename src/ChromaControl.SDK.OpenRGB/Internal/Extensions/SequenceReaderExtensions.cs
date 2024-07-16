// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class SequenceReaderExtensions
{
    public static uint ReadUInt32(this SequenceReader<byte> reader)
    {
        reader.TryReadLittleEndian(out int value);

        return (uint)value;
    }
}
