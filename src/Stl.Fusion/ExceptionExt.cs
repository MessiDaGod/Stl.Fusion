namespace Stl.Fusion;

public static class ExceptionExt
{
    public static ResultException ToResult(this Exception wrappedException)
        => new(wrappedException.Message, wrappedException);

    public static Exception MaybeToResult(this Exception sourceException, bool wrapToResultException)
        => wrapToResultException ? sourceException.ToResult() : sourceException;
}
