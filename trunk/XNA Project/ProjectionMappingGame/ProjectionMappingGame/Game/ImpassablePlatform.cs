using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    class ImpassablePlatform : Platform
    {
        public ImpassablePlatform(Vector2 position, Vector2 velocity, int tilesWide, Texture2D[] images)
            : base(position, velocity, tilesWide, images)
        {

        }

        public override bool Collide(MoveableObject obj, CollisionDirections direction)
        {


            switch (direction)
            {
                case CollisionDirections.Bot:
                    return true;
                case CollisionDirections.Top:
                    return true;
                case CollisionDirections.Left:
                    return true;
                case CollisionDirections.Right:
                    return true;
            }

            return false;
        }
    }
}
