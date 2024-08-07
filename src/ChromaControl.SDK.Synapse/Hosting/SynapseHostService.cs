// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChromaControl.SDK.Synapse.Hosting;

/// <summary>
/// The host service for Synapse.
/// </summary>
public partial class SynapseHostService : IHostedService
{
    private readonly ILogger<SynapseHostService> _logger;
    private readonly IConfiguration _configuration;
    private readonly SynapseService _synapse;

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Synapse SDK is starting up...", EventName = "SynapseStarting")]
    private static partial void LogStartMessage(ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Synapse SDK failed to start, invalid configuration.", EventName = "SynapseStartFailure")]
    private static partial void LogStartErrorMessage(ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Synapse SDK is shutting down...", EventName = "SynapseStopping")]
    private static partial void LogStopMessage(ILogger logger);

    /// <summary>
    /// Creates a <see cref="SynapseHostService"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <param name="synapse">The <see cref="SynapseService"/>.</param>
    public SynapseHostService(ILogger<SynapseHostService> logger, IConfiguration configuration, ISynapseService synapse)
    {
        _logger = logger;
        _configuration = configuration;
        _synapse = (SynapseService)synapse;
    }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker starting.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartMessage(_logger);

        var key = _configuration["CHROMACONTROL_KEY_SYNAPSE"];

        var parseResult = Guid.TryParse(key, out var appId);

        var startResult = parseResult
            ? _synapse.StartService(appId)
            : _synapse.StartService(new());

        if (startResult != SynapseResult.Success)
        {
            LogStartErrorMessage(_logger);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the worker stopping.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        LogStopMessage(_logger);

        _synapse.StopService();

        return Task.CompletedTask;
    }
}
