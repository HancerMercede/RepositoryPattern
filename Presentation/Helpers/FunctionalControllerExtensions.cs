namespace Presentation.Helpers;

/// <summary>
/// Provides extension methods to map functional results (Either) to standard Web API ActionResults.
/// </summary>
public static class FunctionalControllerExtensions
{
    /// <summary>
    /// Maps an Either result with a generic data payload to an HTTP response.
    /// Returns 200 OK with data, 204 No Content for Unit results, or an appropriate error result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result data.</typeparam>
    /// <param name="result">The Either container containing either an error string or the success data.</param>
    /// <returns>An ActionResult containing the success data or an error object.</returns>
    public static ActionResult<T> HandleResult<T>(this Either<string, T> result)
    {
        return result.Match<ActionResult<T>>(
            onLeft: error => MapToErrorResult(error),
            onRight: data => data is Unit ? new NoContentResult() : new OkObjectResult(data)
        );
    }
    
    /// <summary>
    /// Maps an Either result with a Unit (void equivalent) payload to an HTTP response.
    /// Returns 204 No Content on success or an appropriate error result on failure.
    /// </summary>
    /// <param name="result">The Either container containing either an error string or a Unit success.</param>
    /// <returns>An IActionResult representing the operation outcome.</returns>
    public static IActionResult HandleResult(this Either<string, Unit> result)
    {
        return result.Match<IActionResult>(
            onLeft: error => MapToErrorResult(error),
            onRight: _ => new NoContentResult()
        );
    }
    
    /// <summary>
    /// Analyzes the error message to determine the specific HTTP error status.
    /// Maps "not found" or "not exist" to 404 NotFound, otherwise returns 400 BadRequest.
    /// </summary>
    /// <param name="error">The error message string to analyze.</param>
    /// <returns>An ObjectResult (NotFound or BadRequest) with a JSON payload containing the error message.</returns>
    private static ObjectResult MapToErrorResult(string error)
    {
        return error switch
        {
            var msg when msg.Contains("not found", StringComparison.OrdinalIgnoreCase) || 
                         msg.Contains("not exist", StringComparison.OrdinalIgnoreCase)
                => new NotFoundObjectResult(new { Message = msg }),

            _ => new BadRequestObjectResult(new { Message = error })
        };
    }
    
    /// <summary>
    /// Maps an Either result to an HTTP 201 Created response.
    /// This method is flexible: it can handle a single ID or an anonymous object for complex routes.
    /// </summary>
    /// <typeparam name="T">The type of the newly created entity DTO.</typeparam>
    /// <param name="result">The Either result from the service layer containing the created entity or an error message.</param>
    /// <param name="routeName">The name of the GET route used to retrieve the resource (e.g., "GetCompanyById").</param>
    /// <param name="idSelector">
    /// A function that selects the route data. It can return a simple value (like a Guid or int), 
    /// in which case it's mapped to an 'id' parameter, or an anonymous object (new { companyId, id }) 
    /// for routes with multiple parameters.
    /// </param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/>: <see cref="CreatedAtRouteResult"/> on success (Right), 
    /// or the corresponding error result (e.g., BadRequest, NotFound) on failure (Left).
    /// </returns>
    public static ActionResult<T> HandleCreated<T>(
        this Either<string, T> result,
        string routeName,
        Func<T, object> idSelector)
    {
        return result.Match<ActionResult<T>>(
            onLeft: error => MapToErrorResult(error),
            onRight: data =>
            {
                var selectorResult = idSelector(data);
                object routeValues;

                // Check if the result is already an anonymous object (complex route)
                // or a primitive value (simple route with 'id' parameter).
                if (selectorResult != null && selectorResult.GetType().Name.Contains("AnonymousType"))
                {
                    routeValues = selectorResult;
                }
                else
                {
                    // For simple selectors like c => c.Id, we wrap it in an 'id' property
                    // so ASP.NET can match the {id} parameter in the route.
                    routeValues = new { id = selectorResult };
                }

                return new CreatedAtRouteResult(routeName, routeValues, data);
            }
        );
    }

}