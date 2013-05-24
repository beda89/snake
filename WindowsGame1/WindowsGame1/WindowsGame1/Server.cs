using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Server
    {
        private int port;
        private List<TcpClient> currentClients;
        private TcpListener tcpListener;
        private InGameState inGameState = InGameState.STARTING;
        private int snakeNumber = 0;
   
        public Server(int port)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            currentClients = new List<TcpClient>();
        }

        public void start(){
            tcpListener.Start();


            while (true)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    currentClients.Add(tcpClient);
                }
                catch (SocketException)
                {
                    return;
                }
            }
        }

        public void stop()
        {
            tcpListener.Stop();

            foreach (TcpClient tcpClient in currentClients)
            {
                //TODO is null check needed?
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }

            currentClients.Clear();


        }

        //TODO: not thread safe
        public List<TcpClient> getCurrentClients()
        {
            return currentClients;
        }

        public void sendStartSignal()
        {
            //player number which gets assigned to every client, starts with 1 because server is 0
            int index=1;

            foreach (TcpClient tcpClient in currentClients)
            {
                StreamWriter writer=new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!start "+index+" !");
                writer.Flush();
            }

            inGameState = InGameState.RUNNING;
        }


        public void sendCurrentPositions()
        {
            foreach (TcpClient tcpClient in currentClients)
            {
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!game start");
                writer.Flush();
            }

            inGameState = InGameState.RUNNING;

        }

        public InGameState getInGameState()
        {
            return inGameState;
        }

    }
}
