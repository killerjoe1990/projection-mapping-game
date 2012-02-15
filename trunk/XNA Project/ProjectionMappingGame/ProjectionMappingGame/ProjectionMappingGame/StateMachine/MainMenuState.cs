
#region File Description

/******************************************************************
 * Filename:        MainMenuState.cs
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
   class MainMenuState : GameState
   {
      // Fonts
      SpriteFont m_ArialFont;

      public MainMenuState(GameDriver game)
         : base(game, StateType.MainMenu)
      {
      }

      public override void Reset()
      {
      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
      }

      public override void Update(float elapsedTime)
      {
         // Update any logic here
      }

      public override void HandleInput(float elapsedTime)
      {
         // Handle any input here
         if (Keyboard.GetState().IsKeyDown(Keys.Space))
         {
            FiniteStateMachine.GetInstance().SetState(StateType.GamePlay);
         }
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();

         // Render anything here
         spriteBatch.DrawString(m_ArialFont, "Main Menu: press space to go to game play", new Vector2(5, 5), Color.Black);

         spriteBatch.End();
      }
   }
}
