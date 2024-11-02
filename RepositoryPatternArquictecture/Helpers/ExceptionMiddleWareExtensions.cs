using Entities.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Entities.Exceptions;

namespace RepositoryPatternArquitecture.Helpers;

public static class ExceptionMiddleWareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType= "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                       NotFoundException => StatusCodes.Status404NotFound,
                       BadRequestException => StatusCodes.Status400BadRequest,
                       _=> StatusCodes.Status500InternalServerError
                    };
                    logger.LogError($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,

                    }.ToString());
                }
            });
        });
    
    }
}
