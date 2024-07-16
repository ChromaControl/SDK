// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Packets;
using ChromaControl.SDK.OpenRGB.Internal.Sockets;
using System.Drawing;
using System.Net;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class NativeOpenRGBService : IAsyncDisposable
{
    private CancellationTokenSource? _deviceListUpdatedCancellationTokenSource;

    private readonly SocketConnectionFactory _connectionFactory;
    private readonly OpenRGBPProtocol _protocol;

    public event EventHandler? DeviceListUpdated;

    public NativeOpenRGBService()
    {
        _deviceListUpdatedCancellationTokenSource = null;
        _connectionFactory = new();
        _protocol = new();

        _protocol.DeviceListUpdated += OnDeviceListUpdated;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _protocol.StartAsync(_connectionFactory, new IPEndPoint(IPAddress.Loopback, OpenRGBConstants.PortNumber), cancellationToken);
    }

    public async Task<RequestControllerCount> RequestControllerCountAsync(CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestControllerCount(), cancellationToken);
    }

    public async Task<RequestControllerData> RequestControllerDataAsync(uint deviceIndex, CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestControllerData(deviceIndex, OpenRGBConstants.ProtocolVersion), cancellationToken);
    }

    public async Task<RequestProtocolVersion> RequestProtocolVersionAsync(CancellationToken cancellationToken = default)
    {
        return await _protocol.SendPacketWithResponse(new RequestProtocolVersion(OpenRGBConstants.ProtocolVersion), cancellationToken);
    }

    public async Task SetClientNameAsync(string name, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new SetClientName(name), cancellationToken);
    }

    public async Task ResizeZoneAsync(uint deviceIndex, uint zoneIndex, uint size, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new ResizeZone(deviceIndex, zoneIndex, size), cancellationToken);
    }

    public async Task UpdateLedsAsync(uint deviceIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new UpdateLeds(deviceIndex, colors), cancellationToken);
    }

    public async Task UpdateZoneLedsAsync(uint deviceIndex, uint zoneIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new UpdateZoneLeds(deviceIndex, zoneIndex, colors), cancellationToken);
    }

    public async Task UpdateSingleLedAsync(uint deviceIndex, uint ledIndex, Color color, CancellationToken cancellationToken = default)
    {
        await _protocol.SendPacketWithoutResponse(new UpdateSingleLed(deviceIndex, ledIndex, color), cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _protocol.DisposeAsync();
    }

    internal void OnDeviceListUpdated(object? sender, EventArgs e)
    {
        _deviceListUpdatedCancellationTokenSource?.Cancel();
        _deviceListUpdatedCancellationTokenSource = new();

        _ = Task.Delay(1000, _deviceListUpdatedCancellationTokenSource.Token)
            .ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DeviceListUpdated?.Invoke(sender, e);
                }
            });
    }
}
