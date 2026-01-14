namespace Shared.Shared;

/// <summary>
/// A wrapper for asynchronous operations that can result in either a Left (Failure) or a Right (Success).
/// It encapsulates a function that returns a Task of Either.
/// </summary>
/// <typeparam name="L">The type representing the failure/error result.</typeparam>
/// <typeparam name="R">The type representing the successful result.</typeparam>
public record EitherAsync<L, R>(Func<Task<Either<L, R>>> Run)
{
    /// <summary>
    /// Transforms the successful value (Right) of the EitherAsync using a synchronous mapping function.
    /// If the result is a Left, the mapping function is skipped.
    /// </summary>
    /// <typeparam name="T">The type of the new successful result.</typeparam>
    /// <param name="map">The function to transform the Right value.</param>
    /// <returns>A new EitherAsync containing the mapped value or the original Left.</returns>
    public EitherAsync<L, T> Map<T>(Func<R, T> map) => 
        FlatMap(r => Task.FromResult<Either<L, T>>(new Either<L, T>.Right(map(r))));

    /// <summary>
    /// Chains another asynchronous operation that returns an Either. 
    /// If the current state is Left, the operation is skipped.
    /// </summary>
    /// <typeparam name="T">The type of the new successful result.</typeparam>
    /// <param name="map">A function that takes the Right value and returns a Task of Either.</param>
    /// <returns>A new EitherAsync representing the result of the chained operation.</returns>
    public EitherAsync<L, T> FlatMap<T>(Func<R, Task<Either<L, T>>> map)
    {
        return new EitherAsync<L, T>(async () =>
        {
            var result = await Run();
            return result switch
            {
                Either<L, R>.Left l => new Either<L, T>.Left(l.Value),
                Either<L, R>.Right r => await map(r.Value),
                _ => throw new InvalidOperationException()
            };
        });
    }
    
    /// <summary>
    /// Creates an EitherAsync instance from a successful value.
    /// </summary>
    public static EitherAsync<L, R> FromRight(R value) => 
        new(() => Task.FromResult((Either<L, R>)new Either<L, R>.Right(value)));
    
    /// <summary>
    /// Creates an EitherAsync instance from a failure value.
    /// </summary>
    public static EitherAsync<L, R> FromLeft(L value) => 
        new(() => Task.FromResult((Either<L, R>)new Either<L, R>.Left(value)));
    
    /// <summary>
    /// Executes an asynchronous task safely. If an exception occurs, it is caught and transformed using the error handler.
    /// </summary>
    /// <param name="action">The asynchronous operation to execute.</param>
    /// <param name="errorHandler">A function to transform the exception into the Left type.</param>
    /// <returns>An EitherAsync containing the result of the task or the captured error.</returns>
    public static EitherAsync<L, R> Try<L, R>(
        Func<Task<R>> action, 
        Func<Exception, L> errorHandler)
    {
        return new EitherAsync<L, R>(async () =>
        {
            try
            {
                var result = await action();
                return Either<L, R>.ToRight(result);
            }
            catch (Exception ex)
            {
                return Either<L, R>.ToLeft(errorHandler(ex));
            }
        });
    }
    
    /// <summary>
    /// Validates the successful value against a predicate. 
    /// If the predicate is false, the flow switches to a Left state with the provided error message.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <param name="errorMessage">The error to return if the predicate fails.</param>
    /// <returns>The original EitherAsync if successful and predicate passes; otherwise, a Left.</returns>
    public EitherAsync<L, R> Ensure(Func<R, bool> predicate, L errorMessage)
    {
        return FlatMap(result => Task.FromResult(
            predicate(result) 
                ? Either<L, R>.ToRight(result) 
                : Either<L, R>.ToLeft(errorMessage)
        ));
    }
}