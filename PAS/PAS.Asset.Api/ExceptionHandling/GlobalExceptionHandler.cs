using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAS.Common.Exceptions;

namespace PAS.Asset.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
        var problem = exception switch {
            DomainException domainException => new ProblemDetails {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Règle métier non respectée",
                Detail = domainException.Message,
                Extensions =
                {
                    ["code"] = domainException.Code
                }
            },

            NotFoundException notFoundException => new ProblemDetails {
                Status = StatusCodes.Status404NotFound,
                Title = "Ressource introuvable",
                Detail = notFoundException.Message,
            },

            ConflictException conflictException => new ProblemDetails {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflit de ressource",
                Detail = conflictException.Message,
            },

            _ => new ProblemDetails {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Une erreur interne est survenue",
                Detail = "Une erreur inattendue est survenue."
            }
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = problem.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}