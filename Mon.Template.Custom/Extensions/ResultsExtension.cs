using FluentValidation.Results;

namespace Mon.Template.Custom.Extensions
{
    public static class ResultsExtension
    {
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
    }
}

namespace Mon.Template.Custom.ErreursValidation
{
    public sealed record ErreurValidation
    {
        public string Parametre { get; init; } = null!;
        public string Message { get; init; } = null!;
    }
}


