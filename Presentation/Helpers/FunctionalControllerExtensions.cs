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
    /// It automatically generates the 'Location' header using the provided route name and ID.
    /// </summary>
    /// <typeparam name="T">The type of the newly created entity DTO.</typeparam>
    /// <param name="result">The Either result from the service layer.</param>
    /// <param name="routeName">The name of the Get route (e.g., "GetCompanyById").</param>
    /// <param name="idSelector">A function to extract the ID from the created entity.</param>
    /// <returns>A 201 Created result with the entity, or an appropriate error result.</returns>
    public static ActionResult<T> HandleCreated<T>(
        this Either<string, T> result, 
        string routeName, 
        Func<T, object> idSelector)
    {
        return result.Match<ActionResult<T>>(
            onLeft: error => MapToErrorResult(error),
            onRight: data => new CreatedAtRouteResult(routeName, new { id = idSelector(data) }, data)
        );
    }
}