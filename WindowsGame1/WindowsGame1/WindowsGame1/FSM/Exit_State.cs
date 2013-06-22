using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.FSM
{
    class Exit_State:StateBase
    {

        public override void Handle(Context context)
        {
        //    System.Exit();
        }

        public override void Draw(SpriteBatch spriteBatch,Texture2D snakePic,SpriteFont font)
        {
            //nothing to do here
        }

    }
}
