using System.Diagnostics;
using EasyNetQ;
using Events;
using Helpers;
using Monolith;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PubSubExtensions = Helpers.PubSubExtensions;

public class Program
{
    private static Game _game = new Game();

    public static async Task Main()
    {
        var connectionEstablished = false;

            using var bus = ConnectionHelper.GetRMQConnection();
            while (!connectionEstablished)
            {
                var subscriptionResult = bus.PubSub
                    .SubscribeWithTracingAsync<PlayerMovedEvent>("RPS", e =>
                    {
                        var propagator = new TraceContextPropagator();
                        
                        var finishedEvent = _game.ReceivePlayerEvent(e);
                        if (finishedEvent != null)
                        {
                            bus.PubSub.PublishWithTracingAsync(finishedEvent);
                        }
                    })
                    .AsTask();

                await subscriptionResult.WaitAsync(CancellationToken.None);
                connectionEstablished = subscriptionResult.Status == TaskStatus.RanToCompletion;
                if (!connectionEstablished) Thread.Sleep(1000);
            }

            await bus.PubSub.PublishWithTracingAsync(_game.Start());

            while (true) Thread.Sleep(5000);
        }
}