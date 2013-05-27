using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Server
    {
        private int port;
        private List<TcpClient> currentClients;
        private TcpListener tcpListener;
        private InGameState inGameState = InGameState.STARTING;

        public Server(int port)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.currentClients = new List<TcpClient>();
        }

        public void start(){
            tcpListener.Start();

            while (currentClients.Count()<4)
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


        public void sendStartSignal(List<Snake> snakes)
        {
            //player number which gets assigned to every client, starts with 1 because server is 0
            int index=1;

         //   String positions = buildPositionString(snakes);

            foreach (TcpClient tcpClient in currentClients)
            {

                StreamWriter writer=new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!start " + index + buildSnakePositionsString(snakes) + "!");
                writer.Flush();
            }

            inGameState = InGameState.RUNNING;
        }


        private void sendCurrentPosition(TcpClient tcpClient,List<Snake> snakes)
        {
            //send currentPosition
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            writer.WriteLine("!game" + buildSnakePositionsString(snakes)+ "!");
            writer.Flush();
        }

        public List<Snake> communicateWithClients(List<Snake> snakes)
        {

            int index = 1;

            foreach (TcpClient tcpClient in currentClients)
            {
                sendCurrentPosition(tcpClient,snakes);
                snakes.ElementAt(index).SnakeDirection=receiveCurrentDirection(tcpClient);

                index++;
            }

            return snakes;
        }

        private Snake.Direction receiveCurrentDirection(TcpClient tcpClient){
            StreamReader reader=new StreamReader(tcpClient.GetStream());

            String message = reader.ReadLine();

            String[] splittedMessage= message.Split(' ');

            if (splittedMessage.Length == 3)
            {
                if(splittedMessage[1].Equals("Left")){
                    return Snake.Direction.Left;
                }else if(splittedMessage[1].Equals("Up")){
                    return Snake.Direction.Up;
                }else if(splittedMessage[1].Equals("Right")){
                    return Snake.Direction.Right;
                }else if(splittedMessage[1].Equals("Down")){
                    return Snake.Direction.Down;
                }
            }

            //TODO: check if communication fails
            return Snake.Direction.Down;

        }


        public InGameState getInGameState()
        {
            return inGameState;
        }

        private String buildSnakePositionsString(List<Snake> snakes)
        {
            String positions=" ";

            foreach (Snake snake in snakes)
            {
                positions += buildPositionString(snake);

                positions += " ";
            }

            return positions;
        }


        //builds position String for given snake: [posX1 posY1 posX2 posY2 .... ]
        private String buildPositionString(Snake snake)
        {
            String positions = "[";

            foreach (Vector2 part in snake.parts)
            {
                positions+=part.X+" "+part.Y+" ";
            }

            positions=positions.Remove(positions.Length - 1);

            positions += "]";

            return positions;
        }

    }
}
