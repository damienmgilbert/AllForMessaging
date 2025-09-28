namespace AllForMessaging.Core;

/// <summary>
/// Defines an asynchronous messaging abstraction for passing reference-typed payloads between producers and consumers.
/// </summary>
/// <typeparam name="TData">
/// The message payload type. Must be a reference type and provide a parameterless constructor.
/// </typeparam>
public interface IMessenger<TData> where TData : class?
{

    /// <summary>
    /// Asynchronously enqueues a message for consumption.
    /// </summary>
    /// <param name="data">The message to write. May be <see langword="null"/> depending on the implementation.</param>
    /// <returns>A task-like value that completes when the message has been accepted for delivery.</returns>
    /// <exception cref="OperationCanceledException">The write operation was canceled.</exception>
    /// <remarks>
    /// Implementations may throw other exceptions if the messenger has been closed or completed with an error.
    /// </remarks>
    ValueTask WriteAsync(TData data);

    /// <summary>
    /// Requests cancellation of pending and future write operations initiated via <see cref="WriteAsync(TData)"/>.
    /// </summary>
    /// <returns>A task that completes when cancellation has been signaled.</returns>
    /// <remarks>This method should be idempotent and safe to call multiple times.</remarks>
    Task CancelWriteAsync();

    /// <summary>
    /// Requests cancellation of pending and future read operations initiated via <see cref="ReadAsync"/>.
    /// </summary>
    /// <returns>A task that completes when cancellation has been signaled.</returns>
    /// <remarks>This method should be idempotent and safe to call multiple times.</remarks>
    Task CancelReadAsync();

    /// <summary>
    /// Completes the messenger, preventing further writes and optionally propagating an error to readers.
    /// </summary>
    /// <param name="error">
    /// An optional exception used to complete with a fault. If <see langword="null"/>, the messenger completes successfully.
    /// </param>
    /// <remarks>
    /// After calling this method, subsequent calls to <see cref="WriteAsync(TData)"/> may fail or be rejected by the implementation.
    /// </remarks>
    void Close(Exception? error = null);

    /// <summary>
    /// Asynchronously reads the next available message, awaiting availability as needed.
    /// </summary>
    /// <returns>
    /// The next message when available, or <see langword="null"/> if the messenger completes without yielding a message.
    /// </returns>
    /// <exception cref="OperationCanceledException">The read operation was canceled.</exception>
    /// <remarks>
    /// Implementations may choose to buffer internally; semantics for multiple enqueued items are implementation-specific.
    /// </remarks>
    ValueTask<TData?> ReadAsync();
}
