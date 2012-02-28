
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
   class GamePlayState : GameState
   {
       const float START_Y = 50;

      // Fonts
      SpriteFont m_ArialFont;
      
      // Render target
      bool m_RenderTargetMode;
      Texture2D m_RenderTargetTexture;
      RenderTarget2D m_RenderTarget;

      Texture2D m_PlayerIdleTex;
      Texture2D m_PlayerRunTex;
      Texture2D m_PlayerJumpTex;

      Texture2D m_Background;

      Texture2D[][] m_PlatformTex;

      GUI.KeyboardInput m_Keyboard;
      GUI.GamepadInput m_Gamepad;

      Game.Player[] m_Players;
      List<Game.Platform> m_Platforms;
      Game.PlatformSpawner m_PlatSpawn;


      int m_NumPlayers;

      public GamePlayState(GameDriver game)
         : base(game, StateType.GamePlay)
      {
        #if WINDOWS
          m_Keyboard = new GUI.KeyboardInput();
          m_Keyboard.RegisterKeyPress(OnKeyRelease);
        #endif
          m_Gamepad = new GUI.GamepadInput();
          m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP1, PlayerIndex.One);
          m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP2, PlayerIndex.Two);
          m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP3, PlayerIndex.Three);
          m_Gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_UP, OnButtonUpP4, PlayerIndex.Four);


          m_Players = new Game.Player[GameConstants.MAX_PLAYERS];

          m_NumPlayers = 1;

          m_Platforms = new List<Game.Platform>();

          // Initialize render target
          m_RenderTargetMode = false;
          m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, GameConstants.WindowWidth, GameConstants.WindowHeight, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
      }

      public override void Reset()
      {
          for (int i = 0; i < m_Players.Length; ++i)
          {
              m_Players[i] = null;
          }

          m_Players[(int)PlayerIndex.One] = new Game.Player(m_PlayerIdleTex, new Rectangle(GameConstants.DEFAULT_WINDOW_WIDTH / (GameConstants.MAX_PLAYERS + 1), (int)START_Y, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y), m_Keyboard, PlayerIndex.One);

          m_Players[(int)PlayerIndex.Two] = new Game.Player(m_PlayerIdleTex, new Rectangle(GameConstants.DEFAULT_WINDOW_WIDTH / (GameConstants.MAX_PLAYERS + 1) + 50, (int)START_Y, GameConstants.PLAYER_DIM_X, GameConstants.PLAYER_DIM_Y),
              m_Gamepad, PlayerIndex.One);

          m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
          m_Players[(int)PlayerIndex.One].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));

          m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.RUN, new Game.Animation(m_PlayerRunTex, 10, GameConstants.PLAYER_FRAMERATE, true));
          m_Players[(int)PlayerIndex.Two].AddAnimation(Game.Player.Animations.JUMP, new Game.Animation(m_PlayerJumpTex, 11, GameConstants.PLAYER_FRAMERATE, false));

          m_Platforms.Clear();

          m_Platforms.Add(new Game.Platform(new Vector2(GameConstants.DEFAULT_WINDOW_WIDTH / (GameConstants.MAX_PLAYERS + 1), 200), Vector2.Zero, 5, Game.PlatformTypes.Impassable, m_PlatformTex[1]));
      }

      public void AddPlayer(Game.Player player, PlayerIndex playerNumber)
      {
          if (m_Players[(int)playerNumber] == null)
          {
              m_NumPlayers++;
          }

          m_Players[(int)playerNumber] = player;
      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
          // Load Sprites
         m_PlayerIdleTex = content.Load<Texture2D>("Sprites/Idle");
         m_PlayerRunTex = content.Load<Texture2D>("Sprites/Run");
         m_PlayerJumpTex = content.Load<Texture2D>("Sprites/Jump");

         // TEMPORARY LOAD FOR RENDER TARGET
         m_RenderTargetTexture = content.Load<Texture2D>("Textures/default_editor_input");

         m_Background = content.Load<Texture2D>("Textures/Layer0_2");

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

         m_PlatSpawn = new Game.PlatformSpawner(m_PlatformTex);
      }

      public override void Update(float elapsedTime)
      {
         // Update any logic here
          List<Game.Platform> newPlats = m_PlatSpawn.SpawnPlatforms(elapsedTime);

          if (newPlats != null)
          {
              m_Platforms.AddRange(newPlats);
          }

          foreach (Game.Platform platform in m_Platforms)
          {
              platform.Update(elapsedTime);
          }

          foreach (Game.Player player in m_Players)
          {
              if (player != null)
              {
                  player.CheckCollisions(m_Platforms, elapsedTime);

                  foreach (Game.Player p in m_Players)
                  {
                      if (p != null && p != player)
                      {
                          player.CheckCollision(p, elapsedTime);
                      }
                  }

                  player.Update(elapsedTime);
              }

              
          }
      }

      public override void HandleInput(float elapsedTime)
      {
         // Handle any input here
#if WINDOWS
          switch (m_NumPlayers)
          {
              case 1:
                m_Keyboard.HandleInput(PlayerIndex.One);
                break;
              case 2:
                m_Keyboard.HandleInput(PlayerIndex.One);
                m_Keyboard.HandleInput(PlayerIndex.Two);
                break;
              case 3:
                m_Keyboard.HandleInput(PlayerIndex.One);
                m_Keyboard.HandleInput(PlayerIndex.Two);
                m_Keyboard.HandleInput(PlayerIndex.Three);
                break;
              case 4:
                m_Keyboard.HandleInput(PlayerIndex.One);
                m_Keyboard.HandleInput(PlayerIndex.Two);
                m_Keyboard.HandleInput(PlayerIndex.Three);
                m_Keyboard.HandleInput(PlayerIndex.Four);
                break;
          }
#endif
          switch (m_NumPlayers)
          {
              case 1:
                m_Gamepad.HandleInput(PlayerIndex.One);
                break;
              case 2:
                m_Gamepad.HandleInput(PlayerIndex.One);
                m_Gamepad.HandleInput(PlayerIndex.Two);
                break;
              case 3:
                m_Gamepad.HandleInput(PlayerIndex.One);
                m_Gamepad.HandleInput(PlayerIndex.Two);
                m_Gamepad.HandleInput(PlayerIndex.Three);
                break;
              case 4:
                m_Gamepad.HandleInput(PlayerIndex.One);
                m_Gamepad.HandleInput(PlayerIndex.Two);
                m_Gamepad.HandleInput(PlayerIndex.Three);
                m_Gamepad.HandleInput(PlayerIndex.Four);
                break;
          }
      }

      public void OnButtonUpP1(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              FiniteStateMachine.GetInstance().ChangeState(StateType.Player1Menu);
          }
      }
      public void OnButtonUpP2(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              //FiniteStateMachine.GetInstance().ChangeState(StateType.Player2Menu);
          }
      }
      public void OnButtonUpP3(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              //FiniteStateMachine.GetInstance().ChangeState(StateType.Player3Menu);
          }
      }
      public void OnButtonUpP4(object sender, GUI.GamepadInput.Buttons button)
      {
          if (button == GUI.GamepadInput.Buttons.START)
          {
              // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
              //FiniteStateMachine.GetInstance().ChangeState(StateType.Player4Menu);
          }
      }

      public void OnKeyRelease(object sender, Keys[] keys)
      {
          foreach (Keys k in keys)
          {
              if (k == Keys.Enter)
              {
                  FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
              }

              if (k == Keys.Back)
              {
                  // JOHANNES: ADD CODE HERE TO PUSH YOUR STATE ONTO STACK
                  FiniteStateMachine.GetInstance().ChangeState(StateType.Player1Menu);
                  //FiniteStateMachine.GetInstance().ChangeState(StateType.Player2Menu);
                  //FiniteStateMachine.GetInstance().ChangeState(StateType.Player3Menu);
                  //FiniteStateMachine.GetInstance().ChangeState(StateType.Player4Menu);
              }
          }
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
         if (m_RenderTargetMode)
         {
            Color clear = new Color(0.0f, 0.0f, 0.0f, 0.0f);

            // Render the quads into the render target
            m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
            m_Game.GraphicsDevice.Clear(clear);

            RenderGame(spriteBatch);

            // Extract and store the contents of the render target in a texture
            m_Game.GraphicsDevice.SetRenderTarget(null);
            m_Game.GraphicsDevice.Clear(Color.CornflowerBlue);
            m_RenderTargetTexture = (Texture2D)m_RenderTarget;
         }
         else
         {
            RenderGame(spriteBatch); 
         } 
      }

      private void RenderGame(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();

         // Render anything here
         //spriteBatch.DrawString(m_ArialFont, "Game Play: press enter to go back to main menu", new Vector2(5, 5), Color.Black);
         spriteBatch.Draw(m_Background, new Rectangle(0, 0, GameConstants.DEFAULT_WINDOW_WIDTH, GameConstants.DEFAULT_WINDOW_HEIGHT), Color.White);

         foreach (Game.Platform platform in m_Platforms)
         {
            platform.Draw(spriteBatch);
         }

         foreach (Game.Player player in m_Players)
         {
            if (player != null)
            {
               player.Draw(spriteBatch);
            }
         }
         spriteBatch.End();
      }

      #region Public Access TV

      public Texture2D RenderTarget
      {
         get { return m_RenderTargetTexture; }
      }

      public bool RenderTargetMode
      {
         get { return m_RenderTargetMode; }
         set { m_RenderTargetMode = value; }
      }

      #endregion

   }
}
