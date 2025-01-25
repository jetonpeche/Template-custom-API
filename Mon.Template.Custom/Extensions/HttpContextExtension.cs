using System.Security.Claims;

namespace Api.Extensions;

public static class HttpContextExtension
{
    /// <summary>
    /// Recupere l'id publique dans le JWT
    /// </summary>
    /// <param name="_httpContext"></param>
    /// <returns>Id publique de l'utilisateur</returns>
    public static string RecupererIdPublique(this HttpContext _httpContext) => _httpContext.User.FindFirstValue("idUtilisateur")!;

    /// <summary>
    /// Recupere le role de l'utilisateur dans le JWT
    /// </summary>
    /// <param name="_httpContext"></param>
    /// <returns>Role de l'utilsiateur</returns>
    public static string RecupererRole(this HttpContext _httpContext) => _httpContext.User.FindFirstValue(ClaimTypes.Role)!;
}
