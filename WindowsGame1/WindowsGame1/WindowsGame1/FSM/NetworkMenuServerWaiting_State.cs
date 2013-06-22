using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class NetworkMenuServerWaiting_State:StateBase
    {

        private Menu networkMenuServerWaiting;

        public NetworkMenuServerWaiting_State(Vector2 menuPosition)
        {
            networkMenuServerWaiting = new NetworkMenuServerWaiting(menuPosition, this);
        }

        public override void Handle(Context context)
        {
            networkMenuServerWaiting.Update(context);

        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            networkMenuServerWaiting.Draw(spriteBatch, snakePic, font);
        }
    }
}
