// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Enums;
using ChromaControl.SDK.Synapse.Internal;
using ChromaControl.SDK.Synapse.Structs;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChromaControl.SDK.Synapse;

/// <inheritdoc/>
public partial class SynapseService : ISynapseService
{
    /// <inheritdoc/>
    public bool Started { get; internal set; }

    /// <inheritdoc/>
    public SynapseStatus CurrentStatus { get; internal set; }

    /// <inheritdoc/>
    public event EventHandler<SynapseStatus>? StatusChanged;

    /// <inheritdoc/>
    public event EventHandler<SynapseEffect>? EffectReceived;

    private RegisterEventNotificationCallback RegisterEventNotificationCallbackState { get; set; }

    /// <summary>
    /// Creates a <see cref="SynapseService"/> instance.
    /// </summary>
    public SynapseService()
    {
        RegisterEventNotificationCallbackState = new(OnEventNotification);
    }

    /// <inheritdoc/>
    public SynapseResult StartService(in Guid appId)
    {
        var initResult = NativeSynapseService.Init(appId);

        if (initResult != SynapseResult.Success)
        {
            return initResult;
        }

        var registerResult = NativeSynapseService.RegisterEventNotification(RegisterEventNotificationCallbackState);

        if (registerResult == SynapseResult.Success)
        {
            Started = true;
        }

        return registerResult;
    }

    /// <inheritdoc/>
    public SynapseResult StopService()
    {
        var unRegisterResult = NativeSynapseService.UnRegisterEventNotification();

        if (unRegisterResult != SynapseResult.Success)
        {
            return unRegisterResult;
        }

        var unInitResult = NativeSynapseService.UnInit();

        if (unInitResult == SynapseResult.Success)
        {
            Started = false;
            CurrentStatus = SynapseStatus.NotLive;
        }

        return unInitResult;
    }

    private SynapseResult OnEventNotification(SynapseEventType type, nint pData)
    {
        if (type == SynapseEventType.Effect)
        {
            var effectDataPtr = Unsafe.AddByteOffset(ref Unsafe.NullRef<int>(), pData);
            var effectData = MemoryMarshal.CreateSpan(ref effectDataPtr, 5);

            var effect = new SynapseEffect
            {
                Color1 = Color.FromArgb((effectData[0] >> 0) & 0xff, (effectData[0] >> 8) & 0xff, (effectData[0] >> 16) & 0xff),
                Color2 = Color.FromArgb((effectData[1] >> 0) & 0xff, (effectData[1] >> 8) & 0xff, (effectData[1] >> 16) & 0xff),
                Color3 = Color.FromArgb((effectData[2] >> 0) & 0xff, (effectData[2] >> 8) & 0xff, (effectData[2] >> 16) & 0xff),
                Color4 = Color.FromArgb((effectData[3] >> 0) & 0xff, (effectData[3] >> 8) & 0xff, (effectData[3] >> 16) & 0xff),
                Color5 = Color.FromArgb((effectData[4] >> 0) & 0xff, (effectData[4] >> 8) & 0xff, (effectData[4] >> 16) & 0xff)
            };

            EffectReceived?.Invoke(this, effect);
        }
        else if (type == SynapseEventType.Status)
        {
            CurrentStatus = (SynapseStatus)pData.ToInt32();

            StatusChanged?.Invoke(this, CurrentStatus);
        }

        return SynapseResult.Success;
    }
}
