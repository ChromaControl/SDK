// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Structs;

namespace ChromaControl.SDK.OpenRGB.Sample;

/// <summary>
/// The worker.
/// </summary>
public class Worker : BackgroundService
{
    private readonly IOpenRGBService _openRGB;
    private bool _devicesReady;

    /// <summary>
    /// Creates a <see cref="Worker"/> instance.
    /// </summary>
    /// <param name="openRGB">The <see cref="IOpenRGBService"/>.</param>
    public Worker(IOpenRGBService openRGB)
    {
        _openRGB = openRGB;
        _openRGB.DeviceListUpdated += OnDeviceListUpdated;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_devicesReady)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            Console.WriteLine($"Worker running at: {DateTimeOffset.Now}");

            await Task.Delay(1000, stoppingToken);
        }
    }

    private void OnDeviceListUpdated(object? sender, IReadOnlyList<OpenRGBDevice> e)
    {
        _devicesReady = true;
    }
}
