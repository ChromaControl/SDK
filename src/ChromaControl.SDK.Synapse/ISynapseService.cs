// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Enums;
using ChromaControl.SDK.Synapse.Structs;

namespace ChromaControl.SDK.Synapse;

/// <summary>
/// The Razer Synapse Service
/// </summary>
public interface ISynapseService
{
    /// <summary>
    /// If the Synapse service has started.
    /// </summary>
    bool Started { get; }

    /// <summary>
    /// The current status.
    /// </summary>
    SynapseStatus CurrentStatus { get; }

    /// <summary>
    /// Occurs when the service status changes.
    /// </summary>
    event EventHandler<SynapseStatus>? StatusChanged;

    /// <summary>
    /// Occurs when an effect is received.
    /// </summary>
    event EventHandler<SynapseEffect>? EffectReceived;

    /// <summary>
    /// Starts the Synapse service.
    /// </summary>
    /// <param name="appId">Application Id for authentication.</param>
    /// <returns>A <see cref="SynapseResult"/>.</returns>
    SynapseResult StartService(in Guid appId);

    /// <summary>
    /// Stopes the Synapse service.
    /// </summary>
    /// <returns>A <see cref="SynapseResult"/>.</returns>
    SynapseResult StopService();
}
