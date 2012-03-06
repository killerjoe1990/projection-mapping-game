
#region File Description

//-----------------------------------------------------------------------------
// OrthoQuad.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/1/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.PrimitivesExt
{
   class OrthoQuad
   {
      // Quad fields
      short[] m_Indices;                        // 6 indices = 2 triangles
      VertexPositionColorTexture[] m_Vertices;  // 4 vertices
      public int P0, P1, P2, P3;                // Quick vertex index reference from edge graph

      /// <summary>
      /// Main constructor for type OrthoQuad defines an orthographical 2D quad
      /// with the supplied vertices.
      /// </summary>
      /// <param name="vertices">4 corners of the quad.</param>
      public OrthoQuad(VertexPositionColorTexture[] vertices)
      {
         // Store vertices; assumed to be CCW winding order
         m_Vertices = vertices;

         // Set indices to render two triangles in CCW winding order
         m_Indices = new short[6];
         m_Indices[0] = 2;
         m_Indices[1] = 1;
         m_Indices[2] = 0;
         m_Indices[3] = 0;
         m_Indices[4] = 3;
         m_Indices[5] = 2;
      }

      /// <summary>
      /// Main constructor for type OrthoQuad defines an orthographical 2D quad
      /// with the supplied vertices.
      /// </summary>
      /// <param name="vertices">4 corners of the quad.</param>
      public OrthoQuad(VertexPositionColorTexture[] vertices, int p0, int p1, int p2, int p3)
      {
         // Store vertices; assumed to be CCW winding order
         m_Vertices = vertices;
         P0 = p0;
         P1 = p1;
         P2 = p2;
         P3 = p3;

         // Set indices to render two triangles in CCW winding order
         m_Indices = new short[6];
         m_Indices[0] = 2;
         m_Indices[1] = 1;
         m_Indices[2] = 0;
         m_Indices[3] = 0;
         m_Indices[4] = 3;
         m_Indices[5] = 2;
      }

      #region Rendering

      /// <summary>
      /// Render a given quad with the 3D renderer, but only from an orthographic perspective.
      /// </summary>
      /// <param name="graphics">Graphics device for rendering.</param>
      /// <param name="effect">Vertex/Pixel shader for rendering.</param>
      public void Draw(GraphicsDevice graphics, Effect effect)
      {
         // Render the quad once for each shader technique pass
         foreach (EffectPass pass in effect.CurrentTechnique.Passes)
         {
            pass.Apply();
            graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
               PrimitiveType.TriangleList,
               m_Vertices,
               0,
               4,
               m_Indices,
               0,
               2
            );
         }
      }

      #endregion 

      #region Sweet Math

      public bool TestPointInsideConvexPolygon(Vector2 point)
      {
         int sign = 0;
         for (int i = 0; i < m_Vertices.Length; ++i)
         {
            // Get first points in R2
            Vector2 p0 = new Vector2(m_Vertices[i].Position.X, m_Vertices[i].Position.Y);
            Vector2 p1;
            if (i == m_Vertices.Length - 1)
               p1 = new Vector2(m_Vertices[0].Position.X, m_Vertices[0].Position.Y);
            else
               p1 = new Vector2(m_Vertices[i + 1].Position.X, m_Vertices[i + 1].Position.Y);

            // Compute cross product between edge and a vector from the first edge point to the test point
            Vector2 edge = p1 - p0;
            Vector2 toPoint = point - p0;
            float cp = edge.X * toPoint.Y - edge.Y * toPoint.X;

            // Normalize and test sign
            int k = (int)(cp / Math.Abs(cp));
            if (sign == 0)          // Just assume k on the first case.
               sign = k;            // This is the sign that will determine the success of the test.
            else if (k != sign)     // If we ever change signs then we fail the test
               return false;
         }

         // We passed the test, return true
         return true;
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor for 4 corner vertices of the quad.
      /// </summary>
      public VertexPositionColorTexture[] Vertices
      {
         get { return m_Vertices; }
      }

      #endregion 

   }
}
