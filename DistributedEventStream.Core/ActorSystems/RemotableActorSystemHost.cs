using System;
using System.Collections.Generic;
using Akka.Actor;
using DistributedEventStream.Core.Actors;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.ActorSystems
{
    public class RemotableActorSystemHost : ActorSystemHostBase
    {
        private int _port;
        private readonly IEnumerable<string> _otherActors;
        private IActorRef _localPublisher;
        public RemotableActorSystemHost(int port, IEnumerable<string> otherActors)
        {
            _port = port;
            _otherActors = otherActors;
        }

        protected override void AddConfigurationSettings(IDictionary<string, string> configEntries)
        {
            configEntries["akka.actor.provider"] = "\"Akka.Remote.RemoteActorRefProvider, Akka.Remote\"";
            configEntries["akka.remote.helios.tcp.port"] = HostAddressHelpers.GetNextFreeTcpPort().ToString();
            configEntries["akka.remote.helios.tcp.hostname"] = HostAddressHelpers.GetLocalIPAddress();

            Action<string, string> setKey = (key, value) => { configEntries[key] = value; };

            setKey("akka.stdout-loglevel", "DEBUG");
            setKey("akka.loglevel", "DEBUG");
            setKey("akka.log-config-on-start", "on");


            setKey("akka.actor.debug.receive", "on");
            setKey("akka.actor.debug.autoreceive", "on");
            setKey("akka.actor.debug.lifecycle", "on");
            setKey("akka.actor.debug.event-stream", "on");
            setKey("akka.actor.debug.unhandled", "on");
        }

        protected override void InstallActors(ActorSystem actorSystem)
        {
            ForwardingActor = actorSystem.ActorOf<ForwardingActor>("forwarder");
            _localPublisher = actorSystem.ActorOf<LocalEventStreamPublisher>("local-publisher");

            foreach (var actor in _otherActors)
            {
                ForwardingActor.Tell(new ActorAssociation(actor));
            }
        }

        protected IActorRef ForwardingActor { get; private set; }
    }
}