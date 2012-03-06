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

// Local imports
using ProjectionMappingGame.PrimitivesExt;

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVEdge
   {
      public int P1;
      public int P2;
      public int TwinEdge;
      public int NextEdge;
      public int PrevEdge;
      public OrthoQuad Bounds;

      public UVEdge(int p1, int p2)
      {
         P1 = p1;
         P2 = p2;
         NextEdge = -1;
         PrevEdge = -1;
         TwinEdge = -1;
      }

      public static bool IsTwin(UVEdge e0, UVEdge e1)
      {
         UVEdge e2 = new UVEdge(e0.P2, e0.P1);
         return (e1 == e2);
      }

      // Overloaded comparison operator win
      public static bool operator ==(UVEdge a, UVEdge b)
      {
         if (System.Object.ReferenceEquals(a, b))
            return true;
         if ((object)a == null || (object)b == null)
            return false;
         
         return ((a.P1 == b.P1) && (a.P2 == b.P2));
      }
      public static bool operator !=(UVEdge a, UVEdge b)
      {
         return !(a == b);
      }
   }
}
