using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

//
// Adapted from various microsoft tutorials
//

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

      public GridComponent(GraphicsDevice device, int cellSize, int gridSize, bool active)
      {
         m_IsActive = active;
         m_GridSize = gridSize;
         m_CellSpacing = cellSize;

         m_Effect = new BasicEffect(device);
         m_Effect.VertexColorEnabled = true;
         m_Effect.World = Matrix.Identity;

         m_Graphics = device;

         m_LineColor = Color.DimGray;
         m_HighliteColor = Color.Blue;

         ResetLines();
      }

      #region Grid Assembly

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

      public int CellSpacing
      {
         get { return m_CellSpacing; }
         set
         {
            m_CellSpacing = value;
            ResetLines();
         }
      }

      public bool IsActive
      {
         get { return m_IsActive; }
         set { m_IsActive = value; }
      }

      #endregion

   }
}
