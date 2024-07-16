// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB.Structs;

/// <summary>
/// Represents an OpenRGB device.
/// </summary>
public struct OpenRGBDevice
{
    /// <summary>
    /// The devices type.
    /// </summary>
    public OpenRGBDeviceType Type { get; internal set; }

    /// <summary>
    /// The devices name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The devices index.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// The devices vendor.
    /// </summary>
    public string Vendor { get; internal set; }

    /// <summary>
    /// The devices description.
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    /// The devices version number.
    /// </summary>
    public string Version { get; internal set; }

    /// <summary>
    /// The devices serial number.
    /// </summary>
    public string Serial { get; internal set; }

    /// <summary>
    /// The devices location.
    /// </summary>
    public string Location { get; internal set; }

    /// <summary>
    /// The devices zones.
    /// </summary>
    public OpenRGBZone[] Zones { get; internal set; }

    /// <summary>
    /// The devices leds.
    /// </summary>
    public OpenRGBLed[] Leds { get; internal set; }

    /// <summary>
    /// Creates a <see cref="Color"/> buffer of the appropriate size for this device.
    /// </summary>
    /// <returns>A <see cref="Color"/> <see cref="Array"/>.</returns>
    public readonly Color[] CreateColorBuffer()
    {
        return new Color[Leds.Length];
    }

    /// <summary>
    /// Converts this <see cref="OpenRGBDevice"/> into a string representation.
    /// </summary>
    /// <returns>A string representation of this <see cref="OpenRGBDevice"/>.</returns>
    public override readonly string ToString()
    {
        return Name;
    }

    internal static OpenRGBDevice Parse(ref SequenceReader<byte> input, uint index)
    {
        var type = input.ReadDeviceType();
        var name = input.ReadString();
        var vendor = input.ReadString();
        var description = input.ReadString();
        var version = input.ReadString();
        var serial = input.ReadString();
        var location = input.ReadString();

        input.SkipModes();

        var zones = input.ReadZones();
        var leds = input.ReadLeds();

        input.SkipColors();

        return new()
        {
            Type = type,
            Name = name,
            Index = (int)index,
            Vendor = vendor,
            Description = description,
            Version = version,
            Serial = serial,
            Location = location,
            Zones = zones,
            Leds = leds
        };
    }
}
