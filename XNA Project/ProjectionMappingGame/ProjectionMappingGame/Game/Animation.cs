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

            m_FrameRate = 1.0f/rate;
            m_Counter = 0;
            m_NumFrames = numFrames;
            m_Repeat = repeat;
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
                m_CurrentFrame = (m_CurrentFrame >= m_NumFrames) ? m_NumFrames - 1 : m_CurrentFrame;
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
}
