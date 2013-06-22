using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class NetworkMenuServer_State:StateBase
    {
        private Menu networkMenuServer; 

        public NetworkMenuServer_State(Vector2 menuPosition)
        {
            networkMenuServer = new NetworkMenuServer(menuPosition, this);
        }

        public override void Handle(Context context)
        {
            networkMenuServer.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            networkMenuServer.Draw(spriteBatch, snakePic, font);
        }

    }
}
