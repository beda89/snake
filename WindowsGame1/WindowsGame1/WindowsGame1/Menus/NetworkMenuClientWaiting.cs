using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Snake.FSM;

namespace Snake.Menus
{
    class NetworkMenuClientWaiting:Menu
    {

        public NetworkMenuClientWaiting(Vector2 startPosition,StateBase menuState)
            : base(startPosition,menuState)
        {
       //     items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), GameState.DISCONNECT_CLIENT));
        }

        public override void Draw(SpriteBatch spriteBatch,Texture2D snakePic, SpriteFont font)
        {
            base.Draw(spriteBatch,snakePic,font);
            spriteBatch.DrawString(font, "Waiting for Server ...", new Vector2(startPosition.X+200, startPosition.Y), Color.Black);
        }
    }
}
