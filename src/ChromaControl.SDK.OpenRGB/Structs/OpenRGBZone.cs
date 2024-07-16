// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Structs;

/// <summary>
/// Represents an OpenRGB zone.
/// </summary>
public struct OpenRGBZone
{
    /// <summary>
    /// The zones name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The zones index.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// The zones type.
    /// </summary>
    public OpenRGBZoneType Type { get; internal set; }

    /// <summary>
    /// The zones minimum LEDs.
    /// </summary>
    public int MinimumLeds { get; internal set; }

    /// <summary>
    /// The zones maximum LEDs.
    /// </summary>
    public int MaximumLeds { get; internal set; }

    /// <summary>
    /// The zones LED count.
    /// </summary>
    public int LedCount { get; internal set; }

    /// <summary>
    /// The zones matrix map.
    /// </summary>
    public OpenRGBMatrixMap? MatrixMap { get; internal set; }

    /// <summary>
    /// The zones segments.
    /// </summary>
    public OpenRGBSegment[] Segments { get; internal set; }

    /// <summary>
    /// Converts this <see cref="OpenRGBZone"/> into a string representation.
    /// </summary>
    /// <returns>A string representation of this <see cref="OpenRGBZone"/>.</returns>
    public override readonly string ToString()
    {
        return Name;
    }

    internal static OpenRGBZone Parse(ref SequenceReader<byte> input, uint index)
    {
        var name = input.ReadString();
        var type = input.ReadZoneType();
        var minimumLeds = input.ReadUInt32();
        var maximumLeds = input.ReadUInt32();
        var ledCount = input.ReadUInt32();

        var zoneMatrixLength = input.ReadUInt16();

        OpenRGBMatrixMap? matrixMap = null;

        if (zoneMatrixLength > 0)
        {
            matrixMap = OpenRGBMatrixMap.Parse(ref input);
        }

        var segments = input.ReadSegments();

        return new()
        {
            Name = name,
            Index = (int)index,
            Type = type,
            MinimumLeds = (int)minimumLeds,
            MaximumLeds = (int)maximumLeds,
            LedCount = (int)ledCount,
            MatrixMap = matrixMap,
            Segments = segments
        };
    }
}
