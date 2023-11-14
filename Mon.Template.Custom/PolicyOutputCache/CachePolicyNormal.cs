using Microsoft.AspNetCore.OutputCaching;

namespace Mon.Template.Custom.PolicyOutputCache;

public sealed class CachePolicyNormal: IOutputCachePolicy
{
    public string NomTag { get; init; }

    public CachePolicyNormal(string _nomTag) => NomTag = _nomTag;

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);

        context.Tags.Add(NomTag);

        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // durée de vie du cache
        context.ResponseExpirationTimeSpan = TimeSpan.FromMinutes(20);

        return ValueTask.CompletedTask;
    }

    // n'est pas utiliser quand il y a un authorize
    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        context.AllowCacheStorage = true;

        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context)
    {
        var request = context.HttpContext.Request;

        // activer cache si la méthode HTTP est GET
        return HttpMethods.IsGet(request.Method);
    }
}
