using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    public enum PlatformTypes
        {
            Passable,
            Impassable,
            Platform
        }

    public class Tile : MoveableObject
    {
        

        PlatformTypes m_Type;

        public Tile(Vector2 position, Vector2 velocity, Texture2D image, PlatformTypes type)
            : base(new Rectangle((int)position.X,(int)position.Y, GameConstants.TILE_DIM,GameConstants.TILE_DIM), velocity, image)
        {
            m_Type = type;
        }

        public PlatformTypes Type
        {
            get
            {
                return m_Type;
            }
        }
    }

    public class Platform
    {
        Tile[] m_Tiles;

        public Platform(Vector2 position, Vector2 velocity, int tilesWide, PlatformTypes type, Texture2D[] images)
        {
            m_Tiles = new Tile[tilesWide];

            for (int i = 0; i < tilesWide; ++i)
            {
                int image = GameConstants.RANDOM.Next(images.Length);

                m_Tiles[i] = new Tile(new Vector2(position.X + (i * GameConstants.TILE_DIM), position.Y), velocity, images[image], type);
            }
        }

        public Tile[] Tiles
        {
            get
            {
                return m_Tiles;
            }
        }

        public PlatformTypes Type
        {
            get
            {
                if (m_Tiles.Length > 0)
                {
                    return m_Tiles[0].Type;
                }
                return PlatformTypes.Passable;
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

        public void Update(float deltaTime)
        {
            foreach (Tile t in m_Tiles)
            {
                t.Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (Tile t in m_Tiles)
            {
                t.Draw(batch);
            }
        }
    }
}
