using System;
using Akka.Actor;
using DistributedEventStream.Core.Actors;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.ActorSystems
{
    public static class ActorSystemExtensions
    {
        public static IActorRef RedirectForwardedMessagesTo<TMessage>(this ActorSystem actorSystem, IActorRef targetActor, Func<Forward<TMessage>, bool> messageFilter = null)
        {
            var redirector = actorSystem.ActorOf(Props.Create(() => 
                new ForwardedMessageRedirector<TMessage>(targetActor, messageFilter)));

            actorSystem.EventStream.Subscribe(redirector, typeof(Forward<TMessage>));

            return redirector;
        }
    }
}