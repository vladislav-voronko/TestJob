using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using System.Text.Json;
using TestJob.Constants;
using TestJob.Factories;
using TestJob.Interfaces;
using TestJob.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorMessage = string.Join(
            "; ",
            context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message)));

        var response = TestJobResponseFactory.Error(
            TestJobErrorCodes.ValidationFailed,
            errorMessage);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddScoped<ITestJobService, TestJobService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "TestJob API",
            Version = "v1"
        });
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        var response = TestJobResponseFactory.Error(
            TestJobErrorCodes.InternalError,
            exception?.Message
                ?? "An unexpected error occurred.");

        await context.Response.WriteAsJsonAsync(
            response,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(builder
    => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());

app.MapControllers();

app.Run();
