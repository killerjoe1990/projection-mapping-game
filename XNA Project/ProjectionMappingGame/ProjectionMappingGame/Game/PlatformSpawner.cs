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

        bool m_SpawnFlag;

        int m_WindowTilesWide;

        Texture2D[][] m_PlatTextures;

        public PlatformSpawner(Texture2D[][] platformTextures, int windowWidth)
        {
            //float yt = -GameConstants.JUMP_IMPULSE / (2 * GameConstants.GRAVITY);
            //float maxY = GameConstants.JUMP_IMPULSE * yt + GameConstants.GRAVITY * yt * yt;
            //float minY = maxY / 3.0f;
            m_MaxRange = new Vector2(0, 400);
            m_MinRange = new Vector2(0, 100);

            m_LastSpawnY = -GameConstants.TILE_DIM*2;

            m_WindowTilesWide = 1 + windowWidth / GameConstants.TILE_DIM;

            m_PlatTextures = platformTextures;

            m_SpawnFlag = false;
        }

        public List<Platform> SpawnPlatforms(float deltaTime)
        {
            m_LastSpawnY += deltaTime * GameConstants.PLATFORM_VELOCITY;

            if (m_LastSpawnY >= m_MinRange.Y)
            {
                List<Platform> plats = new List<Platform>();

                int tilesToSpawn = m_WindowTilesWide;
                bool flag = m_SpawnFlag;
                int num;

                while (tilesToSpawn > 0)
                {
                    if (flag == true)
                    {
                        //spawn space
                         num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH + GameConstants.RANDOM.Next(0, 5);
                        if (num > tilesToSpawn)
                        {
                            num = tilesToSpawn;
                        }

                        tilesToSpawn -= num;
                        flag = !flag;
                    }
                    else
                    {
                        //spawn platform
                        num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH;
                        int tex = GameConstants.RANDOM.Next(m_PlatTextures.Length);

                        if (num > tilesToSpawn)
                        {
                            num = tilesToSpawn;
                        }

                        plats.Add(new Platform(new Vector2((m_WindowTilesWide - tilesToSpawn) * GameConstants.TILE_DIM, -GameConstants.TILE_DIM * 2 - (GameConstants.RANDOM.Next(GameConstants.PLAT_MIN_Y_SPAWN_DELTA, GameConstants.PLAT_MAX_Y_SPAWN_DELTA))), Vector2.UnitY * GameConstants.PLATFORM_VELOCITY, num, PlatformTypes.Platform, m_PlatTextures[tex]));
                        tilesToSpawn -= num;
                        flag = !flag;
                    }
                }

                m_LastSpawnY = -GameConstants.TILE_DIM*2;
                m_SpawnFlag = flag;

                return plats;
            }

            return null;
        }

    }
}
