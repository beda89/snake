using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1.Menus
{
    class NetworkMenuClient:Menu
    {
        private IPInputField ipInput;
        private PortInputField portInput;

        public NetworkMenuClient(Texture2D snakePic,SpriteFont customFont, Vector2 startPosition)
            : base(snakePic,customFont,startPosition,GameState.NETWORK_MENU_CLIENT)
        {

            items.Add(new MenuEntry("Join Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, WIDTH, HEIGHT), GameState.CONNECT_TO_SERVER));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (HEIGHT + 5), WIDTH, HEIGHT), GameState.MAIN_MENU));

            ipInput = new IPInputField("IP-Address:", new Vector2(startPosition.X, 100), base.font, Color.Black, 15);
            portInput = new PortInputField("Port:", new Vector2(startPosition.X,100+70),base.font,Color.Black,6);

            ipInput.Focus();
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
                    ipInput.Focus();
                    portInput.UnFocus();
                }
                else if (portInput.inputFieldSize.Intersects(mousePosition))
                {
                    ipInput.UnFocus();
                    portInput.Focus();
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

        public String GetPort()
        {
            return portInput.InputText;
        }

        public String GetIP()
        {
            return ipInput.InputText;   
        }

        public Boolean CheckInputFields()
        {
            if (!ipInput.CheckIpInput())
            {
                return false;
            }

            if (!portInput.CheckPortInput())
            {
                return false;
            }

            return true;
        }
    }
}
