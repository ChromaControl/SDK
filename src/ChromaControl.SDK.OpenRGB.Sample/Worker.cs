// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ChromaControl.SDK.OpenRGB.Sample;

/// <summary>
/// The worker.
/// </summary>
public partial class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOpenRGBService _openRGB;

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting OpenRGB Service...")]
    private static partial void LogStartMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Stopping OpenRGB Service...")]
    private static partial void LogStopMessage(ILogger logger);

    /// <summary>
    /// Creates a <see cref="Worker"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="openRGB">The <see cref="IOpenRGBService"/>.</param>
    public Worker(ILogger<Worker> logger, IOpenRGBService openRGB)
    {
        _logger = logger;
        _openRGB = openRGB;
    }

    /// <summary>
    /// Starts the worker.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker starting.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartMessage(_logger);

        await _openRGB.StartServiceAsync();
    }

    /// <summary>
    /// Stops the worker.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker stopping.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        LogStopMessage(_logger);

        await _openRGB.StopServiceAsync();
    }
}
