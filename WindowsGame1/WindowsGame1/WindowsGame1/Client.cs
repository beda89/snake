using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WindowsGame1.Exceptions;

namespace WindowsGame1
{
    class Client
    {
        private String ip;
        private int port;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private GameState clientState = GameState.NETWORK_MENU_WAITING_FOR_SERVER;
        private InGameState inGameState = InGameState.STARTING;
        
        //snakeNumber assigned by server, server is always 0
        private int snakeNumber;


        public Client(String ipString, int port)
        {
           this.ip = ipString;
           this.port = port;
        }

        public void start(){

            try
            {
                tcpClient = new TcpClient(ip, port);
                stream = tcpClient.GetStream();

                StreamReader reader = new StreamReader(stream);
                String message= reader.ReadLine();

                processMessage(message);

            }
            catch (SocketException)
            {
                clientState = GameState.CONNECTION_REFUSED;

            }
        } 


        public GameState getClientState()
        {
            return clientState;
        }

        public InGameState getClientInGameState()
        {
            return inGameState;
        }

        public void setClientInGameState(InGameState inGameState)
        {
            this.inGameState = inGameState;
        }

        public void stop()
        {
            if (stream != null)
            {
                stream.Close();
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }

        public int getSnakeNumber()
        {
            return snakeNumber;
        }




        private void processMessage(String message)
        {
            if (message.StartsWith("!start"))
            {
                String[] splittedMessage=message.Split(' ');

                if (splittedMessage.Length != 3)
                {
                    //TODO make custom exception
                    throw new MessageException();
                }

                clientState = GameState.PLAY_CLIENT;
                inGameState = InGameState.STARTING;


                snakeNumber = Convert.ToInt32(splittedMessage[1]);

            }
            else if (message.StartsWith("!game"))
            {



            }
            else if (message.StartsWith("!end"))
            {


            }
        }
    }
}
