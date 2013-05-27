using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    class Snake
    {
        // The time we display a frame until the next one
        private const int FRAME_TIME = 250;

        public Vector2 Position;
        public enum Direction { Up, Right, Down, Left };

        //the parts of the snake as a list (every snake segment)
        public List<Vector2> parts{get;set;}
        public Direction SnakeDirection;

        // The time since we last updated the frame
        private int elapsedTime;

        private Texture2D Texture;


        //only called by server
        public void Initialize(Texture2D texture, Vector2 position,Direction SnakeDirection)
        {
            this.Position = position;
            this.Texture = texture;

            this.SnakeDirection = SnakeDirection;

            parts = new List<Vector2>();
            parts.Add(position);

            Vector2 tempPosition = position;

            for (int i = 0; i < 4; i++)
            {
                switch (SnakeDirection)
                {
                    case Direction.Up:
                        tempPosition.Y-=16;
                        break;
                    case Direction.Left:
                        tempPosition.X += 16;
                        break;
                    case Direction.Right:
                        tempPosition.X-= 16;
                        break;
                    case Direction.Down:
                        tempPosition.Y += 16;
                        break;
                }

                parts.Add(tempPosition);
            }

        }

        //only called by client
        public void Initialize(Texture2D texture, List<Vector2> parts)
        {
            this.parts = parts;
            this.Texture = texture;
        }

        //only called by server
        public void Update(GameTime gameTime,GameField gameField)
        {
            //TODO there is a bug with the set direction if the snake collides with the borders

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > FRAME_TIME)
            {
                Vector2 tempPosition = Position;

                switch (SnakeDirection)
                {
                    case Direction.Up:
                        tempPosition.Y -= 16;
                        break;
                    case Direction.Down:
                        tempPosition.Y += 16;
                        break;
                    case Direction.Left:
                        tempPosition.X -= 16;
                        break;
                    case Direction.Right:
                        tempPosition.X += 16;
                        break;
                }

                if (gameField.Collides(tempPosition))
                {
                    return;
                }

                Position = tempPosition;

                for (int i = 0; i < parts.Count - 1; i++)
                {
                    parts[parts.Count - 1 - i] = parts[parts.Count - 2 - i];
                }
                parts[0] = Position;
                elapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Vector2 part in parts)
            {
                spriteBatch.Draw(Texture, part, null, Color.White, 0f,
                    new Vector2(16, 16) , 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
