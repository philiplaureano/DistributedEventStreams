using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Remote.Transport;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ForwardingActor : ForwardingActorBase
    {
        private readonly Dictionary<string, IActorRef> _actorAddresses = new Dictionary<string, IActorRef>();
        public ForwardingActor()
        {
            Receive<IActorAssociation>(message =>
            {
                if (_actorAddresses.ContainsKey(message.ActorAddress) || string.IsNullOrEmpty(message.ActorAddress))
                    return;

                var logger = Context.GetLogger();
                logger.Warning($"Actor associated: {message.ActorAddress}");
                _actorAddresses[message.ActorAddress] =
                    Context.ActorOf(Props.Create(() => new ActorAdapter(message.ActorAddress)));
            });

            Receive<IActorDissasociation>(message =>
            {
                if (!_actorAddresses.ContainsKey(message.ActorAddress) && !string.IsNullOrEmpty(message.ActorAddress))
                    return;

                var logger = Context.GetLogger();
                logger.Warning($"Actor disassociated: {message.ActorAddress}");

                _actorAddresses.Remove(message.ActorAddress);
            });            
        }

        protected override IEnumerable<IActorRef> GetForwardingActors(IForwardMessage currentMessage)
        {
            return _actorAddresses.Values;
        }
    }
}