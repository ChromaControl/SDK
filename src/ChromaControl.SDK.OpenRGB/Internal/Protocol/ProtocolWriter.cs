// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Pipelines;

namespace ChromaControl.SDK.OpenRGB.Internal.Protocol;

internal sealed class ProtocolWriter : IAsyncDisposable
{
    private bool _disposed;

    private readonly PipeWriter _writer;
    private readonly SemaphoreSlim _semaphore = new(1);

    public ProtocolWriter(PipeWriter writer)
    {
        _writer = writer;
    }

    public async ValueTask WriteAsync<TWritePacket>(IPacketWriter<TWritePacket> writer, TWritePacket protocolPacket, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_disposed)
            {
                return;
            }

            writer.WritePacket(protocolPacket, _writer);

            var result = await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsCanceled)
            {
                throw new OperationCanceledException();
            }

            if (result.IsCompleted)
            {
                _disposed = true;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
