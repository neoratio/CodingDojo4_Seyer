using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communication
{
    class ClientHandler
    {
        private Action<string, Socket> action;
        private byte[] buffer = new byte[512];
        private Thread ClientThread;
        const string endMessage = "@exit";

        public string Name { get; set; }
        public Socket ClientSocket { get; set;}

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            this.ClientSocket = socket;
            this.action = action;
            ClientThread = new Thread(Receive);
            ClientThread.Start();
        }

        private void Receive()
        {
            string message = "";
            while (!message.Contains(endMessage))
            {
                int length = ClientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);
                if (Name == null && message.Contains(":"))
                {
                    Name = message.Split(':')[0];
                }
                action(message, ClientSocket);
            }
            Close();
        }

        public void Close()
        {
            Send(endMessage); 
            ClientSocket.Close(1);
            ClientThread.Abort();
        }

        public void Send(string message)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(message));
        }
    }
}
