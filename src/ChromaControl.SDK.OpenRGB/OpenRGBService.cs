// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;
using ChromaControl.SDK.OpenRGB.Structs;
using System.Drawing;
using System.Text.Json.Nodes;

namespace ChromaControl.SDK.OpenRGB;

/// <inheritdoc/>
public class OpenRGBService : IOpenRGBService, IAsyncDisposable
{
    private NativeOpenRGBService _service;
    private readonly OpenRGBManager _manager;

    /// <inheritdoc/>
    public bool Started { get; internal set; }

    /// <inheritdoc/>
    public event EventHandler<bool>? StartedChanged;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyList<OpenRGBDevice>>? DeviceListUpdated;

    /// <summary>
    /// Creates a <see cref="OpenRGBService"/> instance.
    /// </summary>
    public OpenRGBService()
    {
        _manager = new();
        _service = new();

        _service.DeviceListUpdated += OnDeviceListUpdated;
    }

    /// <inheritdoc/>
    public void UpdateConfiguration(JsonNode config)
    {
        _manager.UpdateConfigFile(config);
    }

    /// <inheritdoc/>
    public async Task Restart(CancellationToken cancellationToken = default)
    {
        if (Started)
        {
            Started = false;
            StartedChanged?.Invoke(this, Started);

            _service.DeviceListUpdated -= OnDeviceListUpdated;
            await _service.DisposeAsync();
            _service = new();
            _service.DeviceListUpdated += OnDeviceListUpdated;

            _manager.Stop();

            await StartServiceAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OpenRGBDevice>> GetDeviceListAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<OpenRGBDevice>();

        var controllerCountResult = await _service.RequestControllerCountAsync(cancellationToken);

        for (uint i = 0; i < controllerCountResult.Count; i++)
        {
            var controllerDataResult = await _service.RequestControllerDataAsync(i, cancellationToken);

            result.Add(controllerDataResult.Device);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task ResizeZoneAsync(int deviceIndex, int zoneIndex, int size, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return;
        }

        await _service.ResizeZoneAsync((uint)deviceIndex, (uint)zoneIndex, (uint)size, cancellationToken);

        _service.OnDeviceListUpdated(_service, new());
    }

    /// <inheritdoc/>
    public Task ResizeZoneAsync(OpenRGBDevice device, int zoneIndex, int size, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return ResizeZoneAsync(device.Index, zoneIndex, size, cancellationToken);
    }

    /// <inheritdoc/>
    public Task ResizeZoneAsync(OpenRGBDevice device, OpenRGBZone zone, int size, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return ResizeZoneAsync(device.Index, zone.Index, size, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateLedsAsync(int deviceIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return;
        }

        await _service.UpdateLedsAsync((uint)deviceIndex, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateLedsAsync(OpenRGBDevice device, Color[] colors, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return UpdateLedsAsync(device.Index, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateZoneLedsAsync(int deviceIndex, int zoneIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return;
        }

        await _service.UpdateZoneLedsAsync((uint)deviceIndex, (uint)zoneIndex, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateZoneLedsAsync(OpenRGBDevice device, int zoneIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return UpdateZoneLedsAsync(device.Index, zoneIndex, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateZoneLedsAsync(OpenRGBDevice device, OpenRGBZone zone, Color[] colors, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return UpdateZoneLedsAsync(device.Index, zone.Index, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateSingleLedAsync(int deviceIndex, int ledIndex, Color color, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return;
        }

        await _service.UpdateSingleLedAsync((uint)deviceIndex, (uint)ledIndex, color, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateSingleLedAsync(OpenRGBDevice device, int ledIndex, Color color, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return UpdateSingleLedAsync(device.Index, ledIndex, color, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateSingleLedAsync(OpenRGBDevice device, OpenRGBLed led, Color color, CancellationToken cancellationToken = default)
    {
        if (!Started)
        {
            return Task.CompletedTask;
        }

        return UpdateSingleLedAsync(device.Index, led.Index, color, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _service.DisposeAsync();

        _manager.Dispose();

        GC.SuppressFinalize(this);
    }

    internal async Task StartServiceAsync(CancellationToken cancellationToken = default)
    {
        _manager.Start();

        await _service.ConnectAsync(cancellationToken);

        await _service.SetClientNameAsync("Chroma Control", cancellationToken);
        await _service.RequestProtocolVersionAsync(cancellationToken);

        _service.OnDeviceListUpdated(_service, new());
    }

    internal async Task StopServiceAsync()
    {
        await _service.DisposeAsync();

        _manager.Dispose();
    }

    private async void OnDeviceListUpdated(object? sender, EventArgs e)
    {
        var devices = await GetDeviceListAsync();
        DeviceListUpdated?.Invoke(this, devices);

        Started = true;
        StartedChanged?.Invoke(this, Started);
    }
}
