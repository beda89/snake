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
    class State_NetworkMenuClientWaiting:StateBase
    {
        private NetworkMenuClientWaiting networkMenuClientWaiting;

        private StateBase currentState;
        private StateBase mainMenuState;

        public State_NetworkMenuClientWaiting(Vector2 menuPosition,StateBase networkMenuState,StateBase mainMenuState)
        {
            networkMenuClientWaiting = new NetworkMenuClientWaiting(menuPosition,this,mainMenuState,networkMenuState);

            currentState = this;
            this.mainMenuState = mainMenuState;
        }

        public void Update(ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            networkMenuClientWaiting.Update();

            if (client.ClientGameState == ClientState.DISCONNECT)
            {
                currentState = mainMenuState;
            }
            else if (client.ClientGameState == ClientState.PLAYING)
            {
                currentState = new State_ClientPlaying(mainMenuState);
            }


        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuClientWaiting.Draw(spriteBatch, gameGraphics);

        }

        public StateBase getCurrentState()
        {
            if (networkMenuClientWaiting.CurrentState is MainMenu)
            {
                return networkMenuClientWaiting.CurrentState;
            }
            else
            {
                return currentState;

            }

        }
    }
}
