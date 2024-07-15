// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ChromaControl.SDK.OpenRGB.Internal.Protocol;

internal readonly struct ProtocolReadResult<TPacket>
{
    public TPacket? Packet { get; }

    public bool IsCanceled { get; }

    public bool IsCompleted { get; }

    public ProtocolReadResult(TPacket? packet, bool isCanceled, bool isCompleted)
    {
        Packet = packet;
        IsCanceled = isCanceled;
        IsCompleted = isCompleted;
    }
}
