using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace NEA_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Circle[] circles = new Circle[1];
        private Square[] squares = new Square[100];

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
            // TODO: Add your initialization logic here

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
            // TODO: use this.Content to load your game content here
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
            // TODO: Add your update logic here
            CheckCollisions();
            base.Update(gameTime);
        }
        private void CheckCollisions()
        {
            for (int i = 0; i < circles.Length - 1; i++)
            {
                for (int j = i + 1; j < circles.Length; j++)
                {
                    if ((circles[i].Position - circles[j].Position).Length() < (circles[i].Origin.X + circles[j].Origin.X))
                    {
                        ResolveCollision(circles[i], circles[j]);
                    }
                }
            }
            for (int i = 0; i < squares.Length - 1; i++)
            {
                for (int j = i + 1; j < squares.Length; j++)
                {
                    if (squares[i].Collided(squares[j].HitBox))
                    {
                        ResolveCollision(squares[i], squares[j]);
                    }
                }
            }
            foreach (Square square in squares)
            {
                foreach (Circle circle in circles)
                {
                    if (circle.Collided(square.HitBox))
                    {
                        ResolveCollision(circle, square);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
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
