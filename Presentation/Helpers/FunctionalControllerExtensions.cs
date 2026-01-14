namespace Presentation.Helpers;

public static class FunctionalControllerExtensions
{
    public static ActionResult<T> HandleResult<T>(this Either<string, T> result)
    {
        return result.Match<ActionResult<T>>(
            onLeft: error => MapToErrorResult(error),
            onRight: data => data is Unit ? new NoContentResult() : new OkObjectResult(data)
        );
    }
    
    public static IActionResult HandleResult(this Either<string, Unit> result)
    {
        return result.Match<IActionResult>(
            onLeft: error => MapToErrorResult(error),
            onRight: _ => new NoContentResult()
        );
    }
    
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
}