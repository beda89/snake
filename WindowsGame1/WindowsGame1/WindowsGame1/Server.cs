using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WindowsGame1
{
    class Server
    {
        private int port;
        private List<TcpClient> currentClients = new List<TcpClient>();
        private TcpListener tcpListener;

        public Server(int port)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void start(){
            tcpListener.Start();

            while (true)
            {
                currentClients.Add(tcpListener.AcceptTcpClient());
            }
        }

        public void stop()
        {
            tcpListener.Stop();
        }

        //TODO: not thread safe
        public List<TcpClient> getCurrentClients()
        {
            return currentClients;
        }

    }
}
