using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Menus
{
    class NetworkMenuClient:Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;


        public NetworkMenuClient(ContentManager content, Vector2 startPosition)
            : base(content,GameState.NETWORK_MENU_CLIENT)
        {

            this.startPosition = startPosition;

            items.Add(new MenuEntry("Join Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.NETWORK_MENU_WAITING_FOR_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));
        }
    }
}
