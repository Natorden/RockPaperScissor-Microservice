namespace Events;

public class TracingEventBase
{
    public Dictionary<string, object> Headers { get; } = new();
    public string Text { get; set; }
    
    public TracingEventBase()
    { }

    public TracingEventBase(string text)
    {
        Text = text;
    }
}