using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("defaultConnection");

// Adding rate limiting and other services
builder.Services.ConfigureRateLimiter();
builder.Services.ConfiguredSqlContext(connection!);
builder.Services.ConfiguredCors();
builder.Services.ConfiguredIISIntegration();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();

// Mapster
builder.Services.RegisterMapsterConfiguration();

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Api Behavior Options
// builder.Services.Configure<ApiBehaviorOptions>(opts =>
// {
//     opts.SuppressModelStateInvalidFilter = true;
// });
NewtonsoftJsonPatchInputFormatter GetjsonPatchInputFormatter()=>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();


// Adding Content Negotiation and Ignoring the reference cycles.
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0,GetjsonPatchInputFormatter());
}).AddApplicationPart(typeof(AssemblyReference).Assembly)
    .AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
  .AddJsonOptions(opt =>
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


// Serilog 
builder.Host.UseSerilog((ctx, lc) =>
            lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Provide the api validations to validate the models is null or not.
builder.Services.Configure<ApiBehaviorOptions>(opts =>
{
    opts.SuppressModelStateInvalidFilter = true;
});

// Api version
builder.Services.AddApiVersioning(v =>
{
    v.DefaultApiVersion = new(1, 0);
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Map health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
}
else
    app.UseHsts();



app.ConfigureExceptionHandler(app.Logger);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRateLimiter();

app.UseCors("AllowAll");

app.UseAuthorization();
//app.Run(async context =>
//{
//    await context.Response.WriteAsync("Hello from the middlewate");
//});
app.MapHealthChecks("/Health");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapControllers();

app.Run();
