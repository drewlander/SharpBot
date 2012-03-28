using System;
using NUnit;
using NUnit.Core;
using NUnit.Framework;
using WordScramblerBot;
using SharpBotClient;
using Moq;
using Meebey.SmartIrc4net;
using System.Collections.Generic;
namespace WordScamberBot
{
	[TestFixture]
	public class WordScramblerBotTests
	{
		private Mock<IrcClient> client;
		[SetUp]
		public void Setup()
		{
			MockRepository factory = new MockRepository(MockBehavior.Loose);
			client =  factory.Create<IrcClient>();
		}

		[Test]
		public void TestRandomWord()
		{
			var bot = new WordScramblerBot.WordScrambler(SharpBotClient.SharpBotClient.Client);
			bot.RandomizeWord("test");
			Console.WriteLine(bot.RandomizeWord("test"));	
		}
		[Test]
		public void TestReadFile()
		{
			var bot = new WordScramblerBot.WordScrambler(SharpBotClient.SharpBotClient.Client);
			IList<string> list = new List<string>();
			list=bot.ReadFile();
			var x = new Random();
			string currentWord = list[x.Next(0,list.Count)];
			Console.WriteLine(currentWord);
			
		}
	}
}

