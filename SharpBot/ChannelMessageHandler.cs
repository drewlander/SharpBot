using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Meebey.SmartIrc4net;
namespace SharpBot
{
	public class ChannelMessageHandler
	{
		private IrcClient client;
		public ChannelMessageHandler (IrcClient client)
		{
			this.client = client;
		}
		public void HandleMessage(object sender, IrcEventArgs e)
		{
			//client.SendMessage(SendType.Message,"Handled your message {0}",e.Data.Message);
			client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message "+e.Data.Message);
		}
	}
}

