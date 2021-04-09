### Background Info

The project is originated from the repository - https://github.com/pgstath/Sharp.Xmpp

The problem with the above project is that it is based on .Net framework, which is not suitable for it being used in Xamarin.

Actually, in these days, there is more need to build a chat app in mobile, thus, the repository port the codes to .Net standard.

This repository contains an easy-to-use and well-documented .NET standard assembly for communicating with
an XMPP server. It supports basic Instant Messaging and Presence funtionality as well as a variety
of XMPP extensions.


### NuGet

Install-Package Portable.BouncyCastle -Version 1.8.10

Install-Package ARSoft.Tools.NetStandard.DXSdata -Version 1.0.0

### Supported XMPP Features

The library fully implements the [XMPP Core](http://xmpp.org/rfcs/rfc3920.html) and 
[XMPP IM](http://xmpp.org/rfcs/rfc3921.html) specifications and thusly provides the basic XMPP instant
messaging (IM) and presence functionality. In addition, the library offers support for most of the
optional procotol extensions. More specifically, the following features are supported:

+ SASL Authentication (PLAIN, DIGEST-MD5, SCRAM-SHA-1, SCRAM-SHA-256, SCRAM-SHA-512)
+ User Avatars
+ SOCKS5 and In-Band File-Transfer
+ In-Band Registration
+ User Mood
+ User Tune
+ User Activity
+ Simplified Blocking
+ API designed to be very easy to use
+ Well documented with lots of example code
+ Free to use in commercial and personal projects (MIT License)


### Usage & Examples

To use the library add the XMPPClient.dll assembly to your project references in Visual Studio. Here's
a simple example that initializes a new instance of the XmppClient class and connects to an XMPP
server:

	using System;
	using Sharp.Xmpp;
	using Sharp.Xmpp.Client;

	namespace Test {
		class Program {
			static void Main(string[] args) {

            /* connect on port 5222 using TLS/SSL if available */
            using (var client = new XmppClient("localhost", "user1", "12345678"))
            {
                // enabled debug stanza output.
                client.DebugStanzas = true;

                // connect to xmpp server.
                client.Connect();

                // list all the supported features 
                Console.WriteLine("user1's XMPP client supports: ");
                foreach (var feat in client.GetFeatures(client.Jid))
                { 
                    Console.WriteLine(" - " + feat);
                }

                // output full jid.
                Console.WriteLine("Connected as " + client.Jid);
            }
		}
	}

### Credits

The Shar.Xmpp.Client library is copyright © 2021 Peter Liang

The Sharp.Xmpp library is copyright © 2015 Panagiotis Georgiou Stathopoulos.

The initial S22.Xmpp library is copyright © 2013-2014 Torben Könke.


### License

This library is released under the [MIT license](https://github.com/liangdefeng/Sharp.Xmpp.Client/blob/master/XMPPClient/License.md).


### Bug reports

Please send your bug reports to [defeng.liang.cn@gmail.com](mailto:defeng.liang.cn@gmail.com) or create a new
issue on the GitHub project homepage.
