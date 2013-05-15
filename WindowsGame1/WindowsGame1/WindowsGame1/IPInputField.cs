﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
        class IPInputField
        {
            private Keys lastKey = Keys.None;
            private String InputText = "";
            private Vector2 position;
            private String label;
            private SpriteFont font;
            private Color txtColor;
            public Rectangle inputFieldPosition { get; set; }
            private int maxInput;
            private Color backgroundColor;
            private Boolean isFocused = false;

            public IPInputField(String label, Vector2 position, SpriteFont font, Color txtColor, int maxInputLength)
            {
                this.position = position;
                this.label = label;
                this.font = font;
                this.txtColor = txtColor;
                this.maxInput = maxInputLength;
                this.backgroundColor = Color.LightGray;

                this.inputFieldPosition = new Rectangle((int)position.X, (int)position.Y + 35, maxInput * 12+10, 30);
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

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font, label, position, txtColor);

                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData<Color>(new Color[] { backgroundColor });


                spriteBatch.Draw(pixel, inputFieldPosition, Color.White);

                spriteBatch.DrawString(font, InputText, new Vector2(inputFieldPosition.X + 10, inputFieldPosition.Y), Color.Gray);
            }


            public String getInputString()
            {
                return InputText;
            }

            public void focus()
            {
                isFocused = true;
                backgroundColor = Color.LightBlue;
            }

            public void unFocus()
            {
                isFocused = false;
                backgroundColor = Color.LightGray;
            }
        }

   
}
