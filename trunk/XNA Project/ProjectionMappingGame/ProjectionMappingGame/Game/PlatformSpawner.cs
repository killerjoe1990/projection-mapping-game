using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    public class PlatformSpawner
    {

        Vector2 m_MaxRange;
        Vector2 m_MinRange;

        float m_LastSpawnY;

        int m_WindowTilesWide;

        Texture2D[][] m_PlatTextures;

        public PlatformSpawner(Texture2D[][] platformTextures, int windowWidth)
        {
            //float yt = -GameConstants.JUMP_IMPULSE / (2 * GameConstants.GRAVITY);
            //float maxY = GameConstants.JUMP_IMPULSE * yt + GameConstants.GRAVITY * yt * yt;
            //float minY = maxY / 3.0f;
            m_MaxRange = new Vector2(0, 500);
            m_MinRange = new Vector2(0, 100);

            m_LastSpawnY = -GameConstants.TILE_DIM;

            m_WindowTilesWide = windowWidth / GameConstants.TILE_DIM;

            m_PlatTextures = platformTextures;
        }

        public List<Platform> SpawnPlatforms(float deltaTime)
        {
            m_LastSpawnY += deltaTime * GameConstants.PLATFORM_VELOCITY;

            if (m_LastSpawnY >= m_MinRange.Y)
            {
                List<Platform> plats = new List<Platform>();

                int tilesToSpawn = m_WindowTilesWide;

                while (tilesToSpawn > 0)
                {
                    //spawn space
                    int num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH;
                    if (num > tilesToSpawn)
                    {
                        num = tilesToSpawn;
                    }

                    tilesToSpawn -= num;

                    //spawn platform
                    num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH;
                    int tex = GameConstants.RANDOM.Next(m_PlatTextures.Length);

                    if (num > tilesToSpawn)
                    {
                        num = tilesToSpawn;
                    }

                    plats.Add(new RegularPlatform(new Vector2((m_WindowTilesWide - tilesToSpawn) * GameConstants.TILE_DIM,-GameConstants.TILE_DIM), Vector2.UnitY * GameConstants.PLATFORM_VELOCITY, num, m_PlatTextures[tex]));
                    tilesToSpawn -= num;
                }

                m_LastSpawnY = -GameConstants.TILE_DIM;

                return plats;
            }

            return null;
        }

    }
}
