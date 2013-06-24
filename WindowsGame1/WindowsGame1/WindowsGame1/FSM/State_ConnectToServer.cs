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
        private StateBase currentState;

        public State_ConnectToServer(Vector2 menuPosition,StateBase mainMenuState,StateBase oldState,NetworkMenuClient networkMenuClient)
        {
            this.menuPosition = menuPosition;
            this.mainMenuState = mainMenuState;
            this.oldState = oldState;
            this.networkMenuClient = networkMenuClient;
            this.currentState = this;
        }


        public void Update(ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            if (!networkMenuClient.InputFieldsAreValid())
            {
                currentState = oldState;
            }
            else
            {
                tryConnectionToServer(ref client,ref clientThread);
                currentState = new State_NetworkMenuClientWaiting(menuPosition,oldState,mainMenuState);
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            //nothing to do here
        }

        public StateBase getCurrentState()
        {
            return currentState;
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
