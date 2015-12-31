using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.Event;
using Akka.Remote.Transport;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ForwardingActor : ForwardingActorBase
    {
        private readonly Dictionary<string, IActorRef> _actorAddresses = new Dictionary<string, IActorRef>();
        private readonly List<IActorRef> _heartbeatMonitors = new List<IActorRef>();
        public ForwardingActor()
        {
            Receive<IActorAssociation>(message =>
            {
                if (_actorAddresses.ContainsKey(message.ActorAddress) || string.IsNullOrEmpty(message.ActorAddress))
                    return;

                var logger = Context.GetLogger();

                // Add the address only if it can be resolved from the start
                if (!Context.ActorSelection(message.ActorAddress).CanResolve(TimeSpan.FromSeconds(20)))
                {
                    logger.Error($"Unable to resolve forwarding actor address '{message.ActorAddress}'");
                    return;
                }

                logger.Warning($"Actor associated: {message.ActorAddress}");
                _actorAddresses[message.ActorAddress] =
                    Context.ActorOf(Props.Create(() => new ActorAdapter(message.ActorAddress)));

                // Monitor the address for a valid connection state
                var refreshRate = TimeSpan.FromSeconds(2);
                var timeoutPeriod = TimeSpan.FromSeconds(5);
                var monitor =
                    Context.ActorOf(
                        Props.Create(
                            () => new ActorHeartbeatResolver(message.ActorAddress, Self, refreshRate, timeoutPeriod)));

                _heartbeatMonitors.Add(monitor);
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