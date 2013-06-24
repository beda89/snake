using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    class GameField
    {
        private const int TEXTURE_SIZE = 16;

        // The image representing a border segment
        private List<Vector2> boundPositions;
        private int gameFieldWidth;
        private int gameFieldHeight;
        private int topBorderY;

        public void Initialize(int topBorderY)
        {

            this.gameFieldWidth = 800;
            this.gameFieldHeight = 480;
            this.topBorderY = topBorderY;

            boundPositions=new List<Vector2>();

            for (int i = 0; i < gameFieldWidth; i = i + 16)
            {
                boundPositions.Add(new Vector2(i,topBorderY));
                boundPositions.Add(new Vector2(i, gameFieldHeight- 16));
            }

            for (int i = topBorderY; i < gameFieldHeight; i = i + 16)
            {
                boundPositions.Add(new Vector2(0,i));
                boundPositions.Add(new Vector2(gameFieldWidth - 16, i));
            }
        }

        public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {
            foreach(Vector2 vector in boundPositions){
                spriteBatch.Draw(gameGraphics.BoundsTexture, vector, Color.White);
            }
        }

        public Boolean Collides(Vector2 position){
            if (position.X < TEXTURE_SIZE || position.X >= (gameFieldWidth - TEXTURE_SIZE))
            {
                return true;
            }

            if (position.Y < (topBorderY + TEXTURE_SIZE) || position.Y >= (gameFieldHeight - TEXTURE_SIZE))
            {
                return true;
            }

            return false;
        }
    }
}
