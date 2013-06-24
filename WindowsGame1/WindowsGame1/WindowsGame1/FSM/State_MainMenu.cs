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

        public void Update(Server server, Thread serverThread, Client client, Thread clientThread)
        {
            mainMenu.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameGraphics gameGraphics)
        {

            mainMenu.Draw(spriteBatch, gameGraphics);
        }

        public StateBase getCurrentState()
        {
            return mainMenu.CurrentState;
        }

    }
}
