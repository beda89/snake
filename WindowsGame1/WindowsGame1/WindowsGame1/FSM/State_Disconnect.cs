using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    class State_Disconnect:StateBase
    {
        private StateBase mainMenu;

        public State_Disconnect(StateBase mainMenu)
        {
            this.mainMenu = mainMenu;
        }

        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
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

            context.state = mainMenu;

        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to do here
        }
    }
}
