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

        public override void Collide(Player p, Tile t, CollisionDirections direction)
        {
            switch (direction)
            {
                case CollisionDirections.Bot:
                    p.CollideBot(t);
                    break;
                case CollisionDirections.Top:
                    p.CollideTop();
                    break;
                case CollisionDirections.Left:
                    p.CollideLeft();
                    break;
                case CollisionDirections.Right:
                    p.CollideRight();
                    break;
            }
        }
    }
}
