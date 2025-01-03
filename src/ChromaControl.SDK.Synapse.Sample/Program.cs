﻿// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.Synapse.Extensions;
using ChromaControl.SDK.Synapse.Sample;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSynapseSDK();

builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();
