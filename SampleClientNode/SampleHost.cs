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
                msg => new Forward<GreetMessage>(msg, "TestChannel", null, typeof (GreetMessage)));

            ForwardingActor.Tell(new ActorAssociation("akka.tcp://NonExistentSystem@192.168.1.100:1234/user/non-existent-actor"));

            for (var i = 0; i < 100; i++)
            {
                actorSystem.EventStream.Publish(new GreetMessage($"Message #{i}: Hello World!"));
            }
        }
    }
}