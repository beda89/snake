using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1.Menus
{
    class NetworkMenuClient:Menu
    {
        private const int HEIGHT = 30;

        private ContentManager content;
        private Vector2 startPosition;
        private IPInputField ipInput;
        private InputField portInput;


        public NetworkMenuClient(ContentManager content, Vector2 startPosition)
            : base(content,GameState.NETWORK_MENU_CLIENT)
        {

            this.startPosition = startPosition;

            items.Add(new MenuEntry("Join Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, 150, HEIGHT), GameState.CONNECT_TO_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), 150, HEIGHT), GameState.MAIN_MENU));

            ipInput = new IPInputField("IP-Address:", new Vector2(startPosition.X, 100), base.font, Color.Black, 15);
            portInput = new InputField("Port:", new Vector2(startPosition.X,100+70),base.font,Color.Black,6);

        }

        public override void Update()
        {
            base.Update();

            MouseState state = Mouse.GetState();
            Rectangle mousePosition=new Rectangle(state.X,state.Y,1,1);

            if (state.LeftButton == ButtonState.Pressed)
            {
                if (ipInput.inputFieldPosition.Intersects(mousePosition))
                {
                    ipInput.focus();
                    portInput.unFocus();
                }
                else if (portInput.inputFieldSize.Intersects(mousePosition))
                {
                    ipInput.unFocus();
                    portInput.focus();
                }
            }

            ipInput.Update();
            portInput.Update();
        }


        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            ipInput.Draw(spriteBatch);
            portInput.Draw(spriteBatch);
        }

        public String getPort()
        {
            return portInput.getInputString();
        }

        public String getIP()
        {
            return ipInput.getInputString();    
        }

    }
}
