using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private GameState currentState;
        public GameState CurrentState
        {
            get
            {
                //currentState is set back to the standardMenu state,if user comes back to this menu
                GameState temp = currentState;
                currentState = standardState;
                return temp;
            }

            private set{currentState=value;}
        }

        protected List<MenuEntry> items;
        protected SpriteFont font;
        protected GameState standardState;
        protected Vector2 startPosition;

        private MouseState oldMouseState;
        private MouseState currentMouseState;
        private Texture2D snakePic;
        private Rectangle snakePosition;

        #endregion

        public Menu(Texture2D snakePic, SpriteFont font,Vector2 startPosition, GameState standardState)
        {
            this.font = font;
            this.snakePic = snakePic;
            this.startPosition = startPosition;

            this.snakePosition = new Rectangle(400, 50, snakePic.Width, snakePic.Height);
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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(snakePic, snakePosition, Color.White);

            foreach (MenuEntry entry in items)
            {
                entry.Draw(spriteBatch, font);
            }
        }

    }
}
