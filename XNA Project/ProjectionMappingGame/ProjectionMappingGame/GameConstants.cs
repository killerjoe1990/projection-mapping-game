
#region File Description

/******************************************************************
 * Filename:        GameConstants.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         1/24/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Text;

// Local includes
using ProjectionMappingGame.StateMachine;

#endregion

namespace ProjectionMappingGame
{
   class GameConstants
   {
      // Debugging
      public const bool DEBUG_MODE = true;

      // Window
      public static int WindowWidth = DEFAULT_WINDOW_WIDTH;
      public static int WindowHeight = DEFAULT_WINDOW_HEIGHT;
      public const int DEFAULT_WINDOW_WIDTH = 1280;
      public const int DEFAULT_WINDOW_HEIGHT = 720;

      // FSM
      public const StateType DEFAULT_STATE = StateType.ProjectionEditor;

       // Physics
      public const float GRAVITY = 1300.0f;

       //Platforms
      public const int TILE_DIM = 30;
      public const float PLATFORM_VELOCITY = 70;
      public const int PLAT_MAX_WIDTH = 6;
      public const int PLAT_MIN_WIDTH = 2;
      public const int MAX_PATHS = 3;

       //Game
      public const int MAX_PLAYERS = 4;
      public const int PLAYER_DIM_X = 35;
      public const int PLAYER_DIM_Y = 75;
      public const float JUMP_IMPULSE = -37000;
      public const float MOVE_SPEED = 7300;
      public const float BOUNCE_IMPULSE_UP = -25000;
      public const float BOUNCE_IMPULSE_DOWN = 0;

       //animation
      public const float PLAYER_FRAMERATE = 10;

      public static Random RANDOM = new Random();

   }
}
