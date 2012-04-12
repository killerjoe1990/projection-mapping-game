
#region File Description

//-----------------------------------------------------------------------------
// ThemeConfig.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    4/10/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;

#endregion

namespace ProjectionMappingGame.FileSystem
{
   public struct ThemeConfig
   {
      // Constants for the expected directories in a given theme folder that contain
      // the background, spritesheet and platform textures respectively.
      public const string BACKGROUND_DIRECTORY = "Background";
      public const string SPRITESHEET_DIRECTORY = "SpriteSheets";
      public const string PLATFORM_DIRECTORY = "Platforms";

      // Constants for the accepted file type for background, spritesheet and platform textures
      // respectively.  Example type are standard image formats ie. png, tga, jpg, etc...
      // Anything that XNA accepts essentially.
      public const string BACKGROUND_FILE_EXT = "*.png";
      public const string SPRITESHEET_FILE_EXT = "*.png";
      public const string PLATFORM_FILE_EXT = "*.png";

      // Theme configuration
      public string Name;                    // Theme name; also directory it is loaded from
      public List<string> Backgrounds;       // Background texture file names to load into runtime
      public List<string> SpriteSheets;      // Spritesheet texture file names to load into runtime
      public List<List<string>> Platforms;   // Platform texture file names to load into runtime
   }
}
