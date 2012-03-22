using System;
using SharpBotClient;
using Meebey.SmartIrc4net;
namespace TriviaBot
{
	public class Trivia : IHandleChannelMessages
	{
		private IrcClient client;
		public Trivia (IrcClient client)
		{
			this.client = client;
		}
		public void HandleMessage(object sender, IrcEventArgs e)
		{
			client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message From The dll! "+e.Data.Message);
		}
		
	}
}

