
#region File Description

//-----------------------------------------------------------------------------
// ScrollView.cs
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
   class ScrollView : ClickableElement
   {
      event EventHandler m_OnClick;

      const int SCROLL_BAR_WIDTH = 16;

      Rectangle m_DisplayBounds;
      Rectangle m_ScrollBounds;
      Rectangle m_ScrollBarBounds;
      Texture2D m_BackgroundTexture;
      Texture2D m_ScrollPadTexture;
      Texture2D m_ScrollAreaTexture;

      public ScrollView(Rectangle displayBounds, Rectangle scrollBounds, Texture2D background, Texture2D scrollpad, Texture2D scrollarea, MouseInput mouse)
      {
         m_DisplayBounds = displayBounds;
         m_ScrollBounds = scrollBounds;
         m_ScrollBarBounds = new Rectangle(displayBounds.X + displayBounds.Width - SCROLL_BAR_WIDTH, displayBounds.Y, SCROLL_BAR_WIDTH, displayBounds.Height);
         m_BackgroundTexture = background;
         m_ScrollPadTexture = scrollpad;
         m_ScrollAreaTexture = scrollarea;
      }

      public override void Update(float deltaTime)
      {
         // Update children
         for (int i = 0; i < m_Children.Count; ++i)
         {
            m_Children[i].Update(deltaTime);
         }

         base.Update(deltaTime);
      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
      {
         // Draw scroll area and scrollbar
         sprite.Draw(m_BackgroundTexture, m_DisplayBounds, Color.White);
         sprite.Draw(m_ScrollAreaTexture, m_ScrollBarBounds, Color.White);

         // Render children
         for (int i = 0; i < m_Children.Count; ++i)
         {
            m_Children[i].Draw(graphics, sprite);
         }
      }

      #region Public Access TV

      public Rectangle DisplayBounds
      {
         get { return m_DisplayBounds; }
         set 
         {
            m_DisplayBounds = value;
            m_ScrollBarBounds = new Rectangle(m_DisplayBounds.X + m_DisplayBounds.Width - SCROLL_BAR_WIDTH, m_DisplayBounds.Y, SCROLL_BAR_WIDTH, m_DisplayBounds.Height);
         }
      }

      #endregion

   }
}
