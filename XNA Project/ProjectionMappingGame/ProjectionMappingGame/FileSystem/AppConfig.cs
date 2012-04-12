
#region File Description

//-----------------------------------------------------------------------------
// AppConfig.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    4/10/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;

#endregion

namespace ProjectionMappingGame.FileSystem
{
   [Serializable]
   public struct AppConfig
   {
      public int DefaultScreenWidth;
      public int DefaultScreenHeight;
      public string RootThemePath;
      public bool Loaded;
   }
}
