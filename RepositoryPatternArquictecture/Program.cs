using RepositoryPatternArquitecture.Helpers;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("defaultConnection");

builder.Services.ConfiguredSqlContext(connection!);
builder.Services.ConfiguredCors();
builder.Services.ConfigureRepositoryManager();

// Adding Content Negotiation and Ignoring the reference cycles.
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
  .AddJsonOptions(opt =>
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Serilog
builder.Host.UseSerilog((ctx, lc)=>
            lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//var logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.File("logs/myapp-{Date}.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
var _logger = app.Logger;
app.ConfigureExceptionHandler(_logger);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
