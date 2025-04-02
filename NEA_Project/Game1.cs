using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace NEA_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Circle[] circles = new Circle[300];
        private Square[] squares = new Square[300];
        private QuadTree _quadtree;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.PreferredBackBufferWidth = 1950;
            _graphics.PreferredBackBufferHeight = 1100;
            _graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            _quadtree = new QuadTree(0, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Random rnd = new Random();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D ball = Content.Load<Texture2D>("Ball");
            Texture2D square = Content.Load<Texture2D>("Brick");
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i] = new(ball, new Vector2(rnd.Next(ball.Width + 5, _graphics.PreferredBackBufferWidth - (ball.Width + 5)), rnd.Next(ball.Height + 5, _graphics.PreferredBackBufferHeight - (ball.Height + 5))), Color.White, 200, new Vector2((float)Math.Sin(rnd.NextDouble() * 2 * Math.PI), -(float)Math.Cos(rnd.NextDouble() * 2 * Math.PI)));
            }
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = new(square, new Vector2(rnd.Next(ball.Width + 5, _graphics.PreferredBackBufferWidth - (ball.Width + 5)), rnd.Next(ball.Height + 5, _graphics.PreferredBackBufferHeight - (ball.Height + 5))), Color.White, 200, new Vector2((float)Math.Sin(rnd.NextDouble() * 2 * Math.PI), -(float)Math.Cos(rnd.NextDouble() * 2 * Math.PI)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach (Circle circle in circles)
            {
                circle.Update(_graphics, time);
            }
            foreach (Square square in squares)
            {
                square.Update(_graphics, time);
            }
            CheckCollisions();
            base.Update(gameTime);
        }

        private void CheckCollisions()
        {
            _quadtree.Clear();
            foreach (var circle in circles)
            {
                _quadtree.Insert(circle);
            }
            foreach (var square in squares)
            {
                _quadtree.Insert(square);
            }

            List<Sprite> possibleCollisions = new List<Sprite>();

            foreach (var circle in circles)
            {
                possibleCollisions.Clear();
                _quadtree.Retrieve(possibleCollisions, circle);
                foreach (var other in possibleCollisions)
                {
                    if (circle != other && (circle.Position - other.Position).Length() < (circle.Texture.Width / 2 + other.Texture.Width / 2))
                    {
                        ResolveCollision(circle, other);
                    }
                }
            }

            foreach (var square in squares)
            {
                possibleCollisions.Clear();
                _quadtree.Retrieve(possibleCollisions, square);
                foreach (var other in possibleCollisions)
                {
                    if (square != other && square.Collided(other.HitBox))
                    {
                        ResolveCollision(square, other);
                    }
                }
            }

            foreach (var square in squares)
            {
                foreach (var circle in circles)
                {
                    if ((circle.Position - square.Position).Length() < (circle.Texture.Width / 2 + square.Texture.Width / 2) && circle.Collided(square.HitBox))
                    {
                        ResolveCollision(circle, square);
                        square.ColliedWithCircle = true;
                    }
                    else
                    {
                        square.ColliedWithCircle = false;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (Sprite s in circles)
            {
                _spriteBatch.Draw(s.Texture, s.Position, s.Color);
            }
            foreach (Sprite s in squares)
            {
                _spriteBatch.Draw(s.Texture, s.Position, s.Color);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ResolveCollision(Sprite b1, Sprite b2)
        {
            var dir = Vector2.Normalize(b1.Position - b2.Position);
            b1.Direction = dir;
            b2.Direction = -dir;
        }
    }
}
