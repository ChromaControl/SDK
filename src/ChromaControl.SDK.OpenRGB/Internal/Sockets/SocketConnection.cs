// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Pipes;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

namespace ChromaControl.SDK.OpenRGB.Internal.Sockets;

internal sealed class SocketConnection : ConnectionContext, IConnectionInherentKeepAliveFeature
{
    private IDuplexPipe? _application;
    private bool _aborted;

    private readonly EndPoint _endPoint;
    private readonly Socket _socket;
    private readonly SocketSender _sender;
    private readonly SocketReceiver _receiver;

    public bool HasInherentKeepAlive { get; } = true;

    public override IDuplexPipe Transport { get; set; } = default!;
    public override IFeatureCollection Features { get; } = new FeatureCollection();
    public override string ConnectionId { get; set; } = Guid.NewGuid().ToString();
    public override IDictionary<object, object?> Items { get; set; } = new ConnectionItems();

    public SocketConnection(EndPoint endPoint)
    {
        _endPoint = endPoint;
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        _sender = new SocketSender(_socket, PipeScheduler.ThreadPool);
        _receiver = new SocketReceiver(_socket, PipeScheduler.ThreadPool);

        Features.Set<IConnectionInherentKeepAliveFeature>(this);
    }

    public async ValueTask<ConnectionContext> StartAsync()
    {
        await _socket.ConnectAsync(_endPoint).ConfigureAwait(false);

        var pair = DuplexPipePair.CreateConnectionPair();

        LocalEndPoint = _socket.LocalEndPoint;
        RemoteEndPoint = _socket.RemoteEndPoint;

        Transport = pair.Transport;
        _application = pair.Application;

        _ = ExecuteAsync();

        return this;
    }

    public override async ValueTask DisposeAsync()
    {
        if (Transport != null)
        {
            await Transport.Output.CompleteAsync().ConfigureAwait(false);
            await Transport.Input.CompleteAsync().ConfigureAwait(false);
        }

        await base.DisposeAsync();
    }

    private async Task ExecuteAsync()
    {
        Exception? error = null;

        try
        {
            var receiveTask = ExecuteReceive();
            var sendTask = ExecuteSend();

            if (await Task.WhenAny(receiveTask, sendTask).ConfigureAwait(false) == sendTask)
            {
                _socket.Dispose();
            }

            await receiveTask;
            error = await sendTask;

            _socket.Dispose();
        }
        catch (Exception) { }
        finally
        {
            _application!.Input.Complete(error);
        }
    }

    private async Task ExecuteReceive()
    {
        Exception? error = null;

        try
        {
            await ProcessReceive().ConfigureAwait(false);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
        {
            error = new ConnectionResetException(ex.Message, ex);
        }
        catch (SocketException ex) when (ex.SocketErrorCode is SocketError.OperationAborted or
                                         SocketError.ConnectionAborted or
                                         SocketError.Interrupted or
                                         SocketError.InvalidArgument)
        {
            if (!_aborted)
            {
                error = new ConnectionAbortedException();
            }
        }
        catch (ObjectDisposedException)
        {
            if (!_aborted)
            {
                error = new ConnectionAbortedException();
            }
        }
        catch (IOException ex)
        {
            error = ex;
        }
        catch (Exception ex)
        {
            error = new IOException(ex.Message, ex);
        }
        finally
        {
            if (_aborted)
            {
                error ??= new ConnectionAbortedException();
            }

            await _application!.Output.CompleteAsync(error).ConfigureAwait(false);
        }
    }

    private async Task ProcessReceive()
    {
        while (true)
        {
            var buffer = _application!.Output.GetMemory();

            var bytesReceived = await _receiver.ReceiveAsync(buffer);

            if (bytesReceived is 0)
            {
                break;
            }

            _application.Output.Advance(bytesReceived);

            var result = await _application.Output.FlushAsync().ConfigureAwait(false);

            if (result.IsCompleted)
            {
                break;
            }
        }
    }

    private async Task<Exception?> ExecuteSend()
    {
        Exception? error = null;

        try
        {
            await ProcessSend().ConfigureAwait(false);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted) { }
        catch (ObjectDisposedException) { }
        catch (IOException) { }
        catch (Exception ex)
        {
            error = new IOException(ex.Message, ex);
        }
        finally
        {
            _aborted = true;
            _socket.Shutdown(SocketShutdown.Both);
        }

        return error;
    }

    private async Task ProcessSend()
    {
        while (true)
        {
            var result = await _application!.Input.ReadAsync().ConfigureAwait(false);
            var buffer = result.Buffer;

            if (result.IsCanceled)
            {
                break;
            }

            var end = buffer.End;
            var isCompleted = result.IsCompleted;

            if (!buffer.IsEmpty)
            {
                await _sender.SendAsync(buffer);
            }

            _application.Input.AdvanceTo(end);

            if (isCompleted)
            {
                break;
            }
        }
    }
}
