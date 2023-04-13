﻿using System.Diagnostics;
using EasyNetQ;
using Events;
using Helpers;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PubSubExtensions = Helpers.PubSubExtensions;

namespace CopyPlayerService;

public static class Program
{
    private static readonly IPlayer Player = new CopyPlayer();

    public static async Task Main()
    {
        var connectionEstablished = false;

            while (!connectionEstablished)
            {
                var bus = ConnectionHelper.GetRMQConnection();
                var subscriptionResult = bus.PubSub.SubscribeWithTracingAsync<GameStartedEvent>("RPS_Copy", e =>
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