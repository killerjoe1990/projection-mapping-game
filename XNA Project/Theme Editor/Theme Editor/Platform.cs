﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Theme_Editor
{
    public enum PlatformTypes
        {
            Passable,
            Impassable,
            Regular
        }

    public enum PlatformStatus
    {
        Asleep,
        Active,
        Dead
    }

    

    public class Tile : MoveableObject
    {
        

        public Tile(Vector2 position, Vector2 velocity, Texture2D image)
            : base(new Rectangle((int)position.X, (int)position.Y, GameConstants.TILE_DIM, GameConstants.TILE_DIM), velocity, image)
        {
        }

        

        public void ChangeTexture(Texture2D texture)
        {
            m_CurrentAnimation = new Animation(texture);
        }
    }

    public abstract class Platform
    {
        protected Tile[] m_Tiles;
        protected PlatformStatus m_Status;

        public bool Blink
        {
            get;
            protected set;
        }
        
        public Platform(Vector2 position, Vector2 velocity, int tilesWide, Texture2D[] images)
        {
            m_Status = PlatformStatus.Asleep;
            m_Tiles = new Tile[tilesWide];

            if (tilesWide > 0)
            {
                m_Tiles[0] = new Tile(new Vector2(position.X, position.Y), velocity, images[0]);

                for (int i = 1; i < tilesWide - 1; ++i)
                {
                    int image = 0;

                    if (images.Length > 1)
                    {
                        image = GameConstants.RANDOM.Next(images.Length - 2) + 1;
                    }

                    m_Tiles[i] = new Tile(new Vector2(position.X + (i * GameConstants.TILE_DIM), position.Y), velocity, images[image]);
                }

                m_Tiles[tilesWide - 1] = new Tile(new Vector2(position.X + ((tilesWide - 1) * GameConstants.TILE_DIM), position.Y), velocity, images[images.Length - 1]);
            }

            Blink = false;
        }

        public Tile[] Tiles
        {
            get
            {
                return m_Tiles;
            }
        }

        public PlatformStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                if (m_Tiles.Length > 0)
                {
                    return m_Tiles[0].Position;
                }
                return Vector2.Zero;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (m_Tiles.Length > 0)
                {
                    return new Rectangle(m_Tiles[0].Bounds.X, m_Tiles[0].Bounds.Y, m_Tiles[0].Bounds.Width * m_Tiles.Length, m_Tiles[0].Bounds.Height);
                }
                return Rectangle.Empty;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                if (m_Tiles[0] != null)
                {
                    return m_Tiles[0].Velocity;
                }
                else
                {
                    return Vector2.Zero;
                }
            }
        }

        public void ChangeTheme(Texture2D[] textures)
        {
            m_Tiles[0].ChangeTexture(textures[0]);

            for (int i = 1; i < m_Tiles.Length - 1; ++i)
            {
                int image = 0;

                if (textures.Length > 1)
                {
                    image = GameConstants.RANDOM.Next(textures.Length - 2) + 1;
                }

                m_Tiles[i].ChangeTexture(textures[image]);
            }

            m_Tiles[m_Tiles.Length - 1].ChangeTexture(textures[textures.Length - 1]);
        }

        public abstract bool Collide(MoveableObject obj, CollisionDirections dir);

        public virtual void Update(float deltaTime)
        {
            foreach (Tile t in m_Tiles)
            {
                t.Update(deltaTime);
            }
        }

        public virtual void Draw(SpriteBatch batch, Color color)
        {
            foreach (Tile t in m_Tiles)
            {
                t.Draw(batch, color);
            }
        }
    }
}
