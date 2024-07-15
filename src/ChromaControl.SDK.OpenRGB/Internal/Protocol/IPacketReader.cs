// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Protocol;

internal interface IPacketReader<TPacket>
{
    bool TryParsePacket(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out TPacket packet);
}
