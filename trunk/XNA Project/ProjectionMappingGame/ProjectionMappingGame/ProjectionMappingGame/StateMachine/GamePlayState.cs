
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
using System.IO;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Audio;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   class GamePlayState : GameState
   {
      // Fonts
      SpriteFont m_ArialFont;

      //private GraphicsDeviceManager graphics;
      //public ContentManager content;

      private int levelIndex = -1;
      private Level level;
      private bool wasContinuePressed;
      private float elapsedTimeReusable;

      private GamePadState gamePadState;
      private KeyboardState keyboardState;

      private const int numberOfLevels = 1;

      public GamePlayState(GameDriver game)
         : base(game, StateType.GamePlay)
      {
          
      }

      public override void Reset()
      {
      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");

         //Known issue that you get exceptions if you use Media PLayer while connected to your PC
         //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
         //Which means its impossible to test this from VS.
         //So we have to catch the exception and throw it away
         try
         {
             MediaPlayer.IsRepeating = true;
             MediaPlayer.Play(content.Load<Song>("Sounds/Music"));
         }
         catch { }

         LoadNextLevel();
      }

      public override void Update(float elapsedTime)
      {
         // Update any logic here
          elapsedTimeReusable = elapsedTime;
          HandleInput(elapsedTime);

          // update our level, passing down the GameTime along with all of our input states
          //level.Update(gameTime, keyboardState, gamePadState, touchState,accelerometerState,Window.CurrentOrientation);
          level.Update(elapsedTime, keyboardState, gamePadState);

          //base.Update(gameTime);
          //base.Update(elapsedTime);
      }

      public override void HandleInput(float elapsedTime)
      {
         // Handle any input here
         if (Keyboard.GetState().IsKeyDown(Keys.Enter))
         {
            FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
         }
         keyboardState = Keyboard.GetState();
         gamePadState = GamePad.GetState(PlayerIndex.One);
         //touchState = TouchPanel.GetState();
         //accelerometerState = Accelerometer.GetState();

         // Exit the game when back is pressed.
        /* if (gamePadState.Buttons.Back == ButtonState.Pressed)
             Exit();*/

         bool continuePressed =
             keyboardState.IsKeyDown(Keys.Space) ||
             gamePadState.IsButtonDown(Buttons.A); //||
             //touchState.AnyTouch();

         // Perform the appropriate action to advance the game and
         // to get the player back to playing.
         if (!wasContinuePressed && continuePressed)
         {
             if (!level.Player.IsAlive)
             {
                 level.StartNewLife();
             }
            /* else if (level.TimeRemaining == TimeSpan.Zero)
             {
                 if (level.ReachedExit)
                     LoadNextLevel();
                 else
                     ReloadCurrentLevel();
             }*/
         }

         wasContinuePressed = continuePressed;
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();

         // Render anything here
         float elapsedTime = elapsedTimeReusable;
         level.Draw(elapsedTime, spriteBatch);

         spriteBatch.DrawString(m_ArialFont, "Game Play: press enter to go back to main menu", new Vector2(5, 5), Color.Black);

         spriteBatch.End();
      }
      private void LoadNextLevel()
      {
          // move to the next level
          levelIndex = (levelIndex + 1) % numberOfLevels;

          // Unloads the content for the current level before loading the next one.
          if (level != null)
              level.Dispose();

          // Load the level.
          string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
          using (Stream fileStream = TitleContainer.OpenStream(levelPath))
          {
              
              level = new Level(m_Game.Services,
                  // Services,
                   fileStream, levelIndex);
          }
      }
   }
}
