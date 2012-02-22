using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.GUI
{
    public class Slider : ClickableElement
    {
        protected const float SLIDER_PERCENT_BORDER = 0.1f;
        protected float m_MinValue;
        protected float m_MaxValue;
        protected float m_StepSize;
        protected float m_Gradient;

        protected bool m_Clicked;

        protected Texture2D m_BackImage;
        protected Texture2D m_SlideImage;

        public Slider(Rectangle bound, Texture2D backImg, Texture2D slideImg, float min, float max, float stepSize, MouseInput mouse)
        {
            m_Bounds = bound;
            m_BackImage = backImg;
            m_SlideImage = slideImg;
            m_StepSize = stepSize / (max - min);

            m_MaxValue = max;
            m_MinValue = min;
            m_Gradient = 0;

            m_Clicked = false;

            if (mouse != null)
            {
                mouse.RegisterMouseEvent(MouseEventType.LeftDrag, OnLeftDrag);
                mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);
            }
        }

        public float Value
        {
            get
            {
                return m_MinValue * (1 - m_Gradient) + m_MaxValue * m_Gradient;
            }
            set
            {
                m_Gradient = value / (m_MaxValue - m_MinValue);
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
            sprite.Draw(m_BackImage, m_Bounds, Color.White);

            int slideHeight = (int)(m_Bounds.Height * (1 - 2 * SLIDER_PERCENT_BORDER));
            int slideWidth = (int)(slideHeight / 3);
            Rectangle slider = new Rectangle((int)(m_Bounds.X + m_Gradient * (m_Bounds.Width - slideWidth)), (int)(m_Bounds.Y + m_Bounds.Height * SLIDER_PERCENT_BORDER), slideWidth, slideHeight);

            sprite.Draw(m_SlideImage, slider, Color.White);
        }

        protected override void OnLeftDrag(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
                m_Clicked = true;
                int posX = args.X - m_Bounds.X;
                m_Gradient = MathHelper.Clamp((float)posX / (float)m_Bounds.Width, 0, 1);
            }
            else
            {
                if (m_Clicked)
                {
                    int posX = args.X - m_Bounds.X;
                    m_Gradient = MathHelper.Clamp((float)posX / (float)m_Bounds.Width, 0, 1);
                }
            }
        }

        protected override void OnLeftClick(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
                int posX = args.X - m_Bounds.X;
                m_Gradient = MathHelper.Clamp((float)posX / (float)m_Bounds.Width, 0, 1);
            }
            else
            {
                m_Clicked = false;
            }
        }

        public void OnKeyPress(object sender, Keys[] keys)
        {
            if (keys.Contains(Keys.Left))
            {
                m_Gradient = MathHelper.Clamp(m_Gradient - m_StepSize, 0, 1);
            }
            if (keys.Contains(Keys.Right))
            {
                m_Gradient = MathHelper.Clamp(m_Gradient + m_StepSize, 0, 1);
            }
        }
    }
}
