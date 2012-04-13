using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    public enum CollisionDirections
    {
        Top,
        Bot,
        Left,
        Right,
        None
    }

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

        public Animation Animation
        {
            get
            {
                return m_CurrentAnimation;
            }
            set
            {
                m_CurrentAnimation = value;
            }
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

        public void SetColor(Color color)
        {
            m_CurrentAnimation.SetColor(color);
        }

        public CollisionDirections CheckBounds(int width, int height)
        {
            CollisionDirections dir = CollisionDirections.None;

            if (m_Position.X < 0)
            {
                dir = CollisionDirections.Left;
            }
            if (m_Position.X + m_Bounds.Width > width)
            {
                dir = CollisionDirections.Right;
            }
            if (m_Position.Y < 0)
            {
                dir = CollisionDirections.Top;
            }
            if (m_Position.Y + m_Bounds.Height > height)
            {
                dir = CollisionDirections.Bot;
            }

            return dir;
        }

        public void ScreenBounce(CollisionDirections direction, Point windowSize)
        {
            switch (direction)
            {
                case CollisionDirections.Left:
                    m_Velocity.X *= -1;
                    m_Position.X = 0;
                    break;
                case CollisionDirections.Right:
                    m_Velocity.X *= -1;
                    m_Position.X = windowSize.X - m_Bounds.Width;
                    break;
                case CollisionDirections.Top:
                    m_Velocity.Y *= -1;
                    m_Position.Y = 0;
                    break;
                case CollisionDirections.Bot:
                    m_Velocity.Y *= -1;
                    m_Position.Y = windowSize.Y - m_Bounds.Height;
                    break;
            }
        }

        public virtual void Collide(MoveableObject obj)
        {

        }

        public virtual void Update(float deltaTime)
        {
            m_Position += m_Velocity * deltaTime;

            m_Bounds.X = (int)m_Position.X;
            m_Bounds.Y = (int)m_Position.Y;

            if (m_CurrentAnimation != null)
            {
                m_CurrentAnimation.Update(deltaTime);
            }
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (m_CurrentAnimation != null)
            {
                m_CurrentAnimation.SetColor(Color.White);
                m_CurrentAnimation.Draw(batch, m_Bounds, SpriteEffects.None);
            }
        }

        public virtual void Draw(SpriteBatch batch, Color color)
        {
            if (m_CurrentAnimation != null)
            {
                m_CurrentAnimation.SetColor(color);
                m_CurrentAnimation.Draw(batch, m_Bounds, SpriteEffects.None);
            }
        }

    }
}
