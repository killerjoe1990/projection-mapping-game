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
        Texture2D[] m_Frames;

        float m_FrameRate;
        int m_CurrentFrame;
        float m_Counter;

        public Animation(Texture2D texture)
        {
            m_Frames = new Texture2D[1];
            m_Frames[0] = texture;

            m_FrameRate = 0;
            m_CurrentFrame = 0;
            m_Counter = 0;

        }

        public Animation(Texture2D[] textures, float rate)
        {
            m_CurrentFrame = 0;
            m_Frames = textures;

            m_FrameRate = rate;
            m_Counter = 0;
        }

        public void Update(float deltaTime)
        {
            m_Counter += deltaTime;

            int moveFrames = (int)(m_Counter / m_FrameRate);

            m_CurrentFrame += moveFrames;

            if (moveFrames > 0)
            {
                m_Counter = m_Counter % m_FrameRate;
            }
        }

        public void Draw(SpriteBatch batch, Rectangle bounds)
        {
            batch.Draw(m_Frames[m_CurrentFrame], bounds, Color.White);
        }

    }
}
