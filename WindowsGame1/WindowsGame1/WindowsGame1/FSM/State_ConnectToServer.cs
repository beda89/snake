using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Snake.Menus;

namespace Snake.FSM
{
    class State_ConnectToServer:StateBase
    {
        private NetworkMenuClient networkMenuClient;
        private Vector2 menuPosition;
        private StateBase mainMenuState;
        private StateBase oldState;

        public State_ConnectToServer(Vector2 menuPosition,StateBase mainMenuState,StateBase oldState,NetworkMenuClient networkMenuClient)
        {
            this.menuPosition = menuPosition;
            this.mainMenuState = mainMenuState;
            this.oldState = oldState;
            this.networkMenuClient = networkMenuClient;
        }


        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            if (!networkMenuClient.InputFieldsAreValid())
            {
                context.state = oldState;
            }
            else
            {
                tryConnectionToServer(ref client,ref clientThread);
                context.state = new State_NetworkMenuClientWaiting(menuPosition,mainMenuState);
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to do here
        }

        private void tryConnectionToServer(ref Client client,ref Thread clientThread)
        {
            int port = Convert.ToInt32(networkMenuClient.PortInput.InputText);
            String ip = networkMenuClient.IpInput.InputText;

            client = new Client(ip, port);
            clientThread = new System.Threading.Thread(client.Start);
            clientThread.Start();
        } 

    }
}
