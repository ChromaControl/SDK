// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Pipelines;

namespace ChromaControl.SDK.OpenRGB.Internal.Pipes;

internal readonly struct DuplexPipePair
{
    public IDuplexPipe Transport { get; }
    public IDuplexPipe Application { get; }

    public DuplexPipePair(IDuplexPipe transport, IDuplexPipe application)
    {
        Transport = transport;
        Application = application;
    }

    public static DuplexPipePair CreateConnectionPair()
    {
        var input = new Pipe(PipeOptions.Default);
        var output = new Pipe(PipeOptions.Default);

        var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
        var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);

        return new DuplexPipePair(applicationToTransport, transportToApplication);
    }
}
