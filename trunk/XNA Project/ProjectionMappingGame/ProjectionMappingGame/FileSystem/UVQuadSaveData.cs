
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
   struct UVQuadSaveData
   {
      public int P0, P1, P2, P3;
      public int InputLayer;
      public bool IsWall;
      public bool IsScoreboard;
      VertexPositionColorTexture[] OrthoQuadVertices;

      public UVQuadSaveData(int p0, int p1, int p2, int p3, int inputLayer, bool wall, bool scoreboard, VertexPositionColorTexture[] vertices)
      {
         P0 = p0;
         P1 = p1;
         P2 = p2;
         P3 = p3;
         InputLayer = inputLayer;
         IsWall = wall;
         IsScoreboard = scoreboard;
         OrthoQuadVertices = vertices;
      }
   }
}
