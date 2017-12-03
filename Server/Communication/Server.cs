using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communication
{
    class Server
    {
        Socket SSocket;
        List<ClientHandler> CL = new List<ClientHandler>();
        Action<string> GU;
        Thread AcceptingTask;

        public Server(String IP, int Port, Action<string> GU)
        {
            this.GU = GU;
            SSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
            SSocket.Listen(5);
        }

        public void StartServer()
        {
            AcceptingTask = new Thread(new ThreadStart(Accepting));
            AcceptingTask.IsBackground = true;
            AcceptingTask.Start();
        }

        public void StopServer()
        {
            SSocket.Close();
            AcceptingTask.Abort();
            foreach (var item in CL)
            {
                item.Close();
            }
            CL.Clear();

        }

        private void Accepting()
        {
            while (AcceptingTask.IsAlive)
            {
                try
                {
                    CL.Add(new ClientHandler(SSocket.Accept(), new Action<string, Socket>(NewChatMessage)));
                }
                catch (Exception e)
                {

                }
            }
        }

        private void NewChatMessage(string message, Socket senderSocket)
        {
            GU(message);
            foreach (var item in CL)
            {
                if (item.ClientSocket != senderSocket)
                {
                    item.Send(message);
                }
            }

            string[] msgs = message.Split(':');
            string msg = msgs.Length > 1 ? msgs[1].TrimStart() : null;
            if (msg == "@quit")
            {
                DisconnectClient(msgs[0]);
            }
        }

        public void DisconnectClient(string name)
        {
            foreach (var item in CL)
            {
                if (item.Name.Equals(name))
                {
                    CL.Remove(item);
                    item.Close();
                    break;
                }
            }
        }



    }
}
