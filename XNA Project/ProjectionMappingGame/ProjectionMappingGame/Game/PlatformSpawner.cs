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

        bool m_SpawnFlag;

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

            m_SpawnFlag = false;
        }

        public void ChangeTheme(Texture2D[][] platforms)
        {
            m_PlatTextures = platforms;
        }

        public List<Platform> SpawnPlatforms(float deltaTime)
        {
            int tex = 0;
            m_LastSpawnY += deltaTime * GameConstants.PLATFORM_VELOCITY;

            if (m_LastSpawnY >= m_MinRange.Y)
            {
                List<Platform> plats = new List<Platform>();

                int tilesToSpawn = m_WindowTilesWide;
                int num;
               
                while (tilesToSpawn > 0)
                {
                    if (m_SpawnFlag == true)
                    {
                        //spawn space
                        num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH;
                        if (num > tilesToSpawn)
                        {
                            num = tilesToSpawn;
                        }

                        tilesToSpawn -= num;
                        m_SpawnFlag = !m_SpawnFlag;
                    }
                    else
                    {
                        //spawn platform
                        num = GameConstants.RANDOM.Next(GameConstants.PLAT_MAX_WIDTH - GameConstants.PLAT_MIN_WIDTH) + GameConstants.PLAT_MIN_WIDTH;
                        tex = GameConstants.RANDOM.Next(m_PlatTextures.Length - 1) + 1;

                        if (num > tilesToSpawn)
                        {
                            num = tilesToSpawn;
                        }

                        int chanceForBlinkPlat = GameConstants.RANDOM.Next(0,100);
                        if (chanceForBlinkPlat < GameConstants.CHANCE_TO_SPAWN_BLINKPLAT)
                        {
                            BlinkingPlatform b = new BlinkingPlatform(new Vector2((m_WindowTilesWide - tilesToSpawn) * GameConstants.TILE_DIM, -GameConstants.TILE_DIM - GameConstants.RANDOM.Next(GameConstants.PLAT_MIN_Y_SPAWN_DELTA, GameConstants.PLAT_MAX_Y_SPAWN_DELTA)), Vector2.UnitY * GameConstants.PLATFORM_VELOCITY, num, m_PlatTextures[0]);
                            plats.Add(b);
                        }
                        else
                        {
                             RegularPlatform p = new RegularPlatform(new Vector2((m_WindowTilesWide - tilesToSpawn) * GameConstants.TILE_DIM, -GameConstants.TILE_DIM - GameConstants.RANDOM.Next(GameConstants.PLAT_MIN_Y_SPAWN_DELTA,GameConstants.PLAT_MAX_Y_SPAWN_DELTA)), Vector2.UnitY * GameConstants.PLATFORM_VELOCITY, num, m_PlatTextures[tex]);
                             plats.Add(p);
                        }
                        
                        tilesToSpawn -= num;
                        m_SpawnFlag = !m_SpawnFlag;
                    }
                   
                }

                m_LastSpawnY = -GameConstants.TILE_DIM;

                return plats;
            }

            return null;
        }

    }
}
