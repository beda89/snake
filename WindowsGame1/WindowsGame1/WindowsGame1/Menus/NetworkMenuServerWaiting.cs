using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Menus
{
    class NetworkMenuServerWaiting:Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;


        public NetworkMenuServerWaiting(ContentManager content, Vector2 startPosition) : base(content,GameState.NETWORK_MENU_WAITING_FOR_CLIENTS)
        {
            this.startPosition = startPosition;
            items.Add(new MenuEntry("Start Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.PLAY_SERVER));
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));
        }

        public void Update(Server server)
        {
            if (server.getCurrentClients().Count > 0)
            {
                items.Add(new MenuEntry("GEIL", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));
            }

            base.Update();
        }


        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);


        }
    }
}
