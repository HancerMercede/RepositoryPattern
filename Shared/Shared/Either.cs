namespace Shared.Shared;

/// <summary>
/// Represents a structure that can hold one of two values: a Left (typically for errors) 
/// or a Right (typically for successful results).
/// </summary>
/// <typeparam name="L">The type of the Left (Error) value.</typeparam>
/// <typeparam name="R">The type of the Right (Success) value.</typeparam>
public abstract record Either<L, R>
{
    private Either() { }

    /// <summary>
    /// Represents the Failure state of the Either container.
    /// </summary>
    public sealed record Left(L Value) : Either<L, R>;

    /// <summary>
    /// Represents the Success state of the Either container.
    /// </summary>
    public sealed record Right(R Value) : Either<L, R>;
    
    /// <summary>
    /// Wraps a value into a Left (Error) state.
    /// </summary>
    public static Either<L, R> ToLeft(L value) => new Left(value);

    /// <summary>
    /// Wraps a value into a Right (Success) state.
    /// </summary>
    public static Either<L, R> ToRight(R value) => new Right(value);

    /// <summary>
    /// Executes a function based on the current state of the Either container.
    /// This is the standard way to extract the value from the container.
    /// </summary>
    /// <typeparam name="T">The return type of the mapping functions.</typeparam>
    /// <param name="onLeft">Function to execute if the state is Left.</param>
    /// <param name="onRight">Function to execute if the state is Right.</param>
    /// <returns>The result of the executed function.</returns>
    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
        => this switch
        {
            Left l => onLeft(l.Value),
            Right r => onRight(r.Value),
            _ => throw new InvalidOperationException()
        };
}