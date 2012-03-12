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
      bool m_PlacingPoint;
      bool m_HasUpdates;

      // Fonts
      SpriteFont m_ArialFont;

      // Grid
      UVGrid m_Grid;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;
      int m_SelectedVertex;
      int m_HoveredVertex;

      public UVGridEditor(GameDriver game, int x, int y, int w, int h)
      {
         // Store game and dimensions
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Define default space matrices
         m_ViewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 1.0f, 1000.0f);
         m_WorldMatrix = Matrix.Identity;
         
         // Vertex declaration for rendering lines
         m_VertexDeclaration = new VertexDeclaration(new VertexElement[]
               {
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
               }
         );

         // Configure basic line drawing effect
         m_BasicEffect = new BasicEffect(m_Game.GraphicsDevice);
         m_BasicEffect.VertexColorEnabled = true;
         m_BasicEffect.World = m_WorldMatrix;
         m_BasicEffect.View = m_ViewMatrix;
         m_BasicEffect.Projection = m_ProjectionMatrix;

         // Defaults
         m_PlacingPoint = false;
         m_HasUpdates = false;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;

         // Initialize grid 
         m_Grid = null;
      }

      public void LoadContent(ContentManager content)
      {
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial10");
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");
      }

      public void Reset()
      {
         m_HasUpdates = false;
         m_PlacingPoint = false;
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;

         // Reset the grid
         m_Grid.Reset(m_Viewport.Width, m_Viewport.Height);
         
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

         if (m_Grid != null)
         {
            Vector2 mousePos = new Vector2(mouseState.X - m_Viewport.X, mouseState.Y - m_Viewport.Y);
            Rectangle mouseBounds = new Rectangle((int)(mousePos.X - MOUSE_SELECTION_BUFFER), (int)(mousePos.Y - MOUSE_SELECTION_BUFFER), MOUSE_SELECTION_BUFFER * 2, MOUSE_SELECTION_BUFFER * 2);

            // For now, I'm also turning off the selection...may not want to do this later
            if (mouseState.LeftButton == ButtonState.Released)
            {
               m_SelectedVertex = -1;
            }

            // Handle vertex hovering
            m_HoveredVertex = -1;
            int numVertices = m_Grid.Vertices.Count;
            for (int i = 0; i < numVertices; ++i)
            {
               Rectangle vertex = new Rectangle((int)m_Grid.Vertices[i].X, (int)m_Grid.Vertices[i].Y, 1, 1);
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
               InsertPoint(mousePos);
            }
            // Check for point selecting if we are hovering over something
            else if (m_HoveredVertex >= 0 && !m_PlacingPoint && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               // Pretend like we never placed the point and we are placing it again with it selected...yeeeeaaaahhhhh
               m_Grid.RemovePoint(m_HoveredVertex);
               InsertPoint(mousePos);

               // Select the hovered vertex
               m_SelectedVertex = m_HoveredVertex;
               m_HoveredVertex = -1;
               m_PlacingPoint = true;
            }
            // Check for point placement decided; this is if we were dragging the point around before we placed it
            else if (m_PlacingPoint && mouseState.LeftButton == ButtonState.Released)
            {
               m_PlacingPoint = false;
               m_Grid.RemovePoint(m_Grid.Vertices.Count - 1);
               InsertPoint(mousePos);
               m_HasUpdates = true;
            }
            // Update point placement while we are dragging it around
            else if (m_PlacingPoint && mouseState.LeftButton == ButtonState.Pressed)
            {
               m_Grid.RemovePoint(m_Grid.Vertices.Count - 1);
               InsertPoint(mousePos);
            }
            // Check for direct point placement if we aren't hovering over anything
            if (m_HoveredVertex == -1 && mouseState.LeftButton == ButtonState.Released && m_PrevMouseState.LeftButton == ButtonState.Pressed)
            {
               m_PlacingPoint = false;
               InsertPoint(mousePos);
               m_HasUpdates = true;
            }
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      private void InsertPoint(Vector2 point)
      {
         m_Grid.InsertPoint(point);

         BuildGridLines();
      }

      private void BuildGridLines()
      {
         int numPoints = m_Grid.Vertices.Count;
         List<Vector2> points = new List<Vector2>();
         for (int i = 0; i < numPoints; ++i)
         {
            Vector2 left = new Vector2(0, m_Grid.Vertices[i].Y);
            Vector2 top = new Vector2(m_Grid.Vertices[i].X, 0);
            Vector2 right = new Vector2(m_Viewport.Width, m_Grid.Vertices[i].Y);
            Vector2 bottom = new Vector2(m_Grid.Vertices[i].X, m_Viewport.Height);
            points.Add(m_Grid.Vertices[i]);
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
         int numIndices = m_Grid.Vertices.Count * 8;
         m_LineListIndices = new short[numIndices];

         // Populate the array with references to indices in the vertex buffer
         for (int i = 0; i < m_Grid.Vertices.Count; i++)
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

      #region Rendering

      public void Draw(SpriteBatch spriteBatch)
      {
         if (m_Grid != null)
         {
            // Render the render target texture under the grid
            spriteBatch.Begin();
            spriteBatch.Draw(m_RenderTarget, new Rectangle(0, 0, m_Viewport.Bounds.Width, m_Viewport.Bounds.Height), Color.White);
            spriteBatch.End();

            int numVertices = m_Grid.Vertices.Count;
            if (numVertices > 0)
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
                      numVertices * 4   // number of primitives to draw
                  );
               }

               // Draw each drop point
               spriteBatch.Begin();
               for (int i = 0; i < numVertices; ++i)
               {
                  spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_Grid.Vertices[i].X - 2, (int)m_Grid.Vertices[i].Y - 2, 5, 5), Color.Black);

                  if (m_HoveredVertex == i)
                     spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_Grid.Vertices[i].X - 1, (int)m_Grid.Vertices[i].Y - 1, 3, 3), Color.Red);
                  else if (m_SelectedVertex == i)
                     spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_Grid.Vertices[i].X - 1, (int)m_Grid.Vertices[i].Y - 1, 3, 3), Color.Red);
               }
               DrawRulerTickMarks(spriteBatch);
               spriteBatch.End();
            }
         }
         else
         {
            string message = "No Projector Selected";
            spriteBatch.Begin();
            spriteBatch.DrawString(m_ArialFont, message, new Vector2((m_Viewport.Width / 2.0f) - (m_ArialFont.MeasureString(message).Length() / 2.0f), m_Viewport.Height / 2.0f - 20), Color.Black);
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

      #region Public Access TV

      public void SetGrid(UVGrid grid)
      {
         m_Grid = grid;
         BuildGridLines();
      }

      public void RemoveGrid()
      {
         m_Grid = null;
      }

      public UVGrid Grid
      {
         get { return m_Grid; }
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
