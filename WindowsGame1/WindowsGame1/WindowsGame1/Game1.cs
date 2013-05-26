using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGame1.Menus;

namespace WindowsGame1
{

    public enum GameState { MAIN_MENU, 
                            NETWORK_MENU_SERVER, 
                            NETWORK_MENU_CLIENT, 
                            NETWORK_MENU_WAITING_FOR_CLIENTS, 
                            NETWORK_MENU_WAITING_FOR_SERVER,
                            CONNECT_TO_SERVER,
                            PLAY_SERVER, 
                            PLAY_CLIENT, 
                            FINISH_GAME,
                            START_SERVER,
                            DISCONNECT_SERVER,
                            DISCONNECT_CLIENT,
                            CONNECTION_REFUSED,
                            EXIT };

    public enum InGameState
    {
        WAITING,
        STARTING,
        RUNNING,
        END
    };


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    ///
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
    //    Snake snake;
        GameField gameField;
        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;
        Vector2 menuPosition;
        Thread serverThread;
        Thread clientThread;



       
        private GameState gameState;
        private InGameState inGameState;

        private Menu mainMenu;
        private Menu networkMenuServer;
        private NetworkMenuServerWaiting networkMenuServerWaiting;
        private Menu networkMenuClient;
        private Menu networkMenuClientWaiting;

        private Server server;
        private Client client;

        private Boolean isClient = false;

        //server manages the snakes
        private List<Snake> snakes;

        Texture2D snakeTexture;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            menuPosition = new Vector2(50, graphics.GraphicsDevice.Viewport.Height - 150);
            spriteBatch = new SpriteBatch(GraphicsDevice);
          //  snake = new Snake();
            gameField = new GameField();
            mainMenu = new MainMenu(Content,menuPosition);
            networkMenuServer = new NetworkMenuServer(Content, menuPosition);
            networkMenuClient = new NetworkMenuClient(Content, menuPosition);
            networkMenuClientWaiting = new NetworkMenuClientWaiting(Content, menuPosition);
            networkMenuServerWaiting = new NetworkMenuServerWaiting(Content, menuPosition);


            gameState = GameState.MAIN_MENU;


            //Load snake texture from Content project
            snakeTexture = Content.Load<Texture2D>("snakeTexture");
        //    snake.Initialize(snakeTexture, new Vector2(256f, 256f));

            Texture2D boundsTexture = Content.Load<Texture2D>("boundsTexture");
            gameField.Initialize(boundsTexture,graphics);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.MAIN_MENU:
                    mainMenu.Update();
                    gameState = mainMenu.getCurrentState();
                    isClient = false;
                    break;

                case GameState.NETWORK_MENU_CLIENT:
                    networkMenuClient.Update();
                    gameState=networkMenuClient.getCurrentState();
                    isClient = true;
                    break;

                case GameState.CONNECT_TO_SERVER:
                    client = new Client("localhost",20000,snakeTexture);
                    clientThread = new System.Threading.Thread(client.start);
                    clientThread.Start();
                    gameState = GameState.NETWORK_MENU_WAITING_FOR_SERVER;

                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_SERVER:
                    networkMenuClientWaiting.Update();
                    gameState = networkMenuClientWaiting.getCurrentState();


                    //if gamestate of menu is waiting for Server, we have to check if the connection was refused
                    if (gameState == GameState.NETWORK_MENU_WAITING_FOR_SERVER)
                    {
                        gameState = client.getClientState();
                    }

                    break;

                case GameState.START_SERVER:
                    server = new Server(20000);
                    //TODO check if threadStart is needed!
                    serverThread = new System.Threading.Thread(server.start);
                    serverThread.Start();
                    networkMenuServerWaiting.Update(server);
                    gameState = GameState.NETWORK_MENU_WAITING_FOR_CLIENTS;
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Update(server);
                    gameState = networkMenuServerWaiting.getCurrentState();
                    break;

                case GameState.CONNECTION_REFUSED:
                    networkMenuClient.Update();
                    gameState=networkMenuClient.getCurrentState();
                    break;

                case GameState.NETWORK_MENU_SERVER:
                    networkMenuServer.Update();
                    gameState = networkMenuServer.getCurrentState();
                    break;

                case GameState.FINISH_GAME:
                  //  gameState = GameState.DISCONNECT;
                    break;

                case GameState.DISCONNECT_SERVER:
                    server.stop();
                    serverThread.Abort();
                    gameState = GameState.MAIN_MENU;
                    server = null;
                    break;

                case GameState.DISCONNECT_CLIENT:
                    client.stop();
                    clientThread.Abort();
                    gameState = GameState.MAIN_MENU;
                    client = null;
                    break;

                case GameState.PLAY_SERVER:
                    inGameState = server.getInGameState();

                    if (inGameState == InGameState.STARTING)
                    {

                        
                        //init snakes, first one is needed for server
                        snakes = new List<Snake>();
                        Snake snake = new Snake();
                        snake.Initialize(snakeTexture, new Vector2(32f, 32f),Snake.Direction.Right);
                        snakes.Add(snake);


                        for (int i = 0; i < server.getCurrentClients().Count(); i++)
                        {
                            snake= new Snake();
                            snake.Initialize(snakeTexture, new Vector2(256f, 256f), Snake.Direction.Up);
                            snakes.Add(snake);
                        }
                        
                        inGameState = InGameState.RUNNING;

                        server.sendStartSignal(snakes);
                    }

                    snakes=server.communicateWithClients(snakes);


                    UpdateSnakes(snakes,gameTime);
                      
                    break;

                case GameState.PLAY_CLIENT:
                    if (client.getClientInGameState() == InGameState.STARTING)
                    {
                        client.setClientInGameState(InGameState.RUNNING);
                    }

                    snakes = client.getSnakes();

                    UpdateSnakes(snakes,gameTime);
                    break;

                case GameState.EXIT:

                    //TODO clean up all resources on exit
                    this.Exit();
                    break;
            }





            /*
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            */
            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
           // currentGamePadState = GamePad.GetState(PlayerIndex.One);

            base.Update(gameTime);
        }


     /*   private void UpdateSnakesClient(List<Snake> snakes)
        {


        } */


         private void UpdateSnakes(List<Snake> snakes,GameTime gameTime)
         {

            if (isClient)
            {
                int index = 0;
                //server snake is always the first one
                foreach (Snake snake in snakes)
                {
                      if (index == client.getSnakeNumber())
                      {
                        setDirection(snake);
                        client.clientSnakeDirection = snake.SnakeDirection;

                     }

                   // snake.Update(gameTime);
                    index++;
                }

            }
            else
            {
                int index = 0;
                //server snake is always the first one
                foreach (Snake snake in snakes)
                {
                    if (index == 0)
                    {
                        setDirection(snake);
                    }

                    snake.Update(gameTime);
                    index++;
                }
            }
        }


        private void setDirection(Snake snake){
             Snake.Direction tempDirection;

            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Left;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Right;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Up;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Down;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (server != null)
                {
                    gameState = GameState.DISCONNECT_SERVER;
                }

                if (client != null)
                {
                    gameState = GameState.DISCONNECT_CLIENT;
                }

                return ;

            }else
            {
                return;
            }

            int distance = Math.Abs((int)tempDirection - (int)snake.SnakeDirection);
            if (distance != 2)
                snake.SnakeDirection = tempDirection;

        }





        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            this.IsMouseVisible = true;

            switch (gameState)
            {
                case GameState.MAIN_MENU:
                    mainMenu.Draw(spriteBatch);
                    break;

                case GameState.NETWORK_MENU_CLIENT:
                    networkMenuClient.Draw(spriteBatch);
                    break;

                case GameState.NETWORK_MENU_SERVER:
                    networkMenuServer.Draw(spriteBatch);
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_SERVER:
                    networkMenuClientWaiting.Draw(spriteBatch);
                    break;

                case GameState.FINISH_GAME:
                    break;

                case GameState.CONNECTION_REFUSED:
                    networkMenuClient.Draw(spriteBatch);
                    break;

              /*  case GameState.PLAY_CLIENT:
                    this.IsMouseVisible = false;
                    if (snakes != null)
                    {
                        foreach (Snake snake in snakes)
                        {
                            snake.Draw(spriteBatch);
                            gameField.Draw(spriteBatch);
                        }
                    }
                    break; */

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Draw(spriteBatch);
                    break;

                case GameState.PLAY_CLIENT:
                case GameState.PLAY_SERVER:
                    this.IsMouseVisible = false;

                    if(snakes!=null){
                         foreach (Snake snake in snakes)
                         {
                            snake.Draw(spriteBatch);
 
                         }
                         gameField.Draw(spriteBatch);
                    }
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
