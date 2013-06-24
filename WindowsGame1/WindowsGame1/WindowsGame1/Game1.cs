using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Snake.Exceptions;
using Snake.FSM;
using Snake.Menus;

namespace Snake
{
    /*
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
        STARTING,
        RUNNING,
    }; */


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    ///
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region fields

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Vector2 menuPosition;
        private Thread serverThread;
        private Thread clientThread;

        private Server server;
        private Client client;

        private Context context;
        private GameGraphics gameGraphics;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Exiting+=Game1_Exiting;
        }

        void Game1_Exiting(object sender, EventArgs e)
        {
            if (serverThread != null)
            {
                serverThread.Abort();
            }

            if (server != null)
            {
                server.Stop();
            }

            if (clientThread != null)
            {
                clientThread.Abort();
            }

            if (client != null)
            {
                client.Stop();
            }

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
            this.Window.Title = "Snake";

            menuPosition = new Vector2(50, graphics.GraphicsDevice.Viewport.Height - 150);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //State pattern
            context = new Context(new State_MainMenu(menuPosition));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //all textures are stored in GameGraphics object
            Texture2D snakePic;
            Texture2D boundsTexture;
            Texture2D redAppleTexture;
            SpriteFont customFont;
            Texture2D[] snakeTexture = new Texture2D[4];
            Texture2D[][] snakeHeads = new Texture2D[4][];

            snakeTexture[0] = Content.Load<Texture2D>("snakeTexture1");
            snakeTexture[1] = Content.Load<Texture2D>("snakeTexture2");
            snakeTexture[2] = Content.Load<Texture2D>("snakeTexture3");
            snakeTexture[3] = Content.Load<Texture2D>("snakeTexture4");

            customFont = Content.Load<SpriteFont>("customFont");
            snakePic = Content.Load<Texture2D>("snake-cartoon_small");
            boundsTexture = Content.Load<Texture2D>("boundsTexture");
            redAppleTexture = Content.Load<Texture2D>("redApple");

            snakeHeads[0] = new Texture2D[4];
            snakeHeads[0][0] = Content.Load<Texture2D>("snakeTexture1HeadUp");
            snakeHeads[0][1] = Content.Load<Texture2D>("snakeTexture1HeadDown");
            snakeHeads[0][2] = Content.Load<Texture2D>("snakeTexture1HeadLeft");
            snakeHeads[0][3] = Content.Load<Texture2D>("snakeTexture1HeadRight");

            snakeHeads[1] = new Texture2D[4];
            snakeHeads[1][0] = Content.Load<Texture2D>("snakeTexture2HeadUp");
            snakeHeads[1][1] = Content.Load<Texture2D>("snakeTexture2HeadDown");
            snakeHeads[1][2] = Content.Load<Texture2D>("snakeTexture2HeadLeft");
            snakeHeads[1][3] = Content.Load<Texture2D>("snakeTexture2HeadRight");

            snakeHeads[2] = new Texture2D[4];
            snakeHeads[2][0] = Content.Load<Texture2D>("snakeTexture3HeadUp");
            snakeHeads[2][1] = Content.Load<Texture2D>("snakeTexture3HeadDown");
            snakeHeads[2][2] = Content.Load<Texture2D>("snakeTexture3HeadLeft");
            snakeHeads[2][3] = Content.Load<Texture2D>("snakeTexture3HeadRight");

            snakeHeads[3] = new Texture2D[4];
            snakeHeads[3][0] = Content.Load<Texture2D>("snakeTexture4HeadUp");
            snakeHeads[3][1] = Content.Load<Texture2D>("snakeTexture4HeadDown");
            snakeHeads[3][2] = Content.Load<Texture2D>("snakeTexture4HeadLeft");
            snakeHeads[3][3] = Content.Load<Texture2D>("snakeTexture4HeadRight");

            gameGraphics = new GameGraphics(customFont, snakeTexture, snakePic, boundsTexture, redAppleTexture,snakeHeads);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            gameGraphics.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            context.Update(ref server,ref serverThread,ref client,ref clientThread,gameTime);


            /*
            switch (gameState)
            {
                case GameState.MAIN_MENU:
                    mainMenu.Update();
                    gameState = mainMenu.CurrentState;
                    isClient = false;
                    break;

                case GameState.NETWORK_MENU_CLIENT:
                    networkMenuClient.Update();
                    gameState = networkMenuClient.CurrentState;
                    isClient = true;
                    break;

                case GameState.CONNECT_TO_SERVER:
                    if (!networkMenuClient.InputFieldsAreValid())
                    {
                        //if invalid input, we stay at network client menu
                        gameState = GameState.NETWORK_MENU_CLIENT;
                        break;
                    }

                    tryConnectionToServer();
                    gameState = GameState.NETWORK_MENU_WAITING_FOR_SERVER;

                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_SERVER:

                    //checking if user pressed back button before the game started
                    networkMenuClientWaiting.Update();
                    gameState = networkMenuClientWaiting.CurrentState;

                    //if gamestate of menu is still "waiting for Server", we have to check if the connection was refused
                    if (gameState == GameState.NETWORK_MENU_WAITING_FOR_SERVER)
                    {
                        gameState = client.ClientGameState;
                    }

                    break;

                case GameState.START_SERVER:
                    
                    //if wrong user input we stay at the server menu
                    if(!networkMenuServer.hasValidInput()){
                        gameState = GameState.NETWORK_MENU_SERVER;
                        break;
                    }

                    startServer();
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Update(server);
                    gameState = networkMenuServerWaiting.CurrentState;
                    break;

                case GameState.CONNECTION_REFUSED:
                    networkMenuClient.Update();
                    gameState = networkMenuClient.CurrentState;
                    break;

                case GameState.NETWORK_MENU_SERVER:
                    networkMenuServer.Update();
                    gameState = networkMenuServer.CurrentState;
                    break;

                case GameState.FINISH_GAME:
                    //gameState = GameState.DISCONNECT;
                    break;

                case GameState.DISCONNECT_SERVER:
                    server.Stop();
                    serverThread.Abort();
                    gameState = GameState.MAIN_MENU;
                    server = null;
                    break;

                case GameState.DISCONNECT_CLIENT:
                    client.Stop();
                    clientThread.Abort();
                    gameState = GameState.MAIN_MENU;
                    client = null;
                    break;

                case GameState.PLAY_SERVER:
                    inGameState = server.InGameState;

                    if (inGameState == InGameState.STARTING)
                    {
                        initServerGame();
                    }

                    inPlay(gameTime);

                    break;

                case GameState.PLAY_CLIENT:
                    if (client.InGameState == InGameState.STARTING)
                    {
                        client.InGameState=InGameState.RUNNING;
                    }

                    snakes = client.Snakes;
                    snakeFood.Position = client.SnakeFoodPosition;

                    updateSnakes(snakes,true);
                    break;

                case GameState.EXIT:
                    //TODO clean up all resources on exit
                    this.Exit();
                    break;
 
             
            } */


            /*
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            */
            // Read the current state of the keyboard and gamepad and store it
           
           // currentGamePadState = GamePad.GetState(PlayerIndex.One);

            base.Update(gameTime);
        }

        /*
        private void startServer()
        {
            Int32 port = Convert.ToInt32(networkMenuServer.PortInput.InputText);

            server = new Server(port);


            //TODO check if threadStart is needed!
            serverThread = new System.Threading.Thread(server.Start);
            serverThread.Start();
            networkMenuServerWaiting.Update(server);
            gameState = GameState.NETWORK_MENU_WAITING_FOR_CLIENTS;
        } */



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            this.IsMouseVisible = true;
            
            GraphicsDevice.Clear(Color.White);
            context.Draw(spriteBatch, gameGraphics, gameTime);
            
            /*
            spriteBatch.Begin();
            


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

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Draw(spriteBatch);
                    break;

                case GameState.PLAY_CLIENT:
                case GameState.PLAY_SERVER:
                    drawPlayingGame(spriteBatch);
                    break;

            }
            
            spriteBatch.End(); */

            base.Draw(gameTime);
        }  
    }
}
