using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class NetworkMenuClientWaiting_State:StateBase 
    {
        private Menu networkMenuClientWaiting;

        public NetworkMenuClientWaiting_State(Vector2 menuPosition){
            networkMenuClientWaiting = new NetworkMenuClientWaiting(menuPosition,this);
        }

        public override void Handle(Context context)
        {
            networkMenuClientWaiting.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            throw new NotImplementedException();
        }

    }
}
