// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Structs;

/// <summary>
/// Represents an OpenRGB segment.
/// </summary>
public struct OpenRGBSegment
{
    /// <summary>
    /// The segments name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The segments index.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// The segments type.
    /// </summary>
    public OpenRGBZoneType Type { get; internal set; }

    /// <summary>
    /// The segments starting index.
    /// </summary>
    public int StartIndex { get; internal set; }

    /// <summary>
    /// The segments LED count.
    /// </summary>
    public int LedCount { get; internal set; }

    /// <summary>
    /// Converts this <see cref="OpenRGBSegment"/> into a string representation.
    /// </summary>
    /// <returns>A string representation of this <see cref="OpenRGBSegment"/>.</returns>
    public override readonly string ToString()
    {
        return Name;
    }

    internal static OpenRGBSegment Parse(ref SequenceReader<byte> input, uint index)
    {
        var name = input.ReadString();
        var type = input.ReadZoneType();
        var startIndex = input.ReadUInt32();
        var ledCount = input.ReadUInt32();

        return new()
        {
            Name = name,
            Index = (int)index,
            Type = type,
            StartIndex = (int)startIndex,
            LedCount = (int)ledCount
        };
    }
}
