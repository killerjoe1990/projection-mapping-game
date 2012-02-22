
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
      public const int WINDOW_WIDTH = 1280;
      public const int WINDOW_HEIGHT = 720;

      // FSM
      public const StateType DEFAULT_STATE = StateType.MainMenu;

       // Physics
      public const float GRAVITY = 1000.0f;

       //Platforms
      public const int TILE_DIM = 30;
      public const float PLATFORM_VELOCITY = 50;
      public const int PLAT_MAX_WIDTH = 6;
      public const int PLAT_MIN_WIDTH = 2;

       //Game
      public const int MAX_PLAYERS = 4;
      public const int PLAYER_DIM_X = 35;
      public const int PLAYER_DIM_Y = 75;
      public const float JUMP_IMPULSE = -30000;
      public const float MOVE_SPEED = 4500;

       //animation
      public const float PLAYER_FRAMERATE = 10;

      public static Random RANDOM = new Random();

   }
}
