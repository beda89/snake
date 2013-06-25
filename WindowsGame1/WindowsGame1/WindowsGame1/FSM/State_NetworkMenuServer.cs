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
    class State_NetworkMenuServer:StateBase
    {
        private NetworkMenuServer networkMenuServer;

        public State_NetworkMenuServer(Vector2 menuPosition,StateBase mainMenuState)
        {
            networkMenuServer = new NetworkMenuServer(menuPosition,this,mainMenuState);
        }

        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            networkMenuServer.Update();
            context.state = networkMenuServer.CurrentState;
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuServer.Draw(spriteBatch, gameGraphics);
        }
    }
}
