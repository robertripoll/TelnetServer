using System;
using System.Net;

namespace TelnetServer
{
    class Program
    {
        private static Server s;

        static void Main(string[] args)
        {
            s = new Server(IPAddress.Any);
            s.ClientConnected       += clientConnected;
            s.ClientDisconnected    += clientDisconnected;
            s.ConnectionBlocked     += connectionBlocked;
            s.MessageReceived       += messageReceived;
            s.start();

            Console.WriteLine("SERVER STARTED: " + DateTime.Now);

            do
            {
                ; // nothing really
            } while (Console.ReadKey(true).KeyChar != 'q');

            s.stop();
        }

        private static void clientConnected(Client c)
        {
            Console.WriteLine("CONNECTED: " + c);

            s.sendMessageToClient(c, "Telnet Server\r\nLogin: ");
        }

        private static void clientDisconnected(Client c)
        {
            Console.WriteLine("DISCONNECTED: " + c);
        }

        private static void connectionBlocked(IPEndPoint ep)
        {
            Console.WriteLine(string.Format("BLOCKED: {0}:{1} at {2}", ep.Address, ep.Port, DateTime.Now));
        }

        private static void messageReceived(Client c, string message)
        {
            Console.WriteLine("MESSAGE: " + message);

            if (message != "kickmyass")
            {
                EClientStatus status = c.getCurrentStatus();

                if (status == EClientStatus.Guest)
                {
                    if (message == "root")
                    {
                        s.sendMessageToClient(c, "\r\nPassword: ");
                        c.setStatus(EClientStatus.Authenticating);
                    }
                }

                else if (status == EClientStatus.Authenticating)
                {
                    if (message == "r00t")
                    {
                        s.clearClientScreen(c);
                        s.sendMessageToClient(c, "Successfully authenticated.\r\n > ");
                        c.setStatus(EClientStatus.LoggedIn);
                    }
                }

                else
                    s.sendMessageToClient(c, "\r\n > ");
            }

            else
                s.kickClient(c);
        }
    }
}