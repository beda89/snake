using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace WindowsGame1.Menus
{
    class MainMenu:Menu
    {
        private const int HEIGHT = 30;
        private ContentManager content;
        private Vector2 startPosition;

        public MainMenu(ContentManager content, Vector2 startPosition): base(content,GameState.MAIN_MENU)
        {
            this.content = content;

            this.startPosition = startPosition;

            items.Add(new MenuEntry("New Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.NETWORK_MENU_SERVER));
            items.Add(new MenuEntry("Join Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y+(HEIGHT+5), 150, HEIGHT), GameState.NETWORK_MENU_CLIENT));
            items.Add(new MenuEntry("Exit",Color.Black,Color.Green,new Rectangle((int)startPosition.X,(int)startPosition.Y+(HEIGHT+5)*2,150,HEIGHT),GameState.EXIT));
        }
    }
}
