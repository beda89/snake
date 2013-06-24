using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    interface StateBase
    {
        void Update(ref Server server,ref Thread serverThread,ref Client client,ref Thread clientThread,GameTime gameTime);

        void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics);

        StateBase getCurrentState();
    }
}
