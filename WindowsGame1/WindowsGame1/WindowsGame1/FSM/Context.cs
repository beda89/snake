using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

//State pattern

namespace Snake.FSM
{
    class Context
    {
        public StateBase State { get; set; }

        private Texture2D snakePic;
        private SpriteFont font;
        
        public Context(StateBase State,Texture2D snakePic, SpriteFont font)
        {
            this.State = State;
            this.font = font;
            this.snakePic = snakePic;
        }

        public void Update()
        {
            State.Handle(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            State.Draw(spriteBatch,snakePic,font);
        }
    }
}
