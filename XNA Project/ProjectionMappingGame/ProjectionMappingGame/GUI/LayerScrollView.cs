
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
using Microsoft.Xna.Framework.Input;

using ProjectionMappingGame.Editor;

#endregion

namespace ProjectionMappingGame.GUI
{
   class LayerScrollView : ScrollView
   {
      int m_HoveredLayer;
      int m_SelectedLayer;
      List<Layer> m_Layers;
      
      public LayerScrollView(Rectangle displayBounds, Rectangle scrollBounds, Texture2D background, Texture2D scrollpad, Texture2D scrollarea, Texture2D whiteTexture, MouseInput mouse)
         : base(displayBounds, scrollBounds, background, scrollpad, scrollarea, whiteTexture, mouse)
      {
         m_HoveredLayer = -1;
         m_SelectedLayer = -1;
         m_Layers = new List<Layer>();
      }

      public void AddLayer(Layer l)
      {
         m_Layers.Add(l);
      }

      public void DeleteSelectedLayer()
      {
         if (m_SelectedLayer == -1) return;

         m_Layers.RemoveAt(m_SelectedLayer);
         for (int i = m_SelectedLayer; i < m_Layers.Count; ++i)
         {
            m_Layers[i].Offset(new Vector2(0, -(Layer.LAYER_PADDING_Y + Layer.LAYER_HEIGHT)));
            m_Layers[i].Name = "Gameplay " + (i + 1).ToString();
         }
         m_SelectedLayer = -1;
      }

      public void Clear()
      {
         m_Layers.Clear();
      }

      public void Update(float elapsedTime)
      {
         for (int i = 0; i < m_Layers.Count; ++i)
         {
            if (m_Layers[i].NeedToSelectNormal)
            {
               m_Layers[i].NeedToSelectNormal = false;
               if (m_OnEnterNormalSelectionMode != null)
                  m_OnEnterNormalSelectionMode(this, new EventArgs());
               break;
            }
         }
      }

      public void HandleInput(MouseState mouseState, MouseState prevMouseState)
      {
         if (m_IsActive)
         {
            Rectangle mouseBounds = new Rectangle(mouseState.X - 2, mouseState.Y - 2, 5, 5);
            if (mouseBounds.Intersects(m_DisplayBounds))
            {
               mouseBounds.X -= m_Viewport.X;
               mouseBounds.Y -= m_Viewport.Y;

               // Handle layer hovering
               m_HoveredLayer = -1;
               for (int i = 0; i < m_Layers.Count; ++i)
               {
                  Rectangle layerBounds = m_Layers[i].Bounds;
                  if (mouseBounds.Intersects(layerBounds))
                  {
                     m_HoveredLayer = i;
                     break;
                  }
               }

               // Handle layer selection
               if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
               {
                  m_SelectedLayer = -1;
                  if (m_HoveredLayer >= 0)
                  {
                     m_SelectedLayer = m_HoveredLayer;
                     m_HoveredLayer = -1;
                  }
               }
            }
            else
            {
               m_HoveredLayer = -1;
            }
         }
         else
         {
            m_HoveredLayer = -1;
         }
      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         spriteBatch.End();
         Viewport defaultViewport = graphics.Viewport;
         graphics.Viewport = m_Viewport;
         spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

         // Draw scroll area and scrollbar
         spriteBatch.Draw(m_BackgroundTexture, new Rectangle(0, 0, m_DisplayBounds.Width, m_DisplayBounds.Height), Color.White);
         spriteBatch.Draw(m_ScrollAreaTexture, new Rectangle(m_ScrollBarBounds.X - m_DisplayBounds.X, 0, m_ScrollBarBounds.Width, m_ScrollBarBounds.Height), Color.White);

         Vector2 displayPos = new Vector2(m_DisplayBounds.Location.X, m_DisplayBounds.Location.Y);

         // Render hovered shade
         if (m_HoveredLayer >= 0)
         {
            Color hoverColor = Color.CornflowerBlue;
            hoverColor.A = (byte)128;
            spriteBatch.Draw(m_WhiteTexture, m_Layers[m_HoveredLayer].Bounds, hoverColor);
         }

         // Render selected shade
         if (m_SelectedLayer >= 0)
         {
            Color selectedColor = Color.CornflowerBlue;
            spriteBatch.Draw(m_WhiteTexture, m_Layers[m_SelectedLayer].Bounds, selectedColor);
         }

         // Render children
         for (int i = 0; i < m_Layers.Count; ++i)
         {
            m_Layers[i].Draw(graphics, spriteBatch);
         }
         spriteBatch.End();
         spriteBatch.Begin();
         graphics.Viewport = defaultViewport;
      }

      public void ToggleLayerControlsEnabled(bool enabled)
      {
         for (int i = 0; i < m_Layers.Count; ++i)
         {
            //m_Layers[i].VisibleButton.IsActive = enabled;
            m_Layers[i].EditButton.IsActive = enabled;
            m_Layers[i].WidthSpinBox.IsActive = enabled;
            m_Layers[i].HeightSpinBox.IsActive = enabled;
         }
      }

      EventHandler m_OnEnterNormalSelectionMode;
      public void RegisterOnEnterNormalSelectionMode(EventHandler handler)
      {
         m_OnEnterNormalSelectionMode += handler;
      }
      
      public int NumLayers
      {
         get { return m_Layers.Count; }
      }

      public int SelectedLayer
      {
         get { return m_SelectedLayer; }
      }

      public List<Layer> Layers
      {
         get { return m_Layers; }
      }
   }
}
