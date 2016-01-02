using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        }

        protected override void InstallActors(ActorSystem actorSystem)
        {
            ForwardingActor = CreateForwardingActor(actorSystem);
            _localPublisher = actorSystem.ActorOf<LocalEventStreamPublisher>("local-publisher");

            foreach (var actor in _getOtherActors())
            {
                ForwardingActor.Tell(new ActorAssociation(actor));
            }
        }

        protected virtual IActorRef CreateForwardingActor(ActorSystem actorSystem)
        {
            return actorSystem.ActorOf<ForwardingActor>("forwarder");
        }

        protected IActorRef ForwardingActor { get; private set; }

        protected virtual string GetAddress(ActorSystem actorSystem)
        {
            var systemName = actorSystem.Name;
            var currentIpAddress = HostAddressHelpers.GetLocalIPAddress();
            var portNumber = _port;
            return $"akka.tcp://{systemName}@{currentIpAddress}:{portNumber}";
        }

        protected IActorRef ForwardEventStreamMessages<TMessage>(ActorSystem actorSystem, string targetChannelName,
            Func<TMessage, bool> messageFilter = null)
        {
            return actorSystem.ForwardEventStreamMessages(ForwardingActor,
                msg => targetChannelName, GetAddress(actorSystem), messageFilter);
        }

        protected void ForwardMessageTo<TMessage>(ActorSystem sourceActorSystem, string remoteActorSystemAddress, TMessage message, string channel)
        {
            var targetAddress = $"{remoteActorSystemAddress}/user/local-publisher";
            var packagedMessage = new Forward<TMessage>(message, channel, GetAddress(sourceActorSystem), typeof(TMessage));
            
            if(!sourceActorSystem.ActorSelection(targetAddress).CanBeResolved(TimeSpan.FromSeconds(10)))
                throw new InvalidOperationException($"Unable to forward remote message to address '{targetAddress}'");

            packagedMessage.TellFrom(sourceActorSystem, targetAddress);
        }
    }
}