using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.Common_Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();
            else
                return HandleProblem(result.Errors);

        }

        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return HandleProblem(result.Errors);


        }

        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            if (errors.Count == 0)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError
                    });
            }
            if (errors.All(e => e.Type == ErrorType.Validation))
                return HandleValidationProblem(errors);

            return HandleSingleErrorProblem(errors[0]);


        }

        private ActionResult HandleSingleErrorProblem(Error error)
        {
            var problemDetails = new ProblemDetails
            {
                Title = error.Code,
                Detail = error.Description,
                Type = error.Type.ToString(),
                Status = MapErrorTypeToStatusCode(error.Type)
            };

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }

        private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
        {

            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.InvalidCredentials => StatusCodes.Status404NotFound,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,

        };

        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
                modelState.AddModelError(error.Code, error.Description);
            return ValidationProblem(modelState);
        }

    }
}
