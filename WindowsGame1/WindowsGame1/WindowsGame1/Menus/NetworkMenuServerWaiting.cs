using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Snake.FSM;

namespace Snake.Menus
{
    class NetworkMenuServerWaiting:Menu
    {
        private List<String> clientList;

        public NetworkMenuServerWaiting(Vector2 startPosition,StateBase standardState,StateBase mainMenuState) : base(startPosition,standardState)
        {
            clientList= new List<String>();

            items.Add(new MenuEntry("Start Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), new State_ServerInit()));
            items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), new State_Disconnect(mainMenuState)));
        }

        public void Update(Server server)
        {
            if (server.CurrentClients.Count() != clientList.Count())
            {
                clientList = new List<String>();

                foreach (TcpClient client in server.CurrentClients)
                {
                    clientList.Add(client.ToString());
                }
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {

            spriteBatch.DrawString(gameGraphics.CustomFont,"Enemies:"+clientList.Count(), new Vector2(50, 200), Color.Black);
            base.Draw(spriteBatch,gameGraphics);
        }
         
    }
}
