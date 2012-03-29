using System;
using SharpBotClient;
using Meebey.SmartIrc4net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Data.Linq;

namespace WordScramblerBot
{
	public class WordScrambler : IHandleChannelMessages
	{
		private IrcClient client;
		private bool isRunning; 
		private int previousGuesses;
		private IDictionary<string,string> regexDictionary;
		private Regex regex;
		private IrcEventArgs eargs;
		private IList<string> list {get;set;}
		private string currentWord{get;set;}
		public WordScrambler (IrcClient client)
		{
			this.client = client;
			this.list=ReadFile();
		}
		public void HandleMessage(object sender, IrcEventArgs e)
		{
			this.eargs = e;
			//client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message From The dll! "+e.Data.Message);
			//client.SendMessage(SendType.Message, e.Data.Channel,"Handled your message From The dll agaaa! "+e.Data.Message);
			initRegexDict();
			this.ExecuteMethodFromRegex(eargs);
			
		}
		public void ExecuteMethodFromRegex(IrcEventArgs e)
		{
			if(isRunning==true)
			{
				SendUnscramble(e);
			}
			foreach(KeyValuePair<string,string> pair in regexDictionary)
			{
				regex = new Regex(pair.Key);
				if(regex.IsMatch(e.Data.Message))
				{
					string methodName = pair.Value;

						//Get the method information using the method info class
						MethodInfo mi = this.GetType().GetMethod(methodName);
						object[] objargs = new object[1];
					    objargs[0]=e;
//						//Invoke the method
//						// (null- no paramyeseter for the method call
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
		
		public void unscramble(IrcEventArgs e)
		{
			if(this.isRunning==false)
			{
				isRunning=true;
				previousGuesses=0;
				var x = new Random();
				currentWord = list[x.Next(0,this.list.Count)];
				client.SendMessage(SendType.Message,e.Data.Channel,"Unscramble This word: " + RandomizeWord(currentWord));
			}
			else if(this.isRunning==true)
			{
				if(e.Data.Message.Contains("Igiveup"))
				{
					this.isRunning=false;
					client.SendMessage(SendType.Message,e.Data.Channel,"The correct answer is: " + currentWord);
				}
			}
		}
		
		public void SendUnscramble(IrcEventArgs e)
		{

				if(string.Compare(e.Data.Message,currentWord)==0)
				{
					client.SendMessage(SendType.Message,e.Data.Channel,"You got it right! " + e.Data.From);
					isRunning=false;
				}
				else
				{
//					client.SendMessage(SendType.Message,e.Data.Channel,"Incorrect sir "+e.Data.From);
//					previousGuesses++;
				}
			
		}
		private void initRegexDict()
		{
			regexDictionary = new Dictionary<string,string>();
			regexDictionary.Add(@"!help","help");
			regexDictionary.Add(@"!word","unscramble");
		}
		public IList<string> ReadFile()
		{
			string line=null;
			this.list = new List<string>();
			StreamReader reader = new StreamReader("english.1");
			while((line = reader.ReadLine())!= null)
			{
				
				list.Add(line);
			}
			return list;
		}
		public string RandomizeWord(string origword)
		{
			char[] word = origword.ToCharArray();
			Random x = new Random();
			for(int i=0;i<origword.Length;i++)
			{
				int first = x.Next(0,origword.Length-1);
				int second = x.Next(0,origword.Length-1);
				char temp = word[first];
				word[first]=word[second];
				word[second]=temp;
				
			}
			return new string(word);
		}
	}
}

