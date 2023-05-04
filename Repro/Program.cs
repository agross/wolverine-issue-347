using Microsoft.AspNetCore.Mvc;

using Repro;
using Repro.Handlers;
using Repro.Sagas;

using Wolverine;

var builder = WebApplication.CreateBuilder(args: args);

MartenAndWolverineSetup.ConfigureMarten(services: builder.Services,
                                        configuration: builder.Configuration,
                                        env: builder.Environment);

builder.Host.UseWolverine(overrides: MartenAndWolverineSetup.ConfigureWolverine);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapGet(pattern: "/first",
           async([FromQuery] string id, [FromServices] IMessageBus bus, CancellationToken ct) =>
           {
             await bus.InvokeAsync<First>(new CauseFirstToBeEmitted(Id: id), cancellation: ct);
           });

app.MapGet(pattern: "/second",
           async ([FromQuery] string id, [FromServices] IMessageBus bus, CancellationToken ct) =>
           {
             await bus.InvokeAsync(new CauseSecondToBeEmitted(Id: id), cancellation: ct);
           });

app.Run();
