#region File Description

//-----------------------------------------------------------------------------
// UVEditorComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    1/31/2012
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

// Local imports
using ProjectionMappingGame.GUI;
using ProjectionMappingGame.PrimitivesExt;

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVDualEdgeGraphEditor
   {
      Viewport m_RenderTargetViewport;
      Viewport m_Viewport;
      GameDriver m_Game;

      // Space matrices
      Matrix m_WorldMatrix;
      Matrix m_ViewMatrix;
      Matrix m_ProjectionMatrix;

      // Effects
      BasicEffect m_QuadEffect;
      VertexDeclaration m_QuadVertexDeclaration;
      BasicEffect m_LineEffect;
      VertexDeclaration m_LineVertexDeclaration;

      // Graph structure
      UVDualEdgeGraph m_DualEdgeGraph;
      
      // Textures
      Texture2D m_QuadTexture;
      Texture2D m_WallTexture;
      Texture2D m_WhiteTexture, m_RedTexture;
      Texture2D m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_SpinBoxFillTexture;
      Texture2D m_RenderTargetTexture;
      RenderTarget2D m_RenderTarget;

      // Fonts
      SpriteFont m_Arial10;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;
      MouseInput m_MouseInput;
      EventHandler m_OnQuadSelected;
      bool m_DraggingVertex;
      bool m_DraggingEdge;
      bool m_DraggingQuad;
      int m_SelectedVertex;
      int m_SelectedQuad;
      int m_SelectedEdge;
      int m_HoveredVertex;
      int m_HoveredQuad;
      int m_HoveredEdge;
      
      #region Initialization

      public UVDualEdgeGraphEditor(GameDriver game, int x, int y, int w, int h)
      {
         // Store game reference
         m_Game = game;
         m_MouseInput = new MouseInput(new Vector2(-x, -y));

         // Initialize viewport
         m_Viewport = new Viewport(x, y, w, h);
         m_RenderTargetViewport = m_Viewport;

         // Create empty graph
         m_DualEdgeGraph = null;
         
         // Compute space matrices
         m_ViewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 1.0f, 1000.0f);

         // Initialize lines
         m_LineVertexDeclaration = new VertexDeclaration(new VertexElement[]
               {
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
               }
         );
         m_LineEffect = new BasicEffect(m_Game.GraphicsDevice);
         m_LineEffect.VertexColorEnabled = true;
         m_WorldMatrix = Matrix.Identity;
         m_LineEffect.World = m_WorldMatrix;
         m_LineEffect.View = m_ViewMatrix;
         m_LineEffect.Projection = m_ProjectionMatrix;

         // Initialize quads
         m_QuadVertexDeclaration = new VertexDeclaration(new VertexElement[]
               {
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                  new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
               }
         );
         m_QuadEffect = new BasicEffect(m_Game.GraphicsDevice);
         m_QuadEffect.VertexColorEnabled = true;
         m_WorldMatrix = Matrix.Identity;
         m_QuadEffect.World = m_WorldMatrix;
         m_QuadEffect.View = m_ViewMatrix;
         m_QuadEffect.Projection = m_ProjectionMatrix;
         m_QuadEffect.TextureEnabled = true;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
         m_DraggingVertex = false;
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;
         m_DraggingQuad = false;
         m_SelectedQuad = -1;
         m_HoveredQuad = -1;
         m_DraggingEdge = false;
         m_SelectedEdge = -1;
         m_HoveredEdge = -1;

         // Initialize render target
         m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, w, h, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
      }

      public void Reset()
      {
         m_DualEdgeGraph = null;
         //if (m_DualEdgeGraph != null)
         //   m_DualEdgeGraph.Clear();
         m_DraggingVertex = false;
         m_SelectedVertex = -1;
         m_HoveredVertex = -1;
         m_DraggingQuad = false;
         m_SelectedQuad = -1;
         m_HoveredQuad = -1;
         m_DraggingEdge = false;
         m_SelectedEdge = -1;
         m_HoveredEdge = -1;
      }

      public void LoadContent(ContentManager content)
      {
         m_Arial10 = content.Load<SpriteFont>("Fonts/Arial10");
         m_QuadTexture = content.Load<Texture2D>("Textures/Layer0_2");
         m_WallTexture = content.Load<Texture2D>("Textures/wall");
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");
         m_RedTexture = content.Load<Texture2D>("Textures/red");
         m_SpinBoxFillTexture = content.Load<Texture2D>("Textures/GUI/spinbox_fill");
         m_SpinBoxUpTexture = content.Load<Texture2D>("Textures/GUI/spinbox_up");
         m_SpinBoxDownTexture = content.Load<Texture2D>("Textures/GUI/spinbox_down");
      }

      #endregion

      #region Updating

      public void Update(float elapsedTime)
      {
         if (m_DualEdgeGraph != null)
         {
            m_DualEdgeGraph.CalculateEdgeBounds();
         }
      }

      #endregion

      #region Input Handling

      // Mouse bounds for easy selection
      const int MOUSE_SELECTION_BUFFER = 3;

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         if (m_DualEdgeGraph != null)
         {
            m_MouseInput.HandleInput(PlayerIndex.One);

            // Convert mouse position into relative viewport space
            Vector2 mousePos = new Vector2(mouseState.X - m_Viewport.X, mouseState.Y - m_Viewport.Y);
            Rectangle mouseBounds = new Rectangle((int)(mousePos.X - MOUSE_SELECTION_BUFFER), (int)(mousePos.Y - MOUSE_SELECTION_BUFFER), MOUSE_SELECTION_BUFFER * 2, MOUSE_SELECTION_BUFFER * 2);
            int dx = mouseState.X - m_PrevMouseState.X;
            int dy = mouseState.Y - m_PrevMouseState.Y;

            // Can't possibly be dragging
            if (mouseState.LeftButton == ButtonState.Released)
            {
               m_DraggingVertex = false;
               m_DraggingQuad = false;
               m_DraggingEdge = false;
               //m_SelectedVertex = -1;        // For now, I'm also turning off the selection...may not want to do this later
            }
            if (mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               m_SelectedVertex = -1;
               m_SelectedQuad = -1;
               m_SelectedEdge = -1;
            }

            // Handle edge hovering
            m_HoveredEdge = -1;
            if (!m_DraggingVertex && !m_DraggingQuad)
            {
               int numEdges = m_DualEdgeGraph.Edges.Count;
               for (int i = 0; i < numEdges; ++i)
               {
                  if (m_DualEdgeGraph.Edges[i].Bounds.TestPointInsideConvexPolygon(mousePos))
                  {
                     m_HoveredEdge = i;
                     break;
                  }
               }
            }

            // Handle edge selection; we know the mouse is over an item if it is hovered.
            if (m_HoveredEdge >= 0 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               // Select the hovered edge
               m_SelectedEdge = m_HoveredEdge;
               m_HoveredEdge = -1;
               m_DraggingEdge = true;
               m_DraggingQuad = false;
               m_SelectedQuad = -1;
               m_HoveredQuad = -1;
               m_DraggingVertex = false;
               m_SelectedVertex = -1;
               m_HoveredVertex = -1;
            }

            // Handle edge dragging
            if (m_DraggingEdge && m_SelectedEdge >= 0)
            {
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[m_SelectedEdge].P1].Vertex.Position += new Vector3(dx, dy, 0.0f);
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[m_SelectedEdge].P2].Vertex.Position += new Vector3(dx, dy, 0.0f);
            }

            // Handle quad hovering
            m_HoveredQuad = -1;
            if (!m_DraggingVertex && !m_DraggingEdge)
            {
               int numQuads = m_DualEdgeGraph.Quads.Count;
               for (int i = 0; i < numQuads; ++i)
               {
                  if (m_DualEdgeGraph.Quads[i].TestPointInsideConvexPolygon(mousePos))
                  {
                     m_HoveredQuad = i;
                     break;
                  }
               }
            }

            // Handle quad selection; we know the mouse is over an item if it is hovered.
            if (m_HoveredQuad >= 0 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               // Select the hovered quad
               m_SelectedQuad = m_HoveredQuad;
               m_HoveredQuad = -1;
               m_DraggingQuad = true;
               m_DraggingVertex = false;
               m_SelectedVertex = -1;
               m_HoveredVertex = -1;
               m_DraggingEdge = false;
               m_SelectedEdge = -1;
               m_HoveredEdge = -1;
               if (m_OnQuadSelected != null) m_OnQuadSelected(this, new EventArgs());
            }

            // Handle quad dragging
            if (m_DraggingQuad && m_SelectedQuad >= 0)
            {
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P0].Vertex.Position += new Vector3(dx, dy, 0.0f);
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P1].Vertex.Position += new Vector3(dx, dy, 0.0f);
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P2].Vertex.Position += new Vector3(dx, dy, 0.0f);
               m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P3].Vertex.Position += new Vector3(dx, dy, 0.0f);
            }

            // Handle vertex hovering and uv coord displays
            m_HoveredVertex = -1;
            int numVertices = m_DualEdgeGraph.Vertices.Count;
            for (int i = 0; i < numVertices; ++i)
            {
               Rectangle vertex = new Rectangle((int)m_DualEdgeGraph.Vertices[i].Vertex.Position.X, (int)m_DualEdgeGraph.Vertices[i].Vertex.Position.Y, 1, 1);
               if (mouseBounds.Intersects(vertex))
               {
                  m_HoveredVertex = i;
                  break;
               }
            }

            // Handle vertex selection; we know the mouse is over an item if it is hovered.
            if (m_HoveredVertex >= 0 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               // Select the hovered vertex
               m_SelectedVertex = m_HoveredVertex;
               m_HoveredVertex = -1;
               m_DraggingVertex = true;
               m_DraggingQuad = false;
               m_SelectedQuad = -1;
               m_HoveredQuad = -1;
               m_DraggingEdge = false;
               m_SelectedEdge = -1;
               m_HoveredEdge = -1;
            }

            // Handle vertex dragging
            if (m_DraggingVertex && m_SelectedVertex >= 0)
            {
               m_DualEdgeGraph.Vertices[m_SelectedVertex].Vertex.Position += new Vector3(dx, dy, 0.0f);
            }
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      #endregion

      #region Rendering

      public void DrawRenderTarget(SpriteBatch spriteBatch, bool renderOverlay)
      {
         Color clear = new Color(0.0f, 0.0f, 0.0f, 0.0f);

         // Render the quads into the render target
         if (m_DualEdgeGraph != null)
         {
            m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
            m_Game.GraphicsDevice.Clear(clear);
         
            RenderQuads();
            if (renderOverlay)
            {
               RenderEdges();
               RenderVertices(spriteBatch);
            }

            // Extract and store the contents of the render target in a texture
            m_Game.GraphicsDevice.SetRenderTarget(null);
            m_Game.GraphicsDevice.Clear(Color.Black);
            m_Game.GraphicsDevice.Viewport = m_Viewport;
            m_RenderTargetTexture = (Texture2D)m_RenderTarget;
         }
      }

      public void Draw(SpriteBatch spriteBatch)
      {
         if (m_DualEdgeGraph != null)
         {
            // Now render the quads to the screen buffer
            RenderQuads();
            RenderEdges();
            RenderVertices(spriteBatch);
            RenderQuadLayerNumbers(spriteBatch);
            DrawRulerTickMarks(spriteBatch);
         }
         else
         {
            string message = "No Projector Selected";
            spriteBatch.Begin();
            spriteBatch.DrawString(m_Arial10, message, new Vector2((m_Viewport.Width / 2.0f) - (m_Arial10.MeasureString(message).Length() / 2.0f), m_Viewport.Height / 2.0f - 20), Color.Black);
            spriteBatch.End();
         }
      }

      const int SMALL_TICK_WIDTH = 3;
      const int MEDIUM_TICK_WIDTH = 5;
      const int LARGE_TICK_WIDTH = 7;

      private void DrawRulerTickMarks(SpriteBatch spriteBatch)
      {
         // Left side
         spriteBatch.Begin();
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(m_Viewport.Height / 16.0f), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(m_Viewport.Height / 8.0f), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(3.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(m_Viewport.Height / 4.0f), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(5.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(3.0f * (m_Viewport.Height / 8.0f)), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(7.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(m_Viewport.Height / 2.0f), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(9.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(5.0f * (m_Viewport.Height / 8.0f)), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(11.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(3.0f * (m_Viewport.Height / 4.0f)), LARGE_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(13.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(7.0f * (m_Viewport.Height / 8.0f)), MEDIUM_TICK_WIDTH, 1), Color.Black);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, (int)(15.0f * (m_Viewport.Height / 16.0f)), SMALL_TICK_WIDTH, 1), Color.Black);

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
         spriteBatch.End();
      }

      private void RenderQuads()
      {
         for (int i = 0; i < m_DualEdgeGraph.Quads.Count; ++i)
         {
            if (m_DualEdgeGraph.Quads[i].IsWall)
               m_QuadEffect.Texture = m_WallTexture;
            else
               m_QuadEffect.Texture = m_DualEdgeGraph.Quads[i].Texture;
            m_QuadEffect.Alpha = 1.0f;
            m_DualEdgeGraph.Quads[i].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }

         if (m_HoveredQuad > -1)
         {
            m_QuadEffect.Texture = m_WhiteTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_DualEdgeGraph.Quads[m_HoveredQuad].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
         if (m_SelectedQuad > -1)
         {
            m_QuadEffect.Texture = m_WhiteTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_DualEdgeGraph.Quads[m_SelectedQuad].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
      }

      private void RenderEdges()
      {
         // Render edges
         Texture2D tex = m_QuadEffect.Texture;
         m_QuadEffect.Texture = m_WhiteTexture;
         m_QuadEffect.Alpha = 1.0f;
         for (int i = 0; i < m_DualEdgeGraph.Edges.Count; ++i)
         {
            m_DualEdgeGraph.Edges[i].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
         if (m_HoveredEdge > -1)
         {
            m_QuadEffect.Texture = m_RedTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_DualEdgeGraph.Edges[m_HoveredEdge].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
            m_QuadEffect.Alpha = 1.0f;
         }
         if (m_SelectedEdge > -1)
         {
            m_QuadEffect.Texture = m_RedTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_DualEdgeGraph.Edges[m_SelectedEdge].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
            m_QuadEffect.Alpha = 1.0f;
         }
         m_QuadEffect.Texture = tex;
      }

      private void RenderVertices(SpriteBatch spriteBatch)
      {
         // Render vertices
         spriteBatch.Begin();
         for (int i = 0; i < m_DualEdgeGraph.Vertices.Count; ++i)
         {
            spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DualEdgeGraph.Vertices[i].Vertex.Position.X - 2, (int)m_DualEdgeGraph.Vertices[i].Vertex.Position.Y - 2, 5, 5), Color.Black);

            if (i == m_SelectedVertex)
               spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DualEdgeGraph.Vertices[i].Vertex.Position.X - 1, (int)m_DualEdgeGraph.Vertices[i].Vertex.Position.Y - 1, 3, 3), Color.Red);
            else if (i == m_HoveredVertex)
               spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_DualEdgeGraph.Vertices[i].Vertex.Position.X - 1, (int)m_DualEdgeGraph.Vertices[i].Vertex.Position.Y - 1, 3, 3), Color.Red);
         }
         spriteBatch.End();
      }

      private void RenderQuadLayerNumbers(SpriteBatch spriteBatch)
      {
         // Render quad layer #s
         spriteBatch.Begin();
         for (int i = 0; i < m_DualEdgeGraph.Quads.Count; ++i)
         {
            if (m_DualEdgeGraph.Quads[i].IsWall) continue;
            Vector2 textPos = m_DualEdgeGraph.CalculateQuadCenter(m_DualEdgeGraph.Quads[i]);
            spriteBatch.DrawString(m_Arial10, (1 + m_DualEdgeGraph.Quads[i].InputLayer).ToString(), textPos, Color.Black);
         }
         spriteBatch.End();
      }

      #endregion

      #region Public Interaction

      public void SetRenderTargetViewport(Viewport viewport)
      {
         m_RenderTargetViewport = viewport;
         float aspectRatio = (float)viewport.Height / (float)viewport.Width;
         m_RenderTarget.Dispose();
         m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, viewport.Width, viewport.Height, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
         m_Viewport.Height = (int)(m_Viewport.Width * aspectRatio);

         m_ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, m_Viewport.Width, m_Viewport.Height, 0, 1.0f, 1000.0f);
         m_QuadEffect.Projection = m_ProjectionMatrix;
         m_LineEffect.Projection = m_ProjectionMatrix;
      }

      public void DeleteLayer(int layer)
      {
         for (int i = 0; i < m_DualEdgeGraph.Quads.Count; ++i)
         {
            if (m_DualEdgeGraph.Quads[i].InputLayer == layer)
            {
               m_DualEdgeGraph.Quads[i].InputLayer = 0;
            }
            else if (m_DualEdgeGraph.Quads[i].InputLayer > layer)
            {
               m_DualEdgeGraph.Quads[i].InputLayer--;
            }
         }
      }

      public void SetUVs(Vector2[] uvs)
      {
         m_DualEdgeGraph.Quads[m_SelectedQuad].Vertices[0].TextureCoordinate = uvs[0];
         m_DualEdgeGraph.Quads[m_SelectedQuad].Vertices[1].TextureCoordinate = uvs[1];
         m_DualEdgeGraph.Quads[m_SelectedQuad].Vertices[2].TextureCoordinate = uvs[2];
         m_DualEdgeGraph.Quads[m_SelectedQuad].Vertices[3].TextureCoordinate = uvs[3];
         m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P0].Vertex.TextureCoordinate = uvs[0];
         m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P1].Vertex.TextureCoordinate = uvs[1];
         m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P2].Vertex.TextureCoordinate = uvs[2];
         m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P3].Vertex.TextureCoordinate = uvs[3];
      }

      public void SetEdgeGraph(UVDualEdgeGraph graph)
      {
         m_DualEdgeGraph = graph;

         // Build a set of quads from the edge graph data
         /*m_DualEdgeGraph.Quads.Clear();
         for (int i = 0; i < m_DualEdgeGraph.Edges.Count; i += 4)
         {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[i + 0].P1].Vertex;
            vertices[1] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[i + 1].P1].Vertex;
            vertices[2] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[i + 2].P1].Vertex;
            vertices[3] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Edges[i + 3].P1].Vertex;
            m_DualEdgeGraph.Quads.Add(new UVQuad(vertices, m_DualEdgeGraph.Edges[i + 0].P1, m_DualEdgeGraph.Edges[i + 1].P1, m_DualEdgeGraph.Edges[i + 2].P1, m_DualEdgeGraph.Edges[i + 3].P1, 0, m_QuadTexture));
         }*/
      }

      public void DumpEdgeGraph()
      {
         m_DualEdgeGraph = null;
      }

      public void SyncRenderTargets(Texture2D[] renderTargets)
      {
         if (m_DualEdgeGraph != null)
         {
            for (int i = 0; i < m_DualEdgeGraph.Quads.Count; ++i)
            {
               if (m_DualEdgeGraph.Quads[i].IsWall)
               {
                  m_DualEdgeGraph.Quads[i].Texture = m_WallTexture;
               }
               else
               {
                  m_DualEdgeGraph.Quads[i].Texture = renderTargets[m_DualEdgeGraph.Quads[i].InputLayer];
               }
            }
         }
      }

      public void UnHoverQuads()
      {
         m_HoveredQuad = -1;
      }

      public void DeselectQuads()
      {
         m_SelectedQuad = -1;
         m_HoveredQuad = -1;
      }

      public void RegisterQuadSelectedEvent(EventHandler e)
      {
         m_OnQuadSelected += e;
      }

      #endregion

      #region Public Access TV

      public UVDualEdgeGraph EdgeGraph
      {
         get { return m_DualEdgeGraph; }
      }

      public bool SelectedQuadIsWall
      {
         get { return m_DualEdgeGraph.Quads[m_SelectedQuad].IsWall; }
         set { m_DualEdgeGraph.Quads[m_SelectedQuad].IsWall = value; }
      }

      public bool SelectedQuadIsScoreboard
      {
         get { return m_DualEdgeGraph.Quads[m_SelectedQuad].IsScoreboard; }
         set { m_DualEdgeGraph.Quads[m_SelectedQuad].IsScoreboard = value; }
      }

      public Vector2[] SelectedQuadTexCoords
      {
         get
         {
            Vector2[] texCoords = new Vector2[4];
            texCoords[0] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P0].Vertex.TextureCoordinate;
            texCoords[1] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P1].Vertex.TextureCoordinate;
            texCoords[2] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P2].Vertex.TextureCoordinate;
            texCoords[3] = m_DualEdgeGraph.Vertices[m_DualEdgeGraph.Quads[m_SelectedQuad].P3].Vertex.TextureCoordinate;
            return texCoords;
         }
      }

      public int SelectedInputLayer
      {
         get { return m_DualEdgeGraph.Quads[m_SelectedQuad].InputLayer; }
         set { m_DualEdgeGraph.Quads[m_SelectedQuad].InputLayer = value; }
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      public MouseState PrevMouseState
      {
         set { m_PrevMouseState = value; }
      }

      public Texture2D RenderTargetTexture
      {
         get { return m_RenderTargetTexture; }
      }

      public bool IsQuadSelected
      {
         get { return (m_SelectedQuad >= 0); }
      }

      #endregion

   }
}
