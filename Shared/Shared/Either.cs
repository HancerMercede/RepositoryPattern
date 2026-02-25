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
    /// Returns true if this is a Left (Error) value.
    /// </summary>
    public bool IsLeft => this is Left;

    /// <summary>
    /// Returns true if this is a Right (Success) value.
    /// </summary>
    public bool IsRight => this is Right;

    /// <summary>
    /// Gets the Right value if present, otherwise returns null.
    /// </summary>
    public R? RightValue => this is Right r ? r.Value : default;

    /// <summary>
    /// Gets the Left value if present, otherwise returns null.
    /// </summary>
    public L? LeftValue => this is Left l ? l.Value : default;

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

    /// <summary>
    /// Transforms the Right value using a mapping function.
    /// If the state is Left, returns a new Left with the original value.
    /// </summary>
    /// <typeparam name="T">The type of the new Right value.</typeparam>
    /// <param name="map">The function to transform the Right value.</param>
    /// <returns>A new Either with the transformed Right value or the original Left.</returns>
    public Either<L, T> Map<T>(Func<R, T> map)
        => this switch
        {
            Left l => new Either<L, T>.Left(l.Value),
            Right r => new Either<L, T>.Right(map(r.Value)),
            _ => throw new InvalidOperationException()
        };

    /// <summary>
    /// Transforms the Left value using a mapping function.
    /// If the state is Right, returns a new Right with the original value.
    /// </summary>
    /// <typeparam name="T">The type of the new Left value.</typeparam>
    /// <param name="map">The function to transform the Left value.</param>
    /// <returns>A new Either with the transformed Left value or the original Right.</returns>
    public Either<T, R> MapLeft<T>(Func<L, T> map)
        => this switch
        {
            Left l => new Either<T, R>.Left(map(l.Value)),
            Right r => new Either<T, R>.Right(r.Value),
            _ => throw new InvalidOperationException()
        };

    /// <summary>
    /// Returns the Right value if present, otherwise returns the provided default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if Left.</param>
    /// <returns>The Right value or the default.</returns>
    public R GetOrElse(R defaultValue)
        => this is Right r ? r.Value : defaultValue;

    /// <summary>
    /// Returns the Right value if present, otherwise returns the result of the factory function.
    /// </summary>
    /// <param name="defaultValueFactory">The factory function to create a default value if Left.</param>
    /// <returns>The Right value or the factory result.</returns>
    public R GetOrElse(Func<R> defaultValueFactory)
        => this is Right r ? r.Value : defaultValueFactory();

    /// <summary>
    /// Returns the Left value if present, otherwise returns null.
    /// </summary>
    public L? GetLeftOrDefault() => this is Left l ? l.Value : default;

    /// <summary>
    /// Returns the Right value if present, otherwise returns null.
    /// </summary>
    public R? GetRightOrDefault() => this is Right r ? r.Value : default;

    /// <summary>
    /// Implicitly converts Either to EitherAsync.
    /// </summary>
    public static implicit operator EitherAsync<L, R>(Either<L, R> either)
        => either.IsRight 
            ? EitherAsync<L, R>.FromRight(either.RightValue!) 
            : EitherAsync<L, R>.FromLeft(either.LeftValue!);
}
