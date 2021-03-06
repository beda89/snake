﻿using System;
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

        public State_NetworkMenuClientWaiting(Vector2 menuPosition,StateBase mainMenuState)
        {
            networkMenuClientWaiting = new NetworkMenuClientWaiting(menuPosition,this,mainMenuState);

            currentState = this;
            this.mainMenuState = mainMenuState;
        }

        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            networkMenuClientWaiting.Update();

            if (client.ClientGameState == ClientState.DISCONNECT)
            {
                context.state = new State_Disconnect(mainMenuState);
            }
            else if (client.ClientGameState == ClientState.PLAYING)
            {
                context.state = new State_ClientPlaying(mainMenuState);
            }

            StateBase temp= networkMenuClientWaiting.CurrentState;
            if (temp is State_Disconnect)
            {
                context.state = temp;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            networkMenuClientWaiting.Draw(spriteBatch, gameGraphics);
        }
    }
}
