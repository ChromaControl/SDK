// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class OpenRGBConstants
{
    public static readonly string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChromaControl");
    public static readonly string ConfigPath = Path.Combine(DataPath, "config");
    public static readonly int PortNumber = 22742;
    public static readonly uint ProtocolVersion = 4;
}
