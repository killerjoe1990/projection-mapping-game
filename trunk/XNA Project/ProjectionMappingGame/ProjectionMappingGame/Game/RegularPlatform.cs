﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    class RegularPlatform : Platform
    {
        public RegularPlatform(Vector2 position, Vector2 velocity, int tilesWide, Texture2D[] images)
            : base(position, velocity, tilesWide, images)
        {

        }

        public override void Collide(Player p, Tile t, CollisionDirections direction)
        {
            if (direction == CollisionDirections.Bot)
            {
                p.CollideBot(t);
            }
        }
    }
}
