// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Protocol;
using System.Buffers;
using System.Net;
using System.Text;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class OpenRGBPProtocol : IPacketReader<IOpenRGBPacket>, IPacketWriter<IOpenRGBPacket>
{
    private const int HeaderLength = 16;

    private static ReadOnlySpan<byte> Magic => Encoding.ASCII.GetBytes("ORGB");

    public bool TryParsePacket(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out IOpenRGBPacket packet)
    {
        packet = default!;

        if (input.Length < HeaderLength)
        {
            return false;
        }

        Span<byte> buffer = stackalloc byte[HeaderLength];
        input.Slice(0, HeaderLength).CopyTo(buffer);

        if (buffer[0] != Magic[0] ||
            buffer[1] != Magic[1] ||
            buffer[2] != Magic[2] ||
            buffer[3] != Magic[3])
        {
            throw new ProtocolViolationException("OpenRGB header is invalid.");
        }

        var deviceIndex = buffer.ReadUInt32(4);
        var packetId = buffer.ReadPacketId(8);
        var packetSize = buffer.ReadUInt32(12);

        if (input.Length >= (HeaderLength + packetSize))
        {
            var packetBody = input.Slice(HeaderLength, packetSize);

            packet = packetId switch
            {
                PacketId.RequestControllerCount => new RequestControllerCount() { DeviceIndex = deviceIndex },
                PacketId.RequestProtocolVersion => new RequestProtocolVersion() { DeviceIndex = deviceIndex },
                PacketId.SetClientName => new SetClientName() { DeviceIndex = deviceIndex },
                PacketId.DeviceListUpdated => new DeviceListUpdated() { DeviceIndex = deviceIndex },
                _ => throw new ProtocolViolationException("OpenRGB packet id invalid.")
            };

            var reader = new SequenceReader<byte>(packetBody);

            if (packet.TryParse(reader))
            {
                consumed = packetBody.End;
                examined = consumed;

                return true;
            }
        }

        return false;
    }

    public void WritePacket(IOpenRGBPacket packet, IBufferWriter<byte> output)
    {
        output.Write(Magic);
        output.Write(packet.DeviceIndex);
        output.Write(packet.Id);
        output.Write(packet.GetPacketLength());

        packet.WriteToBuffer(output);
    }
}
