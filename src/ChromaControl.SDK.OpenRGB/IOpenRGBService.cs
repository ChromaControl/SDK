// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Structs;

namespace ChromaControl.SDK.OpenRGB;

/// <summary>
/// The OpenRGB Service.
/// </summary>
public interface IOpenRGBService
{
    /// <summary>
    /// The devices available in OpenRGB.
    /// </summary>
    public IReadOnlyList<OpenRGBDevice> Devices { get; }

    /// <summary>
    /// Starts the OpenRGB service.
    /// </summary>
    /// <returns>A <see cref="Task"/>.</returns>
    Task StartServiceAsync();

    /// <summary>
    /// Stopes the OpenRGB service.
    /// </summary>
    /// <returns>A <see cref="Task"/>.</returns>
    Task StopServiceAsync();
}
