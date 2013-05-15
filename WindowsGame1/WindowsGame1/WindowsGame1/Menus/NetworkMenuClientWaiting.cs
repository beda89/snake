using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Menus
{
    class NetworkMenuClientWaiting:Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;


        public NetworkMenuClientWaiting(ContentManager content, Vector2 startPosition) : base(content,GameState.NETWORK_MENU_WAITING_FOR_SERVER)
        {
            this.startPosition = startPosition;
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(base.font, "Successfully connected to Server!", new Vector2(startPosition.X+200, startPosition.Y), Color.Black);
            spriteBatch.DrawString(base.font, "Waiting for Server to start ...", new Vector2(startPosition.X+200, startPosition.Y+HEIGHT+5), Color.Black);
        }
    }
}
