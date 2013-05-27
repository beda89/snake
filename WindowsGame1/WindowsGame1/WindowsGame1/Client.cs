using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using WindowsGame1.Exceptions;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Client
    {
        public Snake.Direction clientSnakeDirection { get; set; }
        public List<Snake> snakes { get; set; }

        private String ip;
        private int port;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private GameState clientState = GameState.NETWORK_MENU_WAITING_FOR_SERVER;
        private InGameState inGameState = InGameState.STARTING;
        private Texture2D[] snakeTexture;
        
        //snakeNumber assigned by server, server is always 0
        public int snakeNumber;

        public Client(String ipString, int port,Texture2D[] texture)
        {
           this.ip = ipString;
           this.port = port;

           //TODO:dummy value
           clientSnakeDirection = Snake.Direction.Left;

           snakes = new List<Snake>();
           this.snakeTexture = texture;
        }

        public void start(){

            try
            {
                tcpClient = new TcpClient(ip, port);
                stream = tcpClient.GetStream();

                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                String message="";

                while(message!=null && !message.StartsWith("!end")){
                    message = reader.ReadLine();

                    if(message!=null){
                        communicationProtocol(message, writer);
                    }
               }

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

        private void sendSnakeDirection(StreamWriter writer)
        {
            writer.WriteLine("!game "+clientSnakeDirection+" !");
            writer.Flush();
        }

        private void communicationProtocol(String message,StreamWriter writer)
        {
            if (message.StartsWith("!start"))
            {
                String[] splittedMessage = message.Split(new Char[]{' '}, 3);

                clientState = GameState.PLAY_CLIENT;
                inGameState = InGameState.STARTING;

                snakeNumber = Convert.ToInt32(splittedMessage[1]);

                String[] snakePositions = seperateSnakes(splittedMessage[2]);

                //sets the global snakes list
                initSnakesAndSetPositions(snakePositions);
            }
            else if (message.StartsWith("!game"))
            {
                String[] splittedMessage=message.Split(new Char[]{' '},2);
                String[] snakePositions = seperateSnakes(splittedMessage[1]);

                setSnakePositions(snakePositions);
                sendSnakeDirection(writer);
            }
            else if (message.StartsWith("!end"))
            {


            }
        }

        private void initSnakesAndSetPositions(String[] snakePositionStrings)
        {
            int snakeIndex = 0;

            for (int i = 0; i < snakePositionStrings.Length; i++)
            {
                List<Vector2> parts = parsePositionString(snakePositionStrings[i]);

                Snake snake = new Snake();
                snake.Initialize(snakeTexture[snakeIndex], parts);
                snakes.Add(snake);
                snakeIndex++;
            }
        }

        private void setSnakePositions(String[] snakePositionStrings)
        {
            int snakeIndex = 0;

            for (int i = 0; i < snakePositionStrings.Length; i++)
            {
                List<Vector2> parts = parsePositionString(snakePositionStrings[i]);

                snakes.ElementAt(snakeIndex).parts = parts;
                snakeIndex++;
            }
        }

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

        private List<Vector2> parsePositionString(String positionString)
        {
            String[] splittedPositions = positionString.Split(' ');
            List<Vector2> parts = new List<Vector2>();

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
