// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ChromaControl.SDK.OpenRGB.Internal.Sockets;

internal sealed class SocketAwaitable : ICriticalNotifyCompletion
{
    private Action? _callback;
    private SocketError _error;
    private int _bytesTransferred;

    private readonly PipeScheduler _scheduler;

    private static readonly Action s_callbackCompleted = () => { };

    public bool IsCompleted => ReferenceEquals(_callback, s_callbackCompleted);

    public SocketAwaitable(PipeScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public SocketAwaitable GetAwaiter()
    {
        return this;
    }

    public int GetResult()
    {
        _callback = null;

        if (_error != SocketError.Success)
        {
            throw new SocketException((int)_error);
        }

        return _bytesTransferred;
    }

    public void OnCompleted(Action continuation)
    {
        if (ReferenceEquals(_callback, s_callbackCompleted) ||
            ReferenceEquals(Interlocked.CompareExchange(ref _callback, continuation, null), s_callbackCompleted))
        {
            Task.Run(continuation);
        }
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        OnCompleted(continuation);
    }

    public void Complete(int bytesTransferred, SocketError socketError)
    {
        _error = socketError;
        _bytesTransferred = bytesTransferred;

        var continuation = Interlocked.Exchange(ref _callback, s_callbackCompleted);

        if (continuation is not null)
        {
            _scheduler.Schedule(state => ((Action)state!)(), continuation);
        }
    }
}
