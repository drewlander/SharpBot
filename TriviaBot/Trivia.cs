using System;
using SharpBotClient;
using Meebey.SmartIrc4net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

namespace TriviaBot
{
	public class Trivia : IHandleChannelMessages
	{
		private IrcClient client;
		//private bool isRunning; 
		private IDictionary<string,string> regexDictionary;
		private Regex regex;
		private IrcEventArgs eargs;
		public Trivia (IrcClient client)
		{
			this.client = client;
		}
		public void HandleMessage(object sender, IrcEventArgs e)
		{
			this.eargs = e;
//			client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message From The dll! "+e.Data.Message);
//			client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message From The dll agaaa! "+e.Data.Message);
			initRegexDict();
			this.ExecuteMethodFromRegex(eargs);
			
		}
		public void ExecuteMethodFromRegex(IrcEventArgs e)
		{
			foreach(KeyValuePair<string,string> pair in regexDictionary)
			{
				regex = new Regex(pair.Key);
				if(regex.IsMatch(e.Data.Message))
				{
					string methodName = "help";

						//Get the method information using the method info class
						MethodInfo mi = this.GetType().GetMethod(methodName);
						object[] objargs = new object[1];
					    objargs[0]=e;
//						//Invoke the method
//						// (null- no parameter for the method call
//						// or you can pass the array of parameters...)
					if(mi==null)
					{
						//client.SendMessage(SendType.Message, e.Data.Channel,"mi is null! "+e.Data.Message);
						SharpBotClient.SharpBotClient.SendChannelMessag(e.Data.Channel,"handled from client");
					}
					else
					{
						mi.Invoke(this,objargs);
					}
				
				}
			}
		}
		
		public void help(IrcEventArgs e)
		{
			client.SendMessage(SendType.Message, e.Data.Channel,"No help for you!  "+e.Data.From);
		}
		private void initRegexDict()
		{
			regexDictionary = new Dictionary<string,string>();
			regexDictionary.Add(@"!help","help");
		}
		
	}
}

