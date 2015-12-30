using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.Actors;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.ActorSystems
{
    public class RemotableActorSystemHost : ActorSystemHostBase
    {
        private readonly Func<IEnumerable<string>> _getOtherActors;
        private int _port;
        private IActorRef _localPublisher;

        public RemotableActorSystemHost(int port, Func<IEnumerable<string>> getOtherActors)
        {
            _port = port;
            _getOtherActors = getOtherActors;
        }

        public RemotableActorSystemHost(int port, IEnumerable<string> otherActors) 
            : this(port, otherActors.ToArray)
        {
        }

        protected override void AddConfigurationSettings(IDictionary<string, string> configEntries)
        {
            configEntries["akka.actor.provider"] = "\"Akka.Remote.RemoteActorRefProvider, Akka.Remote\"";
            configEntries["akka.remote.helios.tcp.port"] = _port.ToString();
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

            foreach (var actor in _getOtherActors())
            {
                ForwardingActor.Tell(new ActorAssociation(actor));
            }
        }

        protected IActorRef ForwardingActor { get; private set; }
    }
}