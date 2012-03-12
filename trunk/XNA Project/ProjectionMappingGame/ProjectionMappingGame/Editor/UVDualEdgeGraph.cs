#region File Description

//-----------------------------------------------------------------------------
// UVDualEdgeGraph.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    3/11/2012
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

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVDualEdgeGraph
   {
      List<UVVertex> m_GraphVertices;
      List<UVEdge> m_GraphEdges;

      public UVDualEdgeGraph()
      {
         // Create an empty graph
         m_GraphEdges = new List<UVEdge>();
         m_GraphVertices = new List<UVVertex>();
      }

      #region Graph Assembly

      /// <summary>
      /// Clear the graph of all its vertices and edges.
      /// </summary>
      public void Clear()
      {
         m_GraphEdges.Clear();
         m_GraphVertices.Clear();
      }

      public void AssembleGraph(List<VertexPositionColorTexture> points)
      {
         Clear();

         for (int i = 0; i < points.Count; ++i)
         {
            UVVertex vert = new UVVertex(points[i]);
            m_GraphVertices.Add(vert);
         }

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

      const float EPSILON = 0.0001f;
      private bool IsEqual(float a, float b)
      {
         return (Math.Abs(a - b) < EPSILON);
      }

      #endregion

      #region Calculated Properties

      public Vector2 CalculateQuadCenter(UVQuad q)
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

      #endregion

      #region Public Access TV

      public List<UVEdge> Edges
      {
         get { return m_GraphEdges; }
      }

      public List<UVVertex> Vertices
      {
         get { return m_GraphVertices; }
      }

      #endregion

   }
}
