namespace Events;

public class GameStartedEvent : TracingEventBase
{
    public Guid GameId { get; set; }
}