namespace Repro.Handlers;

public record First(string Id);
public record Second(string Id);

public record TheDomainModel(string Id)
{
  public First DoFirst() => new(Id: Id);
  public Second DoSecond() => new(Id: Id);

  public static TheDomainModel Create(First ev) => new(Id: ev.Id);

  public static TheDomainModel Apply(Second ev, TheDomainModel current)
    => current;
}
