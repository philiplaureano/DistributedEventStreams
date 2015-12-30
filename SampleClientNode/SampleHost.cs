using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.ActorSystems;
using DistributedEventStream.Core.Messages;

namespace SampleClientNode
{
    public class SampleHost : RemotableActorSystemHost
    {
        public SampleHost(int port, IEnumerable<string> otherActors) : base(port, otherActors)
        {
        }

        protected override void InstallActors(ActorSystem actorSystem)
        {
            base.InstallActors(actorSystem);

            actorSystem.ForwardEventStreamMessages<GreetMessage>(ForwardingActor,
                msg => "TestChannel");

            for (var i = 0; i < 100; i++)
            {
                actorSystem.EventStream.Publish(new GreetMessage($"Message #{i}: Hello World!"));
            }
        }
    }
}