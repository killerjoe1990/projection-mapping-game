#region File Description

//-----------------------------------------------------------------------------
// GameProjectorRenderState.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/1/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ProjectionMappingGame.Components;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   class GameProjectorRenderState : GameState
   {
      // Rendering
      GraphicsDevice m_GraphicsDevice;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;

      // Fonts
      SpriteFont m_ArialFont;

      // Projector Output
      ProjectionPreviewComponent m_ProjectorPreview;

      public GameProjectorRenderState(GameDriver game)
         : base(game, StateType.GameProjectorRender)
      {
         m_GraphicsDevice = game.GraphicsDevice;

         // Initialize projector
         m_ProjectorPreview = new ProjectionPreviewComponent(game, 0, 0, GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT);
         m_ProjectorPreview.RenderFrustum = false;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public override void Reset()
      {
      }

      public override void LoadContent(ContentManager content)
      {
         // Load font
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");

         // Load the projector preview's content
         m_ProjectorPreview.LoadContent(content);
      }

      public override void Update(float elapsedTime)
      {
      }

      public override void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         // Allow for exit to main menu
         if (keyboardState.IsKeyDown(Keys.Escape))
            FiniteStateMachine.GetInstance().SetState(StateType.ProjectionEditor);

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      public override void Draw(SpriteBatch spriteBatch)
      {
         // Render the projector preview
         m_ProjectorPreview.Draw();

         int column1 = 10;
         int column2 = 200;

         spriteBatch.Begin();
         spriteBatch.DrawString(m_ArialFont, "Game Projection Controls", new Vector2(5, 5), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Hide Controls", new Vector2(column1, 30), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Back to Editor", new Vector2(column1, 55), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "C", new Vector2(column2, 30), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Escape", new Vector2(column2, 55), Color.Black);
         spriteBatch.End();
      }

      #region Public Access TV

      public void ConfigureProjector(Vector3 target, float distance, float yaw, float pitch)
      {
         m_ProjectorPreview.Projector.Target = target;
         m_ProjectorPreview.Projector.Distance = distance;
         m_ProjectorPreview.Projector.Yaw = yaw;
         m_ProjectorPreview.Projector.Pitch = pitch;
         m_ProjectorPreview.Projector.UpdateView();
         m_ProjectorPreview.Camera.Target = target;
         m_ProjectorPreview.Camera.Distance = distance;
         m_ProjectorPreview.Camera.Yaw = yaw;
         m_ProjectorPreview.Camera.Pitch = pitch;
         m_ProjectorPreview.Camera.UpdateView();
      }

      public Texture2D ProjectorInput
      {
         set { m_ProjectorPreview.ProjectorTexture = value; }
      }

      #endregion

   }
}
