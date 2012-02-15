#region File Description
//-----------------------------------------------------------------------------
// Tile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    class Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        private const float GravityAcceleration = 1.0f;
        public const int Width = 40;
        public const int Height = 32;
        private Vector2 position;
        private Vector2 velocity;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
            this.position = new Vector2(0f,0f);
            this.velocity = new Vector2(0f,.1f);
        }
        public void setPosition(Vector2 position)
        {
            this.position = position;
        }
        public Vector2 getPosition()
        {
            return this.position;
        }
        public void updatePosition(float elapsedTime)
        {
            velocity.Y = velocity.Y + GravityAcceleration * elapsedTime;

            this.position += velocity * elapsedTime;
            this.position = new Vector2((float)Math.Round(this.position.X), (float)Math.Round(this.position.Y));
        }
        public void drawTile(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                
                spriteBatch.Draw(Texture, this.position, Color.White);

            }
        }
    }
}
