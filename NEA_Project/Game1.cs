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
        private Sprite[] sprites =new Sprite[200];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false ;
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
            for(int i=0; i<sprites.Length;i++)
            {
                sprites[i] = new(ball, new Vector2(rnd.Next(ball.Width+5, _graphics.PreferredBackBufferWidth - (ball.Width + 5)), rnd.Next(ball.Height+5, _graphics.PreferredBackBufferHeight - (ball.Height + 5))), Color.White, 500, new Vector2((float)Math.Sin(rnd.NextDouble() * 2 * Math.PI), -(float)Math.Cos(rnd.NextDouble() * 2 * Math.PI)));
            }
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach(Sprite s in sprites) 
            {
                s.Update(_graphics,time);
            }
            // TODO: Add your update logic here
            CheckCollisions();
            base.Update(gameTime);
        }
        private void CheckCollisions()
        {
            for (int i = 0; i < sprites.Length - 1; i++)
            {
                for (int j = i + 1; j < sprites.Length; j++)
                {
                    if ((sprites[i].Position - sprites[j].Position).Length() < (sprites[i].Origin.X + sprites[j].Origin.X))
                    {
                        ResolveCollision(sprites[i], sprites[j]);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            foreach(Sprite s in sprites)
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
