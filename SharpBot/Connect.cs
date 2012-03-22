using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SharpBotClient;

using Meebey.SmartIrc4net;
namespace SharpBot
{
	public class Connect
	{
		private static IrcClient client;
		public delegate void OnChannelMessage(object sender, IrcEventArgs e);
		private IList<IHandleChannelMessages> onChannelMessagesList;
		public static IrcClient Client
		{
			get
			{
				if(client==null)
				{
					client = SharpBotClient.SharpBotClient.Client;
					
				}
				return client;
			}
			set
			{
				client = value;
			}
		}
		
		public string ServerName {get;set;}
		public int Port {get;set;}
		public string DefaultChannel {get;set;}
		
		public Connect ()
		{
			Client.Encoding = System.Text.Encoding.UTF8;
	        
	        // wait time between messages, we can set this lower on own irc servers
	        Client.SendDelay = 200;
	        
	        // we use channel sync, means we can use irc.GetChannel() and so on
	        Client.ActiveChannelSyncing = true;
			onChannelMessagesList = new List<IHandleChannelMessages>();
		}
		
			
		public void Init()
		{
			try
			{
				Client.OnChannelMessage += new IrcEventHandler(ChannelMessageReceived);
				Client.Connect(ServerName,Port);
				connect();
			}
		    catch (ConnectionException e) 
		    {  
            // something went wrong, the reason will be shown
            System.Console.WriteLine("couldn't connect! Reason: "+e.Message);
            Exit();
			}
		}
		public void RegisterOnChannelMessage(IHandleChannelMessages obj)
		{
			onChannelMessagesList.Add(obj);
		}
		private void connect()
		{
			try {
	            // here we logon and register our nickname and so on 
	            Client.Login("SmartIRC", "SmartIrc4net Test Bot");
	            // join the channel
	            Client.RfcJoin(DefaultChannel);
	            
	            for (int i = 0; i < 3; i++) {
	                // here we send just 3 different types of messages, 3 times for
	                // testing the delay and flood protection (messagebuffer work)
//	                Client.SendMessage(SendType.Message, DefaultChannel, "test message ("+i.ToString()+")");
//	                Client.SendMessage(SendType.Action, DefaultChannel, "thinks this is cool ("+i.ToString()+")");
//	                Client.SendMessage(SendType.Notice, DefaultChannel, "SmartIrc4net rocks ("+i.ToString()+")");
            	}
            
	            // spawn a new thread to read the stdin of the console, this we use
	            // for reading IRC commands from the keyboard while the IRC connection
	            // stays in its own thread
	            new Thread(new ThreadStart(ReadCommands)).Start();
	            
	            // here we tell the IRC API to go into a receive mode, all events
	            // will be triggered by _this_ thread (main thread in this case)
	            // Listen() blocks by default, you can also use ListenOnce() if you
	            // need that does one IRC operation and then returns, so you need then 
	            // an own loop 
	            Client.Listen();
	            
	            // when Listen() returns our IRC session is over, to be sure we call
	            // disconnect manually
            Client.Disconnect();
	        } 
			catch (ConnectionException)
			{
	            // this exception is handled because Disconnect() can throw a not
	            // connected exception
	            Exit();
	        } 
			catch (Exception e) 
			{
	            // this should not happen by just in case we handle it nicely
	            System.Console.WriteLine("Error occurred! Message: "+e.Message);
	            System.Console.WriteLine("Exception: "+e.StackTrace);
	            Exit();
	        }
		}
		
		private void ChannelMessageReceived(object sender, IrcEventArgs e)
		{
			foreach(var call in onChannelMessagesList)
			{
				call.HandleMessage(sender,e);
			}
		}
		
		public static void ReadCommands()
	    {
	        // here we read the commands from the stdin and send it to the IRC API
	        // WARNING, it uses WriteLine() means you need to enter RFC commands
	        // like "JOIN #test" and then "PRIVMSG #test :hello to you"
	        while (true) {
	            string cmd = System.Console.ReadLine();
	            if (cmd.StartsWith("/list")) {
	                int pos = cmd.IndexOf(" ");
	                string channel = null;
	                if (pos != -1) {
	                    channel = cmd.Substring(pos + 1);
	                }
	                
	                IList<ChannelInfo> channelInfos = Client.GetChannelList(channel);
	                Console.WriteLine("channel count: {0}", channelInfos.Count);
	                foreach (ChannelInfo channelInfo in channelInfos) {
	                    Console.WriteLine("channel: {0} user count: {1} topic: {2}",
	                                      channelInfo.Channel,
	                                      channelInfo.UserCount,
	                                      channelInfo.Topic);
	                }
	            } else {
	                Client.WriteLine(cmd);
	            }
	        }
	    }
	    
	    public static void Exit()
	    {
	        // we are done, lets exit...
	        System.Console.WriteLine("Exiting...");
	        System.Environment.Exit(0);
	    }
	}
}

