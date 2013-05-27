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
        //TODO: put this in superClass


        public PortInputField portInput { get; set; }

        private Vector2 startPosition;

        public NetworkMenuServer(ContentManager content, Vector2 startPosition)
            : base(content,GameState.NETWORK_MENU_SERVER)
        {

            this.startPosition = startPosition;

            items.Add(new MenuEntry("Start Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, WIDTH, HEIGHT), GameState.START_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), WIDTH, HEIGHT), GameState.MAIN_MENU));

            portInput = new PortInputField("Port:", new Vector2(startPosition.X, 100), base.font, Color.Black,6);
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
