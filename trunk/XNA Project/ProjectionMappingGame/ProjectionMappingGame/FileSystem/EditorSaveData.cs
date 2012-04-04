
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
   struct EditorSaveData
   {
      public List<BuildingSaveData> BuildingData;
      public List<ProjectorSaveData> ProjectorData;
      public List<WindowSaveData> WindowData;
      public List<LayerSaveData> LayerData;

      public EditorSaveData(List<BuildingSaveData> buildingData, List<ProjectorSaveData> projectorData, List<WindowSaveData> windowData, List<LayerSaveData> layerData)
      {
         BuildingData = buildingData;
         ProjectorData = projectorData;
         WindowData = windowData;
         LayerData = layerData;
      }
   }
}
