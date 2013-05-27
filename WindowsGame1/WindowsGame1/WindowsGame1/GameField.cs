using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class GameField
    {
        // The image representing a border segment
        public Texture2D Texture;

        private GraphicsDeviceManager graphics;
        private List<Vector2> boundPositions;
        private int gameFieldWidth;
        private int gameFieldHeight;
        private int topBorderY;


        public void Initialize(int topBorderY,Texture2D texture,GraphicsDeviceManager graphics)
        {
            this.Texture = texture;
            this.graphics = graphics;


            this.gameFieldWidth = graphics.GraphicsDevice.Viewport.Width;
            this.gameFieldHeight = graphics.GraphicsDevice.Viewport.Height;
            this.topBorderY = topBorderY;


            boundPositions=new List<Vector2>();

            for (int i = 0; i < gameFieldWidth; i = i + 16)
            {
                boundPositions.Add(new Vector2(i,topBorderY));
                boundPositions.Add(new Vector2(i, gameFieldHeight- Texture.Height));
            }

            for (int i = topBorderY; i < gameFieldHeight; i = i + 16)
            {
                boundPositions.Add(new Vector2(0,i));
                boundPositions.Add(new Vector2(gameFieldWidth - Texture.Width, i));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Vector2 vector in boundPositions){
                spriteBatch.Draw(Texture, vector, Color.White);
            }
        }


        public Boolean collides(Vector2 Position){
            if (Position.X <= Texture.Width || Position.X > (gameFieldWidth-Texture.Width))
            {
                return true;
            }

            if (Position.Y <= (topBorderY+Texture.Height) || Position.Y > (gameFieldHeight - Texture.Height))
            {
                return true;
            }

            return false;
        }
    }
}
