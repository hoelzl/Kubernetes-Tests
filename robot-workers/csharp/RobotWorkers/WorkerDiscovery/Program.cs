using RobotWorkers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();

app.MapGet("/worker", WorkerFaker.Generate);
app.MapHealthChecks("/healthz");

app.Run();
