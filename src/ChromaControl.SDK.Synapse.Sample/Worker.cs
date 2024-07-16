// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Enums;
using System.Drawing;

namespace ChromaControl.SDK.Synapse.Sample;

/// <summary>
/// The worker.
/// </summary>
public partial class Worker : BackgroundService
{
    private bool _serviceReady;
    private Color _cachedColor;

    private readonly ILogger<Worker> _logger;
    private readonly ISynapseService _service;

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Color changed to [R = {r}, G = {g}, B = {b}]")]
    private static partial void LogColorChangedMessage(ILogger logger, byte r, byte g, byte b);

    /// <summary>
    /// Creates a <see cref="Worker"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="service">The <see cref="ISynapseService"/>.</param>
    public Worker(ILogger<Worker> logger, ISynapseService service)
    {
        _logger = logger;
        _service = service;

        _service.StatusChanged += OnStatusChanged;
        _service.ColorsReceived += OnColorsReceived;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentColor = _cachedColor;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_serviceReady)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            if (currentColor != _cachedColor)
            {
                currentColor = _cachedColor;

                LogColorChangedMessage(_logger, currentColor.R, currentColor.G, currentColor.B);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private void OnStatusChanged(object? sender, SynapseStatus e)
    {
        _serviceReady = true;
    }

    private void OnColorsReceived(object? sender, Color[] e)
    {
        _cachedColor = e[0];
    }
}
