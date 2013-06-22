using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.Menus;

namespace Snake.Menus.Input
{
    class PortInputField:InputField
    {
        private const int MAX_PORT = 65535;

        public String InputText { get; private set; }
        public Rectangle InputFieldSize { get; set; }

        private Vector2 position;
        private String label;
        private Color txtColor;
        private Keys lastKey = Keys.None;
        private int maxInput;

        public PortInputField(String label, Vector2 position,Color txtColor,int maxInputLength):base()
        {
            this.position = position;
            this.label = label;
            this.txtColor = txtColor;
            this.maxInput = maxInputLength;
            this.backgroundColor= Color.LightGray;
            this.InputText = "";
            this.isFocused = false;

            InputFieldSize = new Rectangle((int)position.X, (int)position.Y + 35, maxInput * 12 + 10, 30);
        }

        public void Update()
        {
            if (!isFocused)
                return;

            if (Keyboard.GetState().IsKeyUp(lastKey))
            {
                lastKey = Keys.None;
            }

            if (Keyboard.GetState().GetPressedKeys().Length > 0 && lastKey == Keys.None)
            {
                lastKey = Keyboard.GetState().GetPressedKeys()[0];
                if (lastKey == Keys.Back)
                {
                    if (InputText.Length != 0)
                    {
                        InputText = InputText.Substring(0, InputText.Length - 1);
                    }
                }
                else if (InputText.Length < maxInput)
                {
                    char input = (char) lastKey.GetHashCode();

                    if (Char.IsDigit(input))
                    {
                        InputText += input;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch,SpriteFont font)
        {
            spriteBatch.DrawString(font, label, position, txtColor);

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { backgroundColor});

            spriteBatch.Draw(pixel, InputFieldSize, Color.White);
            spriteBatch.DrawString(font,InputText,new Vector2(InputFieldSize.X+10,InputFieldSize.Y),Color.Gray);
        }

        public Boolean CheckPortInput()
        {
            int port;
            try
            {
                port=Convert.ToInt32(InputText);
            }
            catch (FormatException)
            {
                return false;
            }

            //if portvalue is out of allowed range
            if (port < 0 || port > MAX_PORT)
            {
                return false;
            }

            return true;
        }
    }

}
