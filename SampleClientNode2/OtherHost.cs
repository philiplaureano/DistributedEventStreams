using System.Collections.Generic;
using Akka.Actor;
using DistributedEventStream.Core.ActorSystems;
using SampleClientNode;

namespace SampleClientNode2
{
    public class OtherHost : RemotableActorSystemHost
    {
        private IActorRef _greetActor;
        private IActorRef _redirector;

        public OtherHost(int port, IEnumerable<string> otherActors) : base(port, otherActors)
        {
        }

        protected override void InstallActors(ActorSystem actorSystem)
        {
            base.InstallActors(actorSystem);

            _greetActor = actorSystem.ActorOf<GreetActor>();
            actorSystem.RedirectForwardedMessagesTo<GreetMessage>(_greetActor);
        }
    }
}