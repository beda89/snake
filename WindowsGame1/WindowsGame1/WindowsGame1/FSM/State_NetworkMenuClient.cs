using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class State_NetworkMenuClient:StateBase
    {
        private NetworkMenuClient networkMenuClient;

        public State_NetworkMenuClient(Vector2 menuPosition,StateBase mainMenuState)
        {
            networkMenuClient = new NetworkMenuClient(menuPosition,this,mainMenuState);
        }

        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            networkMenuClient.Update();
            context.state = networkMenuClient.CurrentState;
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuClient.Draw(spriteBatch, gameGraphics);
        }

    }
}
