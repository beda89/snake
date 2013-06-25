using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Exceptions;

namespace Snake
{
    class Server
    {
        private const int MAX_CLIENTS = 3;
        public List<TcpClient> CurrentClients{get;private set;}

        private int port;
        private TcpListener tcpListener;

        public Server(int port)
        {
            this.port = port;
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.CurrentClients = new List<TcpClient>();
        }

        public void Start(){

            try
            {
                tcpListener.Start();
            }
            catch (SocketException)
            {
                return;
            }


            while (CurrentClients.Count()<MAX_CLIENTS)
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
        //!start clientSnakeNumber [Snake1.X1 Snake1.Y1 Snake1.X2 Snake1.Y2 ... Snake1.Priority] [Snake2.X1 Snake2.Y1 Snake2.X2 Snake.Y2 ... Snake2.Priority] SnakeFood.X SnakeFoodY !
        public void sendStartSignal(List<Snake> snakes,SnakeFood snakeFoodPosition)
        {
            //player number which gets assigned to every client, starts with 1 because server is 0
            int index=1;

            foreach (TcpClient tcpClient in CurrentClients)
            {
                StreamWriter writer=new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!start " + index + buildSnakePositionsString(snakes) + buildFoodPositionString(snakeFoodPosition.Position)+" !");
                writer.Flush();
                index++;
            }

        }

        public void sendEndSignal(int winner)
        {
            foreach (TcpClient tcpClient in CurrentClients)
            {
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                writer.WriteLine("!end");
                writer.Flush();
            }
        }

        //sends string with current positions of every segment of every snake
        private void sendCurrentPosition(TcpClient tcpClient,List<Snake> snakes,SnakeFood snakeFoodPosition)
        {
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            writer.WriteLine("!game" + buildSnakePositionsString(snakes)+ buildFoodPositionString(snakeFoodPosition.Position)+ " !");
            writer.Flush();
        }

        //sends current positions of every segment of every snake to every client and receives the currently snake direction of the client
        public void CommunicateWithClients(List<Snake> snakes,SnakeFood snakeFood)
        {
            //snake with index=0 belongs to server
            int index = 1;

                foreach (TcpClient tcpClient in CurrentClients)
                {
                    sendCurrentPosition(tcpClient, snakes, snakeFood);

                    Snake.Direction snakeDirection = receiveCurrentDirection(tcpClient);
                    Snake snake = snakes.ElementAt(index);

                    //client sends direction and server checks if valid
                    if (GameUtils.IsDirectionValid(snake, snakeDirection))
                    {
                        snakes.ElementAt(index).ActualSnakeDirection = snakeDirection;
                    }

                    index++;
                }
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

        //builds postitions String for all snakes [snake1] [snake2] ....
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
        private String buildFoodPositionString(Vector2 snakeFoodPosition)
        {
            StringBuilder position = new StringBuilder(" ");
            position.Append(snakeFoodPosition.X + " " + snakeFoodPosition.Y);

            return position.ToString();
        }

        //builds position String for given snake: [posX1 posY1 posX2 posY2 .... priority direction] 
        //appends priority
        private String buildPositionString(Snake snake)
        {
            StringBuilder positions = new StringBuilder("[");

            if (snake.IsGameOver)
            {
                positions.Append("0 0 ");
            }
            else
            {
                positions.Append(snake.Head.X + " " +snake.Head.Y + " ");

                foreach (Vector2 part in snake.Body)
                {
                    positions.Append(part.X + " " + part.Y + " ");
                }

            }

            positions.Append(snake.Priority);
            positions.Append(" "+snake.ActualSnakeDirection);

            positions.Append("]");

            return positions.ToString();
        }

    }
}
