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

        public void Update(ref Server server,ref Thread serverThread,ref Client client,ref Thread clientThread, GameTime gameTime)
        {
            state.Update(ref server,ref serverThread,ref client,ref clientThread,gameTime);

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
