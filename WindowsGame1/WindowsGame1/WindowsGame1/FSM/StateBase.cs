using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

//State pattern

namespace Snake.FSM
{
    abstract class StateBase
    {
        public abstract void Handle(Context context);

        public abstract void Draw(SpriteBatch spriteBatch,Texture2D snakePic,SpriteFont font);
    }
}
