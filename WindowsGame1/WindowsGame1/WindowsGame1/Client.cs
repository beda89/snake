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
        private String ip;
        private int port;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private GameState clientState = GameState.NETWORK_MENU_WAITING_FOR_SERVER;
        private InGameState inGameState = InGameState.STARTING;
        public Snake.Direction clientSnakeDirection {get;set;}
        public List<Snake> snakes { get; set; }
        private Texture2D snakeTexture;
        
        //snakeNumber assigned by server, server is always 0
        private int snakeNumber;


        public Client(String ipString, int port,Texture2D texture)
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
                while(!message.StartsWith("!finish") && message!=null){
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
                String[] splittedMessage=message.Split(' ');

                //according to messageProtocol the number of message segements has to be odd
                if (splittedMessage.Length%2!=1)
                {
                    throw new MessageException();
                }

                clientState = GameState.PLAY_CLIENT;
                inGameState = InGameState.STARTING;


                snakeNumber = Convert.ToInt32(splittedMessage[1]);
                String[] snakePositions=new String[splittedMessage.Length-3];
                Array.Copy(splittedMessage, 2, snakePositions, 0, splittedMessage.Length - 3);


                //sets the global snakes list
                parseSnakePositionsAndInitialize(snakePositions);


            }
            else if (message.StartsWith("!game"))
            {
                String[] splittedMessage=message.Split(' ');

                //according to messageProtocol the number of message segements has to be even
                if (splittedMessage.Length%2!=0)
                {
                    throw new MessageException();
                }

                String[] snakePositions=new String[splittedMessage.Length-2];
                Array.Copy(splittedMessage, 1, snakePositions, 0, splittedMessage.Length - 2);


                parseSnakePositions(snakePositions);
                sendSnakeDirection(writer);
            }
            else if (message.StartsWith("!end"))
            {


            }
        }


        private void parseSnakePositionsAndInitialize(String[] splittedMessage)
        {
                for (int i = 0; i < splittedMessage.Length; i++)
                {
                    float x = Convert.ToSingle(splittedMessage[i]);
                    i++;
                    float y = Convert.ToSingle(splittedMessage[i]);

                    Vector2 vector= new Vector2(x,y);
                    Snake snake = new Snake();
                    snake.Initialize(snakeTexture,vector,Snake.Direction.Up);
                    snakes.Add(snake);
                }
        }

        private void parseSnakePositions(String[] splittedMessage)
        {
            int index = 0;
            for (int i = 0; i < splittedMessage.Length; i++)
            {
                float x = Convert.ToSingle(splittedMessage[i]);
                i++;
                float y = Convert.ToSingle(splittedMessage[i]);

                Vector2 vector = new Vector2(x, y);

                snakes.ElementAt(index).Position = vector;
                index++;
            }
        }
    }
}
