using System;
using Meebey.SmartIrc4net;
namespace SharpBotClient
{
	public static class SharpBotClient
	{
		private static IrcClient client;
		public static IrcClient Client
		{
			get
			{
				if(client==null)
				{
					client = new IrcClient();
				}
				return client;
			}
			set
			{
				client = value;
			}
		}
	}
}

