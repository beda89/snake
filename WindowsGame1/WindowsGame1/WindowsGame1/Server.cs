using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WindowsGame1.Exceptions;

namespace WindowsGame1
{
    class Server
    {
        private const int MAX_PLAYERS = 4;

        public InGameState InGameState { get; set; }

        //TODO: check for threadsafety
        public List<TcpClient> CurrentClients{get;private set;}

        private int port;
        private TcpListener tcpListener;

        public Server(int port)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.CurrentClients = new List<TcpClient>();
            this.InGameState=InGameState.STARTING;
        }

        public void Start(){
            tcpListener.Start();

            while (CurrentClients.Count()<MAX_PLAYERS)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    CurrentClients.Add(tcpClient);
                }
                catch (SocketException)
                {
                    return;
                }
            }
        }

        public void Stop()
        {
            tcpListener.Stop();

            foreach (TcpClient tcpClient in CurrentClients)
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }

            CurrentClients.Clear();
        }

        //sends startSignal to every client with snake Positions and the number of the snake assigned to the client
        public void sendStartSignal(List<Snake> snakes)
        {
            //player number which gets assigned to every client, starts with 1 because server is 0
            int index=1;

            foreach (TcpClient tcpClient in CurrentClients)
            {
                StreamWriter writer=new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!start " + index + buildSnakePositionsString(snakes) + "!");
                writer.Flush();
            }

            InGameState = InGameState.RUNNING;
        }

        //sends string with current positions of every segment of every snake
        private void sendCurrentPosition(TcpClient tcpClient,List<Snake> snakes)
        {
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            writer.WriteLine("!game" + buildSnakePositionsString(snakes)+ "!");
            writer.Flush();
        }

        //sends current positions of every segment of every snake to every client and receives the currently snake direction of the client
        public List<Snake> CommunicateWithClients(List<Snake> snakes)
        {
            //snake with index=0 belongs to server
            int index = 1;

            foreach (TcpClient tcpClient in CurrentClients)
            {
                sendCurrentPosition(tcpClient,snakes);
                snakes.ElementAt(index).SnakeDirection=receiveCurrentDirection(tcpClient);

                index++;
            }

            return snakes;
        }

        //reads message from client and sets the currentDirection of the clients Snake
        private Snake.Direction receiveCurrentDirection(TcpClient tcpClient){
            StreamReader reader=new StreamReader(tcpClient.GetStream());

            String message;

            try
            {
                message = reader.ReadLine();
            }
            catch (IOException)
            {
                throw new MessageException();
            }

            if (message == null)
            {
                throw new MessageException();
            }

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

        //builds postitions String for all snakes
        private String buildSnakePositionsString(List<Snake> snakes)
        {
            StringBuilder positions= new StringBuilder(" ");

            foreach (Snake snake in snakes)
            {
                positions.Append(buildPositionString(snake));
                positions.Append(" ");
            }

            return positions.ToString();
        }

        //builds position String for given snake: [posX1 posY1 posX2 posY2 .... ]
        private String buildPositionString(Snake snake)
        {
            StringBuilder positions = new StringBuilder("[");

            foreach (Vector2 part in snake.parts)
            {
                positions.Append(part.X + " " + part.Y + " ");
            }

            positions=positions.Remove(positions.Length - 1,1);

            positions.Append("]");

            return positions.ToString();
        }

    }
}
