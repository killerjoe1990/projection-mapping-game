
#region File Description

//-----------------------------------------------------------------------------
// LayerSaveData.cs
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
   struct LayerSaveData
   {
      public int ID;
      public int TYPE_ID;
      public string LayerName;
      public LayerType Type;
      public int Width;
      public int Height;
      public Vector3 Normal;
      public HashSet<int> LinkedLayers;

      public LayerSaveData(int id, int typeId, string name, LayerType type, int width, int height, Vector3 normal, HashSet<int> linkedLayers)
      {
         ID = id;
         TYPE_ID = typeId;
         Width = width;
         Height = height;
         Normal = normal;
         LinkedLayers = linkedLayers;
         LayerName = name;
         Type = type;
      }
   }
}
