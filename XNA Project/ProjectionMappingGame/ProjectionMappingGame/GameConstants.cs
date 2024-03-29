﻿
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
using Microsoft.Xna.Framework.Graphics;

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

      // Config
      public const string DEFAULT_THEME_ROOT_PATH = "../../../../Themes";

      // FSM
      public const StateType DEFAULT_STATE = StateType.MainMenu;

       // Physics
      public const float GRAVITY = 1500.0f;
      public const float MAX_FALL_SPEED = 2000.0f;

      // Editor constants
      public const float DEFAULT_CUBE_SCALE = 30.0f;
      public const float PLANE_SCALE = 200.0f;
      public const float PROJECTOR_SCALE = 3.0f;
      public const float LIGHT_SCALE = 3.0f;
      public static Vector3 DEFAULT_BUILDING_POSITION = new Vector3(0.0f, DEFAULT_CUBE_SCALE / 2.0f, 0.0f);
      public static Vector3 DEFAULT_CAMERA_POSITION = new Vector3(0.0f, 0.0f, -300.0f);
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
      public const float PLATFORM_VELOCITY = 130;
      public const int PLAT_MAX_WIDTH = 6;
      public const int PLAT_MIN_WIDTH = 3;
      public const int MAX_PATHS = 3;
      public const int PLAT_MAX_Y_SPAWN_DELTA = 64;
      public const int PLAT_MIN_Y_SPAWN_DELTA = 0;
      public const int BLINKPLAT_CHANCE_TO_BLINK_ON_JUMP = 100;  // Out of 100
      public const int CHANCE_TO_SPAWN_BLINKPLAT = 25;          //Out of 100

       //Portals
      public const int PORTAL_DIM = 75;
      public const float MAX_PORTAL_Y = 0.7f;
      public const float MAX_PORTAL_X = 0.7f;
      public const float MIN_PORTAL_Y = 0.3f;
      public const float MIN_PORTAL_X = 0.3f;
      public const float PORTAL_DELAY = 1.0f;
      public const float PORT_AGAIN_DELAY = 3.0f;

       //Game
      public static Color SWAP_COLOR = new Color(142, 243, 34);

      public const int MAX_PLAYERS = 4;
      public const int PLAYER_DIM_X = 48;
      public const int PLAYER_DIM_Y = 48;
      public const float JUMP_IMPULSE = -43000;
      public const float MOVE_SPEED = 12300;
      public const float BOUNCE_IMPULSE_UP = -25000;
      public const float BOUNCE_IMPULSE_DOWN = 0;
      public const float POINTS_FOR_KILL = 0.2f;
      public const int BONUS_POINTS = 10;
      public const int POINTS_PER_INTERVAL = 1;
      public const float POINT_INTERVAL = 1;

      public const float START_SPACING = 10;
      public const float START_Y = 50;

      public const int POWERUP_DIM = 24;
      public const float POWERUP_TIME_MIN = 10;
      public const float POWERUP_TIME_MAX = 30;

      public const int MAX_LAYERS = 10;

      public const float MAX_SHADOW = 20f;

      public const float STUN_TIME = 2.0f;
      public const int NUM_COLLECTABLES = 2;

       //HUD
      public const int HUD_WIDTH = 200;
      public const int HUD_HEIGHT = 60;
      public const int HUD_ICON_DIM = 64;
      public static Color HUD_COLOR = Color.Black;
      public static Color[] GAME_COLORS = new Color[]
       {
           Color.Red,
           Color.Blue,
           Color.Green,
           Color.Yellow,
           Color.Purple,
           Color.Orange,
           Color.HotPink,
           Color.Gray,
           Color.Cyan,
       };

      public static Texture2D WHITE_TEXTURE;

       //animation
      public const int PLAYER_JUMP_FRAMES = 4;
      public const int PLAYER_RUN_FRAMES = 4;
      public const float PLAYER_RUN_FRAMERATE = 10;
      public const float PLAYER_JUMP_FRAMERATE = 20;

      public const float BACKGROUND_FRAMERATE = 15;

      public const float PORTAL_FRAMERATE = 5;
      public const int PORTAL_FRAMES = 4;

      public const float POWERUP_FRAMERATE = 5;
      public const int POWERUP_FRAMES = 4;

      public const float BACKGROUND_FADE = 2.0f;

      public const float CHANGE_THEME_MIN = 10;
      public const float CHANGE_THEME_MAX = 20;

      public const float TITLE_FADE = 2.0f;

       // random
      public static Random RANDOM = new Random();

   }
}
