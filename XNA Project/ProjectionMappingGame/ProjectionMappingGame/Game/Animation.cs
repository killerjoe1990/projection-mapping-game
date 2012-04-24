using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.Game
{
    public class Animation
    {
        Texture2D m_Frames;

        Color m_Color;

        float m_FrameRate;
        int m_CurrentFrame;
        float m_Counter;
        int m_NumFrames;
        bool m_Repeat;

        event EventHandler m_AnimationEnd;

        public Animation(Texture2D texture)
        {
            m_Frames = texture;

            m_Color = Color.White;

            m_FrameRate = 0;
            m_CurrentFrame = 0;
            m_Counter = 0;
            m_NumFrames = 1;
            m_Repeat = false;
        }

        public Animation(Texture2D texture, int numFrames, float rate, bool repeat)
        {
            m_CurrentFrame = 0;
            m_Frames = texture;

            m_Color = Color.White;

            if (rate != 0)
            {
                m_FrameRate = 1.0f / rate;
            }
            else
            {
                m_FrameRate = 1;
            }
            m_Counter = 0;
            m_NumFrames = numFrames;
            m_Repeat = repeat;
        }

        public Texture2D Texture
        {
            get
            {
                return m_Frames;
            }
            set
            {
                m_Frames = value;
            }
        }

        public void RegisterAnimationEnd(EventHandler handler)
        {
            m_AnimationEnd += handler;
        }

        public void SetColor(Color c)
        {
            m_Color = c;
        }
        public Color getColor()
        {
            return m_Color;
        }

        public void Update(float deltaTime)
        {
            m_Counter += deltaTime;

            int moveFrames = (int)(m_Counter / m_FrameRate);

            m_CurrentFrame += moveFrames;

            if (m_Repeat)
            {
                m_CurrentFrame %= m_NumFrames;
            }
            else
            {
                if(m_CurrentFrame >= m_NumFrames)
                {
                    m_CurrentFrame = m_NumFrames - 1;

                    if (m_AnimationEnd != null)
                    {
                        m_AnimationEnd(this, null);
                    }
                }
            }

            if (moveFrames > 0)
            {
                m_Counter = m_Counter % m_FrameRate;
            }
        }

        public void Reset()
        {
            m_CurrentFrame = 0;
            m_Counter = 0;
        }

        public void Draw(SpriteBatch batch, Rectangle bounds, SpriteEffects effect)
        {
            int width = m_Frames.Width / m_NumFrames;
            int height = m_Frames.Height;

            Rectangle source = new Rectangle(width * m_CurrentFrame, 0, width, height);

            batch.Draw(m_Frames, bounds, source, m_Color, 0, Vector2.Zero, effect, 0);
        }

    }

    public class AnimatedBackground
    {
        Texture2D[] m_Frames;

        float m_Rate;
        int m_NumFrames;

        int m_CurrentFrame;
        float m_Counter;

        Point m_Size;

        Color m_Color;

        public AnimatedBackground(Texture2D[] frames, float rate, int width, int height)
        {
            if (rate != 0)
            {
                m_Rate = 1.0f / rate;
            }
            else
            {
                m_Rate = 1;
            }

            m_Frames = frames;
            m_NumFrames = frames.Length;

            m_CurrentFrame = 0;
            m_Counter = 0;
            m_Color = Color.White;

            m_Size = new Point(width, height);
        }

        public void SetColor(Color c)
        {
            m_Color = c;
        }

        public void Update(float deltaTime)
        {
            m_Counter += deltaTime;

            int moveFrames = (int)(m_Counter / m_Rate);

            m_CurrentFrame += moveFrames;

            m_CurrentFrame %= m_NumFrames;

            if (moveFrames > 0)
            {
                m_Counter %= m_Rate;
            }
        }

        public void Draw(SpriteBatch batch, SpriteEffects effect)
        {
            batch.Draw(m_Frames[m_CurrentFrame], new Rectangle(0, 0, m_Size.X, m_Size.Y), null, m_Color, 0, Vector2.Zero, effect, 1);
        }
    }
}
