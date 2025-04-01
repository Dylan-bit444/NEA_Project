using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace NEA_Project
{
    internal class Square:Sprite
    {
        public bool ColliedWithCircle=false;
        public Square(Texture2D Texture, Vector2 Position, Color Color, float Volocity, Vector2 Direction) : base(Texture, Position, Color, Volocity, Direction) { }
    }
}
