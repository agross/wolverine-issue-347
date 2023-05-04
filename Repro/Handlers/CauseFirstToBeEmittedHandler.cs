using Marten;

namespace Repro.Handlers;

public record CauseFirstToBeEmitted(string Id);

public static class CauseFirstToBeEmittedHandler
{
  public static First Handle(CauseFirstToBeEmitted command, IDocumentSession session)
  {
    var model = new TheDomainModel(command.Id);
    var ev = model.DoFirst();
    session.Events.StartStream<TheDomainModel>(command.Id, ev);

    return ev;
  }
}
