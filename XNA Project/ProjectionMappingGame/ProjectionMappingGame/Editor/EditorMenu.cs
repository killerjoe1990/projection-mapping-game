
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

      // Textures
      Texture2D m_WhiteTexture;

      // Fonts
      SpriteFont m_ArialFont;

      // Gizmo section GUI objects
      Label m_GizmoHeaderLabel;
      Label m_GizmoHeaderValueLabel;

      // Building section GUI objects
      Label m_BuildingHeaderLabel;
      Label m_BuildingFilepathLabel;
      Label m_BuildingFilepathValueLabel;
      Label m_BuildingPositionLabel;
      Label m_BuildingPositionXLabel;
      Label m_BuildingPositionYLabel;
      Label m_BuildingPositionZLabel;
      Label m_BuildingPositionXValueLabel;
      Label m_BuildingPositionYValueLabel;
      Label m_BuildingPositionZValueLabel;
      Label m_BuildingRotationLabel;
      Label m_BuildingRotationXLabel;
      Label m_BuildingRotationYLabel;
      Label m_BuildingRotationZLabel;
      Label m_BuildingRotationXValueLabel;
      Label m_BuildingRotationYValueLabel;
      Label m_BuildingRotationZValueLabel;
      Label m_BuildingScaleLabel;
      Label m_BuildingScaleXLabel;
      Label m_BuildingScaleYLabel;
      Label m_BuildingScaleZLabel;
      Label m_BuildingScaleXValueLabel;
      Label m_BuildingScaleYValueLabel;
      Label m_BuildingScaleZValueLabel;
      Label m_BuildingSnapToGroundLabel;

      // Projector section GUI objects
      Label m_ProjectorHeaderLabel;
      Label m_ProjectorFOVLabel;
      Label m_ProjectorAspectRatioLabel;
      Label m_ProjectorPositionLabel;
      Label m_ProjectorPositionXLabel;
      Label m_ProjectorPositionYLabel;
      Label m_ProjectorPositionZLabel;
      Label m_ProjectorPositionXValueLabel;
      Label m_ProjectorPositionYValueLabel;
      Label m_ProjectorPositionZValueLabel;
      Label m_ProjectorRotationLabel;
      Label m_ProjectorRotationXLabel;
      Label m_ProjectorRotationYLabel;
      Label m_ProjectorRotationZLabel;
      Label m_ProjectorRotationXValueLabel;
      Label m_ProjectorRotationYValueLabel;
      Label m_ProjectorRotationZValueLabel;
      Label m_ProjectorEnabledLabel;
      Label m_ProjectorEnabledValueLabel;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;

#region GUI CONSTANTS

      const int GUI_MENU_COLUMN_1_X = 5;
      const int GUI_MENU_COLUMN_1_X_TABBED = 15;
      const int GUI_MENU_COLUMN_1_WIDTH = 100;
      const int GUI_MENU_ITEM_HEIGHT = 20;

      const int GUI_APP_NUM_ROWS = 3;
      const int GUI_GIZMO_NUM_ROWS = 2;
      const int GUI_BUILDING_NUM_ROWS = 15;
      const int GUI_PROJECTOR_NUM_ROWS = 12;
      const int GUI_APP_Y = 5;
      const int GUI_GIZMO_Y = GUI_APP_Y + (GUI_MENU_ITEM_HEIGHT * GUI_APP_NUM_ROWS);
      const int GUI_BUILDING_Y = GUI_GIZMO_Y + (GUI_MENU_ITEM_HEIGHT * GUI_GIZMO_NUM_ROWS);
      const int GUI_PROJECTOR_Y = GUI_BUILDING_Y + (GUI_MENU_ITEM_HEIGHT * GUI_BUILDING_NUM_ROWS);

#endregion

      public EditorMenu(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Initialize GUI
         int menuColumn2X = (w / 2) + GUI_MENU_COLUMN_1_X;
         int menuCoordsX = 60;

         m_AppHeaderLabel = new Label("Application", GUI_MENU_COLUMN_1_X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_AppModeLabel = new Label("Mode", GUI_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_AppModeValueLabel = new Label("Editor", menuColumn2X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_GizmoHeaderLabel = new Label("Gizmo Tool", GUI_MENU_COLUMN_1_X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_GizmoHeaderValueLabel = new Label("Translate", menuColumn2X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_BuildingHeaderLabel = new Label("Building", GUI_MENU_COLUMN_1_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingFilepathLabel = new Label("Filepath", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingFilepathValueLabel = new Label("../../building.fbx", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionXLabel = new Label("X", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionYLabel = new Label("Y", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionZLabel = new Label("Z", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionXValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionYValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionZValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationXLabel = new Label("X", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationYLabel = new Label("Y", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationZLabel = new Label("Z", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 9, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationXValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationYValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationZValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 9, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleLabel = new Label("Scale", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 10, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleXLabel = new Label("X", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 11, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleYLabel = new Label("Y", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 12, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleZLabel = new Label("Z", menuCoordsX, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 13, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleXValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 11, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleYValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 12, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleZValueLabel = new Label("0.0", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 13, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingSnapToGroundLabel = new Label("", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 14, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_ProjectorHeaderLabel = new Label("Projector", GUI_MENU_COLUMN_1_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorFOVLabel = new Label("Field of View", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorAspectRatioLabel = new Label("Aspect Ratio", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionXLabel = new Label("X", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionYLabel = new Label("Y", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionZLabel = new Label("Z", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionXValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionYValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionZValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationXLabel = new Label("X", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationYLabel = new Label("Y", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 9, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationZLabel = new Label("Z", menuCoordsX, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 10, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationXValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationYValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 9, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationZValueLabel = new Label("0.0", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 10, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorEnabledLabel = new Label("Enabled", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 11, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorEnabledValueLabel = new Label("True", menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 11, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public void LoadContent(ContentManager content)
      {
         // Load textures
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");

         // Load font
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial10");
         m_AppHeaderLabel.Font = m_ArialFont;
         m_AppModeLabel.Font = m_ArialFont;
         m_AppModeValueLabel.Font = m_ArialFont;
         m_GizmoHeaderLabel.Font = m_ArialFont;
         m_GizmoHeaderValueLabel.Font = m_ArialFont;
         m_BuildingHeaderLabel.Font = m_ArialFont;
         m_BuildingFilepathLabel.Font = m_ArialFont;
         m_BuildingFilepathValueLabel.Font = m_ArialFont;
         m_BuildingPositionLabel.Font = m_ArialFont;
         m_BuildingPositionXLabel.Font = m_ArialFont;
         m_BuildingPositionYLabel.Font = m_ArialFont;
         m_BuildingPositionZLabel.Font = m_ArialFont;
         m_BuildingPositionXValueLabel.Font = m_ArialFont;
         m_BuildingPositionYValueLabel.Font = m_ArialFont;
         m_BuildingPositionZValueLabel.Font = m_ArialFont;
         m_BuildingRotationLabel.Font = m_ArialFont;
         m_BuildingRotationXLabel.Font = m_ArialFont;
         m_BuildingRotationYLabel.Font = m_ArialFont;
         m_BuildingRotationZLabel.Font = m_ArialFont;
         m_BuildingRotationXValueLabel.Font = m_ArialFont;
         m_BuildingRotationYValueLabel.Font = m_ArialFont;
         m_BuildingRotationZValueLabel.Font = m_ArialFont;
         m_BuildingScaleLabel.Font = m_ArialFont;
         m_BuildingScaleXLabel.Font = m_ArialFont;
         m_BuildingScaleYLabel.Font = m_ArialFont;
         m_BuildingScaleZLabel.Font = m_ArialFont;
         m_BuildingScaleXValueLabel.Font = m_ArialFont;
         m_BuildingScaleYValueLabel.Font = m_ArialFont;
         m_BuildingScaleZValueLabel.Font = m_ArialFont;
         m_BuildingSnapToGroundLabel.Font = m_ArialFont;
         m_ProjectorHeaderLabel.Font = m_ArialFont;
         m_ProjectorFOVLabel.Font = m_ArialFont;
         m_ProjectorAspectRatioLabel.Font = m_ArialFont;
         m_ProjectorPositionLabel.Font = m_ArialFont;
         m_ProjectorPositionXLabel.Font = m_ArialFont;
         m_ProjectorPositionYLabel.Font = m_ArialFont;
         m_ProjectorPositionZLabel.Font = m_ArialFont;
         m_ProjectorPositionXValueLabel.Font = m_ArialFont;
         m_ProjectorPositionYValueLabel.Font = m_ArialFont;
         m_ProjectorPositionZValueLabel.Font = m_ArialFont;
         m_ProjectorRotationLabel.Font = m_ArialFont;
         m_ProjectorRotationXLabel.Font = m_ArialFont;
         m_ProjectorRotationYLabel.Font = m_ArialFont;
         m_ProjectorRotationZLabel.Font = m_ArialFont;
         m_ProjectorRotationXValueLabel.Font = m_ArialFont;
         m_ProjectorRotationYValueLabel.Font = m_ArialFont;
         m_ProjectorRotationZValueLabel.Font = m_ArialFont;
         m_ProjectorEnabledLabel.Font = m_ArialFont;
         m_ProjectorEnabledValueLabel.Font = m_ArialFont;
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
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, m_Viewport.Width, m_Viewport.Height), Color.Gray);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, 2, m_Viewport.Height), Color.Black);
         RenderApplicationSection(spriteBatch);
         //spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, m_Viewport.Width, 2), Color.Black);
         RenderGizmoSection(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_GIZMO_Y - 2, m_Viewport.Width, 2), Color.Black);
         RenderBuildingSection(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_BUILDING_Y - 2, m_Viewport.Width, 2), Color.Black);
         RenderProjectorSection(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_PROJECTOR_Y - 2, m_Viewport.Width, 2), Color.Black);
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
         m_GizmoHeaderValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderBuildingSection(SpriteBatch spriteBatch)
      {
         m_BuildingHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionXValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionYValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionZValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationXValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationYValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationZValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleXValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleYValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleZValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingSnapToGroundLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderProjectorSection(SpriteBatch spriteBatch)
      {
         m_ProjectorHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorFOVLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAspectRatioLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionXValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionYValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionZValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationXValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationYValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationZValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorEnabledLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorEnabledValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      #endregion

      #region Public Access TV

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      public string EditorMode
      {
         set { m_AppModeValueLabel.Text = value; }
      }

      public Vector3 BuildingPosition
      {
         set
         {
            m_BuildingPositionXValueLabel.Text = value.X.ToString();
            m_BuildingPositionYValueLabel.Text = value.Y.ToString();
            m_BuildingPositionZValueLabel.Text = value.Z.ToString();
         }
      }

      public Vector3 BuildingRotation
      {
         set
         {
            m_BuildingRotationXValueLabel.Text = MathHelper.ToDegrees(value.X).ToString();
            m_BuildingRotationYValueLabel.Text = MathHelper.ToDegrees(value.Y).ToString();
            m_BuildingRotationZValueLabel.Text = MathHelper.ToDegrees(value.Z).ToString();
         }
      }

      public Vector3 BuildingScale
      {
         set
         {
            m_BuildingScaleXValueLabel.Text = value.X.ToString();
            m_BuildingScaleYValueLabel.Text = value.Y.ToString();
            m_BuildingScaleZValueLabel.Text = value.Z.ToString();
         }
      }

      public string ProjectorEnabled
      {
         set { m_ProjectorEnabledValueLabel.Text = value; }
      }

      public Vector3 ProjectorPosition
      {
         set
         {
            m_ProjectorPositionXValueLabel.Text = value.X.ToString();
            m_ProjectorPositionYValueLabel.Text = value.Y.ToString();
            m_ProjectorPositionZValueLabel.Text = value.Z.ToString();
         }
      }

      public Vector3 ProjectorRotation
      {
         set
         {
            m_ProjectorRotationXValueLabel.Text = MathHelper.ToDegrees(value.X).ToString();
            m_ProjectorRotationYValueLabel.Text = MathHelper.ToDegrees(value.Y).ToString();
            m_ProjectorRotationZValueLabel.Text = MathHelper.ToDegrees(value.Z).ToString();
         }
      }

      public string GizmoToolSelected
      {
         set { m_GizmoHeaderValueLabel.Text = value; }
      }

      #endregion

   }
}
