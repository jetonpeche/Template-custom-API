#if UtiliserFluentValidator
using FluentValidation.Results;
#endif

using System.Text.Json.Serialization;

namespace Mon.Template.Custom.Extensions
{
    public static class ResultsExtension
    {
        #if UtiliserFluentValidator
        /// <summary>
        /// Lister les erreurs du validator
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="_ListeErreur"></param>
        /// <returns>Liste des erreurs de validation</returns>
        public static IResult ErreurValidator(this IResultExtensions ext, List<ValidationFailure> _ListeErreur)
        {
            return Results.UnprocessableEntity(_ListeErreur.Select(x => new ErreursValidation.ErreurValidation
            {
                Parametre = x.PropertyName,
                Message = x.ErrorMessage
            }));
        }
        #endif

        /// <summary>
        /// Erreur 503
        /// </summary>
        /// <returns>Liste des erreurs de validation</returns>
        public static IResult ErreurConnexionBdd(this IResultExtensions ext)
        {
            return Results.Problem(detail: "Impossible de se connecter à la base de données", statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        /// <summary>
        /// Produit un code HTTP 200 OK sans utiliser la refléxion pour la sérialisation
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="_retour">donnée à retourner</param>
        /// <param name="_retourContext">Le context du param '_retour'</param>
        /// <returns>Renvoie les donnée en code HTTP 200 OK</returns>
        public static IResult OK(this IResultExtensions ext, object? _retour, JsonSerializerContext _retourContext)
        {
            return Results.Json(_retour, _retourContext, statusCode: StatusCodes.Status200OK);
        }
    }
}
#if UtiliserFluentValidator
namespace Mon.Template.Custom.ErreursValidation
{
    public sealed record ErreurValidation
    {
        public string Parametre { get; init; } = null!;
        public string Message { get; init; } = null!;
    }
}
#endif

