using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.GUI
{
    public class Button : ClickableElement
    {
        public enum ImageType
        {
            NORMAL = 0,
            OVER = 1,
            CLICK = 2
        }

        protected Texture2D[] m_Images;
        protected int m_CurrentImage;

        protected bool m_BeenClicked;

        protected event EventHandler m_OnClick;

        public Button(Rectangle bounds, Texture2D image, MouseInput mouse)
        {
            int numImages = Enum.GetNames(typeof(ImageType)).Length;
            m_Images = new Texture2D[numImages];

            m_CurrentImage = 0;

            for (int i = 0; i < numImages; ++i)
            {
                m_Images[i] = null;
            }

            m_Images[(int)ImageType.NORMAL] = image;
           
            m_Bounds = bounds;

            mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);
            mouse.RegisterMouseEvent(MouseEventType.Move, OnOver);
            mouse.RegisterMouseEvent(MouseEventType.LeftDrag, OnLeftDrag);

            m_BeenClicked = false;
        }

        public void SetImage(ImageType type, Texture2D img)
        {
            m_Images[(int)type] = img;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
            if (m_Images[m_CurrentImage] != null)
            {
                sprite.Draw(m_Images[m_CurrentImage], m_Bounds, Color.White);
            }
            else
            {
                sprite.Draw(m_Images[(int)ImageType.NORMAL], m_Bounds, Color.White);
            }
        }

        public void RegisterOnClick(EventHandler handler)
        {
            m_OnClick += handler;
        }

        public void OnOver(object sender, MouseEventArgs args)
        {
            if (!m_BeenClicked)
            {
                if (IsOver(args.X, args.Y))
                {
                    m_CurrentImage = (int)ImageType.OVER;
                }
                else
                {
                    m_CurrentImage = (int)ImageType.NORMAL;
                }
            }
            else
            {
                if (!IsOver(args.X, args.Y))
                {
                    m_CurrentImage = (int)ImageType.NORMAL;
                }
            }
        }

        public override void OnLeftDrag(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
                m_BeenClicked = true;
                m_CurrentImage = (int)ImageType.CLICK;
            }
        }

        public override void OnLeftClick(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
                m_OnClick(this, new EventArgs());
                m_CurrentImage = (int)ImageType.NORMAL;
            }

            m_BeenClicked = false;
        }
    }
}
