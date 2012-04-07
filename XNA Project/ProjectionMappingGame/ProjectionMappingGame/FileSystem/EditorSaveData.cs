
#region File Description

//-----------------------------------------------------------------------------
// EditorSaveData.cs
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
   public struct EditorSaveData
   {
      public List<BuildingSaveData> BuildingData;
      public List<ProjectorSaveData> ProjectorData;
      public List<WindowSaveData> WindowData;
      public List<LayerSaveData> LayerData;
   }
}
