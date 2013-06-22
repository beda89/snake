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
                            EXIT }; */

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
        #region fields
        //TopBound of the gameField (space above is used for current score)
        private const int TOPBOUND_Y = 32;
        // The time we display a frame until the next one
        private const int FRAME_TIME = 250;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameField gameField;
        private SnakeFood snakeFood;
        private KeyboardState currentKeyboardState;
        private GamePadState currentGamePadState;
        private Vector2 menuPosition;
        private Thread serverThread;
        private Thread clientThread;

        //FSM
        private Context context;

      /*  private GameState gameState;
        private InGameState inGameState; */

        private Menu mainMenu;
        private NetworkMenuServer networkMenuServer;
        private NetworkMenuServerWaiting networkMenuServerWaiting;
        private NetworkMenuClient networkMenuClient;
        private Menu networkMenuClientWaiting;

        private Server server;
        private Client client;

        private Texture2D snakePic;
        private Texture2D boundsTexture;
        private Texture2D redAppleTexture;
        private SpriteFont customFont;

        private Boolean isClient = false;
        private int elapsedTime = 0;
        private int moveCondition = 0;

        //server manages the snakes
        private List<Snake> snakes;
        Texture2D[] snakeTexture = new Texture2D[4];

        #endregion

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
            this.Window.Title = "Snake";

            menuPosition = new Vector2(50, graphics.GraphicsDevice.Viewport.Height - 150);
            spriteBatch = new SpriteBatch(GraphicsDevice);

        /*    mainMenu = new MainMenu(menuPosition);
            networkMenuServer = new NetworkMenuServer(snakePic, customFont, menuPosition);
            networkMenuClient = new NetworkMenuClient(snakePic, customFont, menuPosition);
            networkMenuClientWaiting = new NetworkMenuClientWaiting(snakePic, customFont, menuPosition);
            networkMenuServerWaiting = new NetworkMenuServerWaiting(snakePic, customFont, menuPosition); */

            gameField = new GameField();
            gameField.Initialize(TOPBOUND_Y, boundsTexture, graphics);

            snakeFood = new SnakeFood();
            snakeFood.Initialize(TOPBOUND_Y,redAppleTexture,graphics);




            //initial State
            context = new Context(new MainMenu_State(menuPosition),snakePic,customFont);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            snakeTexture[0] = Content.Load<Texture2D>("snakeTexture1");
            snakeTexture[1] = Content.Load<Texture2D>("snakeTexture2");
            snakeTexture[2] = Content.Load<Texture2D>("snakeTexture3");
            snakeTexture[3] = Content.Load<Texture2D>("snakeTexture4");

            customFont = Content.Load<SpriteFont>("customFont");
            snakePic = Content.Load<Texture2D>("snake-cartoon_small");
            boundsTexture = Content.Load<Texture2D>("boundsTexture");
            redAppleTexture = Content.Load<Texture2D>("redApple");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            snakeTexture[0].Dispose();
            snakeTexture[1].Dispose();
            snakeTexture[2].Dispose();
            snakeTexture[3].Dispose();
            boundsTexture.Dispose();
            snakePic.Dispose();
            redAppleTexture.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            context.Update();

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
                  //  gameState = GameState.DISCONNECT;
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
            }
            */

            //TODO: CATCH EXIT SIGNAL WHEN MOUSECLICKS ON CLOSE DURING PLAY



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

        //only called by the server
        private void inPlay(GameTime gameTime)
        {
            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //we communicate with clients 2x during one frame
            if (elapsedTime > FRAME_TIME / 2)
            {
                //snakes are passed by reference since we get the direction information of each snake from the clients and want to persist it
                server.CommunicateWithClients(ref snakes, snakeFood);

                //snakes are moved once during frameTime
                updateSnakes(snakes, true);
                gameLogic(snakes);

                elapsedTime = 0;
            }
            else
            {
                updateSnakes(snakes, false);

            }
        }


        private void initServerGame()
        {
            //init snakes, first one is needed for server
            snakes = new List<Snake>();
            Snake snake = new Snake();
            snake.Initialize(snakeTexture[0], new Vector2(128f, 64f), Snake.Direction.Right);
            snakes.Add(snake);

            for (int i = 0; i < server.CurrentClients.Count(); i++)
            {
                snake = new Snake();
                snake.Initialize(snakeTexture[i + 1], new Vector2(256f, 256f), Snake.Direction.Up);
                snakes.Add(snake);
            }

       //     inGameState = InGameState.RUNNING;

            //sending startsignals to all clients
            server.sendStartSignal(snakes, snakeFood);
        }

        private void startServer()
        {
            Int32 port = Convert.ToInt32(networkMenuServer.PortInput.InputText);

            server = new Server(port);


            //TODO check if threadStart is needed!
            serverThread = new System.Threading.Thread(server.Start);
            serverThread.Start();
         //   networkMenuServerWaiting.Update(server);
        //    gameState = GameState.NETWORK_MENU_WAITING_FOR_CLIENTS;

        }

        private void tryConnectionToServer()
        {
            int port = Convert.ToInt32(networkMenuClient.PortInput.InputText);
            String ip = networkMenuClient.IpInput.InputText;

            client = new Client(ip, port, snakeTexture);
            clientThread = new System.Threading.Thread(client.Start);
            clientThread.Start();
        }

        /*
         * contains the whole gamelogic
         * it is all done by the server
         * 
         */
        private void gameLogic(List<Snake> snakes)
        {
            //TODO: we only have to check collision and eating of apple every time the snakes move (they move according to frametime frameTime)
            foreach(Snake snake in snakes){
                if(snakeFood.IsEaten(snake)){
                    snake.AddPart();
                }
            }
        }

        private void updateSnakes(List<Snake> snakes,Boolean moveSnakes)
         {
            //the only logic a client has, is that he sets the current direction of its snake according to the user input
            if (isClient)
            {
                Snake clientSnake = snakes.ElementAt(client.SnakeNumber);
                setDirection(clientSnake);
                client.ClientSnakeDirection = clientSnake.SnakeDirection;
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

                    if (moveSnakes == true)
                    {
                        //snakes are moved every second time, since updateSnakes is called twice with movesnakes during FRAMETIME
                        if ((moveCondition % 2 )==0)
                        {
                            snake.Update(gameField);
                        }
                    }
                    index++;
                }
            }

            if (moveSnakes == true)
            {
                moveCondition++;
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
             //       gameState = GameState.DISCONNECT_SERVER;
                }
                else if (client != null)
                {
             //       gameState = GameState.DISCONNECT_CLIENT;
                }
                return ;
            }else
            {
                return;
            }



            int distance = Math.Abs((int)tempDirection - (int)snake.LastMovedDirection);
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

            //this.IsMouseVisible = true;

            context.Draw(spriteBatch);


            /*
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

            } */

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawPlayingGame(SpriteBatch spriteBatch)
        {
            //during gameplay mouse should disappear
            this.IsMouseVisible = false;

            if (snakeFood != null)
            {
                snakeFood.Draw(spriteBatch);
            }

            if (snakes != null)
            {
                foreach (Snake snake in snakes)
                {
                    snake.Draw(spriteBatch);

                }
                gameField.Draw(spriteBatch);
            }
        }

    }


}
