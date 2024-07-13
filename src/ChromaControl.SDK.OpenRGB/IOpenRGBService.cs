// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ChromaControl.SDK.OpenRGB;

/// <summary>
/// The OpenRGB Service.
/// </summary>
public interface IOpenRGBService
{
    /// <summary>
    /// Starts the OpenRGB service.
    /// </summary>
    void StartService();

    /// <summary>
    /// Stopes the OpenRGB service.
    /// </summary>
    void StopService();
}
