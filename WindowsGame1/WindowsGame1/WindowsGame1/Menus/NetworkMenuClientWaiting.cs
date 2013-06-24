using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.Menus
{
    class NetworkMenuClientWaiting //:Menu
    {
        /*
        public NetworkMenuClientWaiting(Texture2D snakePic, SpriteFont customFont, Vector2 startPosition)
            : base(snakePic, customFont,startPosition, GameState.NETWORK_MENU_WAITING_FOR_SERVER)
        {
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), GameState.DISCONNECT_CLIENT));
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(base.font, "Waiting for Server ...", new Vector2(startPosition.X+200, startPosition.Y), Color.Black);
        } */
    }
}
