// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;
using ChromaControl.SDK.OpenRGB.Structs;

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
    public async Task UpdateDeviceList()
    {
        _devices.Clear();

        var controllerCountResult = await _service.RequestControllerCountAsync();

        for (uint i = 0; i < controllerCountResult.Count; i++)
        {
            var controllerDataResult = await _service.RequestControllerDataAsync(i);

            _devices.Add(controllerDataResult.Device);
        }

        DeviceListUpdated?.Invoke(this, Devices);
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
        await UpdateDeviceList();

        Started = true;
    }
}
