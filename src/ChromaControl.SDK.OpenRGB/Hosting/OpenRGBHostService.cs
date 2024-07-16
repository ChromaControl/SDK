// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChromaControl.SDK.OpenRGB.Hosting;

/// <summary>
/// The host service for OpenRGB.
/// </summary>
public partial class OpenRGBHostService : IHostedService
{
    private readonly ILogger<OpenRGBHostService> _logger;
    private readonly IOpenRGBService _openRGB;

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "OpenRGB SDK is starting up...")]
    private static partial void LogStartMessage(ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "OpenRGB SDK is shutting down...")]
    private static partial void LogStopMessage(ILogger logger);

    /// <summary>
    /// Creates a <see cref="OpenRGBHostService"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="openRGB">The <see cref="IOpenRGBService"/>.</param>
    public OpenRGBHostService(ILogger<OpenRGBHostService> logger, IOpenRGBService openRGB)
    {
        _logger = logger;
        _openRGB = openRGB;
    }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker starting.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartMessage(_logger);

        await _openRGB.StartServiceAsync();
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker stopping.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        LogStopMessage(_logger);

        await _openRGB.StopServiceAsync();
    }
}
