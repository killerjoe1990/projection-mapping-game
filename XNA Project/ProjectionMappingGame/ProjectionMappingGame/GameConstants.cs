
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
using Microsoft.Xna.Framework;

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
      public const StateType DEFAULT_STATE = StateType.MainMenu;

       // Physics
      public const float GRAVITY = 1300.0f;
      public const float MAX_FALL_SPEED = 2000.0f;

      // Editor constants
      public const float DEFAULT_CUBE_SCALE = 30.0f;
      public const float PLANE_SCALE = 200.0f;
      public const float PROJECTOR_SCALE = 3.0f;
      public const float LIGHT_SCALE = 3.0f;
      public static Vector3 DEFAULT_BUILDING_POSITION = new Vector3(0.0f, DEFAULT_CUBE_SCALE / 2.0f, 0.0f);
      public static Vector3 DEFAULT_CAMERA_POSITION = new Vector3(0.0f, 0.0f, -100.0f);
      public static Vector3 DEFAULT_PROJECTOR_POSITION1 = new Vector3(0.0f, DEFAULT_CUBE_SCALE / 2.0f, (DEFAULT_CUBE_SCALE * 2));
      public static Vector3 DEFAULT_PROJECTOR_POSITION2 = new Vector3((DEFAULT_CUBE_SCALE * 2), DEFAULT_CUBE_SCALE / 2.0f, 0.0f);
      public static Vector3 DEFAULT_PROJECTOR_POSITION3 = new Vector3(0.0f, DEFAULT_CUBE_SCALE / 2.0f, -(DEFAULT_CUBE_SCALE * 2));
      public static Vector3 DEFAULT_PROJECTOR_POSITION4 = new Vector3(-(DEFAULT_CUBE_SCALE * 2), DEFAULT_CUBE_SCALE / 2.0f, 0.0f);

       // Platforms
      public const int TILE_DIM = 32;
      public const int DEFAULT_TILES_WIDE = 32;
      public const int DEFAULT_TILES_HIGH = 32;
      public const int MAX_TILES_WIDE = 100;
      public const int MAX_TILES_HIGH = 100;
      public const float PLATFORM_VELOCITY = 70;
      public const int PLAT_MAX_WIDTH = 6;
      public const int PLAT_MIN_WIDTH = 2;
      public const int MAX_PATHS = 3;

       //Portals
      public const int PORTAL_DIM = 75;
      public const float MAX_PORTAL_Y = 0.7f;
      public const float MAX_PORTAL_X = 0.7f;
      public const float MIN_PORTAL_Y = 0.3f;
      public const float MIN_PORTAL_X = 0.3f;
      public const float PORTAL_DELAY = 1.0f;
      public const float PORT_AGAIN_DELAY = 3.0f;

       //Game
      public const int MAX_PLAYERS = 4;
      public const int PLAYER_DIM_X = 48;
      public const int PLAYER_DIM_Y = 48;
      public const float JUMP_IMPULSE = -37000;
      public const float MOVE_SPEED = 7300;
      public const float BOUNCE_IMPULSE_UP = -25000;
      public const float BOUNCE_IMPULSE_DOWN = 0;
      public const int POINTS_FOR_KILL = 20;
      public const int POINTS_PER_INTERVAL = 1;
      public const float POINT_INTERVAL = 1;

      public const float START_SPACING = 10;
      public const float START_Y = 50;

      public const int MAX_LAYERS = 10;

      public const float MAX_SHADOW = 20f;

       //HUD
      public const int HUD_WIDTH = 200;
      public const int HUD_HEIGHT = 60;
      public const int HUD_ICON_DIM = 32;
      public static Color HUD_COLOR = Color.Black;
      public static Color[] GAME_COLORS = new Color[]
       {
           Color.Red,
           Color.Blue,
           Color.Green,
           Color.Yellow,
           Color.Purple,
           Color.Orange,
           Color.Pink,
           Color.Gray,
           Color.Cyan,
           Color.White
       };

       //animation
      public const int PLAYER_JUMP_FRAMES = 4;
      public const int PLAYER_RUN_FRAMES = 4;
      public const float PLAYER_RUN_FRAMERATE = 10;
      public const float PLAYER_JUMP_FRAMERATE = 20;

      public const float PORTAL_FRAMERATE = 5;
      public const int PORTAL_FRAMES = 4;

       // random
      public static Random RANDOM = new Random();

   }
}
