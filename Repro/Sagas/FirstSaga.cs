using Marten;

using Repro.Handlers;

using Wolverine;

namespace Repro.Sagas;

public record CauseSecondToBeEmitted(string Id);

public class FirstSaga : Saga
{
  public string Id { get; set; } = null!;

  public SecondSaga Start(First first)
  {
    Id = first.Id;

    Console.WriteLine(value: "FirstSaga Start");

    return new SecondSaga { Id = first.Id };
  }

  public static async Task Handle(CauseSecondToBeEmitted command,
                                  IDocumentSession session,
                                  CancellationToken ct)
  {
    var model = await session.Events.FetchForWriting<TheDomainModel>(key: command.Id, cancellation: ct);
    var ev = model.Aggregate.DoSecond();
    model.AppendOne(@event: ev);
  }
}
