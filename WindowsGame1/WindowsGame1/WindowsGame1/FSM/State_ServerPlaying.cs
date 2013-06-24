using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.Exceptions;

namespace Snake.FSM
{
    class State_ServerPlaying:StateBase
    {
        // priority of snakes is changed every 50 moves
        private const int PRIORITY_CHANGE_CONDITION = 50;

        // The time we display a frame until the next one
        private const int FRAME_TIME = 250;

        private int moveCondition = 0;
        private int moveCounter = 0;

        private SnakeFood snakeFood;
        private List<Snake> snakes;
        private GameField gameField;
        private Score score;

        private StateBase currentState;
        private StateBase mainMenuState;

        private KeyboardState currentKeyboardState;

        private int elapsedTime;

        public State_ServerPlaying(SnakeFood snakeFood, List<Snake> snakes, GameField gameField, Score score,StateBase mainMenuState)
        {
            this.snakeFood = snakeFood;
            this.snakes = snakes;
            this.gameField = gameField;
            this.score = score;
            this.mainMenuState = mainMenuState;
            currentState = this;
        }

        public void Update(ref Server server,ref Thread serverThread,ref Client client,ref Thread clientThread,GameTime gameTime)
        {
            inPlay(gameTime,server);

            currentKeyboardState = Keyboard.GetState();
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            drawPlayingGame(spriteBatch, gameGraphics);
        }

        public StateBase getCurrentState()
        {
            return currentState;
        }

        private void drawPlayingGame(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            if (snakeFood != null)
            {
                snakeFood.Draw(spriteBatch,gameGraphics);
            }

            if (snakes != null)
            {
                int index = 0;
                foreach (Snake snake in snakes)
                {
                    if (snake.IsGameOver)
                    {
                        continue;
                    }

                    snake.Draw(spriteBatch, gameGraphics, index);
                    index++;
                }

                gameField.Draw(spriteBatch,gameGraphics);

                //server is always snakeNumber 0
                score.Draw(spriteBatch, gameGraphics, snakes, 0);

            }
        }

        //only called by the server
        private void inPlay(GameTime gameTime,Server server)
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
                    //gameState=GameState.DISCONNECT_SERVER;

                    return;
                }

                //snakes are moved once during frameTime
                updateSnakes(snakes, true);
                gameLogic(snakes,server);

                elapsedTime = 0;
            }
            else
            {
                updateSnakes(snakes, false);

            }
        }


        /*
   * contains the whole gamelogic
   * it is all done by the server
   * 
   */
        private void gameLogic(List<Snake> snakes,Server server)
        {
            moveCounter++;

            if (moveCounter == PRIORITY_CHANGE_CONDITION)
            {
                moveCounter = 0;
                changePriorities(snakes);
            }


            foreach (Snake snake in snakes)
            {
                if (snakeFood.IsEaten(snake))
                {
                    snake.AddPart(1);
                }

                //snake dies if it eats itself
                if (snake.CollidesWithItself())
                {
                    snake.IsGameOver = true;
                }

                snake.CheckIfEatenByEnemy(snakes);
            }

            checkIfGameFinished(snakes,server);

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


        private void updateSnakes(List<Snake> snakes, Boolean moveSnakes)
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
                    if ((moveCondition % 2) == 0)
                    {
                        snake.Update(gameField);
                        checkSnakesForKilling(snakes);
                    }
                }
                index++;
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
                currentState = new State_Disconnect(mainMenuState);

                return ; 
            }else
            {
                return;
            }

            if (GameUtils.IsDirectionValid(snake,tempDirection))
            {
                snake.ActualSnakeDirection = tempDirection;
            }

        } 

        
        private void checkIfGameFinished(List<Snake> snakes,Server server)
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

                 currentState = new State_Disconnect(mainMenuState);
            }
        } 

        private void checkSnakesForKilling(List<Snake> snakes)
        {
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
                            enemy.AddPart(snake.Body.Count() + 1);
                            snake.IsGameOver = true;
                        }
                        else if (enemy.Priority < snake.Priority)
                        {
                            snake.AddPart(enemy.Body.Count() + 1);
                            enemy.IsGameOver = true;
                        }

                    }

                }

            }
        }
    }
}
