using Domain.Abstractions;

namespace TestTask_14._03.Endpoints
{
    public static class AnalyticEndpoint
    {
        public static IEndpointRouteBuilder MapAnalyticEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/analytics")
                .WithTags("Analytics")
                .WithOpenApi();

            group.MapGet("/dashboard", async (IAnalyticService analyticService) =>
            {
                var result = await analyticService.GetAnalyticsAsync();

                if (result.IsFailure)
                    return Results.BadRequest(new { errors = result.Errors });

                return Results.Ok(result.Data);
            })
            .WithName("Make Analytic")
            .WithSummary("Собрать аналитические данные");

            return app;
        }
    }
}
