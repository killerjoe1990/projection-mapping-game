
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
   [Serializable]
   public struct UVEdgeSaveData
   {
      public int P1;
      public int P2;
      public int TwinEdge;
      public int NextEdge;
      public int PrevEdge;
      //VertexPositionColorTexture[] OrthoQuadVertices;  // 4 vertices
   }
}
