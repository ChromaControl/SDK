// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Enums;
using ChromaControl.SDK.OpenRGB.Structs;
using System.Buffers;
using System.Text;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class SequenceReaderExtensions
{
    public static ushort ReadUInt16(this ref SequenceReader<byte> reader)
    {
        reader.TryReadLittleEndian(out short value);

        return (ushort)value;
    }

    public static int ReadInt32(this ref SequenceReader<byte> reader)
    {
        reader.TryReadLittleEndian(out int value);

        return value;
    }

    public static uint ReadUInt32(this ref SequenceReader<byte> reader)
    {
        reader.TryReadLittleEndian(out int value);

        return (uint)value;
    }

    public static OpenRGBDeviceType ReadDeviceType(this ref SequenceReader<byte> reader)
    {
        return (OpenRGBDeviceType)reader.ReadInt32();
    }

    public static OpenRGBZoneType ReadZoneType(this ref SequenceReader<byte> reader)
    {
        return (OpenRGBZoneType)reader.ReadInt32();
    }

    public static string ReadString(this ref SequenceReader<byte> reader)
    {
        var length = reader.ReadUInt16();

        reader.TryReadExact(length - 1, out var value);

        reader.Advance(1);

        return Encoding.ASCII.GetString(value);
    }

    public static void SkipModes(this ref SequenceReader<byte> reader)
    {
        var numberOfModes = reader.ReadUInt16(); // num_modes
        reader.Advance(4); // active_mode

        for (ushort i = 0; i < numberOfModes; i++)
        {
            var modeNameLength = reader.ReadUInt16(); // mode_name_len
            reader.Advance(modeNameLength); // mode_name
            reader.Advance(4); // mode_value
            reader.Advance(4); // mode_flags
            reader.Advance(4); // mode_speed_min
            reader.Advance(4); // mode_speed_max
            reader.Advance(4); // mode_brightness_min
            reader.Advance(4); // mode_brightness_max
            reader.Advance(4); // mode_colors_min
            reader.Advance(4); // mode_colors_max
            reader.Advance(4); // mode_speed
            reader.Advance(4); // mode_brightness
            reader.Advance(4); // mode_direction
            reader.Advance(4); // mode_color_mode
            var modeNumberOfColors = reader.ReadUInt16(); // mode_num_colors
            reader.Advance(4 * modeNumberOfColors); // mode_colors
        }
    }

    public static OpenRGBZone[] ReadZones(this ref SequenceReader<byte> reader)
    {
        var numberOfZones = reader.ReadUInt16(); // num_zones

        var result = new OpenRGBZone[numberOfZones];

        for (ushort i = 0; i < numberOfZones; i++)
        {
            result[i] = OpenRGBZone.Parse(ref reader, i);
        }

        return result;
    }

    public static OpenRGBSegment[] ReadSegments(this ref SequenceReader<byte> reader)
    {
        var numberOfSegments = reader.ReadUInt16(); // num_segments

        var result = new OpenRGBSegment[numberOfSegments];

        for (ushort i = 0; i < numberOfSegments; i++)
        {
            result[i] = OpenRGBSegment.Parse(ref reader, i);
        }

        return result;
    }

    public static void SkipColors(this ref SequenceReader<byte> reader)
    {
        var numberOfColors = reader.ReadUInt16(); // num_colors
        reader.Advance(4 * numberOfColors); // colors
    }

    public static OpenRGBLed[] ReadLeds(this ref SequenceReader<byte> reader)
    {
        var numberOfLeds = reader.ReadUInt16(); // num_leds

        var result = new OpenRGBLed[numberOfLeds];

        for (ushort i = 0; i < numberOfLeds; i++)
        {
            result[i] = OpenRGBLed.Parse(ref reader, i);
        }

        return result;
    }
}
