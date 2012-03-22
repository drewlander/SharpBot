/**
 * $Id$
 * $Revision$
 * $Author$
 * $Date$
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 * This is a simple test client for the library.
 *
 * Copyright (c) 2003-2004 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 * 
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SharpBot;
using Meebey.SmartIrc4net;


// This is an VERY basic example how your IRC application could be written
// its mainly for showing how to use the API, this program just connects sends
// a few message to a channel and waits for commands on the console
// (raw RFC commands though! it's later explained).
// There are also a few commands the IRC bot/client allows via private message.
public class Test
{
    // make an instance of the high-level API
    public static IrcClient Client = Connect.Client;

    // this method we will use to analyse queries (also known as private messages)
    public static void OnQueryMessage(object sender, IrcEventArgs e)
    {
        switch (e.Data.MessageArray[0]) {
            // debug stuff
            case "dump_channel":
                string requested_channel = e.Data.MessageArray[1];
                // getting the channel (via channel sync feature)
                Channel channel = Client.GetChannel(requested_channel);
                
                // here we send messages
                Client.SendMessage(SendType.Message, e.Data.Nick, "<channel '"+requested_channel+"'>");
                
                Client.SendMessage(SendType.Message, e.Data.Nick, "Name: '"+channel.Name+"'");
                Client.SendMessage(SendType.Message, e.Data.Nick, "Topic: '"+channel.Topic+"'");
                Client.SendMessage(SendType.Message, e.Data.Nick, "Mode: '"+channel.Mode+"'");
                Client.SendMessage(SendType.Message, e.Data.Nick, "Key: '"+channel.Key+"'");
                Client.SendMessage(SendType.Message, e.Data.Nick, "UserLimit: '"+channel.UserLimit+"'");
                
                // here we go through all users of the channel and show their
                // hashtable key and nickname 
                string nickname_list = "";
                nickname_list += "Users: ";
                foreach (DictionaryEntry de in channel.Users) {
                    string      key         = (string)de.Key;
                    ChannelUser channeluser = (ChannelUser)de.Value;
                    nickname_list += "(";
                    if (channeluser.IsOp) {
                        nickname_list += "@";
                    }
                    if (channeluser.IsVoice) {
                        nickname_list += "+";
                    }
                    nickname_list += ")"+key+" => "+channeluser.Nick+", ";
                }
                Client.SendMessage(SendType.Message, e.Data.Nick, nickname_list);

                Client.SendMessage(SendType.Message, e.Data.Nick, "</channel>");
            break;
            case "gc":
                GC.Collect();
            break;
            // typical commands
            case "join":
                Client.RfcJoin(e.Data.MessageArray[1]);
            break;
            case "part":
                Client.RfcPart(e.Data.MessageArray[1]);
            break;
            case "die":
                Exit();
            break;
        }
    }

    // this method handles when we receive "ERROR" from the IRC server
    public static void OnError(object sender, ErrorEventArgs e)
    {
        System.Console.WriteLine("Error: "+e.ErrorMessage);
        Exit();
    }
    
    // this method will get all IRC messages
    public static void OnRawMessage(object sender, IrcEventArgs e)
    {
        System.Console.WriteLine("Received: "+e.Data.RawMessage);
    }
    
    public static void Main(string[] args)
    {
        Thread.CurrentThread.Name = "Main";
        var connect = new Connect();
		IHandleChannelMessages chanmessage = new ChannelMessageHandler(Connect.Client);
		connect.RegisterOnChannelMessage(chanmessage);
//        Client.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
//        Client.OnError += new ErrorEventHandler(OnError);
//        Client.OnRawMessage += new IrcEventHandler(OnRawMessage);
		

		connect.ServerName="new.drewstud.com";
		connect.Port=6667;
		connect.DefaultChannel="#37tech";
		connect.Init();
    }

    public static void Exit()
    {
        // we are done, lets exit...
        System.Console.WriteLine("Exiting...");
        System.Environment.Exit(0);
    }
}