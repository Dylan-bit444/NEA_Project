using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace NEA_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Circle[] circles = new Circle[0];
        private Square[] squares = new Square[100];
        private QuadTree _quadtree;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.PreferredBackBufferWidth = 1550;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            // Initialize the quadtree with level 0 and the bounds of the screen
            _quadtree = new QuadTree(0, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Random rnd = new Random();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D ball = Content.Load<Texture2D>("Ball");
            Texture2D square = Content.Load<Texture2D>("Brick");

            // Initialize circles with random positions and directions
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i] = new Circle(
                    ball,
                    new Vector2(rnd.Next(ball.Width + 5, _graphics.PreferredBackBufferWidth - (ball.Width + 5)),
                                rnd.Next(ball.Height + 5, _graphics.PreferredBackBufferHeight - (ball.Height + 5))),
                    Color.White,
                    200,
                    new Vector2((float)Math.Sin(rnd.NextDouble() * 2 * Math.PI), -(float)Math.Cos(rnd.NextDouble() * 2 * Math.PI))
                );
            }

            // Initialize squares with random positions and directions
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = new Square(
                    square,
                    new Vector2(rnd.Next(ball.Width + 5, _graphics.PreferredBackBufferWidth - (ball.Width + 5)),
                                rnd.Next(ball.Height + 5, _graphics.PreferredBackBufferHeight - (ball.Height + 5))),
                    Color.White,
                    200,
                    new Vector2((float)Math.Sin(rnd.NextDouble() * 2 * Math.PI), -(float)Math.Cos(rnd.NextDouble() * 2 * Math.PI))
                );
            }
        }

        protected override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update positions of all circles
            foreach (Circle circle in circles)
            {
                circle.Update(_graphics, time);
            }

            // Update positions of all squares
            foreach (Square square in squares)
            {
                square.Update(_graphics, time);
            }

            // Check for collisions between objects
            CheckCollisions();
            base.Update(gameTime);
        }

        private void CheckCollisions()
        {
            // Clear the quadtree to prepare for new frame
            _quadtree.Clear();

            // Insert all circles into the quadtree
            foreach (Circle circle in circles)
            {
                _quadtree.Insert(circle);
            }

            // Insert all squares into the quadtree
            foreach (Square square in squares)
            {
                _quadtree.Insert(square);
            }

            List<Sprite> possibleCollisions = new List<Sprite>();

            // Check for collisions between circles
            foreach (Circle circle in circles)
            {
                possibleCollisions.Clear();
                _quadtree.Retrieve(possibleCollisions, circle);
                foreach (Sprite other in possibleCollisions)
                {
                    if (circle != other && (circle.Position - other.Position).Length() < (circle.Texture.Width / 2 + other.Texture.Width / 2))
                    {
                        circle.Direction = Vector2.Normalize(circle.Position - other.Position);
                        other.Direction = -Vector2.Normalize(circle.Position - other.Position);
                    }
                }
            }

            // Check for collisions between squares
            foreach (Square square in squares)
            {
                possibleCollisions.Clear();
                _quadtree.Retrieve(possibleCollisions, square);
                foreach (Sprite other in possibleCollisions)
                {
                    if (square != other && square.Collided(other.HitBox))
                    {
                        if (square.TouchingRight(other) || square.TouchingLeft(other))
                        {
                            if (square.Direction.X * other.Direction.X < 0)
                            {
                                other.Direction *= new Vector2(-1, 1);
                            }
                            else if (other.Direction.X >= 0)
                            {
                                other.Direction += new Vector2(square.Direction.X / 1.1f, 0);
                                square.Direction -= new Vector2(square.Direction.X / 1.1f, 0);
                            }
                            else if(other.Direction.X <= 0)
                            {
                                other.Direction -= new Vector2(other.Direction.X / 1.1f, 0);
                                square.Direction -= new Vector2(square.Direction.X / 1.1f, 0);
                            }
                            square.Direction *= new Vector2(-1, 1);
                        }
                        if (square.TouchingTop(other) || square.TouchingBottom(other))
                        {
                            if (square.Direction.Y * other.Direction.Y < 0)
                            {
                                other.Direction *= new Vector2(1, -1);
                            }
                            else if (other.Direction.Y >= 0)
                            {
                                other.Direction += new Vector2(0,square.Direction.Y / 1.1f);
                                square.Direction -= new Vector2(0, square.Direction.Y / 1.1f);
                            }
                            else if (other.Direction.Y <= 0)
                            {
                                other.Direction -= new Vector2(0, square.Direction.Y / 1.1f);
                                square.Direction -= new Vector2(0, square.Direction.Y / 1.1f);
                            }
                            square.Direction *= new Vector2(-1, 1);
                        }
                    }
                }
            }

            // Check for collisions between squares and circles
            Parallel.ForEach(squares, square =>
            {
                foreach(Circle circle in circles)
                {
                    if (circle.Collided(square.HitBox)&&(circle.Origin - square.Origin).Length() < ((circle.Texture.Width / 2 )+ (square.Origin - square.Position).Length()))
                    {
                        if ((circle.TouchingRight(square) && circle.Direction.X <= 0) || (circle.TouchingLeft(square) && circle.Direction.X >= 0))
                        {
                            square.Direction *= new Vector2(-1, 1);
                            circle.Direction *= new Vector2(-1, 1);
                        }
                        if ((circle.TouchingTop(square) && circle.Direction.Y <= 0) || (circle.TouchingBottom(square) && circle.Direction.Y >= 0))
                        {
                            square.Direction *= new Vector2(1, -1);
                            circle.Direction *= new Vector2(1, -1);
                        }
                    }
                }
            });
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Draw all circles
            foreach (Sprite s in circles)
            {
                _spriteBatch.Draw(s.Texture, s.Position, s.Color);
            }
            // Draw all squares
            foreach (Sprite s in squares)
            {
                _spriteBatch.Draw(s.Texture, s.Position, s.Color);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
