﻿// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Structs;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB;

/// <summary>
/// The OpenRGB service.
/// </summary>
public interface IOpenRGBService
{
    /// <summary>
    /// If the OpenRGB service has started.
    /// </summary>
    bool Started { get; }

    /// <summary>
    /// The devices available in OpenRGB.
    /// </summary>
    public IReadOnlyList<OpenRGBDevice> Devices { get; }

    /// <summary>
    /// Occurs when the <see cref="Devices"/> is updated.
    /// </summary>
    event EventHandler<IReadOnlyList<OpenRGBDevice>>? DeviceListUpdated;

    /// <summary>
    /// Updates the device list.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateDeviceListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resizes a zone.
    /// </summary>
    /// <param name="deviceIndex">The device index.</param>
    /// <param name="zoneIndex">The zone index.</param>
    /// <param name="size">The new size.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task ResizeZoneAsync(uint deviceIndex, uint zoneIndex, uint size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates leds.
    /// </summary>
    /// <param name="deviceIndex">The device index.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateLedsAsync(uint deviceIndex, Color[] colors, CancellationToken cancellationToken = default);
}
