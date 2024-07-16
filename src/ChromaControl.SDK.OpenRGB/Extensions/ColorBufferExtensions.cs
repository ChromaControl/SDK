// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace ChromaControl.SDK.OpenRGB.Extensions;

/// <summary>
/// Extension methods for <see cref="Color"/> buffers.
/// </summary>
public static class ColorBufferExtensions
{
    /// <summary>
    /// Fills a <see cref="Color"/> buffer with a single color.
    /// </summary>
    /// <param name="buffer">The <see cref="Color"/> buffer to fill.</param>
    /// <param name="fillColor">The <see cref="Color"/> to fill with.</param>
    public static void Fill(this Color[] buffer, Color fillColor)
    {
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = fillColor;
        }
    }
}
