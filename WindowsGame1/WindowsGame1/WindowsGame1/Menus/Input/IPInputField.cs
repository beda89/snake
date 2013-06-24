using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.Menus;

namespace Snake.Menus.Input
{
        class IPInputField:InputField
        {
            #region fields

            public Rectangle InputFieldPosition { get; set; }
            public String InputText { get; private set; }

            private Keys lastKey = Keys.None;
            private Vector2 position;
            private String label;
            private Color txtColor;
            private int maxInput;

            #endregion

            public IPInputField(String label, Vector2 position, Color txtColor, int maxInputLength):base()
            {
                this.position = position;
                this.label = label;
                this.txtColor = txtColor;
                this.maxInput = maxInputLength;
                this.backgroundColor = Color.LightGray;
                this.InputText = "";

                this.InputFieldPosition = new Rectangle((int)position.X, (int)position.Y + 35, maxInput * 12+10, 30);
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
                        char input = (char)lastKey.GetHashCode();

                        if (input >= '0' && input <= '9')
                        {
                            InputText += input;
                        }

                        if(lastKey==Keys.OemPeriod){
                            InputText +='.';
                        }
                    }
                }
            }

            public void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
            {
                spriteBatch.DrawString(gameGraphics.CustomFont, label, position, txtColor);

                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData<Color>(new Color[] { backgroundColor });


                spriteBatch.Draw(pixel, InputFieldPosition, Color.White);

                spriteBatch.DrawString(gameGraphics.CustomFont, InputText, new Vector2(InputFieldPosition.X + 10, InputFieldPosition.Y), Color.Gray);
            }

            public Boolean CheckIpInput()
            {
                IPAddress address;

                if (!IPAddress.TryParse(InputText, out address))
                {
                    return false;
                }

                return true;
            }
        }

   
}
