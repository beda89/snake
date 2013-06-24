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

        // priority of snakes is changed every 50 moves
        private const int PRIORITY_CHANGE_CONDITION = 50;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameField gameField;
        private SnakeFood snakeFood;
        private KeyboardState currentKeyboardState;
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

        private Context context;

        private Boolean isClient = false;
        private int elapsedTime = 0;
        private int moveCondition = 0;
        private int moveCounter = 0;

        private GameGraphics gameGraphics;

        //server manages the snakes
        private List<Snake> snakes;

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

            mainMenu = new MainMenu(gameGraphics.SnakePic, gameGraphics.CustomFont, menuPosition);
            networkMenuServer = new NetworkMenuServer(gameGraphics.SnakePic, gameGraphics.CustomFont, menuPosition);
            networkMenuClient = new NetworkMenuClient(gameGraphics.SnakePic, gameGraphics.CustomFont, menuPosition);
            networkMenuClientWaiting = new NetworkMenuClientWaiting(gameGraphics.SnakePic, gameGraphics.CustomFont, menuPosition);
            networkMenuServerWaiting = new NetworkMenuServerWaiting(gameGraphics.SnakePic, gameGraphics.CustomFont, menuPosition);

            gameField = new GameField();
            gameField.Initialize(TOPBOUND_Y, gameGraphics.BoundsTexture, graphics);

            snakeFood = new SnakeFood();
            snakeFood.Initialize(TOPBOUND_Y, gameGraphics.RedAppleTexture, graphics);

            score = new Score(gameGraphics.CustomFont);

            context = new Context(new State_MainMenu());

            //initial GameState
            gameState = GameState.MAIN_MENU;



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
            snake.Initialize(gameGraphics.SnakeTexture[0],gameGraphics.SnakeHeads[0], new Vector2(128f, 64f), Snake.Direction.Right, 1, Color.FromNonPremultiplied(81, 220, 50, 255));
            snakes.Add(snake);

            //TODO has to be changed for more than 2 players (player 3 and 4 would start at same position as player 2
            int clientsCount=server.CurrentClients.Count();


            if (clientsCount >= 1)
            {
                snake = new Snake();
                //TODO change priority
                snake.Initialize(gameGraphics.SnakeTexture[1],gameGraphics.SnakeHeads[1], new Vector2(256f, 256f), Snake.Direction.Right, 2, Color.FromNonPremultiplied(176, 61, 201, 255));
                snakes.Add(snake);
            }

            if (clientsCount >= 2)
            {
                snake = new Snake();
                snake.Initialize(gameGraphics.SnakeTexture[2],gameGraphics.SnakeHeads[2], new Vector2(512f, 128f), Snake.Direction.Right, 3, Color.FromNonPremultiplied(253, 162, 4, 255));
                snakes.Add(snake);
            }

            if (clientsCount == 3)
            {
                snake = new Snake();
                snake.Initialize(gameGraphics.SnakeTexture[3],gameGraphics.SnakeHeads[3], new Vector2(512f, 256f), Snake.Direction.Up, 4, Color.FromNonPremultiplied(240, 255, 5, 255));
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

            client = new Client(ip, port, gameGraphics.SnakeTexture,gameGraphics.SnakeHeads);
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
            moveCounter++;

            if (moveCounter == PRIORITY_CHANGE_CONDITION)
            {
                moveCounter = 0;
                changePriorities(snakes);
            }


            foreach(Snake snake in snakes){
                if(snakeFood.IsEaten(snake)){
                    snake.AddPart(1);
                }

                //snake dies if it eats itself
                if (snake.CollidesWithItself())
                {
                    snake.IsGameOver = true;
                }

                snake.CheckIfEatenByEnemy(snakes);
            }

            checkIfGameFinished(snakes);

            


        }

        private void changePriorities(List<Snake> snakes)
        {
            int tempPriority1 = snakes.Last().Priority;
            int tempPriority2 = 0;

            foreach (Snake snake in snakes)
            {
                tempPriority2 = snake.Priority;
                snake.Priority = tempPriority1;
                tempPriority1 = tempPriority2;
            }
        }

        //TODO refactor
        private void updateSnakes(List<Snake> snakes,Boolean moveSnakes)
         {
            //the only logic a client has, is that he sets the current direction of its snake according to the user input
            if (isClient)
            {
                if (snakes.Count() > 0)
                {
                    Snake clientSnake = snakes.ElementAt(client.SnakeNumber);
                    setDirection(clientSnake);
                    client.ClientSnakeDirection = clientSnake.ChoosenSnakeDirection;
                }
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
                            checkSnakesForKilling(snakes);
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

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                tempDirection = Snake.Direction.Left;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                tempDirection = Snake.Direction.Right;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                tempDirection = Snake.Direction.Up;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down))
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
                snake.ChoosenSnakeDirection = tempDirection;
                return;
            }

            if (GameUtils.IsDirectionValid(snake,tempDirection))
            {
                snake.ActualSnakeDirection = tempDirection;
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
                            enemy.AddPart(snake.Body.Count()+1);
                            snake.IsGameOver = true;
                        }
                        else if (enemy.Priority < snake.Priority)
                        {
                            snake.AddPart(enemy.Body.Count()+1);
                            enemy.IsGameOver = true;
                        }

                    }

                }

            }


        }

        private void checkIfGameFinished(List<Snake> snakes)
        {
            int index = 0;
            int winner = 0;

            foreach (Snake snake in snakes)
            {
                if (snake.IsGameOver == false)
                {
                    index++;
                }
                else
                {
                    winner++;
                }
            }

            if (index <= 1)
            {
                 server.sendEndSignal(winner);

                 gameState = GameState.DISCONNECT_SERVER;   
            }
        }
    
    }
}
