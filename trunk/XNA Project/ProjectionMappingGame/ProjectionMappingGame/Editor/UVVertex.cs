#region File Description

//-----------------------------------------------------------------------------
// UVEdge.cs
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

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVVertex
   {
      public VertexPositionColorTexture Vertex;
      public List<int> ConnectedEdges;

      public UVVertex(VertexPositionColorTexture v)
      {
         Vertex = v;
         ConnectedEdges = new List<int>();
      }

      // Overloaded comparison operator win
      public static bool operator ==(UVVertex a, UVVertex b)
      {
         if (System.Object.ReferenceEquals(a, b))
            return true;
         if ((object)a == null || (object)b == null)
            return false;

         return (a.Vertex == b.Vertex);
      }
      public static bool operator !=(UVVertex a, UVVertex b)
      {
         return !(a == b);
      }
   }
}
