using System.Diagnostics;
using EasyNetQ;
using Events;
using Helpers;
using Monolith;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PubSubExtensions = Helpers.PubSubExtensions;

namespace RandomPlayerService;

public static class Program
{
    private static readonly IPlayer Player = new RandomPlayer();

    public static async Task Main()
    {
        using (var activity = Monitoring.ActivitySource.StartActivity())
        {
            var connectionEstablished = false;

            while (!connectionEstablished)
            {
                var bus = ConnectionHelper.GetRMQConnection();
                var subscriptionResult = bus.PubSub.SubscribeWithTracingAsync<GameStartedEvent>(
                    "RPS_Random", e =>
                    {

                        var moveEvent = Player.MakeMove(e);
                    
                    bus.PubSub.PublishWithTracingAsync(moveEvent);
                }).AsTask();

                await subscriptionResult.WaitAsync(CancellationToken.None);
                connectionEstablished = subscriptionResult.Status == TaskStatus.RanToCompletion;
                if (!connectionEstablished) Thread.Sleep(1000);
            }

            while (true) Thread.Sleep(5000);
        }
    }
}