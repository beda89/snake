using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.FSM;

namespace Snake.Menus
{
    abstract class Menu
    {

        #region constants
        protected const int ITEM_HEIGHT = 30;
        protected const int ITEM_WIDTH = 150;
        protected const int ITEM_SPACING_Y = 5;

        #endregion

        #region fields

        
        private StateBase currentState;
        public StateBase CurrentState
        {
            get
            {
                //currentState is set back to the standardMenu state,if user comes back to this menu
                StateBase temp = currentState;
                currentState = standardState;
                return temp;
            }

            private set{currentState=value;}
        } 

        protected List<MenuEntry> items;
        protected StateBase standardState;
        protected Vector2 startPosition;

        private MouseState oldMouseState;
        private MouseState currentMouseState;

        #endregion

        public Menu(Vector2 startPosition, StateBase standardState)
        {
            this.startPosition = startPosition;
            this.standardState = standardState;
            this.currentState = standardState;
            this.items = new List<MenuEntry>();
        }

        public virtual void Update()
        {
            oldMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            foreach (MenuEntry entry in items)
            {

                //checks if menu is clicked
                if (currentMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (entry.Position.Intersects(mousePosition))
                    {
                        CurrentState = entry.Gamestate;
                    }
                }


                //hover effect
                if (entry.Position.Intersects(mousePosition))
                {
                    entry.BackgroundColor = Color.LightGreen;
                }
                else
                {
                    entry.BackgroundColor = Color.Green;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch,GameGraphics gameGraphics)
        {
            Rectangle snakePosition = new Rectangle(400, 50, gameGraphics.SnakePic.Width, gameGraphics.SnakePic.Height);

            spriteBatch.Draw(gameGraphics.SnakePic, snakePosition, Color.White);

            foreach (MenuEntry entry in items)
            {
                entry.Draw(spriteBatch, gameGraphics.CustomFont);
            }
        }

    }
}
