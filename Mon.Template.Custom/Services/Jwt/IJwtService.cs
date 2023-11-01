using System.Security.Claims;

namespace Mon.Template.Custom.Services.Jwt;

public interface IJwtService
{
    /// <summary>
    /// Generer un JWT de 1 heure
    /// </summary>
    /// <param name="_tabClaim">Infos à mettre dans le JWT</param>
    /// <returns>JWT avec les infos</returns>
    string Generer(Claim[] _tabClaim);
}
