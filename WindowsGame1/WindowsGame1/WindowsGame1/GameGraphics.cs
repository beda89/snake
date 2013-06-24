using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    class GameGraphics
    {
        public SpriteFont CustomFont { get; private set; }
        public Texture2D[] SnakeTexture { get; private set; }
        public Texture2D SnakePic { get; private set; }
        public Texture2D BoundsTexture { get; private set; }
        public Texture2D RedAppleTexture { get; private set; }
        public Texture2D[][] SnakeHeads { get; private set; }


        public GameGraphics(SpriteFont customFont,Texture2D[] snakeTexture,Texture2D snakePic, Texture2D boundsTexture, Texture2D redAppleTexture,Texture2D[][] snakeHeads)
        {
            this.CustomFont = customFont;
            this.SnakeTexture = snakeTexture;
            this.SnakePic = snakePic;
            this.BoundsTexture = boundsTexture;
            this.RedAppleTexture = redAppleTexture;
            this.SnakeHeads = snakeHeads;
        }


        public void Dispose()
        {
            SnakePic.Dispose();
            BoundsTexture.Dispose();
            RedAppleTexture.Dispose();

            foreach (Texture2D texture in SnakeTexture)
            {
                texture.Dispose();
            }

            foreach (Texture2D[] textures in SnakeHeads)
            {
                foreach (Texture2D texture in textures)
                {
                    texture.Dispose();
                }
            }

        }

    }
}
