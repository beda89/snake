using System;
using System.Collections.Generic;
using System.Linq;
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
                            PLAY_SERVER, 
                            PLAY_CLIENT, 
                            FINISH_GAME,
                            EXIT };



    /// <summary>
    /// This is the main type for your game
    /// </summary>
    ///
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Snake snake;
        GameField gameField;
        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;
        Vector2 menuPosition;
        System.Threading.Thread serverThread;
        System.Threading.Thread clientThread;



       
        private GameState gameState;


        private Menu mainMenu;
        private Menu networkMenuServer;
        private NetworkMenuServerWaiting networkMenuServerWaiting;
        private Menu networkMenuClient;
        private Menu networkMenuClientWaiting;

        private Server server;
        private Client client;



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
            snake = new Snake();
            gameField = new GameField();
            mainMenu = new MainMenu(Content,menuPosition);
            networkMenuServer = new NetworkMenuServer(Content, menuPosition);
            networkMenuClient = new NetworkMenuClient(Content, menuPosition);
            networkMenuClientWaiting = new NetworkMenuClientWaiting(Content, menuPosition);
            networkMenuServerWaiting = new NetworkMenuServerWaiting(Content, menuPosition);
            gameState = GameState.MAIN_MENU;


            //Load snake texture from Content project
            Texture2D snakeTexture = Content.Load<Texture2D>("snakeTexture");
            snake.Initialize(snakeTexture, new Vector2(256f, 256f));

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
                    break;

                case GameState.NETWORK_MENU_CLIENT:
                    networkMenuClient.Update();
                    gameState=networkMenuClient.getCurrentState();
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_SERVER:
                    client = new Client("localhost",4000);
                    clientThread = new System.Threading.Thread(client.start);

                    networkMenuClientWaiting.Update();
                    gameState = networkMenuClientWaiting.getCurrentState();
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    server = new Server(4000);
                    serverThread = new System.Threading.Thread(server.start);
                    networkMenuServerWaiting.Update(server);
                    gameState = networkMenuServerWaiting.getCurrentState();
                    break;

                case GameState.NETWORK_MENU_SERVER:
                    networkMenuServer.Update();
                    gameState = networkMenuServer.getCurrentState();
                    break;

                case GameState.FINISH_GAME:
                    break;

                case GameState.PLAY_SERVER:
                    UpdateSnake(snake);
                    snake.Update(gameTime);
                    break;

                case GameState.EXIT:
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
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            base.Update(gameTime);
        }

        private void UpdateSnake(Snake snake)
        {
            Snake.Direction tempDirection;
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Left;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Right;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Up;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                tempDirection = Snake.Direction.Down;
            }
            else
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

            // TODO: Add your drawing code here
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

                case GameState.PLAY_CLIENT:
                    this.IsMouseVisible = false;
                    snake.Draw(spriteBatch);
                    gameField.Draw(spriteBatch);
                    break;

                case GameState.NETWORK_MENU_WAITING_FOR_CLIENTS:
                    networkMenuServerWaiting.Draw(spriteBatch);
                    break;

                case GameState.PLAY_SERVER:
                    this.IsMouseVisible = false;
                    snake.Draw(spriteBatch);
                    gameField.Draw(spriteBatch);
                    break;

            }

            spriteBatch.End();

         //   base.Draw(gameTime);
        }
    }
}
