using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectionMappingGame.FileSystem;
using ProjectionMappingGame.StateMachine;

namespace ProjectionMappingGame.Game
{
    public class Theme
    {
        const int NUM_MOVING_OBJECTS = 8;
        const int OBJECT_SIZE_MAX = 64;
        const int OBJECT_SIZE_MIN = 32;

        const float OBJECT_SPEED_MAX = 45;
        const float OBJECT_SPEED_MIN = 22;

        const float OBJECT_TIME_MAX = 3;
        const float OBJECT_TIME_MIN = 0.3f;

        const int OBJECT_FRAMES = 1;
        const int OBJECT_RATE = 5;

        const int BACKGROUND_RATE = 10;

        // Contains all Texture2D data needed.
        protected ThemeTextures m_Textures;
        protected Point m_WindowSize;
        protected AnimatedBackground m_Background;
        protected List<MoveableObject> m_Objects;

        protected float m_StaticTimer;


        public Theme(ThemeTextures textures, Point windowSize)
        {
            m_Textures = textures;
            m_WindowSize = windowSize;

            m_Background = new AnimatedBackground(m_Textures.Background, BACKGROUND_RATE, windowSize.X, windowSize.Y);

            m_Objects = new List<MoveableObject>();

            if (m_Textures.MovingSprites.Length > 0)
            {
                for (int i = 0; i < NUM_MOVING_OBJECTS; ++i)
                {
                    int size = GameConstants.RANDOM.Next(OBJECT_SIZE_MAX - OBJECT_SIZE_MIN) + OBJECT_SIZE_MIN;
                    float speed = (float)GameConstants.RANDOM.NextDouble() * (OBJECT_SPEED_MAX - OBJECT_SPEED_MIN) + OBJECT_SPEED_MIN;
                    Vector2 position = new Vector2();
                    position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                    position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;
                    Vector2 direction = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ((float)GameConstants.RANDOM.NextDouble() * MathHelper.TwoPi));

                    MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), speed * direction);

                    int texIndex = GameConstants.RANDOM.Next(textures.MovingSprites.Length);

                    obj.Animation = new Animation(textures.MovingSprites[texIndex], OBJECT_FRAMES, OBJECT_RATE, true);

                    m_Objects.Add(obj);
                }
            }

            m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (OBJECT_TIME_MAX - OBJECT_TIME_MIN) + OBJECT_TIME_MIN;
        }

        public void Update(float deltaTime)
        {
            m_Background.Update(deltaTime);

            for(int i = m_Objects.Count - 1; i >= 0; --i)
            {
                m_Objects[i].Update(deltaTime);
            }

            if (m_Textures.StaticSprites.Length > 0)
            {
                m_StaticTimer -= deltaTime;

                if (m_StaticTimer < 0)
                {
                    int size = GameConstants.RANDOM.Next(OBJECT_SIZE_MAX - OBJECT_SIZE_MIN) + OBJECT_SIZE_MIN;
                    Vector2 position = new Vector2();
                    position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                    position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;

                    MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), Vector2.Zero);

                    int texIndex = GameConstants.RANDOM.Next(m_Textures.StaticSprites.Length);

                    obj.Animation = new Animation(m_Textures.StaticSprites[texIndex], 6, OBJECT_RATE, false);
                    obj.Animation.RegisterAnimationEnd(StaticObjectDone);

                    m_Objects.Add(obj);

                    m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (OBJECT_TIME_MAX - OBJECT_TIME_MIN) + OBJECT_TIME_MIN;
                }
            }
        }

        public void Draw(SpriteBatch batch, Color color)
        {
            m_Background.SetColor(color);
            m_Background.Draw(batch, SpriteEffects.None);

            foreach (MoveableObject m in m_Objects)
            {
                m.Draw(batch, color);
            }
        }

        public void StaticObjectDone(object sender, EventArgs args)
        {
            for (int i = m_Objects.Count - 1; i >= 0; --i)
            {
                if (m_Objects[i].Animation == sender)
                {
                    m_Objects.RemoveAt(i);
                    break;
                }
            }
        }

        public ThemeTextures Textures
        {
            get
            {
                return m_Textures;
            }
        }
    }
}
