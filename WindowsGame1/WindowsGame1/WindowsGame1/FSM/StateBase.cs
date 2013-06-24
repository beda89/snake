using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    interface StateBase
    {
        void Update(Server server,Thread serverThread, Client client, Thread clientThread);

        void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics);

        StateBase getCurrentState();
    }
}
