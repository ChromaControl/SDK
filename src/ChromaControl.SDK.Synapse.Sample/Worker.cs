// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Enums;
using ChromaControl.SDK.Synapse.Structs;
using System.Drawing;

namespace ChromaControl.SDK.Synapse.Sample;

/// <summary>
/// The worker.
/// </summary>
public partial class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISynapseService _synapse;

    private Color _colorCache;

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting Synapse SDK...")]
    private static partial void LogStartMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Stopping Synapse SDK...")]
    private static partial void LogStopMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Synapse Status Changed To {status}")]
    private static partial void LogStatusMessage(ILogger logger, SynapseStatus status);

    [LoggerMessage(Level = LogLevel.Information, Message = "Synapse Color Set To [{r}, {g}, {b}]")]
    private static partial void LogEffectMessage(ILogger logger, byte r, byte g, byte b);

    /// <summary>
    /// Creates a <see cref="Worker"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="synapse">The <see cref="ISynapseService"/>.</param>
    public Worker(ILogger<Worker> logger, ISynapseService synapse)
    {
        _logger = logger;
        _synapse = synapse;
        _synapse.StatusChanged += StatusChanged;
        _synapse.EffectReceived += EffectReceived;
    }

    /// <summary>
    /// Starts the worker.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker starting.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartMessage(_logger);

        _synapse.StartService(new("00000000-0000-0000-0000-000000000000"));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the worker.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker stopping.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        LogStopMessage(_logger);

        _synapse.StopService();

        return Task.CompletedTask;
    }

    private void StatusChanged(object? sender, SynapseStatus e)
    {
        LogStatusMessage(_logger, e);
    }

    private void EffectReceived(object? sender, SynapseEffect e)
    {
        if (_colorCache != e.Color1)
        {
            _colorCache = e.Color1;

            LogEffectMessage(_logger, e.Color1.R, e.Color1.G, e.Color1.B);
        }
    }
}
