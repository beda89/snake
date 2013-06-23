using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Snake.Menus.Input
{
    abstract class InputField
    {
        protected Color backgroundColor;

        protected Boolean isFocused;

        public InputField()
        {
            isFocused = false;
        }

        public void Focus()
        {
            isFocused = true;
            backgroundColor = Color.LightBlue;
        }

        public void UnFocus()
        {
            isFocused = false;
            backgroundColor = Color.LightGray;
        }
    }
}
