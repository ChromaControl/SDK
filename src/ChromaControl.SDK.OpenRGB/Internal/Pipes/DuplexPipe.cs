// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Pipelines;

namespace ChromaControl.SDK.OpenRGB.Internal.Pipes;

internal sealed class DuplexPipe : IDuplexPipe
{
    public PipeReader Input { get; }
    public PipeWriter Output { get; }

    public DuplexPipe(PipeReader reader, PipeWriter writer)
    {
        Input = reader;
        Output = writer;
    }
}
