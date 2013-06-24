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

        public void Update(Server server, Thread serverThread, Client client, Thread clientThread)
        {
            networkMenuServer.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuServer.Draw(spriteBatch, gameGraphics);
        }

        public StateBase getCurrentState()
        {
            return networkMenuServer.CurrentState;
        }
    }
}
