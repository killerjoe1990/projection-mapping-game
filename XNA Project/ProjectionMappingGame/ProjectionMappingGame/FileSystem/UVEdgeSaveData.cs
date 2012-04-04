
#region File Description

//-----------------------------------------------------------------------------
// UVEdgeSaveData.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    4/3/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.FileSystem
{
   struct UVEdgeSaveData
   {
      public int P1;
      public int P2;
      public int TwinEdge;
      public int NextEdge;
      public int PrevEdge;
      VertexPositionColorTexture[] OrthoQuadVertices;  // 4 vertices
      
      public UVEdgeSaveData(int p1, int p2, int twinEdge, int nextEdge, int prevEdge, VertexPositionColorTexture[] vertices)
      {
         P1 = p1;
         P2 = p2;
         TwinEdge = twinEdge;
         NextEdge = nextEdge;
         PrevEdge = prevEdge;
         OrthoQuadVertices = vertices;
      }
   }
}
