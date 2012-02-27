
#region File Description

//-----------------------------------------------------------------------------
// Panel.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/23/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.GUI
{
   class Panel : UIElement
   {
      const int BORDER_THICKNESS = 2;
      const int TITLE_HEIGHT = 20;

      Color m_TitleColor;
      Color m_TitleFillColorStart;
      Color m_TitleFillColorEnd;
      Color m_BorderColor;
      Texture2D m_WhiteTexture;
      Color m_FillColor;
      string m_TitleText;
      SpriteFont m_Font;

      public Panel(Rectangle bounds, string title, Color titleColor, SpriteFont titleFont, Color titleFillColorStart, Color titleFillColorEnd, Texture2D whiteTexture, Color fillColor, Color borderColor)
         : base()
      {
         m_TitleColor = titleColor;
         m_TitleFillColorStart = titleFillColorStart;
         m_TitleFillColorEnd = titleFillColorEnd;
         m_WhiteTexture = whiteTexture;
         m_FillColor = fillColor;
         m_BorderColor = borderColor;
         m_TitleText = title;
         m_Font = titleFont;
         m_Bounds = bounds;
      }

      public override void Update(float deltaTime)
      {
      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
      {
         sprite.Begin();

         // Draw fill
         sprite.Draw(m_WhiteTexture, m_Bounds, m_FillColor);

         // Draw title
         for (int x = m_Bounds.X; x < m_Bounds.X + m_Bounds.Width; ++x)
         {
            float t = (float)(x - m_Bounds.X) / (float)m_Bounds.Width;

            Color intrpColor = Color.Lerp(m_TitleFillColorEnd, m_TitleFillColorStart, t);
            sprite.Draw(m_WhiteTexture, new Rectangle(x, m_Bounds.Y, 1, TITLE_HEIGHT), intrpColor);
         }
         sprite.DrawString(m_Font, m_TitleText, new Vector2(m_Bounds.X + 4, m_Bounds.Y + 2), m_TitleColor);

         // Draw border
         sprite.Draw(m_WhiteTexture, new Rectangle(m_Bounds.X, m_Bounds.Y, m_Bounds.Width, BORDER_THICKNESS), m_BorderColor);
         sprite.Draw(m_WhiteTexture, new Rectangle(m_Bounds.X, m_Bounds.Y + m_Bounds.Height, m_Bounds.Width, BORDER_THICKNESS), m_BorderColor);
         sprite.Draw(m_WhiteTexture, new Rectangle(m_Bounds.X, m_Bounds.Y, BORDER_THICKNESS, m_Bounds.Height), m_BorderColor);
         sprite.Draw(m_WhiteTexture, new Rectangle(m_Bounds.X + m_Bounds.Width, m_Bounds.Y, BORDER_THICKNESS, m_Bounds.Height), m_BorderColor);

         sprite.End();
      }
   }
}
