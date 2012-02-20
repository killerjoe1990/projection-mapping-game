
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
