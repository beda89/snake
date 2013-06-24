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

        public void Update(Server server, Thread serverThread, Client client, Thread clientThread)
        {
            networkMenuClient.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuClient.Draw(spriteBatch, gameGraphics);
        }

        public StateBase getCurrentState()
        {
            return networkMenuClient.CurrentState;
        }
    }
}
