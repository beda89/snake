using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    class Score
    {
        private const int Y_POSITION = 5;
        private const int X_SPACE = 190;

        public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics,List<Snake> snakes,int playerNumber)
        {
            Snake player=null;
            List<Snake> enemies = new List<Snake>();

            int index = 0;

            foreach (Snake snake in snakes)
            {
                if (index == playerNumber)
                {
                    player = snake;
                }
                else
                {
                    enemies.Add(snake);
                }

                index++;
            }

            if(player!=null){
                if (player.IsGameOver)
                {
                    spriteBatch.DrawString(gameGraphics.CustomFont, "Player: KILLED", new Vector2(20, Y_POSITION), player.SnakeColor);
                }
                else
                {
                    spriteBatch.DrawString(gameGraphics.CustomFont, "Player: " + player.Priority, new Vector2(20, Y_POSITION), player.SnakeColor);
                }
            }

            index=1;

            foreach (Snake enemy in enemies)
            {
                if (enemy.IsGameOver)
                {
                    spriteBatch.DrawString(gameGraphics.CustomFont, "Enemy" + index + ": KILLED", new Vector2(20 + X_SPACE * index, Y_POSITION), enemy.SnakeColor);
                }
                else
                {
                    spriteBatch.DrawString(gameGraphics.CustomFont, "Enemy" + index + ": " + enemy.Priority, new Vector2(20 + X_SPACE * index, Y_POSITION), enemy.SnakeColor);
                }
                index++;
            }


        }
    }
}
