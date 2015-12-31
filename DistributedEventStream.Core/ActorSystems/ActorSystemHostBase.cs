using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Lambdas;

namespace DistributedEventStream.Core.ActorSystems
{
    public abstract class ActorSystemHostBase : IActorSystemHost
    {
        public void Run(string systemName)
        {
            var config = new Dictionary<string, string>();

            AddConfigurationSettings(config);

            Action<ActorSystem> installActors = actorSystem =>
            {
                InstallActors(actorSystem);
                actorSystem.AwaitTermination();
            };

            var host = ActorSystemHelpers.CreateHost(installActors, config);
            host.Run(systemName);
        }

        protected virtual void InstallActors(ActorSystem actorSystem)
        {
        }
        protected virtual void AddConfigurationSettings(IDictionary<string, string> configEntries)
        {
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
    }
}