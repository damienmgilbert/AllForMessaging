using System.Threading.Channels;

using Microsoft.Extensions.Logging;

namespace AllForMessaging.Core;

/// <summary>
/// Provides asynchronous message passing over a bounded channel for a single writer and a single reader.
/// </summary>
/// <typeparam name="TData">
/// The reference type of the message payload. A parameterless constructor is required.
/// </typeparam>
/// <remarks>
/// <para>
/// Internally uses <see cref="Channel{T}"/> configured with <c>SingleReader=true</c> and <c>SingleWriter=true</c>.
/// </para>
/// <para>
/// Use <see cref="WriteAsync(TData)"/> to enqueue messages and <see cref="ReadAsync"/> to consume them.
/// Call <see cref="Close(Exception?)"/> to complete the channel, and
/// <see cref="CancelWriteAsync"/> / <see cref="CancelReadAsync"/> to cooperatively cancel pending operations.
/// </para>
/// </remarks>
/// <seealso cref="IMessenger{TData}"/>
public class Messenger<TData> : IMessenger<TData> where TData : class?
{
    /// <summary>
    /// The bounded channel used to transport messages.
    /// </summary>
    private Channel<TData> _boundedChannel;

    /// <summary>
    /// Optional logger used to record produce/consume activity and errors.
    /// </summary>
    private readonly ILogger<Messenger<TData>>? _logger;

    /// <summary>
    /// Cancellation source used to cancel pending and future write operations.
    /// </summary>
    private readonly CancellationTokenSource _writeCts = new();

    /// <summary>
    /// Cancellation source used to cancel pending and future read operations.
    /// </summary>
    private readonly CancellationTokenSource _readCts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Messenger{TData}"/> class.
    /// </summary>
    /// <param name="logger">An optional <see cref="ILogger{TCategoryName}"/> for diagnostics.</param>
    public Messenger(ILogger<Messenger<TData>>? logger = null)
    {
        _logger = logger;
        static void itemDropped(TData data)
        {
        }
        _boundedChannel = Channel.CreateBounded<TData>(
            new BoundedChannelOptions(capacity: 100)
            {
                SingleReader = false,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait
            },
            itemDropped: itemDropped);
    }

    /// <summary>
    /// Consumes items from the provided <paramref name="reader"/> until no more data can be read
    /// or <paramref name="cancellationToken"/> is signaled.
    /// </summary>
    /// <param name="reader">The channel reader to consume from.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <returns>
    /// The last <typeparamref name="TData"/> instance read from the channel, or <see langword="null"/>
    /// if no items were read.
    /// </returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="Exception">Rethrows any exception encountered while reading.</exception>
    private async ValueTask<TData?> ConsumeAsync(ChannelReader<TData> reader, CancellationToken cancellationToken = default)
    {
        TData? result;
        try
        {
            result = await reader.ReadAsync(cancellationToken);
            _logger?.LogInformation("Consumed data: {Data}", result);
            //while ()
            //{
            //    while (reader.TryRead(out TData? data))
            //    {
            //        if (data is not null)
            //        {
            //            _logger?.LogInformation("Consumed data: {Data}", data);
            //            result = data;
            //        }
            //    }
            //}
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ConsumeWithNestedWhileAsync");
            throw;
        }
        return result;
    }

    /// <summary>
    /// Attempts to write <paramref name="data"/> to the provided <paramref name="writer"/>,
    /// waiting until space is available or <paramref name="cancellationToken"/> is signaled.
    /// </summary>
    /// <param name="writer">The channel writer to produce to.</param>
    /// <param name="data">The item to write to the channel.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <returns>A task that completes when the item has been written to the channel.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="Exception">Rethrows any exception encountered while writing.</exception>
    private async ValueTask ProduceAsync(ChannelWriter<TData> writer, TData data, CancellationToken cancellationToken = default)
    {
        try
        {
            await writer.WriteAsync(data, cancellationToken);
            _logger?.LogInformation("Produced data: {Data}", data);
            //while (await writer.WaitToWriteAsync(cancellationToken))
            //{
            //    if (writer.TryWrite(data))
            //    {
            //        _logger?.LogInformation("Produced data: {Data}", data);
            //        return;
            //    }
            //}
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ProduceWithWriteAsync");
            throw;
        }
    }

    /// <summary>
    /// Enqueues a message for consumption.
    /// </summary>
    /// <param name="data">The message to write.</param>
    /// <returns>A task that completes when the message has been written.</returns>
    /// <exception cref="OperationCanceledException">The write operation was canceled.</exception>
    public async ValueTask WriteAsync(TData data) => await ProduceAsync(_boundedChannel.Writer, data, _writeCts.Token);

    /// <summary>
    /// Requests cancellation of pending and future write operations initiated via <see cref="WriteAsync(TData)"/>.
    /// </summary>
    /// <returns>A task that completes when cancellation has been signaled.</returns>
    public async Task CancelWriteAsync() => await _writeCts.CancelAsync();

    /// <summary>
    /// Requests cancellation of pending and future read operations initiated via <see cref="ReadAsync"/>.
    /// </summary>
    /// <returns>A task that completes when cancellation has been signaled.</returns>
    public async Task CancelReadAsync() => await _readCts.CancelAsync();

    /// <summary>
    /// Completes the channel to prevent further writes.
    /// </summary>
    /// <param name="error">
    /// Optional exception used to complete the writer with an error. If supplied, readers may observe the error.
    /// </param>
    public void Close(Exception? error = null) => _ = error is not null ? _boundedChannel.Writer.TryComplete(error) : _boundedChannel.Writer.TryComplete();

    /// <summary>
    /// Reads the next available message, awaiting availability as needed.
    /// </summary>
    /// <returns>
    /// The last message read while draining available items, or <see langword="null"/> if no items were read.
    /// </returns>
    /// <exception cref="OperationCanceledException">The read operation was canceled.</exception>
    /// <exception cref="ChannelClosedException">May be thrown if the channel has been completed with an error.</exception>
    public async ValueTask<TData?> ReadAsync() => await ConsumeAsync(_boundedChannel.Reader, _readCts.Token);
}