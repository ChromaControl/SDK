// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace ChromaControl.SDK.Synapse.Structs;

/// <summary>
/// Synapse effect.
/// </summary>
public struct SynapseEffect
{
    /// <summary>
    /// Color 1.
    /// </summary>
    public Color Color1 { get; internal set; }

    /// <summary>
    /// Color 2.
    /// </summary>
    public Color Color2 { get; internal set; }

    /// <summary>
    /// Color 3.
    /// </summary>
    public Color Color3 { get; internal set; }

    /// <summary>
    /// Color 4.
    /// </summary>
    public Color Color4 { get; internal set; }

    /// <summary>
    /// Color 5.
    /// </summary>
    public Color Color5 { get; internal set; }
}
