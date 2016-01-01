using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Akka.TestKit;
using Akka.TestKit.VsTest;
using DistributedEventStream.Routing.Actors;
using DistributedEventStream.Routing.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace DistributedEventStream.Routing.Tests
{
    [TestClass]
    public class RegistryActorTests : TestKit
    {
        [TestMethod]
        public void Should_be_able_to_associate_actor_address_with_channel()
        {
            var fakeAddress = "akka.tcp://MyActorSystem@127.0.0.1:8080/user/fakeactor";
            var channelName = "TestChannel";

            var message = new AssociateActorWithChannel(fakeAddress, channelName);
            var registryActor = ActorOfAsTestActorRef<RegistryActor>();
            registryActor.Tell(message);

            var response = ExpectMsg<AssociatedWithChannel>();
            response.ActorAddress.ShouldBe(fakeAddress);
            response.ChannelName.ShouldBe(channelName);
        }        

        [TestMethod]
        public void Should_be_able_to_disassociate_actor_address_from_channel()
        {
            var fakeAddress = "akka.tcp://MyActorSystem@127.0.0.1:8080/user/fakeactor";
            var channelName = "TestChannel";

            var message = new AssociateActorWithChannel(fakeAddress, channelName);
            var registryActor = ActorOfAsTestActorRef<RegistryActor>();
            registryActor.Tell(message);

            // Register the actor
            var response = ExpectMsg<AssociatedWithChannel>();
            response.ActorAddress.ShouldBe(fakeAddress);
            response.ChannelName.ShouldBe(channelName);

            // Then unregister it and confirm the dissociation
            registryActor.Tell(new DisassociateActorFromChannel(fakeAddress, channelName));

            var disassociatedMessage = ExpectMsg<DisassociatedFromChannel>();

            disassociatedMessage.ActorAddress.ShouldBe(fakeAddress);
            disassociatedMessage.ChannelName.ShouldBe(channelName);
        }
    }
}