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

// Local imports
using ProjectionMappingGame.PrimitivesExt;

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVDualEdgeGraph
   {
      List<UVVertex> m_Vertices;
      List<UVEdge> m_Edges;
      List<UVQuad> m_Quads;
      Texture2D m_QuadTexture;
      const int EDGE_BOUNDS_WIDTH = 1;

      public UVDualEdgeGraph(Texture2D quadTexture)
      {
         // Create an empty graph
         m_Edges = new List<UVEdge>();
         m_Vertices = new List<UVVertex>();
         m_Quads = new List<UVQuad>();

         m_QuadTexture = quadTexture;
      }

      #region Graph Assembly

      /// <summary>
      /// Clear the graph of all its vertices and edges.
      /// </summary>
      public void Clear()
      {
         m_Edges.Clear();
         m_Vertices.Clear();
         m_Quads.Clear();
      }

      public void AssembleGraph(List<VertexPositionColorTexture> points)
      {
         Clear();

         for (int i = 0; i < points.Count; ++i)
         {
            UVVertex vert = new UVVertex(points[i]);
            m_Vertices.Add(vert);
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
               int numEdges = m_Edges.Count;
               UVEdge e1 = new UVEdge(c1, c2);
               UVEdge e2 = new UVEdge(c2, c3);
               UVEdge e3 = new UVEdge(c3, c4);
               UVEdge e4 = new UVEdge(c4, c1);
               e1.NextEdge = numEdges + 1; e1.PrevEdge = numEdges + 3;
               e2.NextEdge = numEdges + 2; e2.PrevEdge = numEdges;
               e3.NextEdge = numEdges + 3; e3.PrevEdge = numEdges + 1;
               e4.NextEdge = numEdges; e4.PrevEdge = numEdges + 2;

               // Store in graph
               m_Edges.Add(e1);
               m_Edges.Add(e2);
               m_Edges.Add(e3);
               m_Edges.Add(e4);
            }
         }

         // Link twin edges
         for (int i = 0; i < m_Edges.Count; ++i)
         {
            if (m_Edges[i].TwinEdge >= 0) continue;

            for (int j = 0; j < m_Edges.Count; ++j)
            {
               if (i == j) continue;
               if (m_Edges[j].TwinEdge >= 0) continue;

               if (UVEdge.IsTwin(m_Edges[i], m_Edges[j]))
               {
                  m_Edges[i].TwinEdge = j;
                  m_Edges[j].TwinEdge = i;
               }
            }
         }

         // Create quads
         for (int i = 0; i < m_Edges.Count; i += 4)
         {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = m_Vertices[m_Edges[i + 0].P1].Vertex;
            vertices[1] = m_Vertices[m_Edges[i + 1].P1].Vertex;
            vertices[2] = m_Vertices[m_Edges[i + 2].P1].Vertex;
            vertices[3] = m_Vertices[m_Edges[i + 3].P1].Vertex;
            m_Quads.Add(new UVQuad(vertices, m_Edges[i + 0].P1, m_Edges[i + 1].P1, m_Edges[i + 2].P1, m_Edges[i + 3].P1, 0, m_QuadTexture));
         }
      }

      private List<float> ComputeOrderedXCoordinates()
      {
         List<float> xCoords = new List<float>();
         for (int i = 0; i < m_Vertices.Count; ++i)
         {
            if (!xCoords.Contains(m_Vertices[i].Vertex.Position.X))
            {
               xCoords.Add(m_Vertices[i].Vertex.Position.X);
            }
         }
         xCoords.Sort();
         return xCoords;
      }

      private List<float> ComputeOrderedYCoordinates()
      {
         List<float> yCoords = new List<float>();
         for (int i = 0; i < m_Vertices.Count; ++i)
         {
            if (!yCoords.Contains(m_Vertices[i].Vertex.Position.Y))
            {
               yCoords.Add(m_Vertices[i].Vertex.Position.Y);
            }
         }
         yCoords.Sort();
         return yCoords;
      }

      private int GetCoordinateAt(float x, float y)
      {
         for (int i = 0; i < m_Vertices.Count; ++i)
         {
            if (IsEqual(m_Vertices[i].Vertex.Position.X, x) && IsEqual(m_Vertices[i].Vertex.Position.Y, y))
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

      public void CalculateEdgeBounds()
      {
         int numEdges = m_Edges.Count;
         if (numEdges > 0)
         {
            for (int i = 0; i < m_Quads.Count; ++i)
            {
               m_Quads[i].Vertices[0] = m_Vertices[m_Quads[i].P0].Vertex;
               m_Quads[i].Vertices[1] = m_Vertices[m_Quads[i].P1].Vertex;
               m_Quads[i].Vertices[2] = m_Vertices[m_Quads[i].P2].Vertex;
               m_Quads[i].Vertices[3] = m_Vertices[m_Quads[i].P3].Vertex;
            }

            for (int i = 0; i < numEdges; ++i)
            {
               VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
               vertices[0] = m_Vertices[m_Edges[i].P1].Vertex;
               vertices[1] = m_Vertices[m_Edges[i].P1].Vertex;
               vertices[2] = m_Vertices[m_Edges[i].P2].Vertex;
               vertices[3] = m_Vertices[m_Edges[i].P2].Vertex;

               Vector3 direction = m_Vertices[m_Edges[i].P2].Vertex.Position - m_Vertices[m_Edges[i].P1].Vertex.Position;
               direction.Normalize();
               Vector3 cross = Vector3.Cross(direction, new Vector3(0, 0, 1));
               Vector3 normal = new Vector3(cross.X, cross.Y, 0);

               vertices[0].Position += normal * EDGE_BOUNDS_WIDTH;
               vertices[1].Position += normal * -EDGE_BOUNDS_WIDTH;
               vertices[2].Position += normal * -EDGE_BOUNDS_WIDTH;
               vertices[3].Position += normal * EDGE_BOUNDS_WIDTH;
               m_Edges[i].Bounds = new OrthoQuad(vertices);
            }
         }
      }

      public Vector2 CalculateQuadCenter(UVQuad q)
      {
         Vector2 center = Vector2.Zero;
         Vector3[] verts = {
            m_Vertices[q.P0].Vertex.Position,
            m_Vertices[q.P1].Vertex.Position,
            m_Vertices[q.P2].Vertex.Position,
            m_Vertices[q.P3].Vertex.Position
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
         get { return m_Edges; }
      }

      public List<UVVertex> Vertices
      {
         get { return m_Vertices; }
      }

      public List<UVQuad> Quads
      {
         get { return m_Quads; }
      }

      #endregion

   }
}
