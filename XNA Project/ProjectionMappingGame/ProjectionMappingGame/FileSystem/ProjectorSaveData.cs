
#region File Description

//-----------------------------------------------------------------------------
// ProjectorSaveData.cs
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
   public struct ProjectorSaveData
   {
      // View components
      public Vector3 Position;
      public Vector3 LookAt;
      public Vector3 Up;
      public float RotX, RotY, RotZ;
      public Viewport Viewport;

      // Graph and Grid
      public GridSaveData GridData;
      public EdgeGraphSaveData EdgeGraphData;

      // Projection components
      public float Fov;
      public float AspectRatio;
      public float NearPlane;
      public float FarPlane;
      public bool IsOn;
      public float Alpha;
   }
}
