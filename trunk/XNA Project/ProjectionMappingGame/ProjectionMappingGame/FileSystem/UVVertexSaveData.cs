
#region File Description

//-----------------------------------------------------------------------------
// UVVertexSaveData.cs
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
   struct UVVertexSaveData
   {
      public VertexPositionColorTexture Vertex;
      public List<int> ConnectedEdges;

      public UVVertexSaveData(VertexPositionColorTexture vertex, List<int> connectedEdges)
      {
         Vertex = vertex;
         ConnectedEdges = connectedEdges;
      }
   }
}
