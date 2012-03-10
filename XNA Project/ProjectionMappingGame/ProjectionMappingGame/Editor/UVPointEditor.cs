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
   class UVPointEditor
   {
      const int EDGE_BOUNDS_WIDTH = 1;

      Viewport m_Viewport;
      GameDriver m_Game;
      Matrix m_WorldMatrix;
      Matrix m_ViewMatrix;
      Matrix m_ProjectionMatrix;

      // Lines
      BasicEffect m_LineEffect;
      VertexDeclaration m_LineVertexDeclaration;

      // Quads
      BasicEffect m_QuadEffect;
      VertexDeclaration m_QuadVertexDeclaration;

      // Graph structure
      List<UVVertex> m_GraphVertices;
      List<UVEdge> m_GraphEdges;
      List<UVQuad> m_Quads;

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
      bool m_DraggingVertex;
      int m_SelectedVertex;
      int m_HoveredVertex;
      bool m_DraggingQuad;
      int m_SelectedQuad;
      int m_HoveredQuad;
      bool m_DraggingEdge;
      int m_SelectedEdge;
      int m_HoveredEdge;
      MouseInput m_MouseInput;
      EventHandler m_OnQuadSelected;

      public UVPointEditor(GameDriver game, int x, int y, int w, int h)
      {
         // Store game reference
         m_Game = game;
         m_MouseInput = new MouseInput(new Vector2(-x, -y));

         // Initialize viewport
         m_Viewport = new Viewport(x, y, w, h);

         // Create empty graph
         m_GraphEdges = new List<UVEdge>();
         m_Quads = new List<UVQuad>();
         m_GraphVertices = new List<UVVertex>();
         
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
         m_GraphEdges.Clear();
         m_GraphVertices.Clear();
         m_Quads.Clear();
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

      public void Update(float elapsedTime)
      {
         //m_Quads.Clear();

         if (m_GraphEdges.Count > 0)
         {
            for (int i = 0; i < m_Quads.Count; ++i)
            {
               m_Quads[i].Vertices[0] = m_GraphVertices[m_Quads[i].P0].Vertex;
               m_Quads[i].Vertices[1] = m_GraphVertices[m_Quads[i].P1].Vertex;
               m_Quads[i].Vertices[2] = m_GraphVertices[m_Quads[i].P2].Vertex;
               m_Quads[i].Vertices[3] = m_GraphVertices[m_Quads[i].P3].Vertex;
            }

            for (int i = 0; i < m_GraphEdges.Count; ++i)
            {
               VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
               vertices[0] = m_GraphVertices[m_GraphEdges[i].P1].Vertex;
               vertices[1] = m_GraphVertices[m_GraphEdges[i].P1].Vertex;
               vertices[2] = m_GraphVertices[m_GraphEdges[i].P2].Vertex;
               vertices[3] = m_GraphVertices[m_GraphEdges[i].P2].Vertex;

               Vector3 direction = m_GraphVertices[m_GraphEdges[i].P2].Vertex.Position - m_GraphVertices[m_GraphEdges[i].P1].Vertex.Position;
               direction.Normalize();
               Vector3 cross = Vector3.Cross(direction, new Vector3(0, 0, 1));
               Vector3 normal = new Vector3(cross.X, cross.Y, 0);

               vertices[0].Position += normal * EDGE_BOUNDS_WIDTH;
               vertices[1].Position += normal * -EDGE_BOUNDS_WIDTH;
               vertices[2].Position += normal * -EDGE_BOUNDS_WIDTH;
               vertices[3].Position += normal * EDGE_BOUNDS_WIDTH;
               m_GraphEdges[i].Bounds = new OrthoQuad(vertices);
            }
         }
      }

      #region Input Handling

      // Mouse bounds for easy selection
      const int MOUSE_SELECTION_BUFFER = 3;

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

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
            int numEdges = m_GraphEdges.Count;
            for (int i = 0; i < numEdges; ++i)
            {
               if (m_GraphEdges[i].Bounds.TestPointInsideConvexPolygon(mousePos))
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
            m_GraphVertices[m_GraphEdges[m_SelectedEdge].P1].Vertex.Position += new Vector3(dx, dy, 0.0f);
            m_GraphVertices[m_GraphEdges[m_SelectedEdge].P2].Vertex.Position += new Vector3(dx, dy, 0.0f);
         }

         // Handle quad hovering
         m_HoveredQuad = -1;
         if (!m_DraggingVertex && !m_DraggingEdge)
         {
            int numQuads = m_Quads.Count;
            for (int i = 0; i < numQuads; ++i)
            {
               if (m_Quads[i].TestPointInsideConvexPolygon(mousePos))
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
            m_GraphVertices[m_Quads[m_SelectedQuad].P0].Vertex.Position += new Vector3(dx, dy, 0.0f);
            m_GraphVertices[m_Quads[m_SelectedQuad].P1].Vertex.Position += new Vector3(dx, dy, 0.0f);
            m_GraphVertices[m_Quads[m_SelectedQuad].P2].Vertex.Position += new Vector3(dx, dy, 0.0f);
            m_GraphVertices[m_Quads[m_SelectedQuad].P3].Vertex.Position += new Vector3(dx, dy, 0.0f);
         }

         // Handle vertex hovering and uv coord displays
         m_HoveredVertex = -1;
         int numVertices = m_GraphVertices.Count;
         for (int i = 0; i < numVertices; ++i)
         {
            Rectangle vertex = new Rectangle((int)m_GraphVertices[i].Vertex.Position.X, (int)m_GraphVertices[i].Vertex.Position.Y, 1, 1);
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
            m_GraphVertices[m_SelectedVertex].Vertex.Position += new Vector3(dx, dy, 0.0f);
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

      public void Draw(SpriteBatch spriteBatch)
      {
         // Now render the quads to the screen buffer
         RenderQuads();
         RenderEdges();
         RenderVertices(spriteBatch);
         RenderQuadLayerNumbers(spriteBatch);
      }

      private void RenderQuads()
      {
         for (int i = 0; i < m_Quads.Count; ++i)
         {
            if (m_Quads[i].IsWall)
               m_QuadEffect.Texture = m_WallTexture;
            else
               m_QuadEffect.Texture = m_Quads[i].Texture;
            m_QuadEffect.Alpha = 1.0f;
            m_Quads[i].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }

         if (m_HoveredQuad > -1)
         {
            m_QuadEffect.Texture = m_WhiteTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_Quads[m_HoveredQuad].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
         if (m_SelectedQuad > -1)
         {
            m_QuadEffect.Texture = m_WhiteTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_Quads[m_SelectedQuad].Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
      }

      private void RenderEdges()
      {
         // Render edges
         Texture2D tex = m_QuadEffect.Texture;
         m_QuadEffect.Texture = m_WhiteTexture;
         m_QuadEffect.Alpha = 1.0f;
         for (int i = 0; i < m_GraphEdges.Count; ++i)
         {
            m_GraphEdges[i].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
         }
         if (m_HoveredEdge > -1)
         {
            m_QuadEffect.Texture = m_RedTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_GraphEdges[m_HoveredEdge].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
            m_QuadEffect.Alpha = 1.0f;
         }
         if (m_SelectedEdge > -1)
         {
            m_QuadEffect.Texture = m_RedTexture;
            m_QuadEffect.Alpha = 0.5f;
            m_GraphEdges[m_SelectedEdge].Bounds.Draw(m_Game.GraphicsDevice, m_QuadEffect);
            m_QuadEffect.Alpha = 1.0f;
         }
         m_QuadEffect.Texture = tex;
      }

      private void RenderVertices(SpriteBatch spriteBatch)
      {
         // Render vertices
         spriteBatch.Begin();
         for (int i = 0; i < m_GraphVertices.Count; ++i)
         {
            spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_GraphVertices[i].Vertex.Position.X - 2, (int)m_GraphVertices[i].Vertex.Position.Y - 2, 5, 5), Color.Black);

            if (i == m_SelectedVertex)
               spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_GraphVertices[i].Vertex.Position.X - 1, (int)m_GraphVertices[i].Vertex.Position.Y - 1, 3, 3), Color.Red);
            else if (i == m_HoveredVertex)
               spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_GraphVertices[i].Vertex.Position.X - 1, (int)m_GraphVertices[i].Vertex.Position.Y - 1, 3, 3), Color.Red);
         }
         spriteBatch.End();
      }

      private void RenderQuadLayerNumbers(SpriteBatch spriteBatch)
      {
         // Render quad layer #s
         spriteBatch.Begin();
         for (int i = 0; i < m_Quads.Count; ++i)
         {
            if (m_Quads[i].IsWall) continue;
            Vector2 textPos = CalculateCenter(m_Quads[i]);
            spriteBatch.DrawString(m_Arial10, (1 + m_Quads[i].InputLayer).ToString(), textPos, Color.Black);
         }
         spriteBatch.End();
      }

      #endregion

      #region Edge Graph Assembly

      private Vector2 CalculateCenter(UVQuad q)
      {
         Vector2 center = Vector2.Zero;
         Vector3[] verts = {
            m_GraphVertices[q.P0].Vertex.Position,
            m_GraphVertices[q.P1].Vertex.Position,
            m_GraphVertices[q.P2].Vertex.Position,
            m_GraphVertices[q.P3].Vertex.Position
         };

         // Find min/max x and y
         float minX = float.MaxValue;
         float maxX = float.MinValue;
         float minY = float.MaxValue;
         float maxY = float.MinValue;
         for (int i = 0; i < 4; ++i)
         {
            if (verts[i].X < minX)
               minX = verts[i].X;
            if (verts[i].X > maxX)
               maxX = verts[i].X;
            if (verts[i].Y < minY)
               minY = verts[i].Y;
            if (verts[i].Y > maxY)
               maxY = verts[i].Y;
         }
         center = new Vector2(minX + ((maxX - minX) / 2.0f), minY + ((maxY - minY) / 2.0f));
         return center;
      }

      private void CutPlane()
      {
         m_GraphEdges.Clear();
         m_Quads.Clear();

         List<float> xCoords = ComputeOrderedXCoordinates();
         List<float> yCoords = ComputeOrderedYCoordinates();
         for (int i = 0; i < xCoords.Count - 1; ++i)
         {
            for (int j = 0; j < yCoords.Count - 1; ++j)
            {
               int c1 = GetCoordinateAt(xCoords[i], yCoords[j]);
               int c2 = GetCoordinateAt(xCoords[i], yCoords[j + 1]);
               int c3 = GetCoordinateAt(xCoords[i + 1], yCoords[j + 1]);
               int c4 = GetCoordinateAt(xCoords[i + 1], yCoords[j]);

               // Create and link 4 edges in CCW fashion
               int numEdges = m_GraphEdges.Count;
               UVEdge e1 = new UVEdge(c1, c2);
               UVEdge e2 = new UVEdge(c2, c3);
               UVEdge e3 = new UVEdge(c3, c4);
               UVEdge e4 = new UVEdge(c4, c1);
               e1.NextEdge = numEdges + 1; e1.PrevEdge = numEdges + 3;
               e2.NextEdge = numEdges + 2; e2.PrevEdge = numEdges;
               e3.NextEdge = numEdges + 3; e3.PrevEdge = numEdges + 1;
               e4.NextEdge = numEdges; e4.PrevEdge = numEdges + 2;

               // Store in graph
               m_GraphEdges.Add(e1);
               m_GraphEdges.Add(e2);
               m_GraphEdges.Add(e3);
               m_GraphEdges.Add(e4);
            }
         }

         // Link twin edges
         for (int i = 0; i < m_GraphEdges.Count; ++i)
         {
            if (m_GraphEdges[i].TwinEdge >= 0) continue;

            for (int j = 0; j < m_GraphEdges.Count; ++j)
            {
               if (i == j) continue;
               if (m_GraphEdges[j].TwinEdge >= 0) continue;

               if (UVEdge.IsTwin(m_GraphEdges[i], m_GraphEdges[j]))
               {
                  m_GraphEdges[i].TwinEdge = j;
                  m_GraphEdges[j].TwinEdge = i;
               }
            }
         }

         for (int i = 0; i < m_GraphEdges.Count; i += 4)
         {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = m_GraphVertices[m_GraphEdges[i + 0].P1].Vertex;
            vertices[1] = m_GraphVertices[m_GraphEdges[i + 1].P1].Vertex;
            vertices[2] = m_GraphVertices[m_GraphEdges[i + 2].P1].Vertex;
            vertices[3] = m_GraphVertices[m_GraphEdges[i + 3].P1].Vertex;
            m_Quads.Add(new UVQuad(vertices, m_GraphEdges[i + 0].P1, m_GraphEdges[i + 1].P1, m_GraphEdges[i + 2].P1, m_GraphEdges[i + 3].P1, 0, m_QuadTexture));
         }

         
      }

      private int GetCoordinateAt(float x, float y)
      {
         for (int i = 0; i < m_GraphVertices.Count; ++i)
         {
            if (IsEqual(m_GraphVertices[i].Vertex.Position.X, x) && IsEqual(m_GraphVertices[i].Vertex.Position.Y, y))
            {
               return i;
            }
         }
         return -1;  // This should never happen
      }

      private List<float> ComputeOrderedXCoordinates()
      {
         List<float> xCoords = new List<float>();
         for (int i = 0; i < m_GraphVertices.Count; ++i)
         {
            if (!xCoords.Contains(m_GraphVertices[i].Vertex.Position.X))
            {
               xCoords.Add(m_GraphVertices[i].Vertex.Position.X);
            }
         }
         xCoords.Sort();
         return xCoords;
      }

      private List<float> ComputeOrderedYCoordinates()
      {
         List<float> yCoords = new List<float>();
         for (int i = 0; i < m_GraphVertices.Count; ++i)
         {
            if (!yCoords.Contains(m_GraphVertices[i].Vertex.Position.Y))
            {
               yCoords.Add(m_GraphVertices[i].Vertex.Position.Y);
            }
         }
         yCoords.Sort();
         return yCoords;
      }
      

      const float EPSILON = 0.0001f;
      private bool IsEqual(float a, float b)
      {
         return (Math.Abs(a - b) < EPSILON);
      }

      #endregion

      #region Public Access TV

      public void RegisterQuadSelectedEvent(EventHandler e)
      {
         m_OnQuadSelected += e;
      }

      public bool SelectedQuadIsWall
      {
         get { return m_Quads[m_SelectedQuad].IsWall; }
         set { m_Quads[m_SelectedQuad].IsWall = value; }
      }

      public Vector2[] SelectedQuadTexCoords
      {
         get
         {
            Vector2[] texCoords = new Vector2[4];
            texCoords[0] = m_GraphVertices[m_Quads[m_SelectedQuad].P0].Vertex.TextureCoordinate;
            texCoords[1] = m_GraphVertices[m_Quads[m_SelectedQuad].P1].Vertex.TextureCoordinate;
            texCoords[2] = m_GraphVertices[m_Quads[m_SelectedQuad].P2].Vertex.TextureCoordinate;
            texCoords[3] = m_GraphVertices[m_Quads[m_SelectedQuad].P3].Vertex.TextureCoordinate;
            return texCoords;
         }
      }

      public int SelectedInputLayer
      {
         get { return m_Quads[m_SelectedQuad].InputLayer; }
         set { 
            m_Quads[m_SelectedQuad].InputLayer = value; 
         }
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      public void DeleteLayer(int layer)
      {
         for (int i = 0; i < m_Quads.Count; ++i)
         {
            if (m_Quads[i].InputLayer == layer)
            {
               m_Quads[i].InputLayer = 0;
            }
            else if (m_Quads[i].InputLayer > layer)
            {
               m_Quads[i].InputLayer--;
            }
         }
      }

      public void SetUVs(Vector2[] uvs)
      {
         m_Quads[m_SelectedQuad].Vertices[0].TextureCoordinate = uvs[0];
         m_Quads[m_SelectedQuad].Vertices[1].TextureCoordinate = uvs[1];
         m_Quads[m_SelectedQuad].Vertices[2].TextureCoordinate = uvs[2];
         m_Quads[m_SelectedQuad].Vertices[3].TextureCoordinate = uvs[3];
         m_GraphVertices[m_Quads[m_SelectedQuad].P0].Vertex.TextureCoordinate = uvs[0];
         m_GraphVertices[m_Quads[m_SelectedQuad].P1].Vertex.TextureCoordinate = uvs[1];
         m_GraphVertices[m_Quads[m_SelectedQuad].P2].Vertex.TextureCoordinate = uvs[2];
         m_GraphVertices[m_Quads[m_SelectedQuad].P3].Vertex.TextureCoordinate = uvs[3];
      }

      public void SetPoints(List<VertexPositionColorTexture> points)
      {
         m_GraphVertices.Clear();
         for (int i = 0; i < points.Count; ++i)
         {
            UVVertex vert = new UVVertex(points[i]);
            m_GraphVertices.Add(vert);
         }
         CutPlane();
      }

      public void SyncRenderTargets(Texture2D[] renderTargets)
      {
         for (int i = 0; i < m_Quads.Count; ++i)
         {
            if (m_Quads[i].IsWall)
            {
               m_Quads[i].Texture = m_WallTexture;
            }
            else
            {
               m_Quads[i].Texture = renderTargets[m_Quads[i].InputLayer];
            }
         }
      }

      public MouseState PrevMouseState
      {
         set { m_PrevMouseState = value; }
      }

      public Texture2D RenderTargetTexture
      {
         get { return m_RenderTargetTexture; }
      }

      public void UnHoverQuads()
      {
         m_HoveredQuad = -1;
      }

      public bool IsQuadSelected
      {
         get { return (m_SelectedQuad >= 0); }
      }

      #endregion
   }
}
