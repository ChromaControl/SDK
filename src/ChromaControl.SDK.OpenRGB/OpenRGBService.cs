// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;

namespace ChromaControl.SDK.OpenRGB;

/// <inheritdoc/>
public class OpenRGBService : IOpenRGBService, IDisposable
{
    private readonly OpenRGBManager _openRGBManager = new();
    private readonly NativeOpenRGBService _openRGBService = new();

    /// <inheritdoc/>
    public async Task StartServiceAsync()
    {
        _openRGBManager.Start();

        await _openRGBService.ConnectAsync();
        await _openRGBService.SetClientNameAsync("Chroma Control");
    }

    /// <inheritdoc/>
    public async Task StopServiceAsync()
    {
        _openRGBManager.Stop();

        await Task.Delay(1);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _openRGBManager.Dispose();

        GC.SuppressFinalize(this);
    }
}
