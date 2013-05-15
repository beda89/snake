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


        public void Initialize(Texture2D texture,GraphicsDeviceManager graphics)
        {
            this.Texture = texture;
            this.graphics = graphics;


            this.gameFieldWidth = graphics.GraphicsDevice.Viewport.Width;
            this.gameFieldHeight = graphics.GraphicsDevice.Viewport.Height;


            boundPositions=new List<Vector2>();

            for (int i = 0; i < gameFieldWidth; i = i + 16)
            {
                boundPositions.Add(new Vector2(i,0));
                boundPositions.Add(new Vector2(i, gameFieldHeight- Texture.Height));
            }

            for (int i = 0; i < gameFieldHeight; i = i + 16)
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


        public Boolean collides(Snake snake){
            if (snake.Position.X < 16 || snake.Position.X > (gameFieldWidth-Texture.Width))
            {
                return true;
            }

            if (snake.Position.Y < 16 || snake.Position.Y > (gameFieldWidth - Texture.Width))
            {
                return true;
            }

            return false;
        }
    }
}
