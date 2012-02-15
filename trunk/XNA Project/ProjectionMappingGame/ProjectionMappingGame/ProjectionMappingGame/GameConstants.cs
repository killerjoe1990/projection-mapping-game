
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
      public const int WINDOW_WIDTH = 640;
      public const int WINDOW_HEIGHT = 480;

      // FSM
      public const StateType DEFAULT_STATE = StateType.MainMenu;

   }
}
