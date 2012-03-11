#region File Description

//-----------------------------------------------------------------------------
// UVGridEditorComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/1/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVGridEditor
   {
      Texture2D m_RenderTarget;
      Texture2D m_WhiteTexture;

      Viewport m_Viewport;
      GameDriver m_Game;
      Matrix m_WorldMatrix;
      Matrix m_ViewMatrix;
      Matrix m_ProjectionMatrix;
      BasicEffect m_BasicEffect;
      VertexDeclaration m_VertexDeclaration;
      VertexPositionColor[] m_PointList;
      VertexBuffer m_VertexBuffer;
      short[] m_LineListIndices;
      List<Vector2> m_DropPoints;
      bool m_PlacingPoint;
      bool m_HasUpdates;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;
      int m_SelectedVertex;
      int m_HoveredVertex;

      public UVGridEditor(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         m_DropPoints = new List<Vector2>();

         
         m_ViewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 1.0f, 1000.0f);

         m_VertexDeclaration = new VertexDeclaration(new VertexElement[]
               {
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
               }
         );

         m_BasicEffect = new BasicEffect(m_Game.GraphicsDevice);
         m_BasicEffect.VertexColorEnabled = true;

         m_WorldMatrix = Matrix.Identity;
         m_BasicEffect.World = m_WorldMatrix;
         m_BasicEffect.View = m_ViewMatrix;
         m_BasicEffect.Projection = m_ProjectionMatrix;

         m_PlacingPoint = false;
         m_HasUpdates = false;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;

         PlacePoint(new Vector2(0.0f, 0.0f));
         PlacePoint(new Vector2(m_Viewport.Width, 0.0f));
         PlacePoint(new Vector2(m_Viewport.Width, m_Viewport.Height));
         PlacePoint(new Vector2(0.0f, m_Viewport.Height));
      }

      public void LoadContent(ContentManager content)
      {
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");
      }

      public void Reset()
      {
         m_HasUpdates = false;
         m_PlacingPoint = false;
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;

         m_DropPoints.Clear();
         PlacePoint(new Vector2(0.0f, 0.0f));
         PlacePoint(new Vector2(m_Viewport.Width, 0.0f));
         PlacePoint(new Vector2(m_Viewport.Width, m_Viewport.Height));
         PlacePoint(new Vector2(0.0f, m_Viewport.Height));

         m_ViewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, m_Viewport.Width, m_Viewport.Height, 0, 1.0f, 1000.0f);
      }

      #region Input Handling

      // Mouse bounds for easy selection
      const int MOUSE_SELECTION_BUFFER = 3;

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         Vector2 mousePos = new Vector2(mouseState.X - m_Viewport.X, mouseState.Y - m_Viewport.Y);
         Rectangle mouseBounds = new Rectangle((int)(mousePos.X - MOUSE_SELECTION_BUFFER), (int)(mousePos.Y - MOUSE_SELECTION_BUFFER), MOUSE_SELECTION_BUFFER * 2, MOUSE_SELECTION_BUFFER * 2);

         // For now, I'm also turning off the selection...may not want to do this later
         if (mouseState.LeftButton == ButtonState.Released)
         {
            m_SelectedVertex = -1;        
         }

         // Handle vertex hovering
         m_HoveredVertex = -1;
         int numVertices = m_DropPoints.Count;
         for (int i = 0; i < numVertices; ++i)
         {
            Rectangle vertex = new Rectangle((int)m_DropPoints[i].X, (int)m_DropPoints[i].Y, 1, 1);
            if (mouseBounds.Intersects(vertex))
            {
               m_HoveredVertex = i;
               break;
            }
         }

         // Check for point placement if we aren't hovering over anything
         if (m_HoveredVertex == -1 && !m_PlacingPoint && mouseState.LeftButton == ButtonState.Pressed)
         {
            m_PlacingPoint = true;
            PlacePoint(mousePos);
         }
         // Check for point selecting if we are hovering over something
         else if (m_HoveredVertex >= 0 && !m_PlacingPoint && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
         {
            // Pretend like we never placed the point and we are placing it again with it selected...yeeeeaaaahhhhh
            m_DropPoints.RemoveAt(m_HoveredVertex);
            PlacePoint(mousePos);

            // Select the hovered vertex
            m_SelectedVertex = m_HoveredVertex;
            m_HoveredVertex = -1;
            m_PlacingPoint = true;
         }
         // Check for point placement decided; this is if we were dragging the point around before we placed it
         else if (m_PlacingPoint && mouseState.LeftButton == ButtonState.Released)
         {
            m_PlacingPoint = false;
            m_DropPoints.RemoveAt(m_DropPoints.Count - 1);
            PlacePoint(mousePos);
            m_HasUpdates = true;
         }
         // Update point placement while we are dragging it around
         else if (m_PlacingPoint && mouseState.LeftButton == ButtonState.Pressed)
         {
            m_DropPoints.RemoveAt(m_DropPoints.Count - 1);
            PlacePoint(mousePos);
         }
         // Check for direct point placement if we aren't hovering over anything
         if (m_HoveredVertex == -1 && mouseState.LeftButton == ButtonState.Released && m_PrevMouseState.LeftButton == ButtonState.Pressed)
         {
            m_PlacingPoint = false;
            PlacePoint(mousePos);
            m_HasUpdates = true;
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      #endregion

      #region Rendering

      public void Draw(SpriteBatch spriteBatch)
      {
         // Render the render target texture under the grid
         spriteBatch.Begin();
         spriteBatch.Draw(m_RenderTarget, new Rectangle(0, 0, m_Viewport.Bounds.Width, m_Viewport.Bounds.Height), Color.White);
         spriteBatch.End();

         if (m_DropPoints.Count > 0)
         {
            foreach (EffectPass pass in m_BasicEffect.CurrentTechnique.Passes)
            {
               pass.Apply();

               m_Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                   PrimitiveType.LineList,
                   m_PointList,
                   0,  // vertex buffer offset to add to each element of the index buffer
                   m_PointList.Length,  // number of vertices in pointList
                   m_LineListIndices,  // the index buffer
                   0,  // first index element to read
                   m_DropPoints.Count * 4   // number of primitives to draw
               );
            }

            // Draw each drop point
            spriteBatch.Begin();
            for (int i = 0; i < m_DropPoints.Count; ++i)
            {
               spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DropPoints[i].X - 2, (int)m_DropPoints[i].Y - 2, 5, 5), Color.Black);

               if (m_HoveredVertex == i)
                  spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DropPoints[i].X - 1, (int)m_DropPoints[i].Y - 1, 3, 3), Color.Red);
               else if (m_SelectedVertex == i)
                  spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DropPoints[i].X - 1, (int)m_DropPoints[i].Y - 1, 3, 3), Color.Red);
            }
            DrawRulerTickMarks(spriteBatch);
            spriteBatch.End();
         }
      }

      const int SMALL_TICK_WIDTH = 3;
      const int MEDIUM_TICK_WIDTH = 5;
      const int LARGE_TICK_WIDTH = 7;
      private void DrawRulerTickMarks(SpriteBatch spriteBatch)
      {
         // Left side
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (m_Viewport.Height / 8), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 3 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (m_Viewport.Height / 4), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 5 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 3 * (m_Viewport.Height / 8), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 7 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (m_Viewport.Height / 2), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 9 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 5 * (m_Viewport.Height / 8), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 11 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 3 * (m_Viewport.Height / 4), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 13 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 7 * (m_Viewport.Height / 8), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 15 * (m_Viewport.Height / 16), SMALL_TICK_WIDTH, 1), Color.Black);

         // Bottom side
         spriteBatch.Draw(m_WhiteTexture, new Rectangle((m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle((m_Viewport.Width / 8), m_Viewport.Height - MEDIUM_TICK_WIDTH, 1, MEDIUM_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(3 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle((m_Viewport.Width / 4), m_Viewport.Height - LARGE_TICK_WIDTH, 1, LARGE_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(5 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(3 * (m_Viewport.Width / 8), m_Viewport.Height - MEDIUM_TICK_WIDTH, 1, MEDIUM_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(7 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle((m_Viewport.Width / 2), m_Viewport.Height - LARGE_TICK_WIDTH, 1, LARGE_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(9 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(5 * (m_Viewport.Width / 8), m_Viewport.Height - MEDIUM_TICK_WIDTH, 1, MEDIUM_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(11 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(3 * (m_Viewport.Width / 4), m_Viewport.Height - LARGE_TICK_WIDTH, 1, LARGE_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(13 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(7 * (m_Viewport.Width / 8), m_Viewport.Height - MEDIUM_TICK_WIDTH, 1, MEDIUM_TICK_WIDTH), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(15 * (m_Viewport.Width / 16), m_Viewport.Height - SMALL_TICK_WIDTH, 1, SMALL_TICK_WIDTH), Color.Black);
      }

      #endregion

      #region Grid Manipulation

      public void PlacePoint(Vector2 point)
      {
         if (m_DropPoints.Contains(point)) return;

         m_DropPoints.Add(point);

         int numPoints = m_DropPoints.Count;
         List<Vector2> points = new List<Vector2>();
         for (int i = 0; i < numPoints; ++i)
         {
            Vector2 left = new Vector2(0, m_DropPoints[i].Y);
            Vector2 top = new Vector2(m_DropPoints[i].X, 0);
            Vector2 right = new Vector2(m_Viewport.Width, m_DropPoints[i].Y);
            Vector2 bottom = new Vector2(m_DropPoints[i].X, m_Viewport.Height);
            points.Add(m_DropPoints[i]);
            points.Add(left);
            points.Add(top);
            points.Add(right);
            points.Add(bottom);
         }

         numPoints = points.Count;
         m_PointList = new VertexPositionColor[numPoints];
         for (int i = 0; i < numPoints; ++i)
         {
            m_PointList[i] = new VertexPositionColor(new Vector3(points[i].X, points[i].Y, 0.0f), Color.White);
         }

         // Initialize the vertex buffer, allocating memory for each vertex.
         m_VertexBuffer = new VertexBuffer(m_Game.GraphicsDevice, m_VertexDeclaration, numPoints, BufferUsage.None);

         // Set the vertex buffer data to the array of vertices.
         m_VertexBuffer.SetData<VertexPositionColor>(m_PointList);

         // Initialize an array of indices of type short
         int numIndices = m_DropPoints.Count * 8;
         m_LineListIndices = new short[numIndices];

         // Populate the array with references to indices in the vertex buffer
         for (int i = 0; i < m_DropPoints.Count; i++)
         {
            m_LineListIndices[(i * 8) + 0] = (short)((i * 5));
            m_LineListIndices[(i * 8) + 1] = (short)((i * 5) + 1);
            m_LineListIndices[(i * 8) + 2] = (short)((i * 5));
            m_LineListIndices[(i * 8) + 3] = (short)((i * 5) + 2);
            m_LineListIndices[(i * 8) + 4] = (short)((i * 5));
            m_LineListIndices[(i * 8) + 5] = (short)((i * 5) + 3);
            m_LineListIndices[(i * 8) + 6] = (short)((i * 5));
            m_LineListIndices[(i * 8) + 7] = (short)((i * 5) + 4);
         }
      }

      #endregion

      #region Public Access TV

      public List<VertexPositionColorTexture> GetIntersectionPoints()
      {
         List<VertexPositionColorTexture> intersectionPoints = new List<VertexPositionColorTexture>();

         // Add all of the drop points first as we know they will be intersection points
         for (int i = 0; i < m_DropPoints.Count; ++i)
         {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture(
               new Vector3(m_DropPoints[i].X, m_DropPoints[i].Y, 0.0f),
               Color.White,
               new Vector2(m_DropPoints[i].X / (float)m_Viewport.Width, m_DropPoints[i].Y / (float)m_Viewport.Height)
            );
            intersectionPoints.Add(vertex);
         }

         // Now for each point, we will have a series of intersections in x and y with every
         // other drop point's cast lines.  Calculate each one.
         for (int i = 0; i < m_DropPoints.Count; ++i)
         {
            for (int j = 0; j < m_DropPoints.Count; ++j)
            {
               // Skip ourselves
               if (i == j) continue;

               // Calculate intersection points
               VertexPositionColorTexture i1 = new VertexPositionColorTexture(
                  new Vector3(m_DropPoints[i].X, m_DropPoints[j].Y, 0.0f),
                  Color.White,
                  new Vector2(m_DropPoints[i].X / (float)m_Viewport.Width, m_DropPoints[j].Y / (float)m_Viewport.Height)
               );

               // Add unique points to the intersection list
               if (!intersectionPoints.Contains(i1))
                  intersectionPoints.Add(i1);
            }
         }

         return intersectionPoints;
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      public Texture2D RenderTarget
      {
         set { m_RenderTarget = value; }
      }

      public bool HasUpdates
      {
         get { return m_HasUpdates; }
         set { m_HasUpdates = value; }
      }

      public MouseState PrevMouseState
      {
         set { m_PrevMouseState = value; }
      }

      #endregion

   }
}
