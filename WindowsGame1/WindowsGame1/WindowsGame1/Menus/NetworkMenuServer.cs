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
    class NetworkMenuServer: Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;
        private InputField portInput;


        public NetworkMenuServer(ContentManager content, Vector2 startPosition)
            : base(content,GameState.NETWORK_MENU_SERVER)
        {

            this.startPosition = startPosition;

            items.Add(new MenuEntry("Start Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.START_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));

            portInput = new InputField("Port:", new Vector2(startPosition.X, 100), base.font, Color.Black,6);
            portInput.focus();

        }

        public override void Update()
        {
            portInput.Update();

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            portInput.Draw(spriteBatch);
            
        }
    }
}
