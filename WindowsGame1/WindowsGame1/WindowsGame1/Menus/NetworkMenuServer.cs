using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.Menus.Input;

namespace Snake.Menus
{
    class NetworkMenuServer: Menu
    {
        public PortInputField PortInput { get; private set; }

        public NetworkMenuServer(Texture2D snakePic, SpriteFont customFont, Vector2 startPosition)
            : base(snakePic, customFont,startPosition, GameState.NETWORK_MENU_SERVER)
        {
            items.Add(new MenuEntry("Start Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), GameState.START_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), GameState.MAIN_MENU));

            PortInput = new PortInputField("Port:", new Vector2(startPosition.X, 100), base.font, Color.Black,6);
            PortInput.Focus();
        }

        public override void Update()
        {
            PortInput.Update();
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            PortInput.Draw(spriteBatch);
            
        }

        public Boolean hasValidInput()
        {
             return  PortInput.CheckPortInput();
        }
    
    }
}
