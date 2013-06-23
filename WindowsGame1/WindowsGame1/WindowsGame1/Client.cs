using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Snake.Exceptions;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    class Client
    {
        public Snake.Direction ClientSnakeDirection { get; set; }

        //every snake in play(are initialized and managed by server)
        public List<Snake> Snakes { get; set; }
        public GameState ClientGameState { get; private set; }

        //states while playing
        public InGameState InGameState{get;set;} 

        //snakeNumber assigned by server, server is always 0
        public int SnakeNumber { get; private set; }
        public Vector2 SnakeFoodPosition {get; set;}

        private String ip;
        private int port;
        private TcpClient tcpClient;
        private NetworkStream stream;

        private Texture2D[] snakeTexture;
        
        public Client(String ipString, int port,Texture2D[] texture)
        {
            
            this.InGameState = InGameState.STARTING;
            //TODO: refactor (client gamestate shouldn't be necessary)
            this.ClientGameState=GameState.NETWORK_MENU_WAITING_FOR_SERVER;
            this.ip = ipString;
            this.port = port;
            this.snakeTexture = texture;

           //TODO:dummy value
           ClientSnakeDirection = Snake.Direction.Left;

           Snakes = new List<Snake>();
        }

        public void Start(){
            try
            {
                tcpClient = new TcpClient(ip, port);
                stream = tcpClient.GetStream();

                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                String message = "";

                while (message != null && !message.StartsWith("!end"))
                {
                    message = reader.ReadLine();

                    if (message != null)
                    {
                        communicationProtocol(message, writer);
                    }
                }

            }
            catch (SocketException)
            {
                ClientGameState = GameState.DISCONNECT_CLIENT;
            }
            catch (IOException)
            {
                ClientGameState = GameState.DISCONNECT_CLIENT;
            }
        } 

        public void Stop()
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

        private void sendSnakeDirection(StreamWriter writer)
        {
            writer.WriteLine("!game "+ClientSnakeDirection+" !");
            writer.Flush();
        }

        private void communicationProtocol(String message,StreamWriter writer)
        {
            if (message.StartsWith("!start"))
            {
                String[] splittedMessage = message.Split(new Char[]{' '}, 3);

                ClientGameState = GameState.PLAY_CLIENT;
                InGameState = InGameState.STARTING;

                SnakeNumber = Convert.ToInt32(splittedMessage[1]);

                String[] snakePositions = seperateSnakes(splittedMessage[2]);

                //sets the global snakes list
                initSnakesAndSetPositions(snakePositions);
                SnakeFoodPosition = parseSnakeFoodPosition(splittedMessage[2]);
            }
            else if (message.StartsWith("!game"))
            {
                String[] splittedMessage=message.Split(new Char[]{' '},2);
                String[] snakePositions = seperateSnakes(splittedMessage[1]);

                SnakeFoodPosition = parseSnakeFoodPosition(splittedMessage[1]);

                setSnakePositions(snakePositions);
                sendSnakeDirection(writer);


            }
            else if (message.StartsWith("!end"))
            {
                 //TODO

            }
        }

        //initalizes every snakes at the beginning of the game, according to the start message of the server
        private void initSnakesAndSetPositions(String[] snakePositionStrings)
        {
            int snakeIndex = 0;

            for (int i = 0; i < snakePositionStrings.Length; i++)
            {
                List<Vector2> parts = parsePositionString(snakePositionStrings[i]);

                Snake snake = new Snake();

                //TODO: change priority
                snake.Initialize(snakeTexture[snakeIndex], parts.ElementAt(0), parts.GetRange(1, parts.Count - 1), i, Color.FromNonPremultiplied(0xFF, 0x41, 0xD8, 0x24));
                Snakes.Add(snake);
                snakeIndex++;
            }
        }

        //sets the current positions of every snake according to the server message
        private void setSnakePositions(String[] snakePositionStrings)
        {
            int snakeIndex = 0;

            for (int i = 0; i < snakePositionStrings.Length; i++)
            {
                List<Vector2> parts = parsePositionString(snakePositionStrings[i]);

                //if snake is gameOver server sends position [0 0]
                if (parts.ElementAt(0).Equals(new Vector2(0, 0)))
                {
                    Snakes.ElementAt(snakeIndex).IsGameOver = true;
                }
                else
                {
                    Snakes.ElementAt(snakeIndex).Head = parts.First();
                    Snakes.ElementAt(snakeIndex).Body = parts.GetRange(1,parts.Count()-1);
                }
                
                snakeIndex++;
            }
        }

        //splits the received message in the positionStrings according to every snake
        private String[] seperateSnakes(String message)
        {
            String[] snakePositionStrings;

            snakePositionStrings = message.Split(new Char[]{'['}, StringSplitOptions.RemoveEmptyEntries);

            String[] tempString;

            tempString=snakePositionStrings[snakePositionStrings.Length - 1].Split(new Char[]{']'},2);
            snakePositionStrings[snakePositionStrings.Length - 1] = tempString[0] + "]";

            for (int index = 0; index < snakePositionStrings.Length; index++)
            {
                snakePositionStrings[index] = snakePositionStrings[index].Trim();
                snakePositionStrings[index] = snakePositionStrings[index].Remove(snakePositionStrings[index].Length - 1);
            }

            return snakePositionStrings;
        }

        private Vector2 parseSnakeFoodPosition(String message){
            String[] positionStrings = message.Split(new Char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);

            String foodPositionString = positionStrings.Last();

            //split in x and y position
            String[] positions=foodPositionString.Split(new Char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                float x = Convert.ToSingle(positions[0]);
                float y = Convert.ToSingle(positions[1]);
                return new Vector2(x, y);
            }
            catch (FormatException)
            {
                throw new MessageException();
            }
        }

        //parses the positionString according to one snake
        private List<Vector2> parsePositionString(String positionString)
        {
            String[] splittedPositions = positionString.Split(' ');
            List<Vector2> parts = new List<Vector2>();


            //there always has to be a position.X and position.Y pair
            if (splittedPositions.Length % 2 == 1)
            {
                throw new MessageException();
            }

            for (int index = 0; index < splittedPositions.Length; index++)
            {
                try
                {
                    float x = Convert.ToSingle(splittedPositions[index]);
                    index++;
                    float y = Convert.ToSingle(splittedPositions[index]);

                    parts.Add(new Vector2(x, y));
                }
                catch (FormatException)
                {
                    throw new MessageException();
                }
            }

            return parts;   
        }
    }
}
