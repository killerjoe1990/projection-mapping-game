
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
      // Fonts
      SpriteFont m_ArialFont;
      
      // Render target
      Texture2D m_RenderTarget;

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

         // TEMPORARY LOAD FOR RENDER TARGET
         m_RenderTarget = content.Load<Texture2D>("Textures/pow");
      }

      public override void Update(float elapsedTime)
      {
         // Update any logic here
      }

      public override void HandleInput(float elapsedTime)
      {
         // Handle any input here
         if (Keyboard.GetState().IsKeyDown(Keys.Enter))
         {
            FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
         }
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();

         // Render anything here
         spriteBatch.DrawString(m_ArialFont, "Game Play: press enter to go back to main menu", new Vector2(5, 5), Color.Black);

         spriteBatch.End();
      }

      #region Public Access TV

      public Texture2D RenderTarget
      {
         get { return m_RenderTarget; }
      }

      #endregion

   }
}
