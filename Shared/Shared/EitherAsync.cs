namespace Shared.Shared;

public record EitherAsync<L, R>(Func<Task<Either<L, R>>> Run)
{
    public EitherAsync<L, T> Map<T>(Func<R, T> map) => 
        FlatMap(r => Task.FromResult<Either<L, T>>(new Either<L, T>.Right(map(r))));

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
    
    public static EitherAsync<L, R> FromRight(R value) => 
        new(() => Task.FromResult((Either<L, R>)new Either<L, R>.Right(value)));
    
    public static EitherAsync<L, R> FromLeft(L value) => 
        new(() => Task.FromResult((Either<L, R>)new Either<L, R>.Left(value)));
    
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
    
    public EitherAsync<L, R> Ensure(Func<R, bool> predicate, L errorMessage)
    {
        return FlatMap(result => Task.FromResult(
            predicate(result) 
                ? Either<L, R>.ToRight(result) 
                : Either<L, R>.ToLeft(errorMessage)
        ));
    }
}