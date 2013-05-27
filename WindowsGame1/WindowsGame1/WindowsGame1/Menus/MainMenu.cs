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
        public MainMenu(Texture2D snakePic, SpriteFont customFont, Vector2 startPosition)
            : base(snakePic,customFont,startPosition, GameState.MAIN_MENU)
        {

            items.Add(new MenuEntry("New Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, WIDTH, HEIGHT), GameState.NETWORK_MENU_SERVER));
            items.Add(new MenuEntry("Join Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y+(HEIGHT+5), WIDTH, HEIGHT), GameState.NETWORK_MENU_CLIENT));
            items.Add(new MenuEntry("Exit",Color.Black,Color.Green,new Rectangle((int)startPosition.X,(int)startPosition.Y+(HEIGHT+5)*2,WIDTH,HEIGHT),GameState.EXIT));
        }
    }
}
