
#region File Description

//-----------------------------------------------------------------------------
// EditorMenu.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/12/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local imports
using ProjectionMappingGame.GUI;

#endregion

namespace ProjectionMappingGame.Editor
{
   class EditorMenu
   {
      Viewport m_Viewport;
      GameDriver m_Game;

      // Application section GUI objects
      Label m_AppHeaderLabel;
      Label m_AppModeLabel;
      Label m_AppModeValueLabel;

      // Gizmo section GUI objects
      Label m_GizmoHeaderLabel;

      // Building section GUI objects
      Label m_BuildingHeaderLabel;
      Label m_BuildingFilepathLabel;
      Label m_BuildingFilepathValueLabel;
      Label m_BuildingPositionLabel;
      Label m_BuildingRotationLabel;
      Label m_BuildingScaleLabel;
      Label m_BuildingSnapToGroundLabel;

      // Projector section GUI objects
      Label m_ProjectorHeaderLabel;
      Label m_ProjectorFOVLabel;
      Label m_ProjectorAspectRatioLabel;
      Label m_ProjectorPositionLabel;
      Label m_ProjectorRotationLabel;
      Label m_ProjectorEnabledLabel;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;

      public EditorMenu(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public void LoadContent(ContentManager content)
      {
      }

      public void Reset()
      {
      }

      #region Input Handling

      // Mouse bounds for easy selection
      const int MOUSE_SELECTION_BUFFER = 3;

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         Vector2 mousePos = new Vector2(mouseState.X - m_Viewport.X, mouseState.Y - m_Viewport.Y);
         Rectangle mouseBounds = new Rectangle((int)(mousePos.X - MOUSE_SELECTION_BUFFER), (int)(mousePos.Y - MOUSE_SELECTION_BUFFER), MOUSE_SELECTION_BUFFER * 2, MOUSE_SELECTION_BUFFER * 2);

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      #endregion

      #region Rendering

      public void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();
         RenderApplicationSection(spriteBatch);
         RenderGizmoSection(spriteBatch);
         RenderBuildingSection(spriteBatch);
         RenderProjectorSection(spriteBatch);
         spriteBatch.End();
      }

      private void RenderApplicationSection(SpriteBatch spriteBatch)
      {
         m_AppHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_AppModeLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_AppModeValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderGizmoSection(SpriteBatch spriteBatch)
      {
         m_GizmoHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderBuildingSection(SpriteBatch spriteBatch)
      {
         m_BuildingHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingSnapToGroundLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderProjectorSection(SpriteBatch spriteBatch)
      {
         m_ProjectorHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorFOVLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAspectRatioLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorEnabledLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      #endregion
   }
}
