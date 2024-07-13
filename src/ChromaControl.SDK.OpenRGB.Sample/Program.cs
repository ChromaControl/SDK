// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Extensions;
using ChromaControl.SDK.OpenRGB.Sample;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenRGBSDK();
builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();
