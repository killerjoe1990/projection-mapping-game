#region File Description

//-----------------------------------------------------------------------------
// UVQuad.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    3/8/2012
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
   class UVQuad : OrthoQuad
   {
      public int P0, P1, P2, P3;    // Quick vertex index reference from edge graph
      public int InputLayer;        // Gameplay input layer index
      public Texture2D Texture;
      public bool IsWall;
      public bool IsScoreboard;

      public UVQuad(VertexPositionColorTexture[] vertices, int p0, int p1, int p2, int p3, int layer, Texture2D texture)
         : base(vertices)
      {
         P0 = p0;
         P1 = p1;
         P2 = p2;
         P3 = p3;
         InputLayer = layer;
         Texture = texture;
         IsWall = false;
         IsScoreboard = false;
      }
   }
}
