using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.GUI
{
   class Checkbox : ClickableElement
   {
      public enum ImageType
      {
         CHECKED_NORMAL = 0,
         CHECKED_OVER = 1,
         CHECKED_CLICK = 2,
         UNCHECKED_NORMAL = 3,
         UNCHECKED_OVER = 4,
         UNCHECKED_CLICK = 5
      }

      protected Texture2D[] m_Images;
      protected int m_CurrentImage;
      protected string m_Text;
      protected Color m_TextColor;
      protected Vector2 m_TextPos;
      protected Vector2 m_CheckPos;
      protected SpriteFont m_Font;
      protected bool m_IsChecked;
      protected bool m_BeenClicked;

      protected event EventHandler m_OnClick;

      public Checkbox(Rectangle bounds, bool isChecked, Texture2D checkedTexture, Texture2D checkedOnHoverTexture, Texture2D checkedOnPressTexture, Texture2D uncheckedTexture, Texture2D uncheckedOnHoverTexture, Texture2D uncheckedOnPressTexture, MouseInput mouse, SpriteFont font, string text, Color textColor)
      {
         m_BeenClicked = false;
         m_IsChecked = isChecked;
         int numImages = Enum.GetNames(typeof(ImageType)).Length;
         m_Images = new Texture2D[numImages];
         m_Images[(int)ImageType.CHECKED_NORMAL] = checkedTexture;
         m_Images[(int)ImageType.CHECKED_OVER] = checkedOnHoverTexture;
         m_Images[(int)ImageType.CHECKED_CLICK] = checkedOnPressTexture;
         m_Images[(int)ImageType.UNCHECKED_NORMAL] = uncheckedTexture;
         m_Images[(int)ImageType.UNCHECKED_OVER] = uncheckedOnHoverTexture;
         m_Images[(int)ImageType.UNCHECKED_CLICK] = uncheckedOnPressTexture;
         m_CurrentImage = (m_IsChecked) ? 0 : 3;
         m_Font = font;
         m_Text = text;
         m_TextColor = textColor;
         m_Bounds = bounds;

         mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);
         mouse.RegisterMouseEvent(MouseEventType.Move, OnOver);
         mouse.RegisterMouseEvent(MouseEventType.LeftDrag, OnLeftDrag);
         m_TextPos = new Vector2(m_Bounds.X, (int)(m_Bounds.Y + (m_Bounds.Height / 2) - (m_Font.MeasureString(m_Text).Y / 2)));
         m_CheckPos = new Vector2(m_Bounds.X + m_Bounds.Width - m_Bounds.Height, m_Bounds.Y);
      }

      public override void Update(float deltaTime)
      {
         base.Update(deltaTime);
      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
      {
         if (m_IsVisible)
         {
            sprite.Draw(m_Images[m_CurrentImage], new Rectangle((int)m_CheckPos.X, (int)m_CheckPos.Y, m_Bounds.Height, m_Bounds.Height), Color.White);
            sprite.DrawString(m_Font, m_Text, m_TextPos, m_TextColor);
         }
      }

      public void RegisterOnClick(EventHandler handler)
      {
         m_OnClick += handler;
      }

      protected void OnOver(object sender, MouseEventArgs args)
      {
         if (m_IsActive)
         {
            if (!m_BeenClicked)
            {
               if (IsOver(args.X, args.Y))
               {
                  if (m_IsChecked)
                     m_CurrentImage = (int)ImageType.CHECKED_OVER;
                  else
                     m_CurrentImage = (int)ImageType.UNCHECKED_OVER;
               }
               else
               {
                  if (m_IsChecked)
                     m_CurrentImage = (int)ImageType.CHECKED_NORMAL;
                  else
                     m_CurrentImage = (int)ImageType.UNCHECKED_NORMAL;
               }
            }
            else
            {
               if (!IsOver(args.X, args.Y))
               {
                  if (m_IsChecked)
                     m_CurrentImage = (int)ImageType.CHECKED_NORMAL;
                  else
                     m_CurrentImage = (int)ImageType.UNCHECKED_NORMAL;
               }
            }
         }
      }

      protected override void OnLeftDrag(object sender, MouseEventArgs args)
      {
         if (m_IsActive)
         {
            if (IsOver(args.X, args.Y))
            {
               m_BeenClicked = true;
               if (m_IsChecked)
                  m_CurrentImage = (int)ImageType.CHECKED_CLICK;
               else
                  m_CurrentImage = (int)ImageType.UNCHECKED_CLICK;
            }
         }
      }

      protected override void OnLeftClick(object sender, MouseEventArgs args)
      {
         if (m_IsActive)
         {
            if (IsOver(args.X, args.Y))
            {
               m_IsChecked = !m_IsChecked;
               if (m_OnClick != null)
                  m_OnClick(this, new EventArgs());
               if (m_IsChecked)
                  m_CurrentImage = (int)ImageType.CHECKED_NORMAL;
               else
                  m_CurrentImage = (int)ImageType.UNCHECKED_NORMAL;
            }

            m_BeenClicked = false;
         }

      }

      public bool IsChecked
      {
         get { return m_IsChecked; }
         set
         {
            m_IsChecked = value;
            if (m_IsChecked)
               m_CurrentImage = (int)ImageType.CHECKED_NORMAL;
            else
               m_CurrentImage = (int)ImageType.UNCHECKED_NORMAL;
         }
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
            m_TextPos = new Vector2(m_Bounds.X, (int)(m_Bounds.Y + (m_Bounds.Height / 2) - (m_Font.MeasureString(m_Text).Y / 2)));
         }
      }
   }
}
