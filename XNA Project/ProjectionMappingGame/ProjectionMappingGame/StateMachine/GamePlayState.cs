
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
   public class GamePlayState : GameState
   {
       

      // Fonts
      SpriteFont m_ArialFont;

      Texture2D m_PlayerIdleTex;
      Texture2D m_PlayerRunTex;
      Texture2D m_PlayerJumpTex;

      Texture2D[] m_Backgrounds;

      Texture2D []m_PlayerColors;

      Texture2D[][] m_PlatformTex;

      Texture2D m_PortalTex;
      Texture2D m_HUDTex;

      Game.Player[] m_Players;

      GUI.KeyboardInput m_Keyboard;
      GUI.GamepadInput m_Gamepad;

      List<Game.Level> m_Levels;
      int m_LvlCount;

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

          m_LvlCount = 0;
      }



      public override void Reset()
      {
         m_Levels.Clear();

         AddLevel(GameConstants.WindowWidth, GameConstants.WindowHeight);

         m_Players[(int)PlayerIndex.One] = new Game.Player(m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, m_Keyboard, PlayerIndex.One);

         m_Players[(int)PlayerIndex.Two] = new Game.Player(m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, PlayerIndex.Two);

         m_Players[(int)PlayerIndex.Three] = new Game.Player(m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, PlayerIndex.Three);

         m_Players[(int)PlayerIndex.Four] = new Game.Player(m_PlayerIdleTex, new Rectangle(0, 0, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Gamepad, PlayerIndex.Four);

         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Four].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));

         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Three].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));

         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
         m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));

         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
         m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));


         m_Players[(int)PlayerIndex.One].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex);
         m_Players[(int)PlayerIndex.Two].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex);
         m_Players[(int)PlayerIndex.Three].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex);
         m_Players[(int)PlayerIndex.Four].LoadHudContent(m_ArialFont, m_Game.GraphicsDevice, m_HUDTex);

         foreach (Game.Player p in m_Players)
         {
             p.MovePlayer(m_Levels[0]);
             m_Levels[0].AddPlayer(p);
         }
      }

      public override void Resize(int dx, int dy)
      {

      }


      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
          // Load Sprites
         m_PlayerIdleTex = content.Load<Texture2D>("Sprites/Idle");
         m_PlayerRunTex = content.Load<Texture2D>("Sprites/Run");
         m_PlayerJumpTex = content.Load<Texture2D>("Sprites/Jump");

         m_PlatformTex = new Texture2D[][] 
          {
              new Texture2D[] 
              { 
                  content.Load<Texture2D>("Tiles/Type1/BlockA0"),
                  content.Load<Texture2D>("Tiles/Type1/BlockA2"),
                  content.Load<Texture2D>("Tiles/Type1/BlockA6")
              }, 
              new Texture2D[] 
              { 
                  content.Load<Texture2D>("Tiles/Type2/BlockB0"),
                  content.Load<Texture2D>("Tiles/Type2/BlockB1"),
                  content.Load<Texture2D>("Tiles/Type2/Platform")
              }
          };

         m_HUDTex = content.Load<Texture2D>("Textures/white");

         m_Backgrounds = new Texture2D[]
         {
             content.Load<Texture2D>("Textures/Layer0_2")
         };
      }

       

      public override void Update(float elapsedTime)
      {
          foreach (Game.Level l in m_Levels)
          {
              l.Update(elapsedTime);
          }
      }

      public override void HandleInput(float elapsedTime)
      {
          m_Keyboard.HandleInput(PlayerIndex.One);

          foreach (Game.Level l in m_Levels)
          {
              l.HandleInput();
          }
      }

      public void PlayerDied(Game.Player player)
      {
          foreach (Game.Player p in m_Players)
          {
              if (p != player && p.State == Game.Player.States.PLAYING)
              {
                  p.HUD.PlayerDefeated();
              }
          }
      }

      public int AddLevel(int w, int h)
      {
          Game.PlatformSpawner ps = new Game.PlatformSpawner(m_PlatformTex, w);
          int backIndex = GameConstants.RANDOM.Next(m_Backgrounds.Length);
          m_Levels.Add(new Game.Level(this, m_LvlCount, ps, m_Backgrounds[backIndex], m_Keyboard, m_Gamepad, w, h));
          return ++m_LvlCount;
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
              m_Levels[lvlA].AddPortal(lvlB, m_PortalTex, Color.Red);
              m_Levels[lvlB].AddPortal(lvlA, m_PortalTex, Color.Red);
          }
      }

      public void TransferPlayer(Game.Player player, int from, int to)
      {
          player.State = Game.Player.States.PORTING;
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
              if (k == Keys.Escape)
              {
                  FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
              }
          }
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
          foreach (Game.Level l in m_Levels)
          {
              l.Draw(spriteBatch, m_Game.GraphicsDevice);
          }
      }

      #region Public Access TV

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

      #endregion

   }
}
