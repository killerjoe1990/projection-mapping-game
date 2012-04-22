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

            m_Background = new AnimatedBackground(m_Textures.Background, textures.BackgroundRate, windowSize.X, windowSize.Y);

            m_Objects = new List<MoveableObject>();

            if (m_Textures.MovingSprites.Length > 0)
            {
                for (int i = 0; i < textures.NumMovingSprites; ++i)
                {
                    int size = GameConstants.RANDOM.Next(textures.MovingSpriteSize.Y - textures.MovingSpriteSize.X) + textures.MovingSpriteSize.X;
                    float speed = (float)GameConstants.RANDOM.NextDouble() * (textures.MovingSpriteSpeed.Y - textures.MovingSpriteSpeed.X) + textures.MovingSpriteSpeed.X;
                    Vector2 position = new Vector2();
                    position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                    position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;
                    Vector2 direction = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ((float)GameConstants.RANDOM.NextDouble() * MathHelper.TwoPi));

                    MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), speed * direction);

                    int texIndex = GameConstants.RANDOM.Next(textures.MovingSprites.Length);

                    obj.Animation = new Animation(textures.MovingSprites[texIndex], (int)textures.MovingSpriteInfo[texIndex].X, textures.MovingSpriteInfo[texIndex].Y, true);

                    m_Objects.Add(obj);
                }
            }

            m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (m_Textures.StaticSpriteTime.Y - m_Textures.StaticSpriteTime.X) + m_Textures.StaticSpriteTime.X;
        }

        public void Update(float deltaTime)
        {
            m_Background.Update(deltaTime);

            for(int i = m_Objects.Count - 1; i >= 0; --i)
            {
                m_Objects[i].Update(deltaTime);
            }

            foreach (MoveableObject obj in m_Objects)
            {
                obj.ScreenBounce(obj.CheckBounds(m_WindowSize.X, m_WindowSize.Y), m_WindowSize);
            }

            if (m_Textures.StaticSprites.Length > 0)
            {
                m_StaticTimer -= deltaTime;

                if (m_StaticTimer < 0)
                {
                    int size = GameConstants.RANDOM.Next(m_Textures.StaticSpriteSize.Y - m_Textures.StaticSpriteSize.X) + m_Textures.StaticSpriteSize.X;
                    Vector2 position = new Vector2();
                    position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                    position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;

                    MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), Vector2.Zero);

                    int texIndex = GameConstants.RANDOM.Next(m_Textures.StaticSprites.Length);

                    obj.Animation = new Animation(m_Textures.StaticSprites[texIndex], (int)m_Textures.StaticSpriteInfo[texIndex].X, m_Textures.StaticSpriteInfo[texIndex].Y, false);
                    obj.Animation.RegisterAnimationEnd(StaticObjectDone);

                    m_Objects.Add(obj);

                    m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (m_Textures.StaticSpriteTime.Y - m_Textures.StaticSpriteTime.X) + m_Textures.StaticSpriteTime.X;
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
