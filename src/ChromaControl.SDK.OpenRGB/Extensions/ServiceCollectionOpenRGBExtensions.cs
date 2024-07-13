// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace ChromaControl.SDK.OpenRGB.Extensions;

/// <summary>
/// Extension methods for adding the OpenRGB SDK to an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionOpenRGBExtensions
{
    /// <summary>
    /// Registers the OpenRGB SDK in an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <returns>The original <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddOpenRGBSDK(this IServiceCollection services)
    {
        services.AddSingleton<IOpenRGBService, OpenRGBService>();

        return services;
    }
}
