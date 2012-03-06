
#region File Description

//-----------------------------------------------------------------------------
// ScrollView.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    3/1/2012
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

using ProjectionMappingGame.Editor;

#endregion

namespace ProjectionMappingGame.GUI
{
   class LayerScrollView : ScrollView
   {
      List<Layer> m_Layers;

      public LayerScrollView(Rectangle displayBounds, Rectangle scrollBounds, Texture2D background, Texture2D scrollpad, Texture2D scrollarea, MouseInput mouse)
         : base(displayBounds, scrollBounds, background, scrollpad, scrollarea, mouse)
      {
         m_Layers = new List<Layer>();
      }

      public void AddLayer(Layer l)
      {
         m_Layers.Add(l);
      }

      public void Clear()
      {
         m_Layers.Clear();
      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         // Draw scroll area and scrollbar
         spriteBatch.Draw(m_BackgroundTexture, m_DisplayBounds, Color.White);
         spriteBatch.Draw(m_ScrollAreaTexture, m_ScrollBarBounds, Color.White);

         Vector2 displayPos = new Vector2(m_DisplayBounds.Location.X, m_DisplayBounds.Location.Y);

         // Render children
         for (int i = 0; i < m_Layers.Count; ++i)
         {
            m_Layers[i].NameLabel.Location += displayPos;
            m_Layers[i].EditButton.Location += displayPos;
            m_Layers[i].VisibleButton.Location += displayPos;
            m_Layers[i].NameLabel.Draw(graphics, spriteBatch);
            m_Layers[i].EditButton.Draw(graphics, spriteBatch);
            m_Layers[i].VisibleButton.Draw(graphics, spriteBatch);
            m_Layers[i].NameLabel.Location -= displayPos;
            m_Layers[i].EditButton.Location -= displayPos;
            m_Layers[i].VisibleButton.Location -= displayPos;
         }
      }

      public int NumLayers
      {
         get { return m_Layers.Count; }
      }

      
   }
}
