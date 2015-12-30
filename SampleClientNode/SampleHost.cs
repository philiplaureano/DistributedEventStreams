using System;
using System.Collections.Generic;
using Akka.Actor;
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

            var messageToSend = new Forward<GreetMessage>(new GreetMessage("Hello World!"), "TestChannel", null, typeof(GreetMessage));
                        
            actorSystem.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1), ForwardingActor, messageToSend, null);
        }
    }
}