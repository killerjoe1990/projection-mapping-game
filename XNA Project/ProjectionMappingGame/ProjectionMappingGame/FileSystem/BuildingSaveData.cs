
#region File Description

//-----------------------------------------------------------------------------
// BuildingSaveData.cs
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

// Local imports
using ProjectionMappingGame.Editor;

#endregion

namespace ProjectionMappingGame.FileSystem
{
   [Serializable]
   public struct BuildingSaveData
   {
      public string MeshFilename;
      public int ID;
      public EntityType Type;
      public float RotX, RotY, RotZ;
      public Vector3 Position;
      public Vector3 Scale;
   }
}
