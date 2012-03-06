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
        protected string m_Text;
        protected Color m_TextColor;
        protected Vector2 m_TextPos;
        protected SpriteFont m_Font;
        protected bool m_BeenClicked;
        protected bool m_HasText;

        protected event EventHandler m_OnClick;

        public Button(Rectangle bounds, Texture2D image, MouseInput mouse)
        {
            int numImages = Enum.GetNames(typeof(ImageType)).Length;
            m_Images = new Texture2D[numImages];
            m_HasText = false;
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

        public Button(Rectangle bounds, Texture2D image, MouseInput mouse, SpriteFont font, string text, Color textColor)
        {
           int numImages = Enum.GetNames(typeof(ImageType)).Length;
           m_Images = new Texture2D[numImages];
           m_HasText = true;
           m_CurrentImage = 0;

           for (int i = 0; i < numImages; ++i)
           {
              m_Images[i] = null;
           }

           m_Font = font;
           m_Text = text;
           m_TextColor = textColor;
          m_Images[(int)ImageType.NORMAL] = image;

           m_Bounds = bounds;

           mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);
           mouse.RegisterMouseEvent(MouseEventType.Move, OnOver);
           mouse.RegisterMouseEvent(MouseEventType.LeftDrag, OnLeftDrag);
           m_TextPos = new Vector2((int)(m_Bounds.X + (m_Bounds.Width / 2) - (m_Font.MeasureString(m_Text).Length() / 2)), (int)(m_Bounds.Y + (m_Bounds.Height / 2) - (m_Font.MeasureString(m_Text).Y / 2)));
           
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

            if (m_HasText)
            {
               sprite.DrawString(m_Font, m_Text, m_TextPos, m_TextColor);
            }
        }

        public void RegisterOnClick(EventHandler handler)
        {
            m_OnClick += handler;
        }

        protected void OnOver(object sender, MouseEventArgs args)
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

        protected override void OnLeftDrag(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
                m_BeenClicked = true;
                m_CurrentImage = (int)ImageType.CLICK;
            }
        }

        protected override void OnLeftClick(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y))
            {
               if (m_OnClick != null)
                  m_OnClick(this, new EventArgs());
                m_CurrentImage = (int)ImageType.NORMAL;
            }

            m_BeenClicked = false;
        }

        public Vector2 TextPos
        {
           get { return m_TextPos; }
           set { m_TextPos = value; }
        }

        public string Text
        {
           get { return m_Text; }
           set
           {
              m_Text = value; 
              m_TextPos = new Vector2((int)(m_Bounds.X + (m_Bounds.Width / 2) - (m_Font.MeasureString(m_Text).Length() / 2)), (int)(m_Bounds.Y + (m_Bounds.Height / 2) - (m_Font.MeasureString(m_Text).Y / 2)));
           }
        }
    }
}
