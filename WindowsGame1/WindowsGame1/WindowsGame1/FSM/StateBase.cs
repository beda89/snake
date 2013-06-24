using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    interface StateBase
    {
        void Update(Server server, Client client);

        void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics);
    }
}
