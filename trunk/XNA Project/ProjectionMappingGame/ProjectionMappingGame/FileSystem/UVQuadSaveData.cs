
#region File Description

//-----------------------------------------------------------------------------
// UVQuadSaveData.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    4/3/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.FileSystem
{
   [Serializable]
   public struct UVQuadSaveData
   {
      public int P0, P1, P2, P3;
      public int InputLayer;
      public bool IsWall;
      public bool IsScoreboard;
      VertexPositionColorTexture[] OrthoQuadVertices;
   }
}
