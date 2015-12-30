using System;
using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.Actors;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.ActorSystems
{
    public static class ActorSystemExtensions
    {
        public static IActorRef ForwardEventStreamMessages<TMessage>(this ActorSystem actorSystem,
            IActorRef targetActor,
            Func<TMessage, Forward<TMessage>> eventPackager,
            Func<TMessage, bool> eventFilter = null)
        {
            var eventForwarder =
                actorSystem.ActorOf(
                    Props.Create(() =>
                    new EventForwarder<TMessage>(targetActor, eventPackager, eventFilter)));

            actorSystem.EventStream.Subscribe<TMessage>(eventForwarder);

            return eventForwarder;
        }
        public static IActorRef RedirectForwardedMessagesTo<TMessage>(this ActorSystem actorSystem, IActorRef targetActor, Func<Forward<TMessage>, bool> messageFilter = null)
        {
            var redirector = actorSystem.ActorOf(Props.Create(() => 
                new ForwardedMessageRedirector<TMessage>(targetActor, messageFilter)));

            actorSystem.EventStream.Subscribe(redirector, typeof(Forward<TMessage>));

            return redirector;
        }
    }
}