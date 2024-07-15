// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.IO.Pipelines;

namespace ChromaControl.SDK.OpenRGB.Internal.Protocol;

internal sealed class ProtocolReader : IAsyncDisposable
{
    private bool _disposed;
    private bool _isCanceled;
    private bool _isCompleted;
    private bool _hasPacket;
    private SequencePosition _examined;
    private SequencePosition _consumed;
    private ReadOnlySequence<byte> _buffer;

    private readonly PipeReader _reader;

    public ProtocolReader(PipeReader reader)
    {
        _reader = reader;
    }

    public ValueTask<ProtocolReadResult<TReadPacket>> ReadAsync<TReadPacket>(IPacketReader<TReadPacket> reader, CancellationToken cancellationToken = default)
    {
        return ReadAsync(reader, maximumPacketSize: null, cancellationToken);
    }

    public ValueTask<ProtocolReadResult<TReadPacket>> ReadAsync<TReadPacket>(IPacketReader<TReadPacket> reader, int? maximumPacketSize, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_hasPacket)
        {
            throw new InvalidOperationException($"{nameof(Advance)} must be called before calling {nameof(ReadAsync)}");
        }

        if (_consumed.GetObject() is null)
        {
            return ExecuteReadAsync(maximumPacketSize, reader, cancellationToken);
        }

        if (TryParsePacket(maximumPacketSize, reader, _buffer, out var protocolPacket))
        {
            _hasPacket = true;

            return new ValueTask<ProtocolReadResult<TReadPacket>>(new ProtocolReadResult<TReadPacket>(protocolPacket, _isCanceled, isCompleted: false));
        }
        else
        {
            _reader.AdvanceTo(_consumed, _examined);

            _buffer = default;
            _consumed = default;
            _examined = default;
        }

        if (_isCompleted)
        {
            _consumed = default;
            _examined = default;

            if (!_buffer.IsEmpty)
            {
                throw new InvalidDataException("Connection terminated while reading a Packet.");
            }

            return new ValueTask<ProtocolReadResult<TReadPacket>>(new ProtocolReadResult<TReadPacket>(default, _isCanceled, _isCompleted));
        }

        return ExecuteReadAsync(maximumPacketSize, reader, cancellationToken);
    }

    public void Advance()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _isCanceled = false;

        if (!_hasPacket)
        {
            return;
        }

        _buffer = _buffer.Slice(_consumed);

        _hasPacket = false;
    }

    public ValueTask DisposeAsync()
    {
        _disposed = true;

        return default;
    }

    private ValueTask<ProtocolReadResult<TReadPacket>> ExecuteReadAsync<TReadPacket>(int? maximumPacketSize, IPacketReader<TReadPacket> reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var readTask = _reader.ReadAsync(cancellationToken);

            ReadResult result;

            if (readTask.IsCompletedSuccessfully)
            {
                result = readTask.Result;
            }
            else
            {
                return ContinueExecuteReadAsync(readTask, maximumPacketSize, reader, cancellationToken);
            }

            (var shouldContinue, var hasPacket) = TrySetPacket(result, maximumPacketSize, reader, out var protocolReadResult);

            if (hasPacket)
            {
                return new ValueTask<ProtocolReadResult<TReadPacket>>(protocolReadResult);
            }
            else if (!shouldContinue)
            {
                break;
            }
        }

        return new ValueTask<ProtocolReadResult<TReadPacket>>(new ProtocolReadResult<TReadPacket>(default, _isCanceled, _isCompleted));
    }

    private async ValueTask<ProtocolReadResult<TReadPacket>> ContinueExecuteReadAsync<TReadPacket>(ValueTask<ReadResult> readTask, int? maximumPacketSize, IPacketReader<TReadPacket> reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await readTask;

            (var shouldContinue, var hasPacket) = TrySetPacket(result, maximumPacketSize, reader, out var protocolReadResult);

            if (hasPacket)
            {
                return protocolReadResult;
            }
            else if (!shouldContinue)
            {
                break;
            }

            readTask = _reader.ReadAsync(cancellationToken);
        }

        return new ProtocolReadResult<TReadPacket>(default, _isCanceled, _isCompleted);
    }

    private (bool ShouldContinue, bool HasPacket) TrySetPacket<TReadPacket>(ReadResult result, int? maximumPacketSize, IPacketReader<TReadPacket> reader, out ProtocolReadResult<TReadPacket> readResult)
    {
        _buffer = result.Buffer;
        _isCanceled = result.IsCanceled;
        _isCompleted = result.IsCompleted;
        _consumed = _buffer.Start;
        _examined = _buffer.End;

        if (_isCanceled)
        {
            readResult = default;
            return (false, false);
        }

        if (TryParsePacket(maximumPacketSize, reader, _buffer, out var protocolPacket))
        {
            _hasPacket = true;
            readResult = new ProtocolReadResult<TReadPacket>(protocolPacket, _isCanceled, isCompleted: false);
            return (false, true);
        }
        else
        {
            _reader.AdvanceTo(_consumed, _examined);

            _buffer = default;
            _consumed = default;
            _examined = default;
        }

        if (_isCompleted)
        {
            _consumed = default;
            _examined = default;

            if (!_buffer.IsEmpty)
            {
                throw new InvalidDataException("Connection terminated while reading a packet.");
            }

            readResult = default;
            return (false, false);
        }

        readResult = default;
        return (true, false);
    }

    private bool TryParsePacket<TReadPacket>(int? maximumPacketSize, IPacketReader<TReadPacket> reader, in ReadOnlySequence<byte> buffer, out TReadPacket protocolPacket)
    {
        if (maximumPacketSize == null)
        {
            return reader.TryParsePacket(buffer, ref _consumed, ref _examined, out protocolPacket);
        }

        var maxPacketSize = maximumPacketSize.Value;

        var segment = buffer;
        var overLength = false;

        if (segment.Length > maxPacketSize)
        {
            segment = segment.Slice(segment.Start, maxPacketSize);
            overLength = true;
        }

        if (reader.TryParsePacket(segment, ref _consumed, ref _examined, out protocolPacket))
        {
            return true;
        }
        else if (overLength)
        {
            throw new InvalidDataException($"The maximum packet size of {maxPacketSize}B was exceeded.");
        }

        return false;
    }
}
