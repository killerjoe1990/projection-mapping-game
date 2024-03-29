﻿
#region File Description

/******************************************************************
 * Filename:        GamePlayState.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         1/24/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Collections.Generic;
using System.Text;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace ProjectionMappingGame.StateMachine
{
    public class ThemeTextures
    {
        public string Name
        {
            get;
            set;
        }

        public Texture2D[] Background
        {
            get;
            set;
        }

        public float BackgroundRate
        {
            get;
            set;
        }

        public Texture2D[] MovingSprites
        {
           get;
           set;
        }

        public int NumMovingSprites
        {
            get;
            set;
        }

        public Vector2[] MovingSpriteInfo
        {
            get;
            set;
        }

        public Point MovingSpriteSize
        {
            get;
            set;
        }

        public Vector2 MovingSpriteSpeed
        {
            get;
            set;
        }

        public Texture2D[] StaticSprites
        {
            get;
            set;
        }

        public Vector2[] StaticSpriteInfo
        {
            get;
            set;
        }

        public Point StaticSpriteSize
        {
            get;
            set;
        }

        public Vector2 StaticSpriteTime
        {
            get;
            set;
        }

        public Texture2D[][] Platforms
        {
            get;
            set;
        }
    }

   public class GamePlayState : GameState
   {
      
      // Fonts
      SpriteFont m_ArialFont;
      SpriteFont m_ArialFontLarge;

      Texture2D m_PlayerIdleTex;
      Texture2D m_PlayerRunTex;
      Texture2D m_PlayerJumpTex;
      Texture2D m_PlayerStunTex;

      Game.ColorPicker m_ColorPicker;



      Texture2D m_PortalTex;
      Texture2D m_HUDTex;
      Texture2D m_TitlePage;
      Texture2D m_BestScoreTex;

      Game.Player[] m_Players;

      GUI.KeyboardInput m_Keyboard;
      GUI.GamepadInput m_Gamepad;

      List<Game.Level> m_Levels;

      Game.Collectable[] m_Collectables;

      Game.ScoreBoard m_ScoreBoard;

      int m_PortalColorIndex;
      float m_PowerUpTimer;

      float m_ThemeTimer;

      public GamePlayState(GameDriver game)
         : base(game, StateType.GamePlay)
      {
        #if WINDOWS
          m_Keyboard = new GUI.KeyboardInput();
          m_Keyboard.RegisterKeyPress(OnKeyRelease);
        #endif
          m_Gamepad = new GUI.GamepadInput();
          //m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP1, PlayerIndex.One);
          //m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP2, PlayerIndex.Two);
          //m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP3, PlayerIndex.Three);
          //m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP4, PlayerIndex.Four);

          m_Levels = new List<Game.Level>();
          m_Players = new Game.Player[GameConstants.MAX_PLAYERS];
          m_Collectables = new Game.Collectable[GameConstants.NUM_COLLECTABLES];

          m_ColorPicker = new Game.ColorPicker(GameConstants.GAME_COLORS);

          m_ScoreBoard = new Game.ScoreBoard(game, 0, 0, 768, 1024);
      }



      public override void Reset()
      {
         m_Levels.Clear();

         m_ColorPicker.Reset();

         //AddLevel(GameConstants.WindowWidth, GameConstants.WindowHeight);

         m_Players[(int)PlayerIndex.One] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.One);

         m_Players[(int)PlayerIndex.Two] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Two);

         m_Players[(int)PlayerIndex.Three] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Three);

         m_Players[(int)PlayerIndex.Four] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Four);

         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, GameConstants.PLAYER_RUN_FRAMES, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, GameConstants.PLAYER_JUMP_FRAMES, GameConstants.PLAYER_JUMP_FRAMERATE, false));
         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.STUNNED, new Game.Animation(m_PlayerStunTex, 3, 5, true));

         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, GameConstants.PLAYER_RUN_FRAMES, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, GameConstants.PLAYER_JUMP_FRAMES, GameConstants.PLAYER_JUMP_FRAMERATE, false));
         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.STUNNED, new Game.Animation(m_PlayerStunTex, 3, 5, true));

         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, GameConstants.PLAYER_RUN_FRAMES, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, GameConstants.PLAYER_JUMP_FRAMES, GameConstants.PLAYER_JUMP_FRAMERATE, false));
         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.STUNNED, new Game.Animation(m_PlayerStunTex, 3, 5, true));

         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, GameConstants.PLAYER_RUN_FRAMES, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, GameConstants.PLAYER_JUMP_FRAMES, GameConstants.PLAYER_JUMP_FRAMERATE, false));
         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.STUNNED, new Game.Animation(m_PlayerStunTex, 3, 5, true));

         m_Players[(int)PlayerIndex.One].LoadHudContent(m_ArialFontLarge, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Two].LoadHudContent(m_ArialFontLarge, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Three].LoadHudContent(m_ArialFontLarge, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Four].LoadHudContent(m_ArialFontLarge, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);

         m_ScoreBoard.LoadContent(m_HUDTex,m_ArialFontLarge);

         m_PortalColorIndex = 0;

         m_ThemeTimer = (float)GameConstants.RANDOM.NextDouble() * (GameConstants.CHANGE_THEME_MAX - GameConstants.CHANGE_THEME_MIN) + GameConstants.CHANGE_THEME_MIN;

         m_PowerUpTimer = (float)GameConstants.RANDOM.NextDouble() * (GameConstants.POWERUP_TIME_MAX - GameConstants.POWERUP_TIME_MIN) + GameConstants.POWERUP_TIME_MIN;
      }

      public override void Resize(int dx, int dy)
      {

      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
         m_ArialFontLarge = content.Load<SpriteFont>("Fonts/Arial48");
          // Load Sprites
         m_PlayerIdleTex = content.Load<Texture2D>("Sprites/Idle Complete");
         m_PlayerRunTex = content.Load<Texture2D>("Sprites/Move Complete");
         m_PlayerJumpTex = content.Load<Texture2D>("Sprites/Jump Complete");
         m_PlayerStunTex = content.Load<Texture2D>("Sprites/Stunned");
         m_PortalTex = content.Load<Texture2D>("Sprites/Portal");

         m_HUDTex = content.Load<Texture2D>("Textures/scoreBackground");

         m_BestScoreTex = content.Load<Texture2D>("Textures/bestScore");

         m_TitlePage = content.Load<Texture2D>("Textures/TitleScreen");

         GameConstants.WHITE_TEXTURE = content.Load<Texture2D>("Textures/white");

         m_Collectables[0] = new Game.InvinciblePowerup(new Rectangle(0, 0, GameConstants.POWERUP_DIM, GameConstants.POWERUP_DIM), Vector2.Zero, null);
         m_Collectables[0].SetAnimation(content.Load<Texture2D>("Sprites/Powerups/invincible"), GameConstants.POWERUP_FRAMERATE, GameConstants.POWERUP_FRAMES);

         m_Collectables[1] = new Game.SpeedBoost(new Rectangle(0, 0, GameConstants.POWERUP_DIM, GameConstants.POWERUP_DIM), Vector2.Zero, null);
         m_Collectables[1].SetAnimation(content.Load<Texture2D>("Sprites/Powerups/speedy"), GameConstants.POWERUP_FRAMERATE, GameConstants.POWERUP_FRAMES);

      }

      public override void Update(float elapsedTime)
      {
          m_PowerUpTimer -= elapsedTime;
          m_ThemeTimer -= elapsedTime;

          bool showTitle = true;

          foreach (Game.Player p in m_Players)
          {
              if (p.State != Game.Player.States.DEAD)
              {
                  showTitle = false;
              }
          }

          if (m_ThemeTimer <= 0)
          {
              ChangeThemes();
              m_ThemeTimer = (float)GameConstants.RANDOM.NextDouble() * (GameConstants.CHANGE_THEME_MAX - GameConstants.CHANGE_THEME_MIN) + GameConstants.CHANGE_THEME_MIN;
          }

          foreach(Game.Collectable collect in m_Collectables)
          {
              if (!collect.Active)
              {
                  collect.Update(elapsedTime);
              }

              if (collect.SpawnReady)
              {
                  int lvl = GameConstants.RANDOM.Next(m_Levels.Count);
                  collect.Activate(m_Levels[lvl]);
              }
          }

          foreach (Game.Level l in m_Levels)
          {
              l.ShowTitle(showTitle);
              l.Update(elapsedTime);
          }

          m_ScoreBoard.Update(elapsedTime, m_Players);
      }

      public override void HandleInput(float elapsedTime)
      {
          m_Keyboard.HandleInput(PlayerIndex.One);
          m_Keyboard.HandleInput(PlayerIndex.Two);
          m_Keyboard.HandleInput(PlayerIndex.Three);
          m_Keyboard.HandleInput(PlayerIndex.Four);

          foreach (Game.Level l in m_Levels)
          {
              l.HandleInput();
          }
      }

      public void PlayerDied(Game.Player player)
      {
          int bonus = GameConstants.BONUS_POINTS;

          foreach (Game.Player p in m_Players)
          {
              if (p != player && p.HUD.PlayerScore > player.HUD.PlayerScore)
              {
                  bonus = 0;
              }
          }

          foreach (Game.Player p in m_Players)
          {
              if (p != player && p.State == Game.Player.States.PLAYING)
              {
                  p.HUD.PlayerDefeated((int)(player.HUD.PlayerScore * GameConstants.POINTS_FOR_KILL) + bonus);
              }
          }
      }

      public void SetMainLevel(int lvl)
      {
          if (lvl >= 0 && lvl < m_Levels.Count)
          {
              foreach (Game.Player p in m_Players)
              {
                  p.MovePlayer(m_Levels[lvl]);
                  m_Levels[lvl].AddPlayer(p);
              }
          }
      }

      public int AddLevel(int w, int h, Vector3 n)
      {
          int randIndex = GameConstants.RANDOM.Next(m_Game.Themes.Count);
          ThemeTextures startTheme = m_Game.Themes[randIndex];
          Game.PlatformSpawner ps = new Game.PlatformSpawner(startTheme.Platforms, w);
          Game.Level lvl = new Game.Level(this, m_Levels.Count, ps, startTheme, m_Keyboard, m_Gamepad, w, h, n);

          m_Levels.Add(lvl);

          //for (int i = 0; i < m_Levels.Count-1; ++i)
          //{
          //    CreatePortalLink(m_Levels.Count-1, i);
          //}

          

          /*if (m_Levels.Count == 1)
          {
              foreach (Game.Player p in m_Players)
              {
                  if (p != null)
                  {
                      p.MovePlayer(m_Levels[0]);
                      m_Levels[0].AddPlayer(p);
                  }
              }
          }*/

          return m_Levels.Count;
      }

      private void ChangeThemes()
      {
          foreach (Game.Level lvl in m_Levels)
          {
              int randIndex = GameConstants.RANDOM.Next(m_Game.Themes.Count);
              ThemeTextures newTheme = m_Game.Themes[randIndex];
              lvl.ChangeTheme(newTheme);
          }
      }

      public Game.Player GetPlayer(PlayerIndex player)
      {
          int index = (int)player;

          if (index <= GameConstants.MAX_PLAYERS)
          {
              return m_Players[index];
          }
          else
          {
              return null;
          }
      }

      public void CreatePortalLink(int lvlA, int lvlB)
      {
          if (lvlA != lvlB && lvlA < m_Levels.Count && lvlB < m_Levels.Count)
          {
              m_Levels[lvlA].AddPortal(lvlB, m_PortalTex, GameConstants.GAME_COLORS[m_PortalColorIndex]);
              m_Levels[lvlB].AddPortal(lvlA, m_PortalTex, GameConstants.GAME_COLORS[m_PortalColorIndex]);
              m_PortalColorIndex++;
          }
      }

      public void TransferPlayer(Game.Player player, int from, int to)
      {
          player.State = Game.Player.States.PORTING;
          player.MovePlayer(m_Levels[to]);
          m_Levels[to].AddPlayer(player, from);
      }

      /*public void OnButtonUpP1(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              if(
          }
      }
      public void OnButtonUpP2(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              if (m_Players[1].getDrawHudBool() == false)
              {
                  m_Players[1].setCharSelectionHud(true);
              }
              else
              {
                  m_Players[1].setCharSelectionHud(false);
              }
          }
      }
      public void OnButtonUpP3(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              if (m_Players[2].getDrawHudBool() == false)
              {
                  m_Players[2].setCharSelectionHud(true);
              }
              else
              {
                  m_Players[2].setCharSelectionHud(false);
              }
          }
      }
      public void OnButtonUpP4(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              if (m_Players[3].getDrawHudBool() == false)
              {
                  m_Players[3].setCharSelectionHud(true);
              }
              else
              {
                  m_Players[3].setCharSelectionHud(false);
              }
          }
      }
       */
      public void OnKeyRelease(object sender, Keys[] keys)
      {
          foreach (Keys k in keys)
          {
             // AJ changed because escape is used to exit the game in editor mode.  This
             // caused the state machine to set the state to main menu anyways, because
             // both the gameplay state and the editor state were on the stack.
              if (k == Keys.LeftAlt)
              {
                  FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
              }

              //DEBUG
              if (k == Keys.I)
              {
                  //m_Players[0].ChangeStatus(Game.Player.Bonus.INVINCIBLE, 5);

              }
          }
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
          m_ScoreBoard.DrawRenderTarget(spriteBatch, m_Players);
          
          foreach (Game.Level l in m_Levels)
          {
              l.Draw(spriteBatch, m_Game.GraphicsDevice);
          }
          //m_ScoreBoard.Draw(spriteBatch, m_Players);
      }

      #region Public Access TV

      public Game.ScoreBoard Scoreboard
      {
          get { return m_ScoreBoard; }
      }

      public void StartNewGame()
      {
         m_Players[(int)PlayerIndex.One] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.One);
         m_Players[(int)PlayerIndex.Two] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Two);
         m_Players[(int)PlayerIndex.Three] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Three);
         m_Players[(int)PlayerIndex.Four] = new Game.Player(this, m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.Four);
         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_RUN_FRAMERATE, false));
         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_RUN_FRAMERATE, false));
         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_RUN_FRAMERATE, false));
         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_RUN_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_RUN_FRAMERATE, false));
         m_Players[(int)PlayerIndex.One].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Two].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Three].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_Players[(int)PlayerIndex.Four].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex, m_BestScoreTex);
         m_ScoreBoard.LoadContent(m_HUDTex,m_ArialFontLarge);

         foreach (Game.Player p in m_Players)
         {
            p.MovePlayer(m_Levels[0]);
            m_Levels[0].AddPlayer(p);
         }
      }

      public RenderTarget2D GetRenderTarget(int lvl)
      {
          if (lvl < m_Levels.Count)
          {
              return m_Levels[lvl].RenderTarget;
          }
          else
          {
              return null;
          }
      }

      public GraphicsDevice Graphics
      {
          get
          {
              return m_Game.GraphicsDevice;
          }
      }

      public List<Game.Level> Levels
      {
          get
          {
              return m_Levels;
          }
      }

      public bool RenderTargetMode
      {
          get
          {
              if (m_Levels.Count > 0)
              {
                  return m_Levels[0].RenderTargetMode;
              }
              return false;
          }
          set
          {
              foreach (Game.Level l in m_Levels)
              {
                  l.RenderTargetMode = value;
              }
          }
      }
      
      public Game.ColorPicker Colors
      {
          get
          {
              return m_ColorPicker;
          }
      }

      public void SetLight(Vector3 light)
      {
          foreach (Game.Level lvl in m_Levels)
          {
              lvl.SetLight(light);
          }
      }

      public Texture2D TitleScreen
      {
          get
          {
              return m_TitlePage;
          }
      }


      public int PlayersAlive
      {
          get
          {
              int a = 0;

              foreach (Game.Player p in m_Players)
              {
                  if (p.State == Game.Player.States.PLAYING || p.State == Game.Player.States.PORTING)
                  {
                      a++;
                  }
              }

              return a;
          }
      }
      #endregion

   }
}
