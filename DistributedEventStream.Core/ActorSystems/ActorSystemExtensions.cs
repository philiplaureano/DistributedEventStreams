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
            Func<TMessage, string> getChannelName,
            string actorSystemAddress = null,
            Func<TMessage, bool> eventFilter = null)
        {
            return actorSystem.ForwardEventStreamMessages<TMessage>(targetActor,
                msg => new Forward<TMessage>(msg, getChannelName(msg), actorSystemAddress, typeof(TMessage)));
        }

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
        public static IActorRef RedirectForwardedMessagesTo<TMessage>(this ActorSystem actorSystem,
            IActorRef targetActor, Func<IForwardMessage, bool> messageFilter = null)
        {
            // Match the message type
            Func<IForwardMessage, bool> defaultFilter = message => typeof(TMessage)
                .IsAssignableFrom(message?.MessageType);

            var filter = defaultFilter;
            if (messageFilter != null)
                filter = message => defaultFilter(message) && messageFilter(message);

            return RedirectForwardedMessagesTo(actorSystem, targetActor, filter);
        }

        public static IActorRef RedirectForwardedMessagesTo(this ActorSystem actorSystem, IActorRef targetActor,
            Func<IForwardMessage, bool> messageFilter)
        {
            var redirector = actorSystem.ActorOf(Props.Create(() =>
                new ForwardedMessageRedirector(targetActor, messageFilter)));

            actorSystem.EventStream.Subscribe(redirector, typeof(IForwardMessage));

            return redirector;
        }
    }
}