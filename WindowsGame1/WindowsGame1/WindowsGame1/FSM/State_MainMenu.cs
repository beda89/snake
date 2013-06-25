using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Menus;

namespace Snake.FSM
{
    class State_MainMenu:StateBase
    {
        private MainMenu mainMenu;

       
        public State_MainMenu(Vector2 menuPosition)
        {
            mainMenu = new MainMenu(menuPosition,this);
        }

        public void Update(Context context,ref Server server, ref Thread serverThread, ref Client client, ref Thread clientThread, GameTime gameTime)
        {
            mainMenu.Update();
            context.state = mainMenu.CurrentState;
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {
            mainMenu.Draw(spriteBatch, gameGraphics);
        }

    }
}
