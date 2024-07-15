// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ChromaControl.SDK.OpenRGB.Internal.Sockets;

internal sealed class SocketSender : IDisposable
{
    private List<ArraySegment<byte>>? _bufferList;

    private readonly Socket _socket;
    private readonly SocketAsyncEventArgs _eventArgs = new();
    private readonly SocketAwaitable _awaitable;

    public SocketSender(Socket socket, PipeScheduler scheduler)
    {
        _socket = socket;
        _awaitable = new SocketAwaitable(scheduler);
        _eventArgs.UserToken = _awaitable;
        _eventArgs.Completed += (_, e) => ((SocketAwaitable)e.UserToken!).Complete(e.BytesTransferred, e.SocketError);
    }

    public SocketAwaitable SendAsync(in ReadOnlySequence<byte> buffers)
    {
        if (buffers.IsSingleSegment)
        {
            return SendAsync(buffers.First);
        }

        if (!_eventArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
        {
            _eventArgs.SetBuffer(null, 0, 0);
        }

        _eventArgs.BufferList = GetBufferList(buffers);

        if (!_socket.SendAsync(_eventArgs))
        {
            _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
        }

        return _awaitable;
    }

    public void Dispose()
    {
        _eventArgs.Dispose();
    }

    private SocketAwaitable SendAsync(ReadOnlyMemory<byte> memory)
    {
        if (_eventArgs.BufferList is not null)
        {
            _eventArgs.BufferList = null;
        }

        _eventArgs.SetBuffer(MemoryMarshal.AsMemory(memory));

        if (!_socket.SendAsync(_eventArgs))
        {
            _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
        }

        return _awaitable;
    }

    private List<ArraySegment<byte>> GetBufferList(in ReadOnlySequence<byte> buffer)
    {
        if (_bufferList is null)
        {
            _bufferList = [];
        }
        else
        {
            _bufferList.Clear();
        }

        foreach (var memory in buffer)
        {
            _bufferList.Add(memory.GetArray());
        }

        return _bufferList;
    }
}
