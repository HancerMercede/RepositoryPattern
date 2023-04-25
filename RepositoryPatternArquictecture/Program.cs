using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("defaultConnection");

builder.Services.ConfiguredSqlContext(connection!);
builder.Services.ConfiguredCors();
builder.Services.ConfiguredIISIntegration();
builder.Services.ConfigureRepositoryManager();

// Mapster
builder.Services.RegisterMapsterConfiguration();

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Adding Content Negotiation and Ignoring the reference cycles.
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
  .AddJsonOptions(opt =>
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


// Serilog
builder.Host.UseSerilog((ctx, lc) =>
            lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.DocumentFilter<JsonPatchDocumentFilter>());

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


var logger = app.Logger;
app.ConfigureExceptionHandler(logger);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapControllers();

app.Run();
