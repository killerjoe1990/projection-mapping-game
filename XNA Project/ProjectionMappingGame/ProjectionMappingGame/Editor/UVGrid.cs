#region File Description

//-----------------------------------------------------------------------------
// UVGrid.cs
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
   class UVGrid
   {
      // Dimensions
      float m_Width;
      float m_Height;

      // Vertices
      List<Vector2> m_Vertices;

      public UVGrid(float width, float height)
      {
         // Store dimensions
         m_Width = width;
         m_Height = height;

         // Defaults
         m_Vertices = new List<Vector2>();
         InsertPoint(new Vector2(0.0f, 0.0f));
         InsertPoint(new Vector2(m_Width, 0.0f));
         InsertPoint(new Vector2(m_Width, m_Height));
         InsertPoint(new Vector2(0.0f, m_Height));
      }

      public void Reset(float width, float height)
      {
         m_Width = width;
         m_Height = height;

         m_Vertices.Clear();
         InsertPoint(new Vector2(0.0f, 0.0f));
         InsertPoint(new Vector2(m_Width, 0.0f));
         InsertPoint(new Vector2(m_Width, m_Height));
         InsertPoint(new Vector2(0.0f, m_Height));
      }

      #region Grid Assembly

      public void RemovePoint(int removeAt)
      {
         m_Vertices.RemoveAt(removeAt);
      }

      public void InsertPoint(Vector2 point)
      {
         if (m_Vertices.Contains(point)) return;
         m_Vertices.Add(point);
      }

      public List<VertexPositionColorTexture> GetIntersectionPoints()
      {
         List<VertexPositionColorTexture> intersectionPoints = new List<VertexPositionColorTexture>();

         // Add all of the vertices first as we know they will be intersection points
         for (int i = 0; i < m_Vertices.Count; ++i)
         {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture(
               new Vector3(m_Vertices[i].X, m_Vertices[i].Y, 0.0f),
               Color.White,
               new Vector2(m_Vertices[i].X / m_Width, m_Vertices[i].Y / m_Height)
            );
            intersectionPoints.Add(vertex);
         }

         // Now for each vertex, we will have a series of intersections in x and y with every
         // other vertex's cast lines.  Calculate each one.
         for (int i = 0; i < m_Vertices.Count; ++i)
         {
            for (int j = 0; j < m_Vertices.Count; ++j)
            {
               // Skip ourselves
               if (i == j) continue;

               // Calculate intersection points
               VertexPositionColorTexture i1 = new VertexPositionColorTexture(
                  new Vector3(m_Vertices[i].X, m_Vertices[j].Y, 0.0f),
                  Color.White,
                  new Vector2(m_Vertices[i].X / m_Width, m_Vertices[j].Y / m_Height)
               );

               // Add unique points to the intersection list
               if (!intersectionPoints.Contains(i1))
                  intersectionPoints.Add(i1);
            }
         }

         return intersectionPoints;
      }

      #endregion

      #region Public Access TV

      public List<Vector2> Vertices
      {
         get { return m_Vertices; }
      }

      public float Width
      {
         get { return m_Width; }
         set { m_Width = value; }
      }

      public float Height
      {
         get { return m_Height; }
         set { m_Height = value; }
      }

      #endregion 

   }
}
