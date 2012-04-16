using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectionMappingGame.FileSystem;
using ProjectionMappingGame.StateMachine;

namespace ProjectionMappingGame.Game.Themes
{
    class SpaceTheme : Theme
    {
        const int NUM_ASTEROIDS = 8;
        const int ASTEROID_SIZE_MAX = 64;
        const int ASTEROID_SIZE_MIN = 32;

        const float ASTEROID_SPEED_MAX = 45;
        const float ASTEROID_SPEED_MIN = 22;

        const int ASTEROID_FRAMES = 1;
        const int ASTEROID_RATE = 5;

        const int BACKGROUND_RATE = 10;

        List<MoveableObject> m_Asteroids;

        public SpaceTheme(ThemeTextures textures, Point windowSize)
            : base(textures, windowSize)
        {
            m_Background = new AnimatedBackground(textures.Background, BACKGROUND_RATE, windowSize.X, windowSize.Y);

            m_Asteroids = new List<MoveableObject>();

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                int size = GameConstants.RANDOM.Next(ASTEROID_SIZE_MAX - ASTEROID_SIZE_MIN) + ASTEROID_SIZE_MIN;
                float speed = (float)GameConstants.RANDOM.NextDouble() * (ASTEROID_SPEED_MAX - ASTEROID_SPEED_MIN) + ASTEROID_SPEED_MIN;
                Vector2 position = new Vector2();
                position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;
                Vector2 direction = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ((float)GameConstants.RANDOM.NextDouble() * MathHelper.TwoPi));

                MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), speed * direction);
                
                int texIndex = GameConstants.RANDOM.Next(textures.SpriteSheets.Length);

                obj.Animation = new Animation(textures.SpriteSheets[texIndex], ASTEROID_FRAMES, ASTEROID_RATE, true);

                m_Asteroids.Add(obj);
            }
        }
        public override void Update(float deltaTime)
        {
            m_Background.Update(deltaTime);

            foreach (MoveableObject obj in m_Asteroids)
            {
                obj.Update(deltaTime);
                CollisionDirections direction = obj.CheckBounds(m_WindowSize.X, m_WindowSize.Y);

                if (direction != CollisionDirections.None)
                {
                    obj.ScreenBounce(direction, m_WindowSize);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Microsoft.Xna.Framework.Color color)
        {
            m_Background.SetColor(color);
            m_Background.Draw(batch, SpriteEffects.None);

            foreach (MoveableObject obj in m_Asteroids)
            {
                obj.Draw(batch, color);
            }
        }
    }
}
