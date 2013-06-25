using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Snake.FSM
{
    class State_ServerInit:StateBase
    {
        //TopBound of the gameField (space above is used for current score)
        private const int TOPBOUND_Y = 32;

        private List<Snake> snakes;

        private GameField gameField;
        private SnakeFood snakeFood;
        private Score score;

        private StateBase mainMenuState;

        public State_ServerInit(StateBase mainMenuState)
        {
            gameField = new GameField();
            gameField.Initialize(TOPBOUND_Y);

            snakeFood = new SnakeFood();
            snakeFood.Initialize(TOPBOUND_Y);

            score = new Score();

            this.mainMenuState = mainMenuState;
        }


        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            initServerGame(server);

            context.state = new State_ServerPlaying(snakeFood, snakes, gameField, score,mainMenuState);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to do here
        }

        private void initServerGame(Server server)
        {
            //init snakes, first one is needed for server
            snakes = new List<Snake>();
            Snake snake = new Snake();

            snake.Initialize(new Vector2(128f, 64f), Snake.Direction.Right, 1, Color.FromNonPremultiplied(81, 220, 50, 255));
            snakes.Add(snake);

            int clientsCount = server.CurrentClients.Count();

            if (clientsCount >= 1)
            {
                snake = new Snake();
                snake.Initialize(new Vector2(256f, 256f), Snake.Direction.Right, 2, Color.FromNonPremultiplied(176, 61, 201, 255));
                snakes.Add(snake);
            }

            if (clientsCount >= 2)
            {
                snake = new Snake();
                snake.Initialize( new Vector2(512f, 128f), Snake.Direction.Right, 3, Color.FromNonPremultiplied(253, 162, 4, 255));
                snakes.Add(snake);
            }

            if (clientsCount == 3)
            {
                snake = new Snake();
                snake.Initialize(new Vector2(512f, 256f), Snake.Direction.Up, 4, Color.FromNonPremultiplied(240, 255, 5, 255));
                snakes.Add(snake);
            }

            //sending startsignals to all clients
            try
            {
                server.sendStartSignal(snakes, snakeFood);
            }
            catch (IOException)
            {
                //we don't care, next communication will also fail and than we disconnect
            }
       }

    }
}
