using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    public class MoveableObject
    {
        protected Vector2 m_Velocity;
        protected Vector2 m_Position;

        protected Rectangle m_Bounds;

        protected Animation m_CurrentAnimation;

        public MoveableObject(Rectangle bounds, Vector2 velocity, Texture2D image)
        {
            m_Bounds = bounds;
            m_Velocity = velocity;

            m_Position = new Vector2(bounds.X, bounds.Y);
            m_CurrentAnimation = new Animation(image);
        }

        public MoveableObject(Rectangle bounds, Vector2 velocity)
        {
            m_Bounds = bounds;
            m_Velocity = velocity;

            m_Position = new Vector2(bounds.X, bounds.Y);
            m_CurrentAnimation = null;
        }

        public Vector2 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
                m_Bounds.X = (int)m_Position.X;
                m_Bounds.Y = (int)m_Position.Y;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return m_Velocity;
            }
            set
            {
                m_Velocity = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return m_Bounds;
            }
        }

        public void Update(float deltaTime)
        {
            m_Position += m_Velocity * deltaTime;

            m_Bounds.X = (int)m_Position.X;
            m_Bounds.Y = (int)m_Position.Y;

            if (m_CurrentAnimation != null)
            {
                m_CurrentAnimation.Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (m_CurrentAnimation != null)
            {
                m_CurrentAnimation.Draw(batch, m_Bounds, SpriteEffects.None);
            }
        }
    }
}
