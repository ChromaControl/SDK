// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Enums;
using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;

namespace ChromaControl.SDK.OpenRGB.Structs;

/// <summary>
/// Represents an OpenRGB device.
/// </summary>
public struct OpenRGBDevice
{
    /// <summary>
    /// The <see cref="OpenRGBDeviceType"/>.
    /// </summary>
    public OpenRGBDeviceType DeviceType { get; set; }

    internal static OpenRGBDevice Parse(ref SequenceReader<byte> input)
    {
        var deviceType = input.ReadDeviceType();

        return new()
        {
            DeviceType = deviceType
        };
    }
}
