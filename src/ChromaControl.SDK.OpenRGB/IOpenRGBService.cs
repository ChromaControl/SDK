// Licensed to the Chroma Control Contributors under one or more agreements.
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
    Task ResizeZoneAsync(int deviceIndex, int zoneIndex, int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resizes a zone.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="zoneIndex">The zone index.</param>
    /// <param name="size">The new size.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task ResizeZoneAsync(OpenRGBDevice device, int zoneIndex, int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resizes a zone.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="zone">The zone.</param>
    /// <param name="size">The new size.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task ResizeZoneAsync(OpenRGBDevice device, OpenRGBZone zone, int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates leds.
    /// </summary>
    /// <param name="deviceIndex">The device index.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateLedsAsync(int deviceIndex, Color[] colors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates leds.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateLedsAsync(OpenRGBDevice device, Color[] colors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates zone leds.
    /// </summary>
    /// <param name="deviceIndex">The device index.</param>
    /// <param name="zoneIndex">The zone index.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateZoneLedsAsync(int deviceIndex, int zoneIndex, Color[] colors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates zone leds.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="zoneIndex">The zone index.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateZoneLedsAsync(OpenRGBDevice device, int zoneIndex, Color[] colors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates zone leds.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="zone">The zone.</param>
    /// <param name="colors">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateZoneLedsAsync(OpenRGBDevice device, OpenRGBZone zone, Color[] colors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a single led.
    /// </summary>
    /// <param name="deviceIndex">The device index.</param>
    /// <param name="ledIndex">The led index.</param>
    /// <param name="color">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateSingleLedAsync(int deviceIndex, int ledIndex, Color color, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a single led.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="ledIndex">The led index.</param>
    /// <param name="color">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateSingleLedAsync(OpenRGBDevice device, int ledIndex, Color color, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a single led.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="led">The led.</param>
    /// <param name="color">The colors to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateSingleLedAsync(OpenRGBDevice device, OpenRGBLed led, Color color, CancellationToken cancellationToken = default);
}
