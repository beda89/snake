using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Snake
    {
        public Vector2 Position;
        public Texture2D Texture;
        public enum Direction { Up, Right, Down, Left };
        public List<Direction> parts;
        public Direction SnakeDirection;

        // The time since we last updated the frame
        private int elapsedTime;

        // The time we display a frame until the next one
        private int frameTime;

        public void Initialize(Texture2D texture, Vector2 position,Direction SnakeDirection)
        {
            this.Position = position;
            this.Texture = texture;

            frameTime = 250;

            this.SnakeDirection = SnakeDirection;

            parts = new List<Direction>();
            for (int i = 0; i < 1; i++)
            {
                parts.Add(SnakeDirection);
            }

        }

        public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                switch (SnakeDirection)
                {
                    case Direction.Up:
                        Position.Y -= 16;
                        break;
                    case Direction.Down:
                        Position.Y += 16;
                        break;
                    case Direction.Left:
                        Position.X -= 16;
                        break;
                    case Direction.Right:
                        Position.X += 16;
                        break;
                }
                for (int i = 0; i < parts.Count - 1; i++)
                {
                    parts[parts.Count - 1 - i] = parts[parts.Count - 2 - i];
                }
                parts[0] = SnakeDirection;
                elapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 actualPos = Position;
            foreach (Direction part in parts)
            {
                spriteBatch.Draw(Texture, actualPos, null, Color.White, 0f,
                new Vector2(16, 16), 1f, SpriteEffects.None, 0f);

                switch (part)
                {
                    case Direction.Up:
                        actualPos.Y += 16;
                        break;
                    case Direction.Down:
                        actualPos.Y -= 16;
                        break;
                    case Direction.Left:
                        actualPos.X += 16;
                        break;
                    case Direction.Right:
                        actualPos.X -= 16;
                        break;
                }
            }
        }
    }
}
