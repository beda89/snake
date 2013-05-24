using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Menus
{
    class NetworkMenuServerWaiting:Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;
        private List<String> clientList = new List<String>();


        public NetworkMenuServerWaiting(ContentManager content, Vector2 startPosition) : base(content,GameState.NETWORK_MENU_WAITING_FOR_CLIENTS)
        {
            this.startPosition = startPosition;
            items.Add(new MenuEntry("Start Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.PLAY_SERVER));
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.DISCONNECT_SERVER));
        }

        public void Update(Server server)
        {
            if (server.getCurrentClients().Count() != clientList.Count())
            {
                foreach (TcpClient client in server.getCurrentClients())
                {
                    //TODO change for better performance
                    clientList = new List<String>();
                    clientList.Add(client.ToString());
                }
            }

            //    items.Add(new MenuEntry("GEIL", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));
            base.Update();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            int index=0;
            foreach (string item in clientList){
                spriteBatch.DrawString(font, item, new Vector2(10, index*30), Color.Black);
            }


            base.Draw(spriteBatch);
        }
    }
}
