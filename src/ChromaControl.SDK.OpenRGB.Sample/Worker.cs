// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Extensions;
using ChromaControl.SDK.OpenRGB.Structs;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB.Sample;

/// <summary>
/// The worker.
/// </summary>
public partial class Worker : BackgroundService
{
    private bool _devicesReady;

    private readonly ILogger<Worker> _logger;
    private readonly IOpenRGBService _service;

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Changing color to [R = {r}, G = {g}, B = {b}]")]
    private static partial void LogColorChangeMessage(ILogger logger, byte r, byte g, byte b);

    /// <summary>
    /// Creates a <see cref="Worker"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="service">The <see cref="IOpenRGBService"/>.</param>
    public Worker(ILogger<Worker> logger, IOpenRGBService service)
    {
        _logger = logger;
        _service = service;

        _service.DeviceListUpdated += OnDeviceListUpdated;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentColor = Color.Blue;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_devicesReady)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            if (currentColor == Color.Red)
            {
                currentColor = Color.Green;
            }
            else if (currentColor == Color.Green)
            {
                currentColor = Color.Blue;
            }
            else
            {
                currentColor = Color.Red;
            }

            LogColorChangeMessage(_logger, currentColor.R, currentColor.G, currentColor.B);

            foreach (var device in _service.Devices)
            {
                var buffer = device.CreateColorBuffer();

                buffer.Fill(currentColor);

                await _service.UpdateLedsAsync(device.Index, buffer, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private void OnDeviceListUpdated(object? sender, IReadOnlyList<OpenRGBDevice> e)
    {
        _devicesReady = true;
    }
}
