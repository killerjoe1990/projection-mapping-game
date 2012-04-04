
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
   struct BuildingSaveData
   {
      public string MeshFilename;
      public int ID;
      public EntityType Type;
      public float RotX, RotY, RotZ;
      public Vector3 Position;
      public Vector3 Scale;

      public BuildingSaveData(string meshFilename, int id, EntityType type, float rotx, float roty, float rotz, Vector3 position, Vector3 scale)
      {
         MeshFilename = meshFilename;
         ID = id;
         Type = type;
         RotX = rotx;
         RotY = roty;
         RotZ = rotz;
         Position = position;
         Scale = scale;
      }
   }
}
