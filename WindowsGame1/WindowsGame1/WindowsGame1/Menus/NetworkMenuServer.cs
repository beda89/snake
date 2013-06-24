using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.FSM;
using Snake.Menus.Input;

namespace Snake.Menus
{
    class NetworkMenuServer: Menu
    {   
        public PortInputField PortInput { get; private set; }

        public NetworkMenuServer(Vector2 startPosition,StateBase standardState,StateBase mainMenuState)
            : base(startPosition,standardState)
        {
            items.Add(new MenuEntry("Start Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), new State_StartServer(startPosition,mainMenuState,standardState,this)));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), mainMenuState));

            PortInput = new PortInputField("Port:", new Vector2(startPosition.X, 100), Color.Black,6);
            PortInput.Focus();
        }

        public override void Update()
        {
            PortInput.Update();
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {
            base.Draw(spriteBatch, gameGraphics);

            PortInput.Draw(spriteBatch,gameGraphics);
        }

        public Boolean hasValidInput()
        {
             return  PortInput.CheckPortInput();
        }
    }
}
