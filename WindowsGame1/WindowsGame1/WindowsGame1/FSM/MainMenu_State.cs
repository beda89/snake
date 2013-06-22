using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class MainMenu_State:StateBase
    {
        private Menu mainMenu;

        public MainMenu_State(Vector2 menuPosition)
        {
            mainMenu = new MainMenu(menuPosition, this);
        }

        public override void Handle(Context context)
        {
            mainMenu.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            mainMenu.Draw(spriteBatch,snakePic,font);
        }

    }
}
