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
    class State_StartServer:StateBase
    {
        private StateBase currentState;
        private NetworkMenuServer networkMenuServer;
        private StateBase oldState;
        private StateBase mainMenuState;
        private Vector2 menuPosition;


        public State_StartServer(Vector2 menuPosition,StateBase mainMenuState,StateBase oldState,NetworkMenuServer networkMenuServer)
        {
            this.oldState = oldState;
            this.mainMenuState = mainMenuState;
            this.menuPosition = menuPosition;
            this.networkMenuServer = networkMenuServer;
            this.currentState = this;
        }

        public void Update(Context context,ref Server server,ref Thread serverThread,ref Client client,ref Thread clientThread,GameTime gameTime)
        {
            if (!networkMenuServer.PortInput.CheckPortInput())
            {
                 context.state = oldState;
            }
            else
            {
                Int32 port = Convert.ToInt32(networkMenuServer.PortInput.InputText);
                server = new Server(port);
                serverThread = new System.Threading.Thread(server.Start);
                serverThread.Start();

                context.state = new State_NetworkMenuServerWaiting(menuPosition, mainMenuState);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to to here
        }
    }
}
