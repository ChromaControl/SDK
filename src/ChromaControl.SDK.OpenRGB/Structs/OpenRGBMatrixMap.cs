// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Structs;

/// <summary>
/// Represents an OpenRGB matrix.
/// </summary>
public struct OpenRGBMatrixMap
{
    /// <summary>
    /// The matrix maps height.
    /// </summary>
    public int Height { get; internal set; }

    /// <summary>
    /// The matrix maps width.
    /// </summary>
    public int Width { get; internal set; }

    /// <summary>
    /// The matrix map.
    /// </summary>
    public int[,] Map { get; internal set; }

    /// <summary>
    /// Converts this <see cref="OpenRGBMatrixMap"/> into a string representation.
    /// </summary>
    /// <returns>A string representation of this <see cref="OpenRGBMatrixMap"/>.</returns>
    public override readonly string ToString()
    {
        return $"[{Height},{Width}]";
    }

    internal static OpenRGBMatrixMap Parse(ref SequenceReader<byte> input)
    {
        var height = input.ReadUInt32();
        var width = input.ReadUInt32();
        var map = new int[height, width];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                map[y, x] = (int)input.ReadUInt32();
            }
        }

        return new()
        {
            Height = (int)height,
            Width = (int)width,
            Map = map
        };
    }
}
