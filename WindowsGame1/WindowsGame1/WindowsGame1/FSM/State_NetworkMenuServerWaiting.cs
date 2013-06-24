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
    class State_NetworkMenuServerWaiting:StateBase
    {
        private NetworkMenuServerWaiting networkMenuServerWaiting;

        public State_NetworkMenuServerWaiting(Vector2 menuPosition,StateBase mainMenuState)
        {
            networkMenuServerWaiting = new NetworkMenuServerWaiting(menuPosition,this, mainMenuState);
        }


        public void Update(Server server,Thread serverThread, Client client, Thread clientThread)
        {
            networkMenuServerWaiting.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuServerWaiting.Draw(spriteBatch, gameGraphics);
        }

        public StateBase getCurrentState()
        {
            return networkMenuServerWaiting.CurrentState;
        }
    }
}
