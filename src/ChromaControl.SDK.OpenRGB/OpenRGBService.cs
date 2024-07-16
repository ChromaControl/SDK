// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;
using ChromaControl.SDK.OpenRGB.Structs;
using System.Drawing;

namespace ChromaControl.SDK.OpenRGB;

/// <inheritdoc/>
public class OpenRGBService : IOpenRGBService, IAsyncDisposable
{
    private readonly OpenRGBManager _manager;
    private readonly NativeOpenRGBService _service;
    private readonly List<OpenRGBDevice> _devices;

    /// <inheritdoc/>
    public bool Started { get; internal set; }

    /// <inheritdoc/>
    public IReadOnlyList<OpenRGBDevice> Devices => _devices.AsReadOnly();

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyList<OpenRGBDevice>>? DeviceListUpdated;

    /// <summary>
    /// Creates a <see cref="OpenRGBService"/> instance.
    /// </summary>
    public OpenRGBService()
    {
        _manager = new();
        _service = new();
        _devices = [];

        _service.DeviceListUpdated += OnDeviceListUpdated;
    }

    /// <inheritdoc/>
    public async Task UpdateDeviceListAsync(CancellationToken cancellationToken = default)
    {
        _devices.Clear();

        var controllerCountResult = await _service.RequestControllerCountAsync(cancellationToken);

        for (uint i = 0; i < controllerCountResult.Count; i++)
        {
            var controllerDataResult = await _service.RequestControllerDataAsync(i, cancellationToken);

            _devices.Add(controllerDataResult.Device);
        }

        DeviceListUpdated?.Invoke(this, Devices);
    }

    /// <inheritdoc/>
    public async Task ResizeZoneAsync(int deviceIndex, int zoneIndex, int size, CancellationToken cancellationToken = default)
    {
        await _service.ResizeZoneAsync((uint)deviceIndex, (uint)zoneIndex, (uint)size, cancellationToken);

        _service.OnDeviceListUpdated(_service, new());
    }

    /// <inheritdoc/>
    public async Task UpdateLedsAsync(int deviceIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        await _service.UpdateLedsAsync((uint)deviceIndex, colors, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateZoneLedsAsync(int deviceIndex, int zoneIndex, Color[] colors, CancellationToken cancellationToken = default)
    {
        await _service.UpdateZoneLedsAsync((uint)deviceIndex, (uint)zoneIndex, colors, cancellationToken);
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
        await UpdateDeviceListAsync();

        Started = true;
    }
}
