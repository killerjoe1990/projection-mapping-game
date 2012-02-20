#region File Description

//-----------------------------------------------------------------------------
// ProjectionEditorState.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    1/30/2012
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

// Local imports
using ProjectionMappingGame.Components;
using ProjectionMappingGame.Editor;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   class ProjectionEditorState : GameState
   {
      // Rendering
      GraphicsDevice m_GraphicsDevice;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;
      
      // Editor panes
      ProjectionPreviewComponent m_ProjectorPreview;
      UVGridEditor m_UVGridEditor;
      UVPointEditor m_UVEditor;
      int m_FocusedPane;

      SpriteFont m_ArialFont;

      #region GUI CONSTANTS
      
      // Main viewports
      const int GUI_UV_GRID_EDITOR_X = 0;
      const int GUI_UV_GRID_EDITOR_Y = 0;
      const int GUI_UV_GRID_EDITOR_WIDTH = GUI_UV_GRID_EDITOR_HEIGHT;
      const int GUI_UV_GRID_EDITOR_HEIGHT = (GameConstants.WINDOW_HEIGHT / 2);
      const int GUI_UV_EDITOR_X = 0;
      const int GUI_UV_EDITOR_Y = GUI_UV_GRID_EDITOR_HEIGHT;
      const int GUI_UV_EDITOR_WIDTH = GUI_UV_GRID_EDITOR_WIDTH;
      const int GUI_UV_EDITOR_HEIGHT = GameConstants.WINDOW_HEIGHT - GUI_UV_GRID_EDITOR_HEIGHT;
      const int GUI_PROJECTOR_PREVIEW_X = GUI_UV_GRID_EDITOR_WIDTH;
      const int GUI_PROJECTOR_PREVIEW_Y = 0;
      const int GUI_PROJECTOR_PREVIEW_WIDTH = GameConstants.WINDOW_WIDTH - GUI_UV_GRID_EDITOR_WIDTH - GUI_TOOLBAR_WIDTH;
      const int GUI_PROJECTOR_PREVIEW_HEIGHT = GameConstants.WINDOW_HEIGHT;
      const int GUI_TOOLBAR_X = GUI_PROJECTOR_PREVIEW_X + GUI_PROJECTOR_PREVIEW_WIDTH;
      const int GUI_TOOLBAR_Y = 0;
      const int GUI_TOOLBAR_WIDTH = 200;
      const int GUI_TOOLBAR_HEIGHT = GameConstants.WINDOW_HEIGHT;

      #endregion

      public ProjectionEditorState(GameDriver game)
         : base(game, StateType.ProjectionEditor)
      {
         m_GraphicsDevice = game.GraphicsDevice;

         // Initialize editor panes
         m_UVGridEditor = new UVGridEditor(m_Game, GUI_UV_GRID_EDITOR_X, GUI_UV_GRID_EDITOR_Y, GUI_UV_GRID_EDITOR_WIDTH, GUI_UV_GRID_EDITOR_HEIGHT);
         m_UVEditor = new UVPointEditor(m_Game, GUI_UV_EDITOR_X, GUI_UV_EDITOR_Y, GUI_UV_EDITOR_WIDTH, GUI_UV_EDITOR_HEIGHT);
         m_ProjectorPreview = new ProjectionPreviewComponent(m_Game, GUI_PROJECTOR_PREVIEW_X, GUI_PROJECTOR_PREVIEW_Y, GUI_PROJECTOR_PREVIEW_WIDTH, GUI_PROJECTOR_PREVIEW_HEIGHT);
         m_FocusedPane = 0;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public override void Reset()
      {
         m_UVEditor.Reset();
         m_UVGridEditor.Reset();
         m_ProjectorPreview.Reset();

         m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
      }

      public override void LoadContent(ContentManager content)
      {
         // Load font
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");

         // Load the projector preview's content
         m_UVGridEditor.LoadContent(content);
         m_UVEditor.LoadContent(content);
         m_ProjectorPreview.LoadContent(content);

         // Load initial points
         m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
      }

      public override void Update(float elapsedTime)
      {
         if (m_UVGridEditor.HasUpdates)
         {
            m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
            m_UVGridEditor.HasUpdates = false;
         }

         m_UVEditor.Update(elapsedTime);

         if (m_UVEditor.RenderTargetTexture != null)
            m_ProjectorPreview.ProjectorTexture = m_UVEditor.RenderTargetTexture;
      }

      public override void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         // Allow for exit to main menu
         if (keyboardState.IsKeyDown(Keys.M))
            FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);

         // Allow for gameplay projection
         if (keyboardState.IsKeyDown(Keys.Enter))
            FiniteStateMachine.GetInstance().SetState(StateType.GameProjectorRender);

         if (keyboardState.IsKeyDown(Keys.R) && !m_PrevKeyboardState.IsKeyDown(Keys.R))
         {
            Reset();
            return;
         }

         // Update the UV Grid Editor
         if (mouseState.X >= m_UVGridEditor.Viewport.X && mouseState.X <= m_UVGridEditor.Viewport.X + m_UVGridEditor.Viewport.Width &&
             mouseState.Y >= m_UVGridEditor.Viewport.Y && mouseState.Y <= m_UVGridEditor.Viewport.Y + m_UVGridEditor.Viewport.Height)
         {
            if (m_FocusedPane != 0 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               m_FocusedPane = 0;
               m_UVGridEditor.PrevMouseState = m_PrevMouseState;
            }

            if (m_FocusedPane == 0)
               m_UVGridEditor.HandleInput(elapsedTime);
         }

         // Update the UV Editor
         if (mouseState.X >= m_UVEditor.Viewport.X && mouseState.X <= m_UVEditor.Viewport.X + m_UVEditor.Viewport.Width &&
             mouseState.Y >= m_UVEditor.Viewport.Y && mouseState.Y <= m_UVEditor.Viewport.Y + m_UVEditor.Viewport.Height)
         {
            if (m_FocusedPane != 1 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               m_FocusedPane = 1;
               m_UVEditor.PrevMouseState = m_PrevMouseState;
            }

            if (m_FocusedPane == 1)
               m_UVEditor.HandleInput(elapsedTime);
         }

         // Update the Projection Preview
         if (mouseState.X >= m_ProjectorPreview.Viewport.X && mouseState.X <= m_ProjectorPreview.Viewport.X + m_ProjectorPreview.Viewport.Width &&
             mouseState.Y >= m_ProjectorPreview.Viewport.Y && mouseState.Y <= m_ProjectorPreview.Viewport.Y + m_ProjectorPreview.Viewport.Height)
         {
            if (m_FocusedPane != 2 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
            {
               m_FocusedPane = 2;
               m_ProjectorPreview.PrevMouseState = m_PrevMouseState;
            }

            if (m_FocusedPane == 2)
               m_ProjectorPreview.HandleInput(elapsedTime);
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      #region Rendering

      public override void Draw(SpriteBatch spriteBatch)
      {
         Viewport defaultViewport = m_Game.GraphicsDevice.Viewport;

         // Render the UV editor
         m_Game.GraphicsDevice.Viewport = m_UVEditor.Viewport;
         m_UVEditor.Draw(spriteBatch);

         // Render the UV grid editor
         m_Game.GraphicsDevice.Viewport = m_UVGridEditor.Viewport;
         m_UVGridEditor.Draw(spriteBatch);
         
         // Render the projector preview
         m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Viewport;
         m_ProjectorPreview.Draw();
         m_ProjectorPreview.DrawGUI(spriteBatch);

         // Restore the default viewport
         m_Game.GraphicsDevice.Viewport = defaultViewport;
      }

      #endregion

      #region Public Access TV

      public Texture2D ProjectorInput
      {
         set
         {
            // Assign the render target to the editors
            m_UVGridEditor.RenderTarget = value;
            m_UVEditor.InputTexture = value;
            m_ProjectorPreview.ProjectorTexture = value;
         }
      }

      public Vector3 ProjectorTarget
      {
         get { return m_ProjectorPreview.Projector.Target; }
      }

      public float ProjectorDistance
      {
         get { return m_ProjectorPreview.Projector.Distance; }
      }

      public float ProjectorYaw
      {
         get { return m_ProjectorPreview.Projector.Yaw; }
      }

      public float ProjectorPitch
      {
         get { return m_ProjectorPreview.Projector.Pitch; }
      }


      #endregion

   }
}
