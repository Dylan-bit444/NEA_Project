using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NEA_Project
{
    public class Sprite
    {
        public Sprite(Texture2D Texture, Vector2 Position, Color Color,float Volocity, Vector2 Direction )
        {
            this.Texture = Texture;
            this.Position = Position;
            this.Color = Color;
            this.Direction = Direction;
            this.Volocity = Volocity;
        }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); } }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Volocity { get; set; }
        public Vector2 Direction { get; set; }
        public Texture2D Texture { get; set; }

        public Vector2 Origin { get { return new Vector2(Texture.Width / 2, Texture.Height / 2); } }
        public bool IsDraw = true;
        public void Update(GraphicsDeviceManager graphics, float time)
        {
            Position += Direction * Volocity * time;
            if (Position.X < 0 && Direction.X < 0)
            {
                Direction = new(-Direction.X, Direction.Y);
            }
            else if (Direction.X > 0 && Texture.Width + Position.X > graphics.PreferredBackBufferWidth)
            {
                Direction = new(-Direction.X, Direction.Y);
            }
            if (Direction.Y < 0 && Position.Y < 0 )
            { 
                Direction = new(Direction.X, -Direction.Y);
            }
            else if (Direction.Y > 0 && Texture.Height + Position.Y > graphics.PreferredBackBufferHeight)
            {
                Direction = new(Direction.X, -Direction.Y);
            }
        }
        public bool Collided(Rectangle box)
        {
            if (HitBox.Intersects(box))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool TouchingLeft(Sprite sprite)
        {
            return this.HitBox.Right + this.Position.X > sprite.HitBox.Left &&
              this.HitBox.Left < sprite.HitBox.Left &&
              this.HitBox.Bottom > sprite.HitBox.Top &&
              this.HitBox.Top < sprite.HitBox.Bottom;
        }

        public bool TouchingRight(Sprite sprite)
        {
            return this.HitBox.Left + this.Position.X < sprite.HitBox.Right &&
              this.HitBox.Right > sprite.HitBox.Right &&
              this.HitBox.Bottom > sprite.HitBox.Top &&
              this.HitBox.Top < sprite.HitBox.Bottom;
        }

        public bool TouchingTop(Sprite sprite)
        {
            return this.HitBox.Bottom + this.Position.Y > sprite.HitBox.Top &&
              this.HitBox.Top < sprite.HitBox.Top &&
              this.HitBox.Right > sprite.HitBox.Left &&
              this.HitBox.Left < sprite.HitBox.Right;
        }

        public bool TouchingBottom(Sprite sprite)
        {
            return this.HitBox.Top + this.Position.Y < sprite.HitBox.Bottom &&
              this.HitBox.Bottom > sprite.HitBox.Bottom &&
              this.HitBox.Right > sprite.HitBox.Left &&
              this.HitBox.Left < sprite.HitBox.Right;
        }
    }
}
