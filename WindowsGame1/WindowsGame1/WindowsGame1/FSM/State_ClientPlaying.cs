using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake.FSM
{
    class State_ClientPlaying:StateBase
    {
        //TopBound of the gameField (space above is used for current score)
        private const int TOPBOUND_Y = 32;

        private List<Snake> snakes;
        private SnakeFood snakeFood;
        private GameField gameField;
        private Score score;
        private KeyboardState currentKeyboardState;
        private StateBase mainMenuState;
        private int clientSnakeNumber = 1;

        public State_ClientPlaying(StateBase mainMenuState)
        {
            this.mainMenuState = mainMenuState;

            snakeFood = new SnakeFood();
            gameField = new GameField();
            gameField.Initialize(TOPBOUND_Y);

            score = new Score();
        }


        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            clientSnakeNumber = client.SnakeNumber;
            snakes = client.Snakes;
            snakeFood.Position = client.SnakeFoodPosition;
          
            currentKeyboardState = Keyboard.GetState();

            updateSnakes(context,snakes, client);

            if (client.ClientGameState == ClientState.DISCONNECT)
            {
                context.state = new State_Disconnect(mainMenuState);
            }

        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            drawPlayingGame(spriteBatch, gameGraphics);
        }

        private void drawPlayingGame(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            if (snakeFood != null)
            {
                snakeFood.Draw(spriteBatch, gameGraphics);
            }

            if (snakes != null)
            {
                int index = 0;
                foreach (Snake snake in snakes)
                {
                    if (snake.IsGameOver)
                    {
                        index++;
                        continue;
                    }

                    snake.Draw(spriteBatch, gameGraphics, index);
                    index++;
                }

                gameField.Draw(spriteBatch, gameGraphics);

                //server is always snakeNumber 0
                score.Draw(spriteBatch, gameGraphics, snakes, clientSnakeNumber );
            }
        }



          private void updateSnakes(Context context,List<Snake> snakes,Client client)
          {
            //the only logic a client has, is that he sets the current direction of its snake according to the user input
                if (snakes.Count() > 0)
                {
                    Snake clientSnake = snakes.ElementAt(client.SnakeNumber);
                    setDirection(context,clientSnake);
                    client.ClientSnakeDirection = clientSnake.ChoosenSnakeDirection;
                }
        }


        private void setDirection(Context context,Snake snake){
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
                context.state = new State_Disconnect(mainMenuState);
                return ; 
            }else
            {
                return;
            }

            //direction is sent to server so it doesn't have to be checked for validity => server does that
             snake.ChoosenSnakeDirection = tempDirection;
        }
    }
}
