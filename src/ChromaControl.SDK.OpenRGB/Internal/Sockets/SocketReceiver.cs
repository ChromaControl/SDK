// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Pipelines;
using System.Net.Sockets;

namespace ChromaControl.SDK.OpenRGB.Internal.Sockets;

internal sealed class SocketReceiver : IDisposable
{
    private readonly Socket _socket;
    private readonly SocketAsyncEventArgs _eventArgs = new();
    private readonly SocketAwaitable _awaitable;

    public SocketReceiver(Socket socket, PipeScheduler scheduler)
    {
        _socket = socket;
        _awaitable = new SocketAwaitable(scheduler);
        _eventArgs.UserToken = _awaitable;
        _eventArgs.Completed += (_, e) => ((SocketAwaitable)e.UserToken!).Complete(e.BytesTransferred, e.SocketError);
    }

    public SocketAwaitable ReceiveAsync(Memory<byte> buffer)
    {
        _eventArgs.SetBuffer(buffer);

        if (!_socket.ReceiveAsync(_eventArgs))
        {
            _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
        }

        return _awaitable;
    }

    public void Dispose()
    {
        _eventArgs.Dispose();
    }
}
