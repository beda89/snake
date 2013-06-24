using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    class Context
    {
        public StateBase state{get;set;}

        public Context(StateBase state)
        {
            this.state = state;

        }

        public void Update(Server server,Thread serverThread, Client client,Thread clientThread, GameTime gameTime)
        {
            state.Update(server,serverThread,client,clientThread);

            state = state.getCurrentState();
        }


        public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics,GameTime gameTime)
        {
            spriteBatch.Begin();
            
            state.Draw(spriteBatch,gameGraphics);

            spriteBatch.End();
        }

    }
}
