
#region File Description

//-----------------------------------------------------------------------------
// EdgeGraphSaveData.cs
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

#endregion

namespace ProjectionMappingGame.FileSystem
{
   [Serializable]
   public struct EdgeGraphSaveData
   {
      public List<UVVertexSaveData> VertexData;
      public List<UVEdgeSaveData> EdgeData;
      public List<UVQuadSaveData> QuadData;
   }
}
