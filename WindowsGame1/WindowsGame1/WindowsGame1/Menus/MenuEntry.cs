using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Menus
{
    class MenuEntry
    {
        public Rectangle Position { set; get; }
        public Color BackgroundColor { set; get; }
        public Color Color { set; get; }
        public GameState Gamestate { set; get; }

        private String text;


        public MenuEntry(String text, Color color, Color backgroundColor, Rectangle position,GameState gameState)
        {
            this.text = text;
            this.Color = color;
            this.BackgroundColor = backgroundColor;
            this.Position = position;
            this.Gamestate = gameState;
        }

        public void Draw(SpriteBatch spriteBatch,SpriteFont font)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { BackgroundColor });
            spriteBatch.Draw(pixel, Position, Color.White);
            spriteBatch.DrawString(font, text, new Vector2(Position.Left+10,Position.Top), Color);
        }

    }
}
