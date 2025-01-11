using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using UkrGuru.Sql;

// Constants for API pattern and suffix
const string ApiHolePattern = "ApiHole";
const string ApiProcSufix = "_Api";

var builder = WebApplication.CreateBuilder(args);

// Registering the database service with dependency injection
builder.Services.AddScoped<IDbService, DbService>();

// Adding controllers with a custom input formatter for plain text
builder.Services.AddControllers(options => { options.InputFormatters.Insert(0, new PlaintextMediaTypeFormatter()); });

// Adding OpenAPI/Swagger services to the container
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline for development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enforce HTTPS redirection
app.UseHttpsRedirection();

// Map POST endpoint for executing stored procedures
app.MapPost($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] string? data) =>
    await db.TryExecAsync<string?>($"{proc}{ApiProcSufix}", data));

// Map GET endpoint for executing stored procedures
app.MapGet($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
    await db.TryExecAsync<string?>($"{proc}{ApiProcSufix}", data));

// Map PUT endpoint for executing stored procedures
app.MapPut($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] string? data) =>
    await db.TryExecAsync($"{proc}{ApiProcSufix}", data));

// Map DELETE endpoint for executing stored procedures
app.MapDelete($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
    await db.TryExecAsync($"{proc}{ApiProcSufix}", data));

// Run the application
app.Run();

// Custom input formatter for handling plain text requests
class PlaintextMediaTypeFormatter : TextInputFormatter
{
    public PlaintextMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
    {
        var httpContext = context.HttpContext;

        using var reader = new StreamReader(httpContext.Request.Body, effectiveEncoding);

        return await InputFormatterResult.SuccessAsync(await reader.ReadToEndAsync());
    }
}