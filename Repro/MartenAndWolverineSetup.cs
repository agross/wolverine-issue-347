using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Storage;

using Weasel.Core;

using Wolverine;
using Wolverine.Marten;

namespace Repro;

public class MartenAndWolverineSetup
{
  public static void ConfigureMarten(IServiceCollection services,
                                     IConfiguration configuration,
                                     IHostEnvironment env)
  {
    services
      .AddMarten(serviceProvider =>
      {
        var o = new StoreOptions();

        o.Connection(configuration.GetConnectionString(name: "Marten") ??
                     throw new
                       InvalidOperationException(message:
                                                 "Please set the connection string \"Marten\" in appsettings."));

        o.DatabaseSchemaName = "saga_events";
        o.Events.StreamIdentity = StreamIdentity.AsString;
        o.RetryPolicy(DefaultRetryPolicy.Once(sleep: attempt => TimeSpan.FromMilliseconds(100 * attempt)));

        o.AutoCreateSchemaObjects = AutoCreate.All;

        return o;
      })

      // This is the wolverine integration for the outbox/inbox,
      // transactional middleware, saga persistence we don't care about
      // yet.
      .IntegrateWithWolverine(schemaName: "wolverine_messages")

      // Publish event stream events to Wolverine.
      .EventForwardingToWolverine()

      // Just letting Marten build out known database schema elements upfront
      // Helps with Wolverine integration in development.
      .ApplyAllDatabaseChangesOnStartup()

      // Required for MultiStreamAggregations.
      .AddAsyncDaemon(mode: DaemonMode.Solo)

      // Default session type.
      .UseLightweightSessions();
  }

  public static void ConfigureWolverine(HostBuilderContext context,
                                        WolverineOptions o)
  {
    // Automatic transactions around handlers.
    o.Policies.AutoApplyTransactions();

    // Enrolling all local queues into the durable inbox/outbox processing.
    o.Policies.UseDurableLocalQueues();
  }
}
