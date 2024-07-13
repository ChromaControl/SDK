// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal;

namespace ChromaControl.SDK.OpenRGB;

/// <inheritdoc/>
public class OpenRGBService : IOpenRGBService, IDisposable
{
    private readonly OpenRGBManager _openRGB = new();

    /// <inheritdoc/>
    public void StartService()
    {
        _openRGB.Start();
    }

    /// <inheritdoc/>
    public void StopService()
    {
        _openRGB.Stop();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _openRGB.Dispose();

        GC.SuppressFinalize(this);
    }
}
