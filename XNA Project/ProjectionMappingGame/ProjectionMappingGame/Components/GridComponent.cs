#region File Description

//-----------------------------------------------------------------------------
// GridComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    1/30/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

#endregion

namespace ProjectionMappingGame.Components
{
   public sealed class GridComponent
   {
      // Grid properties
      bool m_IsActive;
      int m_CellSpacing;
      int m_GridSize;
      int m_NumberOfLines;

      // Rendering Members
      BasicEffect m_Effect;
      GraphicsDevice m_Graphics;
      VertexPositionColor[] m_Vertices;
      Color m_LineColor;
      Color m_HighliteColor;

      /// <summary>
      /// Constructor for type GridComponent creates a 3d grid configurable by 
      /// overall size and individual division/cell size.
      /// </summary>
      /// <param name="device">XNA graphics card handle</param>
      /// <param name="cellSize">Individual cell size</param>
      /// <param name="gridSize">Entire grid extents (this is a cube)</param>
      /// <param name="active">Initially active or not</param>
      public GridComponent(GraphicsDevice device, int cellSize, int gridSize, bool active)
      {
         // Store configuration
         m_IsActive = active;
         m_GridSize = gridSize;
         m_CellSpacing = cellSize;
         m_Graphics = device;

         // Set defaults
         m_Effect = new BasicEffect(device);
         m_Effect.VertexColorEnabled = true;
         m_Effect.World = Matrix.Identity;
         m_LineColor = Color.DimGray;
         m_HighliteColor = Color.Blue;

         // Build initial grid
         ResetLines();
      }

      #region Grid Assembly

      /// <summary>
      /// Re-compute the grid lines based on the grid size and cell spacing.
      /// </summary>
      public void ResetLines()
      {
         // calculate nr of lines, +2 for the highlights, +12 for boundingbox
         m_NumberOfLines = ((m_GridSize / m_CellSpacing) * 4) + 2 + 12;

         List<VertexPositionColor> vertexList = new List<VertexPositionColor>(m_NumberOfLines);

         // Fill grid lines
         for (int i = 1; i < (m_GridSize / m_CellSpacing) + 1; i++)
         {
            vertexList.Add(new VertexPositionColor(new Vector3((i * m_CellSpacing), 0, m_GridSize), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3((i * m_CellSpacing), 0, -m_GridSize), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3((-i * m_CellSpacing), 0, m_GridSize), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3((-i * m_CellSpacing), 0, -m_GridSize), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3(m_GridSize, 0, (i * m_CellSpacing)), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3(-m_GridSize, 0, (i * m_CellSpacing)), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3(m_GridSize, 0, (-i * m_CellSpacing)), m_LineColor));
            vertexList.Add(new VertexPositionColor(new Vector3(-m_GridSize, 0, (-i * m_CellSpacing)), m_LineColor));
         }

         // Fill grid axis lines
         vertexList.Add(new VertexPositionColor(Vector3.Forward * m_GridSize, Color.Blue));
         vertexList.Add(new VertexPositionColor(Vector3.Backward * m_GridSize, Color.Blue));
         vertexList.Add(new VertexPositionColor(Vector3.Right * m_GridSize, Color.Red));
         vertexList.Add(new VertexPositionColor(Vector3.Left * m_GridSize, Color.Red));

         // Fill bounding box
         BoundingBox box = new BoundingBox(new Vector3(-m_GridSize, -m_GridSize, -m_GridSize), new Vector3(m_GridSize, m_GridSize, m_GridSize));
         Vector3[] corners = new Vector3[8];

         box.GetCorners(corners);
         vertexList.Add(new VertexPositionColor(corners[0], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[1], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[0], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[3], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[0], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[4], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[1], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[2], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[1], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[5], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[2], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[3], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[2], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[6], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[3], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[7], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[4], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[5], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[4], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[7], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[5], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[6], m_LineColor));

         vertexList.Add(new VertexPositionColor(corners[6], m_LineColor));
         vertexList.Add(new VertexPositionColor(corners[7], m_LineColor));

         // Store entire list as vertices to render
         m_Vertices = vertexList.ToArray();
      }

      #endregion

      #region Rendering

      /// <summary>
      /// Render the grid in the provided view space.
      /// </summary>
      /// <param name="viewMatrix">Viewer transformation matrix</param>
      /// <param name="projMatrix">Projection transformation matrix</param>
      public void Draw(Matrix viewMatrix, Matrix projMatrix)
      {
         m_Graphics.DepthStencilState = DepthStencilState.Default;
         m_Effect.View = viewMatrix;
         m_Effect.Projection = projMatrix;
         m_Effect.CurrentTechnique.Passes[0].Apply();
         {
            m_Graphics.DrawUserPrimitives(PrimitiveType.LineList, m_Vertices, 0, m_NumberOfLines);
         }
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor/Mutator for the cell spacing.  Changing this forces
      /// a recomputation of all grid lines...naturally.
      /// </summary>
      public int CellSpacing
      {
         get { return m_CellSpacing; }
         set
         {
            m_CellSpacing = value;
            ResetLines();
         }
      }

      /// <summary>
      /// Accessor/Mutator to turn the grid on/off.
      /// </summary>
      public bool IsActive
      {
         get { return m_IsActive; }
         set { m_IsActive = value; }
      }

      #endregion

   }
}
