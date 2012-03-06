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
using ProjectionMappingGame.GUI;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   public class ProjectionEditorState : GameState
   {
      // Rendering
      GraphicsDevice m_GraphicsDevice;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;
      MouseInput m_MouseInput;
      MouseInput m_RightMenuMouseInput;
      
      // States
      bool m_DEM_GAMES;

      // Editor panes
      ProjectionPreviewComponent m_ProjectorPreview;
      UVGridEditor m_UVGridEditor;
      UVPointEditor m_UVEditor;
      int m_FocusedPane;
      bool m_EditorMode;
      Panel m_GridEditorPanel;
      Panel m_UVEditorPanel;
      Panel m_ProjectionEditorPanel;

      // Fonts
      SpriteFont m_ArialFont;
      SpriteFont m_ArialFont10;

      // Textures
      Texture2D m_ButtonTexture, m_ButtonTextureOnHover, m_ButtonTextureOnPress;
      Texture2D m_SquareButtonTexture, m_SquareButtonTextureOnHover, m_SquareButtonTextureOnPress;
      Texture2D m_TranslateButtonTexture, m_TranslateButtonTextureOnHover, m_TranslateButtonTextureOnPress;
      Texture2D m_RotateButtonTexture, m_RotateButtonTextureOnHover, m_RotateButtonTextureOnPress;
      Texture2D m_ScaleButtonTexture, m_ScaleButtonTextureOnHover, m_ScaleButtonTextureOnPress;
      Texture2D m_NewTextureLayerTexture, m_NewTextureLayerTextureOnHover, m_NewTextureLayerTextureOnPress;
      Texture2D m_NewParticleLayerTexture, m_NewParticleLayerTextureOnHover, m_NewParticleLayerTextureOnPress;
      Texture2D m_ShiftLayerUpTexture, m_ShiftLayerUpTextureOnHover, m_ShiftLayerUpTextureOnPress;
      Texture2D m_ShiftLayerDownTexture, m_ShiftLayerDownTextureOnHover, m_ShiftLayerDownTextureOnPress;
      Texture2D m_TrashTexture, m_TrashTextureOnHover, m_TrashTextureOnPress;
      Texture2D m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_SpinBoxFillTexture;
      Texture2D m_ScrollViewBackgroundTexture, m_ScrollViewScrollPadTexture, m_ScrollViewScrollAreaTexture;
      Texture2D m_SliderBarTexture, m_SliderPadTexture;
      Texture2D m_WhiteTexture;
      Texture2D m_VisibleButtonTexture, m_VisibleButtonTextureOnHover, m_VisibleButtonTextureOnPress;
      Texture2D m_EditLayerButtonTexture, m_EditLayerButtonTextureOnHover, m_EditLayerButtonTextureOnPress;

      #region Left Menu

      Label m_SelectedFaceHeaderLabel;
      Label m_SelectedFaceTexCoordLabel;
      Label m_SelectedFaceP0Label;
      Label m_SelectedFaceP1Label;
      Label m_SelectedFaceP2Label;
      Label m_SelectedFaceP3Label;
      Label m_SelectedFaceP0NoneLabel;
      Label m_SelectedFaceP1NoneLabel;
      Label m_SelectedFaceP2NoneLabel;
      Label m_SelectedFaceP3NoneLabel;
      UVSpinBoxPair[] m_SelectedFaceUVSpinBox;

      const int GUI_SELECTED_QUAD_Y = 2;

      #endregion

      #region Right Menu

      Viewport m_RightMenuViewport;

      // Application section GUI objects
      Label m_AppHeaderLabel;
      Label m_AppModeLabel;
      Label m_AppModeValueLabel;
      Button m_ModeButton;
      Button m_PlayButton;
      Button m_QuitButton;
      Button m_ResetButton;

      // Textures
      Texture2D m_ColorPickerPreviewPadTexture;

      // Gizmo section GUI objects
      Label m_GizmoHeaderLabel;
      Label m_GizmoHeaderValueLabel;
      Button m_TranslateButton;
      Button m_RotateButton;
      Button m_ScaleButton;

      // Building section GUI objects
      ColorPickerComponent m_BuildingColorPicker;
      Label m_BuildingHeaderLabel;
      Label m_BuildingFilepathLabel;
      Label m_BuildingFilepathValueLabel;
      Label m_BuildingPositionLabel;
      Label m_BuildingPositionXLabel;
      Label m_BuildingPositionYLabel;
      Label m_BuildingPositionZLabel;
      NumUpDown m_BuildingPositionXSpinBox;
      NumUpDown m_BuildingPositionYSpinBox;
      NumUpDown m_BuildingPositionZSpinBox;
      Label m_BuildingRotationLabel;
      Label m_BuildingRotationXLabel;
      Label m_BuildingRotationYLabel;
      Label m_BuildingRotationZLabel;
      NumUpDown m_BuildingRotationXSpinBox;
      NumUpDown m_BuildingRotationYSpinBox;
      NumUpDown m_BuildingRotationZSpinBox;
      Label m_BuildingScaleLabel;
      Label m_BuildingScaleXLabel;
      Label m_BuildingScaleYLabel;
      Label m_BuildingScaleZLabel;
      NumUpDown m_BuildingScaleXSpinBox;
      NumUpDown m_BuildingScaleYSpinBox;
      NumUpDown m_BuildingScaleZSpinBox;
      Label m_BuildingSnapToGroundLabel;
      Label m_BuildingColorLabel;
      Button m_BuildingColorButton;

      // Projector section GUI objects
      Label m_ProjectorHeaderLabel;
      Label m_ProjectorFOVLabel;
      Label m_ProjectorAspectRatioLabel;
      NumUpDown m_ProjectorFovSpinBox;
      NumUpDown m_ProjectorAspectRatioSpinBox;
      Label m_ProjectorAlphaLabel;
      Slider m_ProjectorAlphaSlider;
      Label m_ProjectorAlphaValueLabel;
      Label m_ProjectorPositionLabel;
      Label m_ProjectorPositionXLabel;
      Label m_ProjectorPositionYLabel;
      Label m_ProjectorPositionZLabel;
      NumUpDown m_ProjectorPositionXSpinBox;
      NumUpDown m_ProjectorPositionYSpinBox;
      NumUpDown m_ProjectorPositionZSpinBox;
      Label m_ProjectorRotationLabel;
      Label m_ProjectorRotationXLabel;
      Label m_ProjectorRotationYLabel;
      Label m_ProjectorRotationZLabel;
      NumUpDown m_ProjectorRotationXSpinBox;
      NumUpDown m_ProjectorRotationYSpinBox;
      NumUpDown m_ProjectorRotationZSpinBox;
      Button m_ProjectorEnabledButton;

      // Layers section GUI objects
      Label m_LayersHeaderLabel;
      LayerScrollView m_LayersScrollView;
      Button m_NewTextureLayerButton;
      Button m_NewParticleLayerButton;
      Button m_ShiftLayerUpButton;
      Button m_ShiftLayerDownButton;
      Button m_TrashLayerButton;

      const int GUI_APP_NUM_ROWS = 3;
      const int GUI_GIZMO_NUM_ROWS = 2;
      const int GUI_BUILDING_NUM_ROWS = 10;
      const int GUI_PROJECTOR_NUM_ROWS = 10;
      const int GUI_APP_Y = 2;
      const int GUI_GIZMO_Y = GUI_APP_Y + (GUI_MENU_ITEM_HEIGHT * GUI_APP_NUM_ROWS) + 16;
      const int GUI_BUILDING_Y = GUI_GIZMO_Y + (GUI_MENU_ITEM_HEIGHT * GUI_GIZMO_NUM_ROWS) + 10;
      const int GUI_PROJECTOR_Y = GUI_BUILDING_Y + (GUI_MENU_ITEM_HEIGHT * GUI_BUILDING_NUM_ROWS);
      const int GUI_LAYERS_Y = GUI_PROJECTOR_Y + (GUI_MENU_ITEM_HEIGHT * GUI_PROJECTOR_NUM_ROWS);

      const int GUI_BUILDING_COLOR_PICKER_X = 75;
      const int GUI_BUILDING_COLOR_PICKER_Y = GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4;
      const int GUI_BUILDING_COLOR_PICKER_WIDTH = 100;
      const int GUI_BUILDING_COLOR_PICKER_HEIGHT = 100;
      const int GUI_COLOR_BUTTON_WIDTH = 24;
      const int GUI_COLOR_BUTTON_HEIGHT = 24;


      #endregion

      #region GUI CONSTANTS

      const int GUI_LEFT_MENU_COLUMN_1_X = GUI_LEFT_TOOLBAR_X + 5;
      const int GUI_LEFT_MENU_COLUMN_1_X_TABBED = GUI_LEFT_TOOLBAR_X + 15;
      const int GUI_LEFT_MENU_COLUMN_1_WIDTH = 50;
      const int GUI_MENU_COLUMN_1_X = 5;
      const int GUI_MENU_COLUMN_1_X_TABBED = 15;
      const int GUI_MENU_COLUMN_1_WIDTH = 100;
      const int GUI_MENU_ITEM_HEIGHT = 20;
      const int GUI_MENU_X_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5;
      const int GUI_MENU_Y_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5 + (GUI_SPINBOX_WIDTH + 12);
      const int GUI_MENU_Z_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5 + (GUI_SPINBOX_WIDTH + 12) * 2;

      const int GUI_QUIT_BUTTON_WIDTH = 80;
      const int GUI_QUIT_BUTTON_HEIGHT = 24;
      const int GUI_GIZMO_BUTTON_WIDTH = 24;
      const int GUI_GIZMO_BUTTON_HEIGHT = 24;
      const int GUI_SPINBOX_WIDTH = 50;
      const int GUI_SPINBOX_HEIGHT = 20;

      // Main viewports
      const int GUI_UV_GRID_EDITOR_X = GUI_LEFT_TOOLBAR_WIDTH;
      const int GUI_UV_GRID_EDITOR_Y = 0;
      const int GUI_UV_GRID_EDITOR_WIDTH = GUI_UV_GRID_EDITOR_HEIGHT;
      const int GUI_UV_GRID_EDITOR_HEIGHT = (GameConstants.DEFAULT_WINDOW_HEIGHT / 2);
      const int GUI_UV_EDITOR_X = GUI_LEFT_TOOLBAR_WIDTH;
      const int GUI_UV_EDITOR_Y = GUI_UV_GRID_EDITOR_HEIGHT;
      const int GUI_UV_EDITOR_WIDTH = GUI_UV_GRID_EDITOR_WIDTH;
      const int GUI_UV_EDITOR_HEIGHT = (GameConstants.DEFAULT_WINDOW_HEIGHT / 2);
      const int GUI_PROJECTOR_PREVIEW_X = GUI_UV_EDITOR_X + GUI_UV_GRID_EDITOR_WIDTH;
      const int GUI_PROJECTOR_PREVIEW_Y = 0;
      const int GUI_PROJECTOR_PREVIEW_WIDTH = GameConstants.DEFAULT_WINDOW_WIDTH - GUI_UV_GRID_EDITOR_WIDTH - GUI_TOOLBAR_WIDTH - GUI_UV_EDITOR_X;
      const int GUI_PROJECTOR_PREVIEW_HEIGHT = GameConstants.DEFAULT_WINDOW_HEIGHT;
      const int GUI_LEFT_TOOLBAR_X = 0;
      const int GUI_LEFT_TOOLBAR_Y = 0;
      const int GUI_LEFT_TOOLBAR_WIDTH = 175;
      const int GUI_LEFT_TOOLBAR_HEIGHT = GameConstants.DEFAULT_WINDOW_HEIGHT;
      const int GUI_TOOLBAR_X = GUI_PROJECTOR_PREVIEW_X + GUI_PROJECTOR_PREVIEW_WIDTH;
      const int GUI_TOOLBAR_Y = 0;
      const int GUI_TOOLBAR_WIDTH = 210;
      const int GUI_TOOLBAR_HEIGHT = GameConstants.DEFAULT_WINDOW_HEIGHT;

      #endregion

      public ProjectionEditorState(GameDriver game)
         : base(game, StateType.ProjectionEditor)
      {
         m_GraphicsDevice = game.GraphicsDevice;

         // Initialize input
         m_MouseInput = new MouseInput();
         m_RightMenuMouseInput = new MouseInput(new Vector2(-GUI_TOOLBAR_X, -GUI_TOOLBAR_Y));
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();

         // Initialize editor panes
         m_UVGridEditor = new UVGridEditor(m_Game, GUI_UV_GRID_EDITOR_X + 2, GUI_UV_GRID_EDITOR_Y + 20, GUI_UV_GRID_EDITOR_WIDTH - 2, GUI_UV_GRID_EDITOR_HEIGHT - 20);
         m_UVEditor = new UVPointEditor(m_Game, GUI_UV_EDITOR_X + 2, GUI_UV_EDITOR_Y + 20, GUI_UV_EDITOR_WIDTH - 2, GUI_UV_EDITOR_HEIGHT - 20);
         m_UVEditor.RegisterQuadSelectedEvent(Quad_OnSelected);
         m_ProjectorPreview = new ProjectionPreviewComponent(m_Game, GUI_PROJECTOR_PREVIEW_X + 2, GUI_PROJECTOR_PREVIEW_Y + 20, GUI_PROJECTOR_PREVIEW_WIDTH - 2, GUI_PROJECTOR_PREVIEW_HEIGHT - 20);
         m_FocusedPane = 0;
         m_EditorMode = true;
         m_DEM_GAMES = false;
         m_RightMenuViewport = new Viewport(GUI_TOOLBAR_X, GUI_TOOLBAR_Y, GUI_TOOLBAR_WIDTH, GUI_TOOLBAR_HEIGHT);

         // Initialize GUI
         m_SelectedFaceHeaderLabel = new Label("Selected Face", GUI_LEFT_MENU_COLUMN_1_X, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceTexCoordLabel = new Label("Texture Coordinates", GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 1, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP0Label = new Label("v1", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 2, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP1Label = new Label("v2", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 3, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP2Label = new Label("v3", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 4, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP3Label = new Label("v4", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 5, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP0NoneLabel = new Label("N/A", GUI_LEFT_TOOLBAR_X + 54, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 2, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP1NoneLabel = new Label("N/A", GUI_LEFT_TOOLBAR_X + 54, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 3, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP2NoneLabel = new Label("N/A", GUI_LEFT_TOOLBAR_X + 54, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 4, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_SelectedFaceP3NoneLabel = new Label("N/A", GUI_LEFT_TOOLBAR_X + 54, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 5, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         int menuColumn2X = (GUI_TOOLBAR_WIDTH / 2) + GUI_MENU_COLUMN_1_X;

         m_AppHeaderLabel = new Label("Application", GUI_MENU_COLUMN_1_X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_AppModeLabel = new Label("Mode", GUI_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_AppModeValueLabel = new Label("Editor", menuColumn2X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_GizmoHeaderLabel = new Label("Gizmo Tool", GUI_MENU_COLUMN_1_X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_GizmoHeaderValueLabel = new Label("Translate", menuColumn2X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_BuildingHeaderLabel = new Label("Building", GUI_MENU_COLUMN_1_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingFilepathLabel = new Label("Filepath", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingFilepathValueLabel = new Label("../../building.fbx", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingPositionZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingRotationZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleLabel = new Label("Scale", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingScaleZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingSnapToGroundLabel = new Label("", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_BuildingColorLabel = new Label("Color", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);

         m_ProjectorHeaderLabel = new Label("Projector", GUI_MENU_COLUMN_1_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorFOVLabel = new Label("Field of View", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorAspectRatioLabel = new Label("Aspect Ratio", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorPositionZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorRotationZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorAlphaLabel = new Label("Alpha", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         m_ProjectorAlphaValueLabel = new Label("1.00", GUI_TOOLBAR_WIDTH - 32, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
         
         m_LayersHeaderLabel = new Label("Input Layers", GUI_MENU_COLUMN_1_X, GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black);
      }

      public override void Reset()
      {
         m_UVEditor.Reset();
         m_UVGridEditor.Reset();
         m_ProjectorPreview.Reset();

         m_LayersScrollView.Clear();

         // Create the default gameplay layer
         Label defaultGameplayLabel = new Label("Gameplay", Layer.NAME_X, m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.LAYER_WIDTH, Layer.LAYER_HEIGHT, Color.Black);
         defaultGameplayLabel.Font = m_ArialFont10;
         Button visibleBtn = new Button(new Rectangle(Layer.VISIBILITY_BUTTON_X, m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.VISIBILITY_BUTTON_WIDTH, Layer.VISIBILITY_BUTTON_HEIGHT), m_VisibleButtonTexture, m_MouseInput);
         visibleBtn.SetImage(Button.ImageType.OVER, m_VisibleButtonTextureOnHover);
         visibleBtn.SetImage(Button.ImageType.CLICK, m_VisibleButtonTextureOnPress);
         Button editBtn = new Button(new Rectangle(Layer.EDIT_BUTTON_X, m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.EDIT_BUTTON_WIDTH, Layer.EDIT_BUTTON_HEIGHT), m_EditLayerButtonTexture, m_MouseInput);
         editBtn.SetImage(Button.ImageType.OVER, m_EditLayerButtonTextureOnHover);
         editBtn.SetImage(Button.ImageType.CLICK, m_EditLayerButtonTextureOnPress);
         m_LayersScrollView.AddLayer(new GameplayLayer(defaultGameplayLabel, visibleBtn, editBtn));

         m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
      }

      public override void Resize(int dx, int dy)
      {
         m_RightMenuViewport.X += dx;
         m_RightMenuViewport.Height += dy;
         m_RightMenuMouseInput.Offset = new Vector2(-m_RightMenuViewport.X, 0);

         Viewport v = m_BuildingColorPicker.Viewport;
         v.X = m_RightMenuViewport.X + GUI_BUILDING_COLOR_PICKER_X;
         m_BuildingColorPicker.Viewport = v;

         m_ProjectionEditorPanel.Width += dx;
         m_ProjectionEditorPanel.Height += dy;
         m_ProjectorPreview.Resize(dx, dy);
      }

      public override void LoadContent(ContentManager content)
      {
         #region FONTS
         // Load font
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
         m_ArialFont10 = content.Load<SpriteFont>("Fonts/Arial10");
         m_AppHeaderLabel.Font = m_ArialFont10;
         m_AppModeLabel.Font = m_ArialFont10;
         m_AppModeValueLabel.Font = m_ArialFont10;
         m_GizmoHeaderLabel.Font = m_ArialFont10;
         m_GizmoHeaderValueLabel.Font = m_ArialFont10;
         m_BuildingHeaderLabel.Font = m_ArialFont10;
         m_BuildingFilepathLabel.Font = m_ArialFont10;
         m_BuildingFilepathValueLabel.Font = m_ArialFont10;
         m_BuildingPositionLabel.Font = m_ArialFont10;
         m_BuildingPositionXLabel.Font = m_ArialFont10;
         m_BuildingPositionYLabel.Font = m_ArialFont10;
         m_BuildingPositionZLabel.Font = m_ArialFont10;
         m_BuildingRotationLabel.Font = m_ArialFont10;
         m_BuildingRotationXLabel.Font = m_ArialFont10;
         m_BuildingRotationYLabel.Font = m_ArialFont10;
         m_BuildingRotationZLabel.Font = m_ArialFont10;
         m_BuildingScaleLabel.Font = m_ArialFont10;
         m_BuildingScaleXLabel.Font = m_ArialFont10;
         m_BuildingScaleYLabel.Font = m_ArialFont10;
         m_BuildingScaleZLabel.Font = m_ArialFont10;
         m_BuildingSnapToGroundLabel.Font = m_ArialFont10;
         m_BuildingColorLabel.Font = m_ArialFont10;
         m_ProjectorHeaderLabel.Font = m_ArialFont10;
         m_ProjectorFOVLabel.Font = m_ArialFont10;
         m_ProjectorAspectRatioLabel.Font = m_ArialFont10;
         m_ProjectorPositionLabel.Font = m_ArialFont10;
         m_ProjectorPositionXLabel.Font = m_ArialFont10;
         m_ProjectorPositionYLabel.Font = m_ArialFont10;
         m_ProjectorPositionZLabel.Font = m_ArialFont10;
         m_ProjectorRotationLabel.Font = m_ArialFont10;
         m_ProjectorRotationXLabel.Font = m_ArialFont10;
         m_ProjectorRotationYLabel.Font = m_ArialFont10;
         m_ProjectorRotationZLabel.Font = m_ArialFont10;
         m_LayersHeaderLabel.Font = m_ArialFont10;
         m_ProjectorAlphaLabel.Font = m_ArialFont10;
         m_ProjectorAlphaValueLabel.Font = m_ArialFont10;
         m_SelectedFaceHeaderLabel.Font = m_ArialFont10;
         m_SelectedFaceTexCoordLabel.Font = m_ArialFont10;
         m_SelectedFaceP0Label.Font = m_ArialFont10;
         m_SelectedFaceP1Label.Font = m_ArialFont10;
         m_SelectedFaceP2Label.Font = m_ArialFont10;
         m_SelectedFaceP3Label.Font = m_ArialFont10;
         m_SelectedFaceP0NoneLabel.Font = m_ArialFont10;
         m_SelectedFaceP1NoneLabel.Font = m_ArialFont10;
         m_SelectedFaceP2NoneLabel.Font = m_ArialFont10;
         m_SelectedFaceP3NoneLabel.Font = m_ArialFont10;
         #endregion

         #region TEXTURES

         // Load textures
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");
         m_ButtonTexture = content.Load<Texture2D>("Textures/GUI/button");
         m_ButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/button_on_hover");
         m_ButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/button_on_hover");
         m_TranslateButtonTexture = content.Load<Texture2D>("Textures/GUI/move");
         m_TranslateButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/move_on_hover");
         m_TranslateButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/move_on_hover");
         m_ScaleButtonTexture = content.Load<Texture2D>("Textures/GUI/scale");
         m_ScaleButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/scale_on_hover");
         m_ScaleButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/scale_on_hover");
         m_RotateButtonTexture = content.Load<Texture2D>("Textures/GUI/rotate");
         m_RotateButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/rotate_on_hover");
         m_RotateButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/rotate_on_hover");
         m_SpinBoxFillTexture = content.Load<Texture2D>("Textures/GUI/spinbox_fill");
         m_SpinBoxUpTexture = content.Load<Texture2D>("Textures/GUI/spinbox_up");
         m_SpinBoxDownTexture = content.Load<Texture2D>("Textures/GUI/spinbox_down");
         m_SquareButtonTexture = content.Load<Texture2D>("Textures/GUI/square_button");
         m_SquareButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/square_button_on_hover");
         m_SquareButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/square_button_on_hover");
         m_ColorPickerPreviewPadTexture = content.Load<Texture2D>("Textures/GUI/color_picker_preview");
         m_ScrollViewBackgroundTexture = content.Load<Texture2D>("Textures/GUI/scroll_view_fill");
         m_ScrollViewScrollPadTexture = content.Load<Texture2D>("Textures/GUI/scroll_pad");
         m_ScrollViewScrollAreaTexture = content.Load<Texture2D>("Textures/GUI/scroll_area_fill");
         m_NewTextureLayerTexture = content.Load<Texture2D>("Textures/GUI/new_texture_button");
         m_NewTextureLayerTextureOnHover = content.Load<Texture2D>("Textures/GUI/new_texture_button_on_hover");
         m_NewTextureLayerTextureOnPress = content.Load<Texture2D>("Textures/GUI/new_texture_button_on_hover");
         m_NewParticleLayerTexture = content.Load<Texture2D>("Textures/GUI/new_particle_button");
         m_NewParticleLayerTextureOnHover = content.Load<Texture2D>("Textures/GUI/new_particle_button_on_hover");
         m_NewParticleLayerTextureOnPress = content.Load<Texture2D>("Textures/GUI/new_particle_button_on_hover");
         m_ShiftLayerUpTexture = content.Load<Texture2D>("Textures/GUI/shift_layer_up_button");
         m_ShiftLayerUpTextureOnHover = content.Load<Texture2D>("Textures/GUI/shift_layer_up_button_on_hover");
         m_ShiftLayerUpTextureOnPress = content.Load<Texture2D>("Textures/GUI/shift_layer_up_button_on_hover");
         m_ShiftLayerDownTexture = content.Load<Texture2D>("Textures/GUI/shift_layer_down_button");
         m_ShiftLayerDownTextureOnHover = content.Load<Texture2D>("Textures/GUI/shift_layer_down_button_on_hover");
         m_ShiftLayerDownTextureOnPress = content.Load<Texture2D>("Textures/GUI/shift_layer_down_button_on_hover");
         m_TrashTexture = content.Load<Texture2D>("Textures/GUI/trash_button");
         m_TrashTextureOnHover = content.Load<Texture2D>("Textures/GUI/trash_button_on_hover");
         m_TrashTextureOnPress = content.Load<Texture2D>("Textures/GUI/trash_button_on_hover");
         m_SliderBarTexture = content.Load<Texture2D>("Textures/GUI/slider_bar");
         m_SliderPadTexture = content.Load<Texture2D>("Textures/GUI/slider_pad");
         m_VisibleButtonTexture = content.Load<Texture2D>("Textures/GUI/visible_layer_button");
         m_VisibleButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/visible_layer_button_on_hover");
         m_VisibleButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/visible_layer_button_on_hover");
         m_EditLayerButtonTexture = content.Load<Texture2D>("Textures/GUI/edit_layer_button");
         m_EditLayerButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/edit_layer_button_on_hover");
         m_EditLayerButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/edit_layer_button_on_hover");

         #endregion

         // Load layers scrollview
         m_LayersScrollView = new LayerScrollView(new Rectangle(2, GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 1 - 22, GUI_TOOLBAR_WIDTH - 2, 175),
            new Rectangle(),
            m_ScrollViewBackgroundTexture,
            m_ScrollViewScrollPadTexture,
            m_ScrollViewScrollAreaTexture,
            m_MouseInput
         );

         // Load the projector preview's content
         m_UVGridEditor.LoadContent(content);
         m_UVEditor.LoadContent(content);
         m_ProjectorPreview.LoadContent(content);

         // Load initial points
         m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
         
         // Initialize panels
         m_GridEditorPanel = new Panel(new Rectangle(GUI_UV_GRID_EDITOR_X, GUI_UV_GRID_EDITOR_Y, GUI_UV_GRID_EDITOR_WIDTH, GUI_UV_GRID_EDITOR_HEIGHT), "UV Grid Editor", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.White, Color.Black);
         m_UVEditorPanel = new Panel(new Rectangle(GUI_UV_EDITOR_X, GUI_UV_EDITOR_Y, GUI_UV_EDITOR_WIDTH, GUI_UV_EDITOR_HEIGHT), "UV Editor", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.White, Color.Black);
         m_ProjectionEditorPanel = new Panel(new Rectangle(GUI_PROJECTOR_PREVIEW_X, GUI_PROJECTOR_PREVIEW_Y, GUI_PROJECTOR_PREVIEW_WIDTH, GUI_PROJECTOR_PREVIEW_HEIGHT), "Projection Preview", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.CornflowerBlue, Color.Black);

         int menuColumn2X = (GUI_TOOLBAR_WIDTH / 2) + GUI_MENU_COLUMN_1_X;

         #region SPINBOXES

         // Initialize spinboxes
         m_BuildingPositionXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingPositionYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingPositionZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorRotationXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorRotationYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorRotationZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorFovSpinBox = new NumUpDown(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_SPINBOX_WIDTH - 6, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 1 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 1.0f, 90.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorAspectRatioSpinBox = new NumUpDown(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_SPINBOX_WIDTH - 6, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 2 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.1f, 10.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         
         m_BuildingPositionXSpinBox.RegisterOnValueChanged(BuildingPositionX_OnValueChanged);
         m_BuildingPositionYSpinBox.RegisterOnValueChanged(BuildingPositionY_OnValueChanged);
         m_BuildingPositionZSpinBox.RegisterOnValueChanged(BuildingPositionZ_OnValueChanged);
         m_BuildingRotationXSpinBox.RegisterOnValueChanged(BuildingRotationX_OnValueChanged);
         m_BuildingRotationYSpinBox.RegisterOnValueChanged(BuildingRotationY_OnValueChanged);
         m_BuildingRotationZSpinBox.RegisterOnValueChanged(BuildingRotationZ_OnValueChanged);
         m_BuildingScaleXSpinBox.RegisterOnValueChanged(BuildingScaleX_OnValueChanged);
         m_BuildingScaleYSpinBox.RegisterOnValueChanged(BuildingScaleY_OnValueChanged);
         m_BuildingScaleZSpinBox.RegisterOnValueChanged(BuildingScaleZ_OnValueChanged);
         m_ProjectorPositionXSpinBox.RegisterOnValueChanged(ProjectorPositionX_OnValueChanged);
         m_ProjectorPositionYSpinBox.RegisterOnValueChanged(ProjectorPositionY_OnValueChanged);
         m_ProjectorPositionZSpinBox.RegisterOnValueChanged(ProjectorPositionZ_OnValueChanged);
         m_ProjectorRotationXSpinBox.RegisterOnValueChanged(ProjectorRotationX_OnValueChanged);
         m_ProjectorRotationYSpinBox.RegisterOnValueChanged(ProjectorRotationY_OnValueChanged);
         m_ProjectorRotationZSpinBox.RegisterOnValueChanged(ProjectorRotationZ_OnValueChanged);
         m_ProjectorFovSpinBox.RegisterOnValueChanged(ProjectorFOV_OnValueChanged);
         m_ProjectorAspectRatioSpinBox.RegisterOnValueChanged(ProjectorAspectRatio_OnValueChanged);

         #endregion

         #region BUTTONS

         // Initialize buttons
         int toolbuttonpadding = 2;
         m_ModeButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Preview", Color.Black);
         m_ModeButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ModeButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ModeButton.RegisterOnClick(ModeButton_OnClick);
         m_PlayButton = new Button(new Rectangle(menuColumn2X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Play", Color.Black);
         m_PlayButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_PlayButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_PlayButton.RegisterOnClick(PlayButton_OnClick);
         m_ResetButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + 6 + GUI_MENU_ITEM_HEIGHT * 2, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Reset", Color.Black);
         m_ResetButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ResetButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ResetButton.RegisterOnClick(ResetButton_OnClick);
         m_QuitButton = new Button(new Rectangle(menuColumn2X, GUI_APP_Y + 6 + GUI_MENU_ITEM_HEIGHT * 2, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Quit", Color.Black);
         m_QuitButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_QuitButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_QuitButton.RegisterOnClick(QuitButton_OnClick);
         m_TranslateButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_GIZMO_BUTTON_WIDTH, GUI_GIZMO_BUTTON_HEIGHT), m_TranslateButtonTexture, m_RightMenuMouseInput);
         m_TranslateButton.SetImage(Button.ImageType.OVER, m_TranslateButtonTextureOnHover);
         m_TranslateButton.SetImage(Button.ImageType.CLICK, m_TranslateButtonTextureOnPress);
         m_TranslateButton.RegisterOnClick(TranslateButton_OnClick);
         m_RotateButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED + toolbuttonpadding + GUI_GIZMO_BUTTON_WIDTH, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_GIZMO_BUTTON_WIDTH, GUI_GIZMO_BUTTON_HEIGHT), m_RotateButtonTexture, m_RightMenuMouseInput);
         m_RotateButton.SetImage(Button.ImageType.OVER, m_RotateButtonTextureOnHover);
         m_RotateButton.SetImage(Button.ImageType.CLICK, m_RotateButtonTextureOnPress);
         m_RotateButton.RegisterOnClick(RotateButton_OnClick);
         m_ScaleButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED + toolbuttonpadding * 2 + GUI_GIZMO_BUTTON_WIDTH * 2, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_GIZMO_BUTTON_WIDTH, GUI_GIZMO_BUTTON_HEIGHT), m_ScaleButtonTexture, m_RightMenuMouseInput);
         m_ScaleButton.SetImage(Button.ImageType.OVER, m_ScaleButtonTextureOnHover);
         m_ScaleButton.SetImage(Button.ImageType.CLICK, m_ScaleButtonTextureOnPress);
         m_ScaleButton.RegisterOnClick(ScaleButton_OnClick);
         m_ProjectorEnabledButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Turn Off", Color.Black);
         m_ProjectorEnabledButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ProjectorEnabledButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ProjectorEnabledButton.RegisterOnClick(ProjectorEnabledButton_OnClick);

         m_BuildingColorButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_COLOR_BUTTON_WIDTH - 4, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, GUI_COLOR_BUTTON_WIDTH, GUI_COLOR_BUTTON_HEIGHT), m_SquareButtonTexture, m_RightMenuMouseInput);
         m_BuildingColorButton.SetImage(Button.ImageType.OVER, m_SquareButtonTextureOnHover);
         m_BuildingColorButton.SetImage(Button.ImageType.CLICK, m_SquareButtonTextureOnPress);
         m_BuildingColorButton.RegisterOnClick(BuildingColorButton_OnClick);

         int btnWidth = 20;
         m_NewTextureLayerButton = new Button(new Rectangle(2 + btnWidth * 0, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_NewTextureLayerTexture, m_RightMenuMouseInput);
         m_NewTextureLayerButton.SetImage(Button.ImageType.OVER, m_NewTextureLayerTextureOnHover);
         m_NewTextureLayerButton.SetImage(Button.ImageType.CLICK, m_NewTextureLayerTextureOnPress);
         m_NewTextureLayerButton.RegisterOnClick(NewTextureLayerButton_OnClick);
         m_NewParticleLayerButton = new Button(new Rectangle(2 + btnWidth * 1, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_NewParticleLayerTexture, m_RightMenuMouseInput);
         m_NewParticleLayerButton.SetImage(Button.ImageType.OVER, m_NewParticleLayerTextureOnHover);
         m_NewParticleLayerButton.SetImage(Button.ImageType.CLICK, m_NewParticleLayerTextureOnPress);
         m_NewParticleLayerButton.RegisterOnClick(NewParticleLayerButton_OnClick);
         m_ShiftLayerUpButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - btnWidth * 3, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_ShiftLayerUpTexture, m_RightMenuMouseInput);
         m_ShiftLayerUpButton.SetImage(Button.ImageType.OVER, m_ShiftLayerUpTextureOnHover);
         m_ShiftLayerUpButton.SetImage(Button.ImageType.CLICK, m_ShiftLayerUpTextureOnPress);
         m_ShiftLayerUpButton.RegisterOnClick(ShiftLayerUpButton_OnClick);
         m_ShiftLayerDownButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - btnWidth * 2, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_ShiftLayerDownTexture, m_RightMenuMouseInput);
         m_ShiftLayerDownButton.SetImage(Button.ImageType.OVER, m_ShiftLayerDownTextureOnHover);
         m_ShiftLayerDownButton.SetImage(Button.ImageType.CLICK, m_ShiftLayerDownTextureOnPress);
         m_ShiftLayerDownButton.RegisterOnClick(ShiftLayerDownButton_OnClick);
         m_TrashLayerButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - btnWidth * 1, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_TrashTexture, m_RightMenuMouseInput);
         m_TrashLayerButton.SetImage(Button.ImageType.OVER, m_TrashTextureOnHover);
         m_TrashLayerButton.SetImage(Button.ImageType.CLICK, m_TrashTextureOnPress);
         m_TrashLayerButton.RegisterOnClick(TrashLayerButton_OnClick);

         #endregion

         // Sliders
         m_ProjectorAlphaSlider = new Slider(new Rectangle(60, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 7, 100, GUI_MENU_ITEM_HEIGHT), m_SliderBarTexture, m_SliderPadTexture, 0.0f, 1.0f, 0.05f, m_RightMenuMouseInput);
         m_ProjectorAlphaSlider.Value = 1.0f;

         // UV Coords
         m_SelectedFaceUVSpinBox = new UVSpinBoxPair[4];
         m_SelectedFaceUVSpinBox[0] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_LEFT_TOOLBAR_Y + GUI_MENU_ITEM_HEIGHT * 2), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[1] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_LEFT_TOOLBAR_Y + GUI_MENU_ITEM_HEIGHT * 3), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[2] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_LEFT_TOOLBAR_Y + GUI_MENU_ITEM_HEIGHT * 4), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[3] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_LEFT_TOOLBAR_Y + GUI_MENU_ITEM_HEIGHT * 5), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);

         // Initialize layers
         Label defaultGameplayLabel = new Label("Gameplay", Layer.NAME_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.LAYER_WIDTH, Layer.LAYER_HEIGHT, Color.Black);
         defaultGameplayLabel.Font = m_ArialFont10;
         Button visibleBtn = new Button(new Rectangle(Layer.VISIBILITY_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.VISIBILITY_BUTTON_WIDTH, Layer.VISIBILITY_BUTTON_HEIGHT), m_VisibleButtonTexture, m_RightMenuMouseInput);
         visibleBtn.SetImage(Button.ImageType.OVER, m_VisibleButtonTextureOnHover);
         visibleBtn.SetImage(Button.ImageType.CLICK, m_VisibleButtonTextureOnPress);
         Button editBtn = new Button(new Rectangle(Layer.EDIT_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.EDIT_BUTTON_WIDTH, Layer.EDIT_BUTTON_HEIGHT), m_EditLayerButtonTexture, m_RightMenuMouseInput);
         editBtn.SetImage(Button.ImageType.OVER, m_EditLayerButtonTextureOnHover);
         editBtn.SetImage(Button.ImageType.CLICK, m_EditLayerButtonTextureOnPress);
         m_LayersScrollView.AddLayer(new GameplayLayer(defaultGameplayLabel, visibleBtn, editBtn));

         // Register gizmo selection event
         m_ProjectorPreview.Gizmo.RegisterOnSelect(Gizmo_OnSelect);

         // Load color picker
         m_BuildingColorPicker = new ColorPickerComponent(new Rectangle(m_RightMenuViewport.X + GUI_BUILDING_COLOR_PICKER_X, GUI_BUILDING_COLOR_PICKER_Y, GUI_BUILDING_COLOR_PICKER_WIDTH, GUI_BUILDING_COLOR_PICKER_HEIGHT), m_Game.GraphicsDevice, content, false);
      }

      public override void Update(float elapsedTime)
      {
         if (m_DEM_GAMES)
         {
            if (m_UVEditor.RenderTargetTexture != null)
               m_ProjectorPreview.ProjectorTexture = m_UVEditor.RenderTargetTexture;

            m_ProjectorPreview.Update(elapsedTime);
         }
         else
         {
            if (m_UVGridEditor.HasUpdates)
            {
               m_UVEditor.SetPoints(m_UVGridEditor.GetIntersectionPoints());
               m_UVGridEditor.HasUpdates = false;
            }

            m_UVEditor.Update(elapsedTime);
            m_ProjectorPreview.Update(elapsedTime);
            m_ProjectorPreview.ProjectorAlpha = m_ProjectorAlphaSlider.Value;

            if (m_UVEditor.RenderTargetTexture != null)
               m_ProjectorPreview.ProjectorTexture = m_UVEditor.RenderTargetTexture;

            // Apply color picker color to building
            Material m = m_ProjectorPreview.Building.Material;
            m.Diffuse = m_BuildingColorPicker.SelectedColor.ToVector4();
            m_ProjectorPreview.Building.Material = m;

            // Sync uv coords with selected quad
            if (m_UVEditor.IsQuadSelected)
            {
               Vector2[] uvs = new Vector2[4];
               uvs[0] = m_SelectedFaceUVSpinBox[0].TexCoord;
               uvs[1] = m_SelectedFaceUVSpinBox[1].TexCoord;
               uvs[2] = m_SelectedFaceUVSpinBox[2].TexCoord;
               uvs[3] = m_SelectedFaceUVSpinBox[3].TexCoord;
               m_UVEditor.SetUVs(uvs);
            }

            // Sync menu status
            m_ProjectorAlphaValueLabel.Text = String.Format("{0:0.00}", m_ProjectorAlphaSlider.Value);
            m_AppModeValueLabel.Text = (m_EditorMode) ? "Editor" : "Preview";
            m_GizmoHeaderValueLabel.Text = (m_ProjectorPreview.Gizmo.ActiveMode.ToString().Contains("Scale")) ? "Scale" : m_ProjectorPreview.Gizmo.ActiveMode.ToString();
            m_ProjectorPositionXSpinBox.Value = m_ProjectorPreview.Projector.Position.X;
            m_ProjectorPositionYSpinBox.Value = m_ProjectorPreview.Projector.Position.Y;
            m_ProjectorPositionZSpinBox.Value = m_ProjectorPreview.Projector.Position.Z;
            m_ProjectorRotationXSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projector.RotX);
            m_ProjectorRotationYSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projector.RotY);
            m_ProjectorRotationZSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projector.RotZ);
            m_ProjectorFovSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projector.Fov);
            m_ProjectorAspectRatioSpinBox.Value = m_ProjectorPreview.Projector.AspectRatio;
            m_BuildingPositionXSpinBox.Value = m_ProjectorPreview.Building.Position.X;
            m_BuildingPositionYSpinBox.Value = m_ProjectorPreview.Building.Position.Y;
            m_BuildingPositionZSpinBox.Value = m_ProjectorPreview.Building.Position.Z;
            m_BuildingRotationXSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Building.RotX);
            m_BuildingRotationYSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Building.RotY);
            m_BuildingRotationZSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Building.RotZ);
            m_BuildingScaleXSpinBox.Value = m_ProjectorPreview.Building.Scale.X;
            m_BuildingScaleYSpinBox.Value = m_ProjectorPreview.Building.Scale.Y;
            m_BuildingScaleZSpinBox.Value = m_ProjectorPreview.Building.Scale.Z;
         }
      }

      public override void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         if (m_DEM_GAMES)
         {
            if (keyboardState.IsKeyDown(Keys.Escape) && !m_PrevKeyboardState.IsKeyDown(Keys.Escape))
            {
               m_DEM_GAMES = false;
               m_ProjectorPreview.EditorMode = true;
               m_ProjectorPreview.Viewport = new Viewport(GUI_PROJECTOR_PREVIEW_X + 2, GUI_PROJECTOR_PREVIEW_Y + 20, GUI_PROJECTOR_PREVIEW_WIDTH - 2, GUI_PROJECTOR_PREVIEW_HEIGHT - 20);
               m_ProjectorPreview.Camera.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
               m_ProjectorPreview.Camera.UpdateProjection();
               //m_ProjectorPreview.Projector.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
               //m_ProjectorPreview.Projector.UpdateProjection();
               FiniteStateMachine.GetInstance().QuitGame();
            }
         }
         else
         {
            m_MouseInput.HandleInput(PlayerIndex.One);
            m_RightMenuMouseInput.HandleInput(PlayerIndex.One);

            // Update color picker if it is active
            if (m_BuildingColorPicker.IsActive)
               m_BuildingColorPicker.HandleInput(mouseState, m_PrevMouseState);

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
            else
            {
               m_UVEditor.UnHoverQuads();
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
         }
         

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      void AddLayer(LayerType layerType)
      {
         switch (layerType)
         {
            case LayerType.Texture:
               //m_Layers.Add(new TextureLayer());
               break;
            case LayerType.Particle:
               //m_Layers.Add(new ParticleLayer());
               break;
         }
      }

      void Quad_OnSelected(object sender, EventArgs e)
      {
         m_SelectedFaceUVSpinBox[0].TexCoord = m_UVEditor.SelectedQuadTexCoords[0];
         m_SelectedFaceUVSpinBox[1].TexCoord = m_UVEditor.SelectedQuadTexCoords[1];
         m_SelectedFaceUVSpinBox[2].TexCoord = m_UVEditor.SelectedQuadTexCoords[2];
         m_SelectedFaceUVSpinBox[3].TexCoord = m_UVEditor.SelectedQuadTexCoords[3];
      }

      void Gizmo_OnSelect(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Gizmo.SelectedID == m_ProjectorPreview.ProjectorEntity.ID)
         {
            if (m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.UniformScale)
               m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;
         }
         else if (m_ProjectorPreview.Gizmo.SelectedID == m_ProjectorPreview.LightEntity.ID)
         {
            if (m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.UniformScale ||
                m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.Rotate)
               m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;
         }
      }

      #region SpinBoxes

      private void BuildingPositionX_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Building.Position;
         pos.X = m_BuildingPositionXSpinBox.Value;
         m_ProjectorPreview.Building.Position = pos;
      }
      private void BuildingPositionY_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Building.Position;
         pos.Y = m_BuildingPositionYSpinBox.Value;
         m_ProjectorPreview.Building.Position = pos;
      }
      private void BuildingPositionZ_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Building.Position;
         pos.Z = m_BuildingPositionZSpinBox.Value;
         m_ProjectorPreview.Building.Position = pos;
      }
      private void BuildingRotationX_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Building.RotX = MathHelper.ToRadians(m_BuildingRotationXSpinBox.Value);
      }
      private void BuildingRotationY_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Building.RotY = MathHelper.ToRadians(m_BuildingRotationYSpinBox.Value);
      }
      private void BuildingRotationZ_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Building.RotZ = MathHelper.ToRadians(m_BuildingRotationZSpinBox.Value);
      }
      private void BuildingScaleX_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 scale = m_ProjectorPreview.Building.Scale;
         scale.X = m_BuildingScaleXSpinBox.Value;
         scale.Y = m_BuildingScaleXSpinBox.Value;
         scale.Z = m_BuildingScaleXSpinBox.Value;
         m_BuildingScaleZSpinBox.Value = m_BuildingScaleXSpinBox.Value;
         m_BuildingScaleYSpinBox.Value = m_BuildingScaleXSpinBox.Value;
         m_ProjectorPreview.Building.Scale = scale;
      }
      private void BuildingScaleY_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 scale = m_ProjectorPreview.Building.Scale;
         scale.X = m_BuildingScaleYSpinBox.Value;
         scale.Y = m_BuildingScaleYSpinBox.Value;
         scale.Z = m_BuildingScaleYSpinBox.Value;
         m_BuildingScaleZSpinBox.Value = m_BuildingScaleYSpinBox.Value;
         m_BuildingScaleXSpinBox.Value = m_BuildingScaleYSpinBox.Value;
         m_ProjectorPreview.Building.Scale = scale;
      }
      private void BuildingScaleZ_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 scale = m_ProjectorPreview.Building.Scale;
         scale.X = m_BuildingScaleZSpinBox.Value;
         scale.Y = m_BuildingScaleZSpinBox.Value;
         scale.Z = m_BuildingScaleZSpinBox.Value;
         m_BuildingScaleXSpinBox.Value = m_BuildingScaleZSpinBox.Value;
         m_BuildingScaleYSpinBox.Value = m_BuildingScaleZSpinBox.Value;
         m_ProjectorPreview.Building.Scale = scale;
      }
      private void ProjectorPositionX_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Projector.Position;
         pos.X = m_ProjectorPositionXSpinBox.Value;
         m_ProjectorPreview.Projector.Position = pos;
         m_ProjectorPreview.ProjectorEntity.Position = pos;
      }
      private void ProjectorPositionY_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Projector.Position;
         pos.Y = m_ProjectorPositionYSpinBox.Value;
         m_ProjectorPreview.Projector.Position = pos;
         m_ProjectorPreview.ProjectorEntity.Position = pos;
      }
      private void ProjectorPositionZ_OnValueChanged(object sender, EventArgs e)
      {
         Vector3 pos = m_ProjectorPreview.Projector.Position;
         pos.Z = m_ProjectorPositionZSpinBox.Value;
         m_ProjectorPreview.Projector.Position = pos;
         m_ProjectorPreview.ProjectorEntity.Position = pos;
      }
      private void ProjectorRotationX_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Projector.RotX = MathHelper.ToRadians(m_ProjectorRotationXSpinBox.Value);
         m_ProjectorPreview.ProjectorEntity.RotX = MathHelper.ToRadians(m_ProjectorRotationXSpinBox.Value);
      }
      private void ProjectorRotationY_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Projector.RotY = MathHelper.ToRadians(m_ProjectorRotationYSpinBox.Value);
         m_ProjectorPreview.ProjectorEntity.RotY = MathHelper.ToRadians(m_ProjectorRotationYSpinBox.Value);
      }
      private void ProjectorRotationZ_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Projector.RotZ = MathHelper.ToRadians(m_ProjectorRotationZSpinBox.Value);
         m_ProjectorPreview.ProjectorEntity.RotZ = MathHelper.ToRadians(m_ProjectorRotationZSpinBox.Value);
      }

      private void ProjectorAspectRatio_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Projector.AspectRatio = m_ProjectorAspectRatioSpinBox.Value;
      }
      private void ProjectorFOV_OnValueChanged(object sender, EventArgs e)
      {
         m_ProjectorPreview.Projector.Fov = MathHelper.ToRadians(m_ProjectorFovSpinBox.Value);
      }

      #endregion

      #region Buttons

      private void NewTextureLayerButton_OnClick(object sender, EventArgs e)
      {
         AddLayer(LayerType.Texture);
      }

      private void NewParticleLayerButton_OnClick(object sender, EventArgs e)
      {
         AddLayer(LayerType.Particle);
      }

      private void ShiftLayerUpButton_OnClick(object sender, EventArgs e)
      {

      }

      private void ShiftLayerDownButton_OnClick(object sender, EventArgs e)
      {

      }

      private void TrashLayerButton_OnClick(object sender, EventArgs e)
      {

      }

      private void BuildingColorButton_OnClick(object sender, EventArgs e)
      {
         if (m_BuildingColorPicker.IsActive)
         {
            m_BuildingColorPicker.HidePickerDialog();
            ShiftProjectionPanelContents(new Vector2(0, -80));
            ShiftLayerPanelContents(new Vector2(0, -80));
         }
         else
         {
            m_BuildingColorPicker.ShowPickerDialog();
            ShiftProjectionPanelContents(new Vector2(0, 80));
            ShiftLayerPanelContents(new Vector2(0, 80));
            
         }
      }

      void ShiftLayerPanelContents(Vector2 offset)
      {
         m_LayersHeaderLabel.Location += offset;

         // Resize scrollview
         Rectangle displayBounds = m_LayersScrollView.DisplayBounds;
         displayBounds.X += (int)offset.X;
         displayBounds.Y += (int)offset.Y;
         displayBounds.Height -= (int)offset.Y;
         m_LayersScrollView.DisplayBounds = displayBounds;
      }

      void ShiftProjectionPanelContents(Vector2 offset)
      {
         m_ProjectorHeaderLabel.Location += offset;
         m_ProjectorFOVLabel.Location += offset;
         m_ProjectorAspectRatioLabel.Location += offset;
         m_ProjectorFovSpinBox.Location += offset;
         m_ProjectorFovSpinBox.Children[0].Location += offset;
         m_ProjectorFovSpinBox.Children[1].Location += offset;
         m_ProjectorAspectRatioSpinBox.Location += offset;
         m_ProjectorAspectRatioSpinBox.Children[0].Location += offset;
         m_ProjectorAspectRatioSpinBox.Children[1].Location += offset;
         m_ProjectorPositionLabel.Location += offset;
         m_ProjectorPositionXLabel.Location += offset;
         m_ProjectorPositionYLabel.Location += offset;
         m_ProjectorPositionZLabel.Location += offset;
         m_ProjectorPositionXSpinBox.Location += offset;
         m_ProjectorPositionXSpinBox.Children[0].Location += offset;
         m_ProjectorPositionXSpinBox.Children[1].Location += offset;
         m_ProjectorPositionYSpinBox.Location += offset;
         m_ProjectorPositionYSpinBox.Children[0].Location += offset;
         m_ProjectorPositionYSpinBox.Children[1].Location += offset;
         m_ProjectorPositionZSpinBox.Location += offset;
         m_ProjectorPositionZSpinBox.Children[0].Location += offset;
         m_ProjectorPositionZSpinBox.Children[1].Location += offset;
         m_ProjectorRotationLabel.Location += offset;
         m_ProjectorRotationXLabel.Location += offset;
         m_ProjectorRotationYLabel.Location += offset;
         m_ProjectorRotationZLabel.Location += offset;
         m_ProjectorRotationXSpinBox.Location += offset;
         m_ProjectorRotationXSpinBox.Children[0].Location += offset;
         m_ProjectorRotationXSpinBox.Children[1].Location += offset;
         m_ProjectorRotationYSpinBox.Location += offset;
         m_ProjectorRotationYSpinBox.Children[0].Location += offset;
         m_ProjectorRotationYSpinBox.Children[1].Location += offset;
         m_ProjectorRotationZSpinBox.Location += offset;
         m_ProjectorRotationZSpinBox.Children[0].Location += offset;
         m_ProjectorRotationZSpinBox.Children[1].Location += offset;
         m_ProjectorEnabledButton.Location += offset;
         m_ProjectorEnabledButton.TextPos += offset;
      }

      private void ProjectorEnabledButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.ProjectorIsOn = !m_ProjectorPreview.ProjectorIsOn;
         if (m_ProjectorPreview.ProjectorIsOn)
         {
            m_ProjectorEnabledButton.Text = "Turn Off";
         }
         else
         {
            m_ProjectorEnabledButton.Text = "Turn On";
         }
      }

      private void TranslateButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;
      }

      private void ScaleButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Gizmo.SelectedID != m_ProjectorPreview.ProjectorEntity.ID &&
             m_ProjectorPreview.Gizmo.SelectedID != m_ProjectorPreview.LightEntity.ID)
            m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.UniformScale;
      }

      private void RotateButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Gizmo.SelectedID != m_ProjectorPreview.LightEntity.ID)
            m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Rotate;
      }

      private void ModeButton_OnClick(object sender, EventArgs e)
      {
         m_EditorMode = !m_EditorMode;
         if (m_EditorMode)
         {
            m_ModeButton.Text = "Preview";
            m_ProjectorPreview.EditorMode = true;
         }
         else
         {
            m_ModeButton.Text = "Editor";
            m_ProjectorPreview.EditorMode = false;
         }
      }

      private void PlayButton_OnClick(object sender, EventArgs e)
      {
         // Play da gamez
         m_DEM_GAMES = true;
         m_ProjectorPreview.EditorMode = false;
         m_ProjectorPreview.Viewport = new Viewport(0, 0, GameConstants.WindowWidth, GameConstants.WindowHeight);
         m_ProjectorPreview.Camera.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
         m_ProjectorPreview.Camera.UpdateProjection();
         //m_ProjectorPreview.Projector.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
         //m_ProjectorPreview.Projector.UpdateProjection();
         FiniteStateMachine.GetInstance().StartGame();
      }

      private void ResetButton_OnClick(object sender, EventArgs e)
      {
         Reset();
      }

      private void QuitButton_OnClick(object sender, EventArgs e)
      {
         // Allow for exit to main menu
         FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
      }

      #endregion

      #region Rendering

      public override void Draw(SpriteBatch spriteBatch)
      {
         Viewport defaultViewport = m_Game.GraphicsDevice.Viewport;

         if (m_DEM_GAMES)
         {
            // Render the UV editor contents to a render target
            m_Game.GraphicsDevice.Viewport = m_UVEditor.Viewport;
            m_UVEditor.DrawRenderTarget();

            m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Viewport;
            m_ProjectorPreview.Draw();
         }
         else
         {
            // Render the UV editor contents to a render target
            m_Game.GraphicsDevice.Viewport = m_UVEditor.Viewport;
            m_UVEditor.DrawRenderTarget();

            // Render panels
            m_Game.GraphicsDevice.Viewport = defaultViewport;
            spriteBatch.Begin();
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, GameConstants.WindowWidth, GameConstants.WindowHeight), Color.Gray);
            spriteBatch.End();
            m_GridEditorPanel.Draw(m_Game.GraphicsDevice, spriteBatch);
            m_UVEditorPanel.Draw(m_Game.GraphicsDevice, spriteBatch);
            m_ProjectionEditorPanel.Draw(m_Game.GraphicsDevice, spriteBatch);

            // Left menu
            spriteBatch.Begin();
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(GUI_LEFT_TOOLBAR_X, 0, GUI_LEFT_TOOLBAR_WIDTH, m_RightMenuViewport.Height), Color.Gray);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(GUI_LEFT_TOOLBAR_X, 0, 2, m_RightMenuViewport.Height), Color.Black);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(GUI_LEFT_TOOLBAR_X, 0, GUI_LEFT_TOOLBAR_WIDTH, 2), Color.Black);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(GUI_LEFT_TOOLBAR_X + GUI_LEFT_TOOLBAR_WIDTH, 0, 2, m_RightMenuViewport.Height), Color.Black);
            RenderSelectedQuadSection(spriteBatch);
            spriteBatch.End();

            // Render the UV editor
            m_Game.GraphicsDevice.Viewport = m_UVEditor.Viewport;
            m_UVEditor.Draw(spriteBatch);

            // Render the UV grid editor
            m_Game.GraphicsDevice.Viewport = m_UVGridEditor.Viewport;
            m_UVGridEditor.Draw(spriteBatch);

            // Render the projector preview
            m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Viewport;
            m_ProjectorPreview.Draw();

            // Right menu
            m_Game.GraphicsDevice.Viewport = m_RightMenuViewport;
            spriteBatch.Begin();
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, GUI_TOOLBAR_WIDTH, m_RightMenuViewport.Height), Color.Gray);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, 2, m_RightMenuViewport.Height), Color.Black);
            RenderApplicationSection(spriteBatch);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_APP_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
            RenderGizmoSection(spriteBatch);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_GIZMO_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
            RenderBuildingSection(spriteBatch);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_BUILDING_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
            RenderProjectorSection(spriteBatch);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_PROJECTOR_Y - 2 + ((m_BuildingColorPicker.IsActive) ? 80 : 0), GUI_TOOLBAR_WIDTH, 2), Color.Black);
            RenderLayersSection(spriteBatch);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_LAYERS_Y - 2 + ((m_BuildingColorPicker.IsActive) ? 80 : 0), GUI_TOOLBAR_WIDTH, 2), Color.Black);
            spriteBatch.End();

            // Render color picker if it is active
            if (m_BuildingColorPicker.IsActive)
            {
               m_Game.GraphicsDevice.Viewport = m_BuildingColorPicker.Viewport;
               m_BuildingColorPicker.Draw(m_Game.GraphicsDevice, spriteBatch);
            }

            // Restore the default viewport
            m_Game.GraphicsDevice.Viewport = defaultViewport;
         }

         // Restore the default viewport
         m_Game.GraphicsDevice.Viewport = defaultViewport;
      }

      private void RenderSelectedQuadSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_LEFT_TOOLBAR_X + 2, GUI_SELECTED_QUAD_Y, GUI_LEFT_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_SelectedFaceHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceTexCoordLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP0Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP1Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP2Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP3Label.Draw(m_Game.GraphicsDevice, spriteBatch);

         if (m_UVEditor.IsQuadSelected)
         {
            for (int i = 0; i < 4; ++i)
               m_SelectedFaceUVSpinBox[i].Draw(m_Game.GraphicsDevice, spriteBatch);
         }
         else
         {
            m_SelectedFaceP0NoneLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
            m_SelectedFaceP1NoneLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
            m_SelectedFaceP2NoneLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
            m_SelectedFaceP3NoneLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         }
      }

      private void RenderApplicationSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_TOOLBAR_X + 2, GUI_APP_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_AppHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ModeButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_PlayButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ResetButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_QuitButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderGizmoSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_TOOLBAR_X + 2, GUI_GIZMO_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_GizmoHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_GizmoHeaderValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_TranslateButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_RotateButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ScaleButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderBuildingSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_TOOLBAR_X + 2, GUI_BUILDING_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_BuildingHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingFilepathValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingPositionZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingRotationZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingScaleZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingSnapToGroundLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingColorLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingColorButton.Draw(m_Game.GraphicsDevice, spriteBatch); 
         spriteBatch.Draw(m_ColorPickerPreviewPadTexture, new Rectangle((int)m_BuildingColorButton.Location.X + 4, (int)m_BuildingColorButton.Location.Y + 4, m_BuildingColorButton.Width - 8, m_BuildingColorButton.Height - 8), m_BuildingColorPicker.SelectedColor);
         
      }

      private void RenderProjectorSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_TOOLBAR_X + 2, GUI_PROJECTOR_Y + ((m_BuildingColorPicker.IsActive) ? 80 : 0), GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_ProjectorHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorFOVLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAspectRatioLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorFovSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAspectRatioSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorPositionZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorRotationZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorEnabledButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaSlider.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderLayersSection(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_TOOLBAR_X + 2, GUI_LAYERS_Y + ((m_BuildingColorPicker.IsActive) ? 80 : 0), GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_LayersHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);

         m_LayersScrollView.Draw(m_Game.GraphicsDevice, spriteBatch);

         m_NewTextureLayerButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_NewParticleLayerButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ShiftLayerDownButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ShiftLayerUpButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_TrashLayerButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderGradient(Rectangle bounds, SpriteBatch spriteBatch, Color start, Color end)
      {
         for (int x = bounds.X; x < bounds.X + bounds.Width; ++x)
         {
            float t = (float)(x - bounds.X) / (float)bounds.Width;

            Color intrpColor = Color.Lerp(end, start, t);
            spriteBatch.Draw(m_WhiteTexture, new Rectangle(x, bounds.Y, 1, bounds.Height), intrpColor);
         }
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

      #endregion

   }
}
