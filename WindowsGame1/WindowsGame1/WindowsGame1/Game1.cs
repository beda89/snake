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
using Snake.Menus;

namespace Snake
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
        STARTING,
        RUNNING,
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

        private GameState gameState;
        private InGameState inGameState;

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

        private Score score;

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

            mainMenu = new MainMenu(snakePic, customFont, menuPosition);
            networkMenuServer = new NetworkMenuServer(snakePic, customFont, menuPosition);
            networkMenuClient = new NetworkMenuClient(snakePic, customFont, menuPosition);
            networkMenuClientWaiting = new NetworkMenuClientWaiting(snakePic, customFont, menuPosition);
            networkMenuServerWaiting = new NetworkMenuServerWaiting(snakePic, customFont, menuPosition);

            gameField = new GameField();
            gameField.Initialize(TOPBOUND_Y, boundsTexture, graphics);

            snakeFood = new SnakeFood();
            snakeFood.Initialize(TOPBOUND_Y,redAppleTexture,graphics);

            score=new Score(customFont);

            //initial GameState
            gameState = GameState.MAIN_MENU;

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

        //only called by the server
        private void inPlay(GameTime gameTime)
        {
            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //we communicate with clients 2x during one frame
            if (elapsedTime > FRAME_TIME / 2)
            {
                //TODO: looku if necessary snakes are passed by reference since we get the direction information of each snake from the clients and want to persist it

                try
                {
                    server.CommunicateWithClients(snakes, snakeFood);
                }
                catch (MessageException)
                {
                    gameState=GameState.DISCONNECT_SERVER;

                    return;
                }

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

            //ColorTranslator;

            //TODO: change priority
            snake.Initialize(snakeTexture[0], new Vector2(128f, 64f), Snake.Direction.Right, 0, Color.FromNonPremultiplied(81, 220, 50, 255));
            snakes.Add(snake);

            //TODO has to be changed for more than 2 players (player 3 and 4 would start at same position as player 2
            int clientsCount=server.CurrentClients.Count();


            if (clientsCount >= 1)
            {
                snake = new Snake();
                //TODO change priority
                snake.Initialize(snakeTexture[1], new Vector2(256f, 256f), Snake.Direction.Right, 1, Color.FromNonPremultiplied(176, 61, 201, 255));
                snakes.Add(snake);
            }

            if (clientsCount >= 2)
            {
                snake = new Snake();
                snake.Initialize(snakeTexture[2], new Vector2(512f, 128f), Snake.Direction.Left, 2, Color.FromNonPremultiplied(253, 162, 4, 255));
                snakes.Add(snake);
            }

            if (clientsCount == 3)
            {
                snake = new Snake();
                snake.Initialize(snakeTexture[3], new Vector2(512f, 256f), Snake.Direction.Left, 3, Color.FromNonPremultiplied(240, 255, 5, 255));
                snakes.Add(snake);
            }

            inGameState = InGameState.RUNNING;

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
            networkMenuServerWaiting.Update(server);
            gameState = GameState.NETWORK_MENU_WAITING_FOR_CLIENTS;
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

                //snake dies if it eats itself
                if (snake.CollidesWithItself())
                {
                    snake.IsGameOver = true;
                }

                snake.CheckIfEatenByEnemy(snakes);
            }

            checkSnakesForKilling(snakes);

            checkIfGameFinished(snakes);

        }


        //TODO refactor
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
                    if (snake.IsGameOver)
                    {
                        continue;
                    }

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
                    gameState = GameState.DISCONNECT_SERVER;
                }
                else if (client != null)
                {
                    gameState = GameState.DISCONNECT_CLIENT;
                }
                return ;
            }else
            {
                return;
            }


            //direction is sent to server so it doesn't have to be checked for validity => server does that
            if (isClient)
            {
                snake.SnakeDirection = tempDirection;
                return;
            }

            if (GameUtils.IsDirectionValid(snake,tempDirection))
            {
                snake.SnakeDirection = tempDirection;
            }

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

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Draw(spriteBatch);
                    break;

                case GameState.PLAY_CLIENT:
                case GameState.PLAY_SERVER:
                    drawPlayingGame(spriteBatch);
                    break;

            }

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
                    if (snake.IsGameOver)
                    {
                        continue;
                    }

                    snake.Draw(spriteBatch);

                }
                gameField.Draw(spriteBatch);

                if (isClient)
                {
                    score.Draw(spriteBatch, snakes, client.SnakeNumber);
                }
                else
                {
                    //server is always snakeNumber 0
                    score.Draw(spriteBatch, snakes, 0);
                }

            }
        }

        private void checkSnakesForKilling(List<Snake> snakes)
        {

            //TODO refactor
            foreach (Snake snake in snakes)
            {
                if (snake.IsGameOver)
                {
                    continue;
                }


                foreach (Snake enemy in snakes)
                {
                    if (enemy.Equals(snake))
                    {
                        continue;
                    }

                    if (enemy.IsGameOver)
                    {
                        continue;
                    }


                    if (enemy.Head.Equals(snake.Head))
                    {
                        if (enemy.Priority > snake.Priority)
                        {
                            enemy.AddPart();
                            snake.IsGameOver = true;
                        }
                        else if (enemy.Priority < snake.Priority)
                        {
                            snake.AddPart();
                            enemy.IsGameOver = true;
                        }

                    }

                }

            }


        }

        private void checkIfGameFinished(List<Snake> snakes)
        {
            int index = 0;

            foreach (Snake snake in snakes)
            {
                if (snake.IsGameOver == false)
                {
                    index++;
                }
            }

            if (index <= 1)
            {
                 gameState = GameState.DISCONNECT_SERVER;   
            }
        }
    
    }
}
