
#region File Description

//-----------------------------------------------------------------------------
// WindowSaveData.cs
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
   public struct WindowSaveData
   {
      public int WindowIndex;
      public bool IsProjector;
      public int Width;
      public int Height;
   }
}
