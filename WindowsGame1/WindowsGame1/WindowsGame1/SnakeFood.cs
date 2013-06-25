using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    class SnakeFood
    {
        public Vector2 Position { get; set; }
        private int gameFieldWidth;
        private int gameFieldHeight;
        private int topGameFieldBound;

        public void Initialize(int topGameFieldBound)
        {
            this.gameFieldWidth = 800;
            this.gameFieldHeight = 480;
            this.topGameFieldBound = topGameFieldBound;

            Position = generateNewPosition();
        }


        public Boolean IsEaten(Snake snake)
        {
            if (snake.Head.Equals(Position))
            {
                Position = generateNewPosition();
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {
            spriteBatch.Draw(gameGraphics.RedAppleTexture, Position, Color.White);
        }

        //generates a random position inside the game field
        private Vector2 generateNewPosition()
        {
            Random rndm = new Random();

            //inside the gamefield (position 0 is border and position gamefieldWidth is also border)
            //border texture is 16px
            // division by 16 is needed because we have a gameField with 16px * 16px segments and we don't want to have the texture placed between to segments
            int x = rndm.Next(2,gameFieldWidth/16 - 1);

            //top border is 3 segments beneath top
            //bottom border is 1 segment over bottom
            // division by 16 is needed because we have a gameField with 16px * 16px segments and we don't want to have the texture placed between to segments
            int y = rndm.Next(topGameFieldBound/16+1, gameFieldHeight/16 -1);

            return new Vector2(x*16, y*16);
        } 

    }
}
