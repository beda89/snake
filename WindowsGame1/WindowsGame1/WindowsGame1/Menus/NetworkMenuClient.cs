using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.FSM;
using Snake.Menus.Input;

namespace Snake.Menus
{
    class NetworkMenuClient:Menu
    {
        private const int INPUT_FIELD_Y = 100;
        private const int Y_SPACING = 70;
        private const int MAX_IP_LENGTH = 15;
        private const int MAX_PORT_LENGTH = 5;

        public IPInputField IpInput { get; private set; }
        public PortInputField PortInput { get; private set; }

        public NetworkMenuClient(Vector2 startPosition,StateBase standardState,StateBase mainMenuState)
            : base(startPosition,standardState)
        {

            items.Add(new MenuEntry("Join Server", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y, ITEM_WIDTH, ITEM_HEIGHT), new State_ConnectToServer(startPosition,mainMenuState,standardState,this)));
            items.Add(new MenuEntry("Back", Color.Black, Color.Green, new Rectangle((int)startPosition.X, (int)startPosition.Y + (ITEM_HEIGHT + ITEM_SPACING_Y), ITEM_WIDTH, ITEM_HEIGHT), mainMenuState));

            IpInput = new IPInputField("IP-Address:", new Vector2(startPosition.X, INPUT_FIELD_Y), Color.Black, MAX_IP_LENGTH);
            PortInput = new PortInputField("Port:", new Vector2(startPosition.X, INPUT_FIELD_Y + Y_SPACING), Color.Black, MAX_PORT_LENGTH);

            IpInput.Focus();
        }

        public override void Update()
        {
            base.Update();

            MouseState state = Mouse.GetState();
            Rectangle mousePosition=new Rectangle(state.X,state.Y,1,1);

            if (state.LeftButton == ButtonState.Pressed)
            {
                if (IpInput.InputFieldPosition.Intersects(mousePosition))
                {
                    IpInput.Focus();
                    PortInput.UnFocus();
                }
                else if (PortInput.InputFieldSize.Intersects(mousePosition))
                {
                    IpInput.UnFocus();
                    PortInput.Focus();
                }
            }

            IpInput.Update();
            PortInput.Update();
        }


        public override void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {
            base.Draw(spriteBatch,gameGraphics);
            IpInput.Draw(spriteBatch,gameGraphics);
            PortInput.Draw(spriteBatch,gameGraphics);
        }

        public Boolean InputFieldsAreValid()
        {
            if (!IpInput.CheckIpInput())
            {
                return false;
            }

            if (!PortInput.CheckPortInput())
            {
                return false;
            }

            return true;
        }
    }
}
