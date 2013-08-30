using System;
using Meebey.SmartIrc4net;

namespace SharpBotClient
{
	public interface IHandleChannelMessages
	{
		 void HandleMessage(object sender, IrcEventArgs e);
	}
}

