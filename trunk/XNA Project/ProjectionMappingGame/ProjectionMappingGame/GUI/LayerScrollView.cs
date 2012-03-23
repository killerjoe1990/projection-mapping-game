
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

      GameDriver m_Game;

      int m_DraggingLineFrom;
      int m_DraggingLineTo;
      bool m_DraggingLine;

      Matrix m_ViewMatrix;
      Matrix m_ProjectionMatrix;
      BasicEffect m_LineEffect;
      VertexDeclaration m_LineVertexDeclaration;
      VertexPositionColor[] m_PointList;
      short[] m_LineListIndices;

      public LayerScrollView(GameDriver game, Rectangle displayBounds, Rectangle scrollBounds, Texture2D background, Texture2D scrollpad, Texture2D scrollarea, Texture2D whiteTexture, MouseInput mouse)
         : base(displayBounds, scrollBounds, background, scrollpad, scrollarea, whiteTexture, mouse)
      {
         m_Game = game;
         m_HoveredLayer = -1;
         m_SelectedLayer = -1;
         m_DraggingLineFrom = -1;
         m_DraggingLineTo = -1;
         m_Layers = new List<Layer>();

         // Compute space matrices
         m_ViewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, displayBounds.Width, displayBounds.Height, 0, 1.0f, 1000.0f);

         // Initialize line
         m_DraggingLine = false;
         m_PointList = new VertexPositionColor[2];
         m_PointList[0] = new VertexPositionColor(new Vector3(10, 10, 0.0f), Color.Black);
         m_PointList[1] = new VertexPositionColor(new Vector3(100, 50, 0.0f), Color.Black);
         m_LineListIndices = new short[2];
         m_LineListIndices[0] = 0;
         m_LineListIndices[1] = 1;
         m_LineVertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
               new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
               new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            }
         );
         m_LineEffect = new BasicEffect(m_Game.GraphicsDevice);
         m_LineEffect.VertexColorEnabled = true;
         m_LineEffect.World = Matrix.Identity;
         m_LineEffect.View = m_ViewMatrix;
         m_LineEffect.Projection = m_ProjectionMatrix;
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
         for (int i = 0; i < m_Layers.Count; ++i)
         {
            m_Layers[i].UnLinkWithLayer(m_SelectedLayer);
         }
         m_SelectedLayer = -1;
      }

      public void Clear()
      {
         m_Layers.Clear();
      }

      public override void Update(float elapsedTime)
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

         for (int i = 0; i < m_Layers.Count; ++i)
         {
            if (m_Layers[i].NeedToDragLine)
            {
               m_Layers[i].NeedToDragLine = false;

               if (m_Layers.Count > 1)
               {
                  m_DraggingLineFrom = m_DraggingLineTo = i;
                  m_DraggingLine = true;
                  Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                  mousePos.X -= m_Viewport.X;
                  mousePos.Y -= m_Viewport.Y;
                  m_PointList[0] = new VertexPositionColor(new Vector3(mousePos.X, mousePos.Y, 0.0f), Color.Black);
                  m_PointList[1] = new VertexPositionColor(new Vector3(mousePos.X, mousePos.Y, 0.0f), Color.Black);
               }
               break;
            }
         }
      }

      public void HandleInput(MouseState mouseState, MouseState prevMouseState)
      {
         if (m_IsActive)
         {
            Rectangle mouseBounds = new Rectangle(mouseState.X - 2, mouseState.Y - 2, 5, 5);
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
            
            if (mouseBounds.Intersects(m_DisplayBounds))
            {
               mouseBounds.X -= m_Viewport.X;
               mouseBounds.Y -= m_Viewport.Y;
               mousePos.X -= m_Viewport.X;
               mousePos.Y -= m_Viewport.Y;

               if (m_DraggingLine)
               {
                  m_PointList[1] = new VertexPositionColor(new Vector3(mousePos.X, mousePos.Y, 0.0f), Color.Black);

                  if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                  {
                     m_DraggingLine = false;
                     if (m_DraggingLineFrom != m_DraggingLineTo)
                     {
                        if (m_Layers[m_DraggingLineFrom].AlreadyLinkedWithLayer(m_DraggingLineTo))
                        {
                           m_Layers[m_DraggingLineFrom].UnLinkWithLayer(m_DraggingLineTo);
                           m_Layers[m_DraggingLineTo].UnLinkWithLayer(m_DraggingLineFrom);
                        }
                        else
                        {
                           m_Layers[m_DraggingLineFrom].LinkWithLayer(m_DraggingLineTo);
                           m_Layers[m_DraggingLineTo].LinkWithLayer(m_DraggingLineFrom);
                        }
                     }
                     m_DraggingLineFrom = -1;
                     m_DraggingLineTo = -1;
                  }
               }

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
               if (m_HoveredLayer > -1 && m_DraggingLine)
               {
                  m_DraggingLineTo = m_HoveredLayer;
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
               m_DraggingLine = false;
            }
         }
         else
         {
            m_HoveredLayer = -1;
            m_DraggingLine = false;
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

         if (m_DraggingLine)
         {
            foreach (EffectPass pass in m_LineEffect.CurrentTechnique.Passes)
            {
               pass.Apply();

               m_Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                   PrimitiveType.LineList,
                   m_PointList,
                   0,  // vertex buffer offset to add to each element of the index buffer
                   m_PointList.Length,  // number of vertices in pointList
                   m_LineListIndices,  // the index buffer
                   0,  // first index element to read
                   1   // number of primitives to draw
               );
            }
         }

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

      public void UpdateProjection()
      {
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, m_DisplayBounds.Width, m_DisplayBounds.Height, 0, 1.0f, 1000.0f);
         m_LineEffect.Projection = m_ProjectionMatrix;
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
