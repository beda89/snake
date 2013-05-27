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
    class Menu
    {
        protected const int HEIGHT = 30;
        protected const int WIDTH = 150;

        protected List<MenuEntry> items;
        protected SpriteFont font;
        protected GameState standardState;

        private MouseState oldMouseState;
        private MouseState currentMouseState;
        private Texture2D snakePic;
        private Rectangle snakePosition;
        private GameState currentState;

        public Menu(ContentManager content,GameState standardState)
        {
            this.font = content.Load<SpriteFont>("customFont");
            this.snakePic = content.Load<Texture2D>("snake-cartoon_small");
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
                        currentState = entry.Gamestate;
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

        public virtual GameState getCurrentState()
        {
            //currentState is set back to the standardMenu state,if user comes back to this menu
            GameState temp = currentState;
            currentState = standardState;
            return temp;
        }

    }
}
