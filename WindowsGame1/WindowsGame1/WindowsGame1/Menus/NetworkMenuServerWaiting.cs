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
        private List<String> clientList;

        public NetworkMenuServerWaiting(Texture2D snakePic,SpriteFont customFont, Vector2 startPosition) : base(snakePic,customFont,startPosition,GameState.NETWORK_MENU_WAITING_FOR_CLIENTS)
        {
            clientList= new List<String>();

            items.Add(new MenuEntry("Start Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, WIDTH, HEIGHT), GameState.PLAY_SERVER));
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), WIDTH, HEIGHT), GameState.DISCONNECT_SERVER));
        }

        public void Update(Server server)
        {
            if (server.CurrentClients.Count() != clientList.Count())
            {
                //TODO: use table to represent joined clients
                foreach (TcpClient client in server.CurrentClients)
                {
                    clientList = new List<String>();
                    clientList.Add(client.ToString());
                }
            }

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
