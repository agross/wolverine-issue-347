using Marten;

using Repro.Handlers;

using Wolverine;

namespace Repro.Sagas;

public record Third(string Id);

public class SecondSaga : Saga
{
  public string Id { get; set; } = null!;

  // This is what I use in the real system:
  public ScheduledMessage<Third> Handle(Second second)
  {
    Console.WriteLine("SecondSaga Handle");

    Id = second.Id;

    // Set a breakpoint here.
    return new Third(Id).ScheduledAt(DateTimeOffset.Now.AddSeconds(10));
  }

  // This handler works:
  // public void Handle(Second second)
  // {
  //   Console.WriteLine("SecondSaga Handle");
  //
  //   Id = second.Id;
  // }

  // This one does not work as well:
  // public Third Handle(Second second)
  // {
  //   Console.WriteLine("SecondSaga Handle");
  //
  //   Id = second.Id;
  //
  //   return new Third(second.Id);
  // }

  public Task Handle(Third third,
                     IDocumentSession session,
                     CancellationToken ct)
  {
    Console.WriteLine("SecondSaga Timeout");

    return Task.CompletedTask;
  }
}
