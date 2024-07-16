// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;
using ChromaControl.SDK.OpenRGB.Structs;

namespace ChromaControl.SDK.OpenRGB;

/// <inheritdoc/>
public class OpenRGBService : IOpenRGBService, IAsyncDisposable
{
    private readonly OpenRGBManager _openRGBManager;
    private readonly NativeOpenRGBService _openRGBService;
    private readonly List<OpenRGBDevice> _devices;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyList<OpenRGBDevice>>? DeviceListUpdated;

    /// <inheritdoc/>
    public IReadOnlyList<OpenRGBDevice> Devices => _devices.AsReadOnly();

    /// <summary>
    /// Creates a <see cref="OpenRGBService"/> instance.
    /// </summary>
    public OpenRGBService()
    {
        _openRGBManager = new();
        _openRGBService = new();
        _devices = [];

        _openRGBService.DeviceListUpdated += OnDeviceListUpdated;
    }

    /// <inheritdoc/>
    public async Task StartServiceAsync()
    {
        _openRGBManager.Start();

        await _openRGBService.ConnectAsync();
        await _openRGBService.SetClientNameAsync("Chroma Control");
        await _openRGBService.RequestProtocolVersionAsync();
    }

    /// <inheritdoc/>
    public async Task StopServiceAsync()
    {
        await _openRGBService.DisposeAsync();

        _openRGBManager.Stop();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _openRGBService.DisposeAsync();

        _openRGBManager.Dispose();

        GC.SuppressFinalize(this);
    }

    private async void OnDeviceListUpdated(object? sender, EventArgs e)
    {
        _devices.Clear();

        var controllerCountResult = await _openRGBService.RequestControllerCountAsync();

        for (uint i = 0; i < controllerCountResult.Count; i++)
        {
            var controllerDataResult = await _openRGBService.RequestControllerDataAsync(i);

            _devices.Add(controllerDataResult.Device);
        }

        DeviceListUpdated?.Invoke(this, Devices);
    }
}
