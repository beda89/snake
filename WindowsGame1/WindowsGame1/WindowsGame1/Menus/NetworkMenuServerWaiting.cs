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

        public NetworkMenuServerWaiting(Vector2 startPosition,StateBase menuState) : base(startPosition,menuState)
        {
            clientList= new List<String>();

         //   items.Add(new MenuEntry("Start Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), GameState.PLAY_SERVER));
         //   items.Add(new MenuEntry("Cancel", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), GameState.DISCONNECT_SERVER));
        }

        public void Update(Context context)
        {
            /*
            if (server.CurrentClients.Count() != clientList.Count())
            {
                //TODO: use table to represent joined clients
                foreach (TcpClient client in server.CurrentClients)
                {
                    clientList = new List<String>();
                    clientList.Add(client.ToString());
                }
            } */

            base.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch,Texture2D snakePic, SpriteFont font)
        {

            //TODO: Change to list
            int index=0;
            foreach (string item in clientList){
                spriteBatch.DrawString(font, item, new Vector2(10, index*30), Color.Black);
            }

            base.Draw(spriteBatch,snakePic,font);
        }
    }
}
