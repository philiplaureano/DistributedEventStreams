using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.VsTest;
using DistributedEventStream.Core.Messages;
using DistributedEventStream.Routing.Actors;
using DistributedEventStream.Routing.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleClientNode;
using Shouldly;
using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
namespace DistributedEventStream.Routing.Tests
{
    [TestFixture]
    public class RelayActorTests : TestKit
    {
        [Test]
        public void Should_forward_channel_message_to_associated_actor()
        {
            var sentMessages = new Dictionary<string, IForwardMessage>();
            Action<IUntypedActorContext, IForwardMessage, string>
                sendAction = (context, message, address) =>
                {
                    sentMessages[address] = message;
                };

            var fakeAddress = "akka.tcp://MyActorSystem@127.0.0.1:8080/user/fakeactor";
            var channelName = "TestChannel";

            // Link the channel name to the fake address
            var relayActor = ActorOfAsTestActorRef<RelayActor>(Props.Create(() => new RelayActor(sendAction)));
            relayActor.Tell(new AssociatedWithChannel(fakeAddress, channelName));

            // Send a forwarded message that matches the test channel
            relayActor.Tell(new Forward<GreetMessage>(new GreetMessage("Hello, World!"), channelName, string.Empty, typeof(GreetMessage)));
            ExpectNoMsg();

            sentMessages.Count.ShouldBe(1);
            sentMessages.ContainsKey(fakeAddress).ShouldBe(true);

            // Verify the forwarded message
            var sentMessage = sentMessages[fakeAddress];
            sentMessage.Channel.ShouldBe(channelName);
            sentMessage.MessageType.ShouldBe(typeof(GreetMessage));

            // Ensure that the message isn't sent on a different channel
            sentMessages.Clear();
            relayActor.Tell(new Forward<GreetMessage>(new GreetMessage("Hello, World!"), "SomeOtherChannel", string.Empty, typeof(GreetMessage)));
            sentMessages.Count.ShouldBe(0);
        }
    }
}