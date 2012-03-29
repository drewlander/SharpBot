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
using System.Linq;
using System.Reflection;
using SharpBotClient;
using System.IO;
using System.Diagnostics;

// This is an VERY basic example how your IRC application could be written
// its mainly for showing how to use the API, this program just connects sends
// a few message to a channel and waits for commands on the console
// (raw RFC commands though! it's later explained).
// There are also a few commands the IRC bot/client allows via private message.
namespace SharpBot
{
	public class Test
	{
	    // make an instance of the high-level API
	    public static IrcClient Client = Connect.Client;

	    // this method handles when we receive "ERROR" from the IRC server
	    public static void OnError(object sender,Meebey.SmartIrc4net.ErrorEventArgs e)
	    {
	        System.Console.WriteLine("Error: "+e.ErrorMessage);
	        Exit();
	    }
	    
	    public static void Main(string[] args)
	    {
	        Thread.CurrentThread.Name = "Main";
			
	        var connect = new Connect();
	        Client.OnError += new Meebey.SmartIrc4net.ErrorEventHandler(OnError);

			RegisterAllHandleChannelMessage(connect);
			string[] serverlist;
			serverlist = new string[] {"new.drewstud.com"};
			connect.ServerName= serverlist;
			connect.Port=6667;
			
			connect.DefaultChannel="#37tech";
			connect.Init();
	    }
		
		public static void RegisterAllHandleChannelMessage(Connect connect)
		{
			string exeName = Directory.GetCurrentDirectory();

			string folder = Path.Combine(Path.GetDirectoryName(exeName), "Plugins");
			object[] objargs = new object[1];
			objargs[0]=Connect.Client;
			List<IHandleChannelMessages> list = GetPlugins<IHandleChannelMessages>(folder);
			foreach (IHandleChannelMessages handler in list)
			{
					connect.RegisterOnChannelMessage(handler);
			}
		}
		
		public static  List<T> GetPlugins<T>(string folder)
		{
		    string[] files = Directory.GetFiles(folder, "*.dll");
		
		    List<T> tList = new List<T>();
		
		    Debug.Assert(typeof(T).IsInterface);
		
		    foreach (string file in files) 
		    {
		        try 
		        {
		            Assembly assembly = Assembly.LoadFile(file);
		            foreach (Type type in assembly.GetTypes()) 
		            {
		                if (!type.IsClass || type.IsNotPublic) continue;
		                Type[] interfaces = type.GetInterfaces();
		                if (((IList)interfaces).Contains(typeof(T))) 
		                {
							object[] objargs = new object[1];
							objargs[0]=Connect.Client;
		                    object obj = Activator.CreateInstance(type,objargs);
		                    T t = (T)obj;
		                    tList.Add(t);
		                }
		            } 
		        }
		        catch (Exception ex)
		        {
		            Console.WriteLine(ex);
		
		        }
		    }
			 return tList;
		}
	
	    public static void Exit()
	    {
	        // we are done, lets exit...
	        System.Console.WriteLine("Exiting...");
	        System.Environment.Exit(0);
	    }
	}
}