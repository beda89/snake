using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Snake.FSM
{
    class State_Disconnect:StateBase
    {
        private StateBase currentState;
        private StateBase mainMenu;

        public State_Disconnect(StateBase mainMenu)
        {
            this.mainMenu = mainMenu;
        }

        public void Update(ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            if(server!=null){
                server.Stop();
            }

            if (serverThread != null)
            {
                serverThread.Abort();
            }

            if (client != null)
            {
                client.Stop();
            }

            if (clientThread != null)
            {
                clientThread.Abort();
            }

            server=null;
            serverThread=null;
            client=null;
            clientThread=null;

            currentState = mainMenu;

        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to do here
        }

        public StateBase getCurrentState()
        {
            return currentState;
        }
    }
}
