using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.FSM;


namespace Snake.Menus
{
    class MainMenu:Menu
    {
        public MainMenu(Vector2 startPosition,StateBase standardState)
            : base(startPosition,standardState)
        {
            items.Add(new MenuEntry("New Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), new State_NetworkMenuServer(startPosition,standardState)));
            items.Add(new MenuEntry("Join Game", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), new State_NetworkMenuClient(startPosition,standardState)));
           // items.Add(new MenuEntry("Exit", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y) * 2, ITEM_WIDTH, ITEM_HEIGHT), GameState.EXIT));
        }
    }
}
