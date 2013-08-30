using System;
using Meebey.SmartIrc4net;

namespace SharpBot
{
	public interface IHandleChannelMessages
	{
		 void HandleMessage(object sender, IrcEventArgs e);
	}
}

