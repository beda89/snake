using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    class Context
    {
        private StateBase state;

        public Context(StateBase state)
        {
            this.state = state;

        }

        public void Update(Server server, Client client)
        {

        }


        public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {



        }

    }
}
