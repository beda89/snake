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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.IsMouseVisible = true;
            
            GraphicsDevice.Clear(Color.White);
            context.Draw(spriteBatch, gameGraphics, gameTime);
            
            base.Draw(gameTime);
        }  
    }
}
