namespace Shared.Shared;

public abstract record Either<L, R>
{
    private Either()
    {
    }

    public sealed record Left(L Value) : Either<L, R>;
    public sealed record Right(R Value) : Either<L, R>;
    
    public static Either<L, R> ToLeft(L value) => new Left(value);
    public static Either<L, R> ToRight(R value) => new Right(value);

    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
        => this switch
        {
            Left l => onLeft(l.Value),
            Right r => onRight(r.Value),
            _ => throw new InvalidOperationException()
           
        };
}