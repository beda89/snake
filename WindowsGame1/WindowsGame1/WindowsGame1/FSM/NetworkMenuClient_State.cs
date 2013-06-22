using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class NetworkMenuClient_State:StateBase
    {
        private Menu networkMenuClient;

        public NetworkMenuClient_State(Vector2 menuPosition){
            networkMenuClient=new NetworkMenuClient(menuPosition,this);
        }

        public override void Handle(Context context)
        {
            networkMenuClient.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            networkMenuClient.Draw(spriteBatch,snakePic,font);
        }
    }
}
