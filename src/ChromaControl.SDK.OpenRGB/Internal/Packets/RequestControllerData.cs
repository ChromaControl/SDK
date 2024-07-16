// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Structs;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Internal.Packets;

internal struct RequestControllerData : IOpenRGBPacket
{
    public readonly PacketId Id => PacketId.RequestControllerData;

    public uint DeviceIndex { get; set; }

    public uint ProtocolVersion { get; set; }

    public OpenRGBDevice Device { get; private set; }

    public RequestControllerData(uint deviceIndex, uint protocolVersion)
    {
        DeviceIndex = deviceIndex;
        ProtocolVersion = protocolVersion;
    }

    public bool TryParse(ref SequenceReader<byte> input)
    {
        _ = input.ReadUInt32();

        Device = OpenRGBDevice.Parse(ref input);

        return true;
    }

    public readonly void WriteToBuffer(IBufferWriter<byte> output)
    {
        output.Write(ProtocolVersion);
    }

    public readonly uint GetPacketLength()
    {
        return 4;
    }
}
