using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;

namespace NEA_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Circle[] circles = new Circle[500];
        private Square[] squares = new Square[0];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.PreferredBackBufferWidth = 1950;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.IsFullScreen = false;
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
            Parallel.ForEach(circles, circle =>
            {
                circle.Update(_graphics, time);
            });
            Parallel.ForEach(squares, square =>
            {
                square.Update(_graphics, time);
            });
            // TODO: Add your update logic here
            CheckCollisions();
            base.Update(gameTime);
        }
        private void CheckCollisions()
        {
            const int RadiusIncrement = 350;
            Parallel.For(0, circles.Length, i =>
            {
            for (int j = i + 1; j < circles.Length; j++)
            {
                // this is the unit vector or discance is less than the length of both circles raidi 
                if ((circles[i].Position - circles[j].Position).Length() < (circles[i].Texture.Width / 2 + circles[j].Texture.Width / 2))
                {
                    ResolveCollision(circles[i], circles[j]);
                }
            }
            });
            Parallel.For(0, squares.Length - 1, i =>
            {
                for(int j=i + 1; j < squares.Length;j++)
                {
                    if (squares[i].Collided(squares[j].HitBox))
                    {
                        ResolveCollision(squares[i], squares[j]);
                    }
                }
            });
            Parallel.ForEach(squares, square =>
            {
                Parallel.ForEach(circles, circle =>
                {
                    for (int i = 0; i < (square.Position - square.Origin).Length() - (square.Texture.Width / 2); i += RadiusIncrement)
                    {
                        if (circle.Collided(square.HitBox))
                        {
                            if ((circle.Position - square.Position).Length() < (circle.Texture.Width / 2 + (square.Texture.Width + i / 2)))
                            {
                                ResolveCollision(circle, square);
                            }
                        }
                    }
                });
            });
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
