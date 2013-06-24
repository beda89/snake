using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    class State_ClientInit:StateBase
    {
        public State_ClientInit()
        {

        }

        public void Update(ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
           //nothing to do here
        }

        public StateBase getCurrentState()
        {
            throw new NotImplementedException();
        }
    }
}
