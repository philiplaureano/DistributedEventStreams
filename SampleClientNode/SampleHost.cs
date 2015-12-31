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
            ForwardEventStreamMessages<GreetMessage>(actorSystem, "TestChannel");

            // Send the same message for 5 minutes
            var startTime = DateTime.Now;
            var endTime = startTime.AddMinutes(5);
            var i = 0;
            while(DateTime.Now < endTime)
            {
                actorSystem.EventStream.Publish(new GreetMessage($"Message #{i++}: Hello World!"));
            }
        }
    }
}