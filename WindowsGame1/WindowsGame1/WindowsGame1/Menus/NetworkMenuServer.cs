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

        public NetworkMenuServer(Vector2 startPosition,StateBase menuState)
            : base(startPosition, menuState)
        {
           // items.Add(new MenuEntry("Start Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), GameState.START_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), new MainMenu_State(startPosition)));

            PortInput = new PortInputField("Port:", new Vector2(startPosition.X, 100), Color.Black,6);
            PortInput.Focus();
        }

        public override void Update(Context context)
        {
            PortInput.Update();
            base.Update(context);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D snakePic, SpriteFont font)
        {
            base.Draw(spriteBatch,snakePic,font);

            PortInput.Draw(spriteBatch,font);
            
        }

        public Boolean hasValidInput()
        {
            try
            {
                Convert.ToInt32(PortInput.InputText);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    
    }
}
