﻿#region File Description

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
using System.IO;
using System.Text;
using System.Xml.Serialization;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

// Local imports
using ProjectionMappingGame.Components;
using ProjectionMappingGame.Editor;
using ProjectionMappingGame.FileSystem;
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
      MouseInput m_ScrollMouseInput;

      // States
      bool m_DEM_GAMES;

      // Editor panes
      ProjectionPreviewComponent m_ProjectorPreview;
      UVGridEditor m_UVGridEditor;
      UVDualEdgeGraphEditor m_UVDualEdgeGraphEditor;
      int m_FocusedPane;
      bool m_EditorMode;
      ProjectionMappingGame.GUI.Panel m_GridEditorPanel;
      ProjectionMappingGame.GUI.Panel m_UVEditorPanel;
      ProjectionMappingGame.GUI.Panel m_ProjectionEditorPanel;

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
      Texture2D m_LinkButtonTexture, m_LinkButtonTextureOnHover, m_LinkButtonTextureOnPress;
      Texture2D m_EditLayerButtonTexture, m_EditLayerButtonTextureOnHover, m_EditLayerButtonTextureOnPress;
      Texture2D m_CheckboxCheckedTexture, m_CheckboxCheckedTextureOnHover, m_CheckboxCheckedTextureOnPress;
      Texture2D m_CheckboxUnCheckedTexture, m_CheckboxUnCheckedTextureOnHover, m_CheckboxUnCheckedTextureOnPress;
      Texture2D m_NonUniformScaleButtonTexture, m_NonUniformScaleButtonTextureOnHover, m_NonUniformScaleButtonTextureOnPress;
      Texture2D m_WallTexture;

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

      #region GUI Left Menu Controls

      // Application section GUI objects
      Label m_AppHeaderLabel;
      Label m_AppModeLabel;
      Label m_AppModeValueLabel;
      //Button m_ModeButton;
      Button m_PlayButton;
      Button m_QuitButton;
      Button m_ResetButton;
      Button m_SaveButton;
      Button m_OpenButton;

      // Scene section GUI objects
      Label m_SceneHeaderLabel;
      //Button m_NewProjectorButton;
      Button m_NewMeshButton;

      // Selected face section GUI objects
      Label m_SelectedFaceHeaderLabel;
      Label m_SelectedFaceTexCoordLabel;
      Label m_SelectedFaceP0Label;
      Label m_SelectedFaceP1Label;
      Label m_SelectedFaceP2Label;
      Label m_SelectedFaceP3Label;
      UVSpinBoxPair[] m_SelectedFaceUVSpinBox;
      Button m_ToLocalCoordButton;
      Label m_SelectedFaceGameplayLayerLabel;
      NumUpDown m_SelectedFaceGameplayLayerSpinBox;
      Checkbox m_SelectedFaceWallCheckbox;
      Checkbox m_SelectedFaceScoreboardCheckbox;

      const int GUI_LEFT_MENU_COLUMN_1_X = GUI_LEFT_TOOLBAR_X + 5;
      const int GUI_LEFT_MENU_COLUMN_1_X_TABBED = GUI_LEFT_TOOLBAR_X + 15;
      const int GUI_LEFT_MENU_COLUMN_1_WIDTH = 50;
      const int GUI_APP_NUM_ROWS = 4;
      const int GUI_SCENE_NUM_ROWS = 2;
      const int GUI_APP_Y = 2;
      const int GUI_SCENE_Y = GUI_APP_Y + (GUI_MENU_ITEM_HEIGHT * GUI_APP_NUM_ROWS) + 24;
      const int GUI_SELECTED_QUAD_Y = GUI_SCENE_Y + (GUI_MENU_ITEM_HEIGHT * GUI_SCENE_NUM_ROWS) + 16;
      const int GUI_LOCAL_COORD_BUTTON_WIDTH = 100;
      const int GUI_QUIT_BUTTON_WIDTH = 80;
      const int GUI_QUIT_BUTTON_HEIGHT = 24;
      
      #endregion

      #region GUI Right Menu Controls

      Viewport m_RightMenuViewport;

      // Textures
      Texture2D m_ColorPickerPreviewPadTexture;

      // Gizmo section GUI objects
      Label m_GizmoHeaderLabel;
      Label m_GizmoHeaderValueLabel;
      Button m_TranslateButton;
      Button m_RotateButton;
      Button m_ScaleButton;
      Button m_NonUniformScaleButton;

      // Building section GUI objects
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
      Button m_BuildingResetButton;
      Button m_BuildingDeleteButton;

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
      Button m_ProjectorResetButton;
      Button m_ProjectorDeleteButton;

      // Layers section GUI objects
      Label m_LayersHeaderLabel;
      LayerScrollView m_LayersScrollView;
      Button m_NewGameplayLayerButton;
      Button m_TrashLayerButton;

      const int GUI_GIZMO_BUTTON_WIDTH = 24;
      const int GUI_GIZMO_BUTTON_HEIGHT = 24;

      const int GUI_MENU_COLUMN_1_X = 5;
      const int GUI_MENU_COLUMN_1_X_TABBED = 15;
      const int GUI_MENU_COLUMN_1_WIDTH = 100;
      const int GUI_MENU_ITEM_HEIGHT = 20;
      const int GUI_MENU_X_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5;
      const int GUI_MENU_Y_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5 + (GUI_SPINBOX_WIDTH + 12);
      const int GUI_MENU_Z_COLUMN_X = GUI_MENU_COLUMN_1_X_TABBED + 5 + (GUI_SPINBOX_WIDTH + 12) * 2;

      const int GUI_GIZMO_NUM_ROWS = 2;
      const int GUI_BUILDING_NUM_ROWS = 9;
      const int GUI_PROJECTOR_NUM_ROWS = 8;
      const int GUI_GIZMO_Y = 2;
      const int GUI_BUILDING_Y = GUI_GIZMO_Y + (GUI_MENU_ITEM_HEIGHT * GUI_GIZMO_NUM_ROWS) + 10;
      const int GUI_PROJECTOR_Y = GUI_BUILDING_Y + 4 + (GUI_MENU_ITEM_HEIGHT * GUI_BUILDING_NUM_ROWS) + 12;
      const int GUI_LAYERS_Y = GUI_PROJECTOR_Y - 4 + (GUI_MENU_ITEM_HEIGHT * GUI_PROJECTOR_NUM_ROWS);

      const int GUI_BUILDING_COLOR_PICKER_X = 75;
      const int GUI_BUILDING_COLOR_PICKER_Y = GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4;
      const int GUI_BUILDING_COLOR_PICKER_WIDTH = 100;
      const int GUI_BUILDING_COLOR_PICKER_HEIGHT = 100;
      const int GUI_COLOR_BUTTON_WIDTH = 24;
      const int GUI_COLOR_BUTTON_HEIGHT = 24;

      const int GUI_SPINBOX_WIDTH = 50;
      const int GUI_SPINBOX_HEIGHT = 20;

      #endregion

      #region Initialization

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
         m_UVDualEdgeGraphEditor = new UVDualEdgeGraphEditor(m_Game, GUI_UV_EDITOR_X + 2, GUI_UV_EDITOR_Y + 20, GUI_UV_EDITOR_WIDTH - 2, GUI_UV_EDITOR_HEIGHT - 20);
         m_UVDualEdgeGraphEditor.RegisterQuadSelectedEvent(Quad_OnSelected);
         m_ProjectorPreview = new ProjectionPreviewComponent(m_Game, GUI_PROJECTOR_PREVIEW_X + 2, GUI_PROJECTOR_PREVIEW_Y + 20, GUI_PROJECTOR_PREVIEW_WIDTH - 2, GUI_PROJECTOR_PREVIEW_HEIGHT - 20);
         m_RightMenuViewport = new Viewport(GUI_TOOLBAR_X, GUI_TOOLBAR_Y, GUI_TOOLBAR_WIDTH, GUI_TOOLBAR_HEIGHT);

         // Defaults
         m_FocusedPane = 0;
         m_EditorMode = true;
         m_DEM_GAMES = false;
      }

      public override void Reset()
      {
         m_FocusedPane = 0;
         m_EditorMode = true;
         m_DEM_GAMES = false;

         m_UVDualEdgeGraphEditor.Reset();
         m_UVGridEditor.Reset();
         m_ProjectorPreview.Reset();
         m_ProjectorPreview.DeSelectProjector();
         m_ProjectorPreview.DeSelectBuilding();

         // Create the default gameplay layer
         m_LayersScrollView.DeleteSelectedLayer();
         m_LayersScrollView.Clear();
         AssembleGameplayLayer();

         for (int i = 0; i < m_ProjectorPreview.Projectors.Count; ++i)
         {
            m_ProjectorPreview.Projectors[i].Grid.Reset(m_UVGridEditor.Viewport.Width, m_UVGridEditor.Viewport.Height);
            m_ProjectorPreview.Projectors[i].EdgeGraph.AssembleGraph(m_ProjectorPreview.Projectors[i].Grid.GetIntersectionPoints());
         }
      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
         m_ArialFont10 = content.Load<SpriteFont>("Fonts/Arial10");
         
         // Load textures
         LoadTextures(content);

         // Load components content
         m_UVGridEditor.LoadContent(content);
         m_UVDualEdgeGraphEditor.LoadContent(content);
         m_ProjectorPreview.LoadContent(content);
         m_ProjectorPreview.RegisterOnLeaveNormalSelectionMode(LeaveNormalSelectionMode);

         // Load initial points
         for (int i = 0; i < m_ProjectorPreview.Projectors.Count; ++i)
         {
            m_ProjectorPreview.Projectors[i].Grid.Reset(m_UVGridEditor.Viewport.Width, m_UVGridEditor.Viewport.Height);
            m_ProjectorPreview.Projectors[i].EdgeGraph.AssembleGraph(m_ProjectorPreview.Projectors[i].Grid.GetIntersectionPoints());
         }

         // Register gizmo selection event
         m_ProjectorPreview.Gizmo.RegisterOnSelect(Gizmo_OnSelect);
         m_ProjectorPreview.Gizmo.RegisterOnDeSelect(Gizmo_OnDeSelect);

         // Build the GUI
         BuildGUI();
      }

      private void LoadTextures(ContentManager content)
      {
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
         m_LinkButtonTexture = content.Load<Texture2D>("Textures/GUI/link_layer_button");
         m_LinkButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/link_layer_button_on_hover");
         m_LinkButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/link_layer_button_on_hover");
         m_EditLayerButtonTexture = content.Load<Texture2D>("Textures/GUI/edit_layer_button");
         m_EditLayerButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/edit_layer_button_on_hover");
         m_EditLayerButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/edit_layer_button_on_hover");
         m_CheckboxCheckedTexture = content.Load<Texture2D>("Textures/GUI/checkbox_on");
         m_CheckboxCheckedTextureOnHover = content.Load<Texture2D>("Textures/GUI/checkbox_on_on_hover");
         m_CheckboxCheckedTextureOnPress = content.Load<Texture2D>("Textures/GUI/checkbox_on_on_hover");
         m_CheckboxUnCheckedTexture = content.Load<Texture2D>("Textures/GUI/checkbox_off");
         m_CheckboxUnCheckedTextureOnHover = content.Load<Texture2D>("Textures/GUI/checkbox_off_on_hover");
         m_CheckboxUnCheckedTextureOnPress = content.Load<Texture2D>("Textures/GUI/checkbox_off_on_hover");
         m_NonUniformScaleButtonTexture = content.Load<Texture2D>("Textures/GUI/nonuniform_scale");
         m_NonUniformScaleButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/nonuniform_scale_on_hover");
         m_NonUniformScaleButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/nonuniform_scale_on_hover");
         m_WallTexture = content.Load<Texture2D>("Textures/wall");
      }

      private void BuildGUI()
      {
         // Initialize panels
         m_GridEditorPanel = new Panel(new Rectangle(GUI_UV_GRID_EDITOR_X, GUI_UV_GRID_EDITOR_Y, GUI_UV_GRID_EDITOR_WIDTH, GUI_UV_GRID_EDITOR_HEIGHT), "UV Grid Editor", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.White, Color.Black);
         m_UVEditorPanel = new Panel(new Rectangle(GUI_UV_EDITOR_X, GUI_UV_EDITOR_Y, GUI_UV_EDITOR_WIDTH, GUI_UV_EDITOR_HEIGHT), "UV Editor", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.White, Color.Black);
         m_ProjectionEditorPanel = new Panel(new Rectangle(GUI_PROJECTOR_PREVIEW_X, GUI_PROJECTOR_PREVIEW_Y, GUI_PROJECTOR_PREVIEW_WIDTH, GUI_PROJECTOR_PREVIEW_HEIGHT), "Projection Preview", Color.Black, m_ArialFont10, Color.Gray, Color.LightGray, m_WhiteTexture, Color.Black, Color.Black);

         int leftmenuColumn2X = (GUI_LEFT_TOOLBAR_WIDTH / 2) + GUI_LEFT_MENU_COLUMN_1_X;
         int menuColumn2X = (GUI_TOOLBAR_WIDTH / 2) + GUI_MENU_COLUMN_1_X;

         #region Application Menu

         // Labels
         m_AppHeaderLabel = new Label("Application", GUI_LEFT_MENU_COLUMN_1_X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_AppModeLabel = new Label("Mode", GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_AppModeValueLabel = new Label("Editor", leftmenuColumn2X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Buttons
         //m_ModeButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Preview", Color.Black);
         //m_ModeButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         //m_ModeButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         //m_ModeButton.RegisterOnClick(ModeButton_OnClick);
         m_PlayButton = new Button(new Rectangle(leftmenuColumn2X, GUI_APP_Y + GUI_MENU_ITEM_HEIGHT * 1, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Play", Color.Black);
         m_PlayButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_PlayButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_PlayButton.RegisterOnClick(PlayButton_OnClick);
         m_ResetButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + 12 + GUI_MENU_ITEM_HEIGHT * 3, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Reset", Color.Black);
         m_ResetButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ResetButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ResetButton.RegisterOnClick(ResetButton_OnClick);
         m_QuitButton = new Button(new Rectangle(leftmenuColumn2X, GUI_APP_Y + 12 + GUI_MENU_ITEM_HEIGHT * 3, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Quit", Color.Black);
         m_QuitButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_QuitButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_QuitButton.RegisterOnClick(QuitButton_OnClick);
         m_SaveButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_APP_Y + 6 + GUI_MENU_ITEM_HEIGHT * 2, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Save", Color.Black);
         m_SaveButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_SaveButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_SaveButton.RegisterOnClick(SaveButton_OnClick);
         m_OpenButton = new Button(new Rectangle(leftmenuColumn2X, GUI_APP_Y + 6 + GUI_MENU_ITEM_HEIGHT * 2, (int)(GUI_QUIT_BUTTON_WIDTH * 0.9f), GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Open", Color.Black);
         m_OpenButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_OpenButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_OpenButton.RegisterOnClick(OpenButton_OnClick);
         
         #endregion

         #region Scene Menu

         // Labels
         m_SceneHeaderLabel = new Label("Scene", GUI_LEFT_MENU_COLUMN_1_X, GUI_SCENE_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Buttons
         //m_NewProjectorButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_SCENE_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_LOCAL_COORD_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "New Projector", Color.Black);
         //m_NewProjectorButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         //m_NewProjectorButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         //m_NewProjectorButton.RegisterOnClick(NewProjectorButton_OnClick);
         m_NewMeshButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_SCENE_Y + 6 + GUI_MENU_ITEM_HEIGHT * 1, GUI_LOCAL_COORD_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "New Building", Color.Black);
         m_NewMeshButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_NewMeshButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_NewMeshButton.RegisterOnClick(NewMeshButton_OnClick);
         
         #endregion

         #region Selected Face Menu

         // Labels
         m_SelectedFaceHeaderLabel = new Label("Selected Face", GUI_LEFT_MENU_COLUMN_1_X, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceTexCoordLabel = new Label("Texture Coordinates", GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 1, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceP0Label = new Label("v1", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 2, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceP1Label = new Label("v2", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 3, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceP2Label = new Label("v3", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 4, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceP3Label = new Label("v4", GUI_LEFT_MENU_COLUMN_1_X_TABBED + 15, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 5, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_SelectedFaceGameplayLayerLabel = new Label("Input Layer", GUI_LEFT_MENU_COLUMN_1_X_TABBED, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 6, GUI_LEFT_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Spinboxes
         m_SelectedFaceGameplayLayerSpinBox = new NumUpDown(new Rectangle(GUI_LEFT_TOOLBAR_X + 127, GUI_SELECTED_QUAD_Y + 2 + GUI_MENU_ITEM_HEIGHT * 6, 40, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 1, 1, 1, "{0:0}", m_MouseInput);
         m_SelectedFaceGameplayLayerSpinBox.RegisterOnValueChanged(SelectedFaceInputLayer_OnValueChanged);
         m_SelectedFaceUVSpinBox = new UVSpinBoxPair[4];
         m_SelectedFaceUVSpinBox[0] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 2), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[1] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 3), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[2] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 4), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);
         m_SelectedFaceUVSpinBox[3] = new UVSpinBoxPair(new Vector2(GUI_LEFT_TOOLBAR_X + 50, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 5), m_WhiteTexture, m_SpinBoxFillTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, m_MouseInput);

         // Buttons
         m_ToLocalCoordButton = new Button(new Rectangle(GUI_LEFT_MENU_COLUMN_1_X + 5, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 9 + 12, GUI_LOCAL_COORD_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_ArialFont10, "Reset To Local", Color.Black);
         m_ToLocalCoordButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ToLocalCoordButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ToLocalCoordButton.RegisterOnClick(ToLocalCoordButton_OnClick);

         // Checkboxes
         m_SelectedFaceWallCheckbox = new Checkbox(
            new Rectangle(GUI_LEFT_MENU_COLUMN_1_X + 10, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 7 + 4, 129, GUI_SPINBOX_HEIGHT),
            false,
            m_CheckboxCheckedTexture,
            m_CheckboxCheckedTextureOnHover,
            m_CheckboxCheckedTextureOnPress,
            m_CheckboxUnCheckedTexture,
            m_CheckboxUnCheckedTextureOnHover,
            m_CheckboxUnCheckedTextureOnPress,
            m_MouseInput,
            m_ArialFont10,
            "Wall",
            Color.Black
         );
         m_SelectedFaceWallCheckbox.RegisterOnClick(SelectedFaceWallCheckbox_OnClick);
         m_SelectedFaceScoreboardCheckbox = new Checkbox(
            new Rectangle(GUI_LEFT_MENU_COLUMN_1_X + 10, GUI_SELECTED_QUAD_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, 129, GUI_SPINBOX_HEIGHT),
            false,
            m_CheckboxCheckedTexture,
            m_CheckboxCheckedTextureOnHover,
            m_CheckboxCheckedTextureOnPress,
            m_CheckboxUnCheckedTexture,
            m_CheckboxUnCheckedTextureOnHover,
            m_CheckboxUnCheckedTextureOnPress,
            m_MouseInput,
            m_ArialFont10,
            "Scoreboard",
            Color.Black
         );
         m_SelectedFaceScoreboardCheckbox.RegisterOnClick(SelectedFaceScoreboardCheckbox_OnClick);


         #endregion

         #region Gizmo Menu

         // Labels
         m_GizmoHeaderLabel = new Label("Gizmo Tool", GUI_MENU_COLUMN_1_X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_GizmoHeaderValueLabel = new Label("Translate", menuColumn2X, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Buttons
         int toolbuttonpadding = 2;
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
         m_NonUniformScaleButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED + toolbuttonpadding * 3 + GUI_GIZMO_BUTTON_WIDTH * 3, GUI_GIZMO_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_GIZMO_BUTTON_WIDTH, GUI_GIZMO_BUTTON_HEIGHT), m_NonUniformScaleButtonTexture, m_RightMenuMouseInput);
         m_NonUniformScaleButton.SetImage(Button.ImageType.OVER, m_NonUniformScaleButtonTextureOnHover);
         m_NonUniformScaleButton.SetImage(Button.ImageType.CLICK, m_NonUniformScaleButtonTextureOnPress);
         m_NonUniformScaleButton.RegisterOnClick(NonUniformScaleButton_OnClick);
         
         #endregion

         #region Building Menu

         // Labels
         m_BuildingHeaderLabel = new Label("Selected Building", GUI_MENU_COLUMN_1_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingFilepathLabel = new Label("Filepath", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingFilepathValueLabel = new Label("../../cube.fbx", menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingPositionXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingPositionYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingPositionZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingRotationXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingRotationYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingRotationZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingScaleLabel = new Label("Scale", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingScaleXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingScaleYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_BuildingScaleZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_BuildingSnapToGroundLabel = new Label("", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_BuildingColorLabel = new Label("Color", GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Spinboxes
         m_BuildingPositionXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingPositionYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingPositionZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 3 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingRotationZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 5 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingScaleZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 7 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.01f, 100.0f, 0.5f, "{0:0.00}", m_RightMenuMouseInput);
         m_BuildingPositionXSpinBox.RegisterOnValueChanged(BuildingPositionX_OnValueChanged);
         m_BuildingPositionYSpinBox.RegisterOnValueChanged(BuildingPositionY_OnValueChanged);
         m_BuildingPositionZSpinBox.RegisterOnValueChanged(BuildingPositionZ_OnValueChanged);
         m_BuildingRotationXSpinBox.RegisterOnValueChanged(BuildingRotationX_OnValueChanged);
         m_BuildingRotationYSpinBox.RegisterOnValueChanged(BuildingRotationY_OnValueChanged);
         m_BuildingRotationZSpinBox.RegisterOnValueChanged(BuildingRotationZ_OnValueChanged);
         m_BuildingScaleXSpinBox.RegisterOnValueChanged(BuildingScaleX_OnValueChanged);
         m_BuildingScaleYSpinBox.RegisterOnValueChanged(BuildingScaleY_OnValueChanged);
         m_BuildingScaleZSpinBox.RegisterOnValueChanged(BuildingScaleZ_OnValueChanged);
         m_BuildingPositionXSpinBox.Value = 0.0f;
         m_BuildingPositionYSpinBox.Value = 0.0f;
         m_BuildingPositionZSpinBox.Value = 0.0f;
         m_BuildingRotationXSpinBox.Value = 0.0f;
         m_BuildingRotationYSpinBox.Value = 0.0f;
         m_BuildingRotationZSpinBox.Value = 0.0f;
         m_BuildingScaleXSpinBox.Value = 0.0f;
         m_BuildingScaleYSpinBox.Value = 0.0f;
         m_BuildingScaleZSpinBox.Value = 0.0f;
         
         // Buttons
         //m_BuildingColorButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_COLOR_BUTTON_WIDTH - 4, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, GUI_COLOR_BUTTON_WIDTH, GUI_COLOR_BUTTON_HEIGHT), m_SquareButtonTexture, m_RightMenuMouseInput);
         //m_BuildingColorButton.SetImage(Button.ImageType.OVER, m_SquareButtonTextureOnHover);
         //m_BuildingColorButton.SetImage(Button.ImageType.CLICK, m_SquareButtonTextureOnPress);
         //m_BuildingColorButton.RegisterOnClick(BuildingColorButton_OnClick);
         m_BuildingResetButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Reset", Color.Black);
         m_BuildingResetButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_BuildingResetButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_BuildingResetButton.RegisterOnClick(BuildingResetButton_OnClick);
         m_BuildingDeleteButton = new Button(new Rectangle(menuColumn2X, GUI_BUILDING_Y + GUI_MENU_ITEM_HEIGHT * 8 + 4, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Delete", Color.Black);
         m_BuildingDeleteButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_BuildingDeleteButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_BuildingDeleteButton.RegisterOnClick(BuildingDeleteButton_OnClick);

         ToggleBuildingMenuEnabled(false);

         #endregion

         #region Projector Menu

         // Labels
         m_ProjectorHeaderLabel = new Label("Selected Projector", GUI_MENU_COLUMN_1_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorFOVLabel = new Label("Field of View", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 1, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorAspectRatioLabel = new Label("Aspect Ratio", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 2, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorPositionLabel = new Label("Position", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 3, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorPositionXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorPositionYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorPositionZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_ProjectorRotationLabel = new Label("Rotation", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_ProjectorRotationXLabel = new Label("X", GUI_MENU_X_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_ProjectorRotationYLabel = new Label("Y", GUI_MENU_Y_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         //m_ProjectorRotationZLabel = new Label("Z", GUI_MENU_Z_COLUMN_X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorAlphaLabel = new Label("Alpha", GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);
         m_ProjectorAlphaValueLabel = new Label("1.00", GUI_TOOLBAR_WIDTH - 32, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Spinboxes
         m_ProjectorPositionXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 4 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -100.0f, 100.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         //m_ProjectorRotationXSpinBox = new NumUpDown(new Rectangle(GUI_MENU_X_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         //m_ProjectorRotationYSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Y_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         //m_ProjectorRotationZSpinBox = new NumUpDown(new Rectangle(GUI_MENU_Z_COLUMN_X + 10, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, -360.0f, 360.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorFovSpinBox = new NumUpDown(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_SPINBOX_WIDTH - 6, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 1 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 1.0f, 90.0f, 1.0f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorAspectRatioSpinBox = new NumUpDown(new Rectangle(GUI_TOOLBAR_WIDTH - GUI_SPINBOX_WIDTH - 6, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 2 - 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, 0.1f, 10.0f, 0.1f, "{0:0.00}", m_RightMenuMouseInput);
         m_ProjectorPositionXSpinBox.RegisterOnValueChanged(ProjectorPositionX_OnValueChanged);
         m_ProjectorPositionYSpinBox.RegisterOnValueChanged(ProjectorPositionY_OnValueChanged);
         m_ProjectorPositionZSpinBox.RegisterOnValueChanged(ProjectorPositionZ_OnValueChanged);
         //m_ProjectorRotationXSpinBox.RegisterOnValueChanged(ProjectorRotationX_OnValueChanged);
         //m_ProjectorRotationYSpinBox.RegisterOnValueChanged(ProjectorRotationY_OnValueChanged);
         //m_ProjectorRotationZSpinBox.RegisterOnValueChanged(ProjectorRotationZ_OnValueChanged);
         m_ProjectorFovSpinBox.RegisterOnValueChanged(ProjectorFOV_OnValueChanged);
         m_ProjectorAspectRatioSpinBox.Value = 0.0f;
         m_ProjectorPositionXSpinBox.Value = 0.0f;
         m_ProjectorPositionYSpinBox.Value = 0.0f;
         m_ProjectorPositionZSpinBox.Value = 0.0f;
         //m_ProjectorRotationXSpinBox.Value = 0.0f;
         //m_ProjectorRotationYSpinBox.Value = 0.0f;
         //m_ProjectorRotationZSpinBox.Value = 0.0f;
         m_ProjectorFovSpinBox.Value = 0.0f;
         m_ProjectorAspectRatioSpinBox.Value = 0.0f;
         
         // Buttons
         m_ProjectorResetButton = new Button(new Rectangle(GUI_MENU_COLUMN_1_X_TABBED, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 + 4, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Reset", Color.Black);
         m_ProjectorResetButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ProjectorResetButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ProjectorResetButton.RegisterOnClick(ProjectorResetButton_OnClick);
         m_ProjectorDeleteButton = new Button(new Rectangle(menuColumn2X, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 6 + 4, GUI_QUIT_BUTTON_WIDTH, GUI_QUIT_BUTTON_HEIGHT), m_ButtonTexture, m_RightMenuMouseInput, m_ArialFont10, "Delete", Color.Black);
         m_ProjectorDeleteButton.SetImage(Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ProjectorDeleteButton.SetImage(Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ProjectorDeleteButton.RegisterOnClick(ProjectorDeleteButton_OnClick);

         // Sliders
         m_ProjectorAlphaSlider = new Slider(new Rectangle(60, GUI_PROJECTOR_Y + GUI_MENU_ITEM_HEIGHT * 5, 100, GUI_MENU_ITEM_HEIGHT), m_SliderBarTexture, m_SliderPadTexture, 0.0f, 1.0f, 0.05f, m_RightMenuMouseInput);
         m_ProjectorAlphaSlider.Value = 1.0f;

         ToggleProjectorMenuEnabled(false);

         #endregion

         #region Layers Menu

         // Scrollview
         int scrollHeight = GameConstants.WindowHeight - (GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 1 - 2 + 20);

         m_ScrollMouseInput = new MouseInput(new Vector2(-(GUI_TOOLBAR_X + 2), -(GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 1 - 2)));
         m_LayersScrollView = new LayerScrollView(m_Game, new Rectangle(GUI_TOOLBAR_X + 2, GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 1 - 2, GUI_TOOLBAR_WIDTH - 2, scrollHeight),
            new Rectangle(),
            m_ScrollViewBackgroundTexture,
            m_ScrollViewScrollPadTexture,
            m_ScrollViewScrollAreaTexture,
            m_WhiteTexture,
            m_ScrollMouseInput
         );
         m_LayersScrollView.RegisterOnEnterNormalSelectionMode(EnterNormalSelectionMode);

         // Labels
         m_LayersHeaderLabel = new Label("Input Layers", GUI_MENU_COLUMN_1_X, GUI_LAYERS_Y + GUI_MENU_ITEM_HEIGHT * 0, GUI_MENU_COLUMN_1_WIDTH, GUI_MENU_ITEM_HEIGHT, Color.Black, m_ArialFont10);

         // Buttons
         int btnWidth = 20;
         m_NewGameplayLayerButton = new Button(new Rectangle(2 + btnWidth * 0, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_NewTextureLayerTexture, m_RightMenuMouseInput);
         m_NewGameplayLayerButton.SetImage(Button.ImageType.OVER, m_NewTextureLayerTextureOnHover);
         m_NewGameplayLayerButton.SetImage(Button.ImageType.CLICK, m_NewTextureLayerTextureOnPress);
         m_NewGameplayLayerButton.RegisterOnClick(NewGameplayLayerButton_OnClick);
         /*m_NewParticleLayerButton = new Button(new Rectangle(2 + btnWidth * 1, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_NewParticleLayerTexture, m_RightMenuMouseInput);
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
         m_ShiftLayerDownButton.RegisterOnClick(ShiftLayerDownButton_OnClick);*/
         m_TrashLayerButton = new Button(new Rectangle(GUI_TOOLBAR_WIDTH - btnWidth * 1, GameConstants.WindowHeight - btnWidth, btnWidth, btnWidth), m_TrashTexture, m_RightMenuMouseInput);
         m_TrashLayerButton.SetImage(Button.ImageType.OVER, m_TrashTextureOnHover);
         m_TrashLayerButton.SetImage(Button.ImageType.CLICK, m_TrashTextureOnPress);
         m_TrashLayerButton.RegisterOnClick(TrashLayerButton_OnClick);

         // Initialize layers
         AssembleGameplayLayer();

         #endregion
      }

      #endregion

      #region Updating

      public override void Update(float elapsedTime)
      {
         if (m_DEM_GAMES)
         {
            m_ProjectorPreview.Update(elapsedTime);
         }
         else
         {
            if (m_UVGridEditor.HasUpdates)
            {
               if (m_ProjectorPreview.IsProjectorSelected)
               {
                  m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Grid = m_UVGridEditor.Grid;
                  m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].EdgeGraph.AssembleGraph(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Grid.GetIntersectionPoints());
                  m_UVDualEdgeGraphEditor.SetEdgeGraph(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].EdgeGraph);
                  m_UVGridEditor.HasUpdates = false;
               }
            }

            m_LayersScrollView.Update(elapsedTime);
            m_UVDualEdgeGraphEditor.Update(elapsedTime);
            m_ProjectorPreview.Update(elapsedTime);
            m_ProjectorPreview.ProjectorAlpha = m_ProjectorAlphaSlider.Value;

            // Apply color picker color to building
            if (m_ProjectorPreview.IsBuildingSelected)
            {
               //Material m = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Material;
               //m.Diffuse = m_BuildingColorPicker.SelectedColor.ToVector4();
               //m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Material = m;
            }

            // Sync gui with selected quad
            for (int i = 0; i < 4; ++i)
            {
               m_SelectedFaceUVSpinBox[i].IsEnabled = m_UVDualEdgeGraphEditor.IsQuadSelected;
            }
            m_SelectedFaceGameplayLayerSpinBox.IsActive = m_UVDualEdgeGraphEditor.IsQuadSelected;
            m_SelectedFaceWallCheckbox.IsActive = m_UVDualEdgeGraphEditor.IsQuadSelected;
            m_SelectedFaceScoreboardCheckbox.IsActive = m_UVDualEdgeGraphEditor.IsQuadSelected;
            m_ToLocalCoordButton.IsActive = m_UVDualEdgeGraphEditor.IsQuadSelected;
            if (m_UVDualEdgeGraphEditor.EdgeGraph != null)
            {
               if (m_ProjectorPreview.IsProjectorSelected)
               {
                  if (m_UVDualEdgeGraphEditor.RenderTargetTextures[m_ProjectorPreview.SelectedProjector] != null)
                  {
                     //Texture2D texture = new Texture2D(m_Game.GraphicsDevice, m_UVDualEdgeGraphEditor.RenderTargetTexture.Width, m_UVDualEdgeGraphEditor.RenderTargetTexture.Height);
                     //Color[] data = new Color[texture.Width * texture.Height];
                     //m_UVDualEdgeGraphEditor.RenderTargetTexture.GetData<Color>(data);
                     //texture.SetData<Color>(data);
                     m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Texture = m_UVDualEdgeGraphEditor.RenderTargetTextures[m_ProjectorPreview.SelectedProjector];
                  }
               }

               if (m_UVDualEdgeGraphEditor.IsQuadSelected)
               {
                  Vector2[] uvs = new Vector2[4];
                  uvs[0] = m_SelectedFaceUVSpinBox[0].TexCoord;
                  uvs[1] = m_SelectedFaceUVSpinBox[1].TexCoord;
                  uvs[2] = m_SelectedFaceUVSpinBox[2].TexCoord;
                  uvs[3] = m_SelectedFaceUVSpinBox[3].TexCoord;
                  m_UVDualEdgeGraphEditor.SetUVs(uvs);
               }
            }

            // Sync menu status
            m_ProjectorAlphaValueLabel.Text = String.Format("{0:0.00}", m_ProjectorAlphaSlider.Value);
            m_AppModeValueLabel.Text = (m_EditorMode) ? "Editor" : "Preview";
            m_GizmoHeaderValueLabel.Text = (m_ProjectorPreview.Gizmo.ActiveMode.ToString().Contains("Scale")) ? "Scale" : m_ProjectorPreview.Gizmo.ActiveMode.ToString();
            if (m_ProjectorPreview.IsProjectorSelected)
            {
               m_ProjectorPositionXSpinBox.Value = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position.X;
               m_ProjectorPositionYSpinBox.Value = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position.Y;
               m_ProjectorPositionZSpinBox.Value = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position.Z;
               //m_ProjectorRotationXSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotX);
               //m_ProjectorRotationYSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotY);
               //m_ProjectorRotationZSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotZ);
               m_ProjectorFovSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Fov);
               m_ProjectorAspectRatioSpinBox.Value = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].AspectRatio;
               m_ProjectorAlphaSlider.Value = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Alpha;
            }

            if (m_ProjectorPreview.IsBuildingSelected)
            {
               m_BuildingPositionXSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position.X;
               m_BuildingPositionYSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position.Y;
               m_BuildingPositionZSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position.Z;
               m_BuildingRotationXSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotX);
               m_BuildingRotationYSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotY);
               m_BuildingRotationZSpinBox.Value = MathHelper.ToDegrees(m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotZ);
               m_BuildingScaleXSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale.X;
               m_BuildingScaleYSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale.Y;
               m_BuildingScaleZSpinBox.Value = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale.Z;
            }
         }
      }

      private void ToggleBuildingMenuEnabled(bool enabled)
      {
         m_BuildingPositionXSpinBox.IsActive = enabled;
         m_BuildingPositionYSpinBox.IsActive = enabled;
         m_BuildingPositionZSpinBox.IsActive = enabled;
         m_BuildingRotationXSpinBox.IsActive = enabled;
         m_BuildingRotationYSpinBox.IsActive = enabled;
         m_BuildingRotationZSpinBox.IsActive = enabled;
         m_BuildingScaleXSpinBox.IsActive = enabled;
         m_BuildingScaleYSpinBox.IsActive = enabled;
         m_BuildingScaleZSpinBox.IsActive = enabled;
         //m_BuildingColorButton.IsActive = enabled;
         m_BuildingResetButton.IsActive = enabled;
         m_BuildingDeleteButton.IsActive = enabled;
      }

      private void ToggleProjectorMenuEnabled(bool enabled)
      {
         m_ProjectorPositionXSpinBox.IsActive = enabled;
         m_ProjectorPositionYSpinBox.IsActive = enabled;
         m_ProjectorPositionZSpinBox.IsActive = enabled;
         //m_ProjectorRotationXSpinBox.IsActive = enabled;
         //m_ProjectorRotationYSpinBox.IsActive = enabled;
         //m_ProjectorRotationZSpinBox.IsActive = enabled;
         m_ProjectorFovSpinBox.IsActive = enabled;
         m_ProjectorAspectRatioSpinBox.IsActive = enabled;
         m_ProjectorResetButton.IsActive = enabled;
         m_ProjectorDeleteButton.IsActive = enabled;
         m_ProjectorAlphaSlider.IsActive = enabled;
      }

      private void ToggleGizmoMenuEnabled(bool enabled)
      {
         m_ScaleButton.IsActive = enabled;
         m_RotateButton.IsActive = enabled;
         m_TranslateButton.IsActive = enabled;
      }

      private void ToggleAppMenuEnabled(bool enabled)
      {
         m_ResetButton.IsActive = enabled;
         //m_PlayButton.IsActive = enabled;
         //m_ModeButton.IsActive = enabled;
         m_QuitButton.IsActive = enabled;
         m_SaveButton.IsActive = enabled;
         m_OpenButton.IsActive = enabled;
      }

      private void ToggleSceneMenuEnabled(bool enabled)
      {
         m_NewMeshButton.IsActive = enabled;
      }

      private void ToggleSelectedFaceMenuEnabled(bool enabled)
      {
         m_ToLocalCoordButton.IsActive = enabled;
      }

      private void ToggleLayersMenuEnabled(bool enabled)
      {
         m_NewGameplayLayerButton.IsActive = enabled;
         m_TrashLayerButton.IsActive = enabled;
         m_LayersScrollView.IsActive = enabled;
         m_LayersScrollView.ToggleLayerControlsEnabled(enabled);
      }

      #endregion

      #region Input Handling

      public override void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         if (m_DEM_GAMES)
         {
            m_MouseInput.HandleInput(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Escape) && !m_PrevKeyboardState.IsKeyDown(Keys.Escape))
            {
               m_DEM_GAMES = false;
               m_ProjectorPreview.EditorMode = true;
               m_PlayButton.Text = "Play";
               ToggleAppMenuEnabled(true);
               ToggleSceneMenuEnabled(true);
               //int offsetX = GameConstants.WindowWidth - GameConstants.DEFAULT_WINDOW_WIDTH;
               //int offsetY = GameConstants.WindowHeight - GameConstants.DEFAULT_WINDOW_HEIGHT;
               //m_ProjectorPreview.Viewport = new Viewport(GUI_PROJECTOR_PREVIEW_X + 2, GUI_PROJECTOR_PREVIEW_Y + 20, GUI_PROJECTOR_PREVIEW_WIDTH + offsetX - 2, GUI_PROJECTOR_PREVIEW_HEIGHT + offsetY - 20);
               //m_ProjectorPreview.Camera.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
               //m_ProjectorPreview.Camera.UpdateProjection();
               //m_ProjectorPreview.Projector.AspectRatio = (float)m_ProjectorPreview.Viewport.Width / (float)m_ProjectorPreview.Viewport.Height;
               //m_ProjectorPreview.Projector.UpdateProjection();
               FiniteStateMachine.GetInstance().QuitGame();
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
                  m_ProjectorPreview.HandleInput(m_DEM_GAMES, elapsedTime);
            }
         }
         else
         {
            m_MouseInput.HandleInput(PlayerIndex.One);
            m_RightMenuMouseInput.HandleInput(PlayerIndex.One);
            m_ScrollMouseInput.HandleInput(PlayerIndex.One);

            // Update scroll view
            m_LayersScrollView.HandleInput(mouseState, m_PrevMouseState);

            // Update color picker if it is active
            //if (m_BuildingColorPicker.IsActive)
            //   m_BuildingColorPicker.HandleInput(mouseState, m_PrevMouseState);

            if (!m_ProjectorPreview.RenderNormals)
            {
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
               if (mouseState.X >= m_UVDualEdgeGraphEditor.Viewport.X && mouseState.X <= m_UVDualEdgeGraphEditor.Viewport.X + m_UVDualEdgeGraphEditor.Viewport.Width &&
                   mouseState.Y >= m_UVDualEdgeGraphEditor.Viewport.Y && mouseState.Y <= m_UVDualEdgeGraphEditor.Viewport.Y + m_UVDualEdgeGraphEditor.Viewport.Height)
               {
                  if (m_FocusedPane != 1 && mouseState.LeftButton == ButtonState.Pressed && m_PrevMouseState.LeftButton == ButtonState.Released)
                  {
                     m_FocusedPane = 1;
                     m_UVDualEdgeGraphEditor.PrevMouseState = m_PrevMouseState;
                  }

                  if (m_FocusedPane == 1)
                     m_UVDualEdgeGraphEditor.HandleInput(elapsedTime);
               }
               else
               {
                  m_UVDualEdgeGraphEditor.UnHoverQuads();
               }
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
                  m_ProjectorPreview.HandleInput(m_DEM_GAMES, elapsedTime);
            }
         }

         if (m_ProjectorPreview.RenderNormals)
         {
            if (mouseState.RightButton == ButtonState.Pressed)
               CancelNormalSelectionMode();
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      void AddLayer(LayerType layerType)
      {
         switch (layerType)
         {
            case LayerType.Gameplay:
               AssembleGameplayLayer();
               m_SelectedFaceGameplayLayerSpinBox.Max += 1;
               break;
         }
      }

      /// <summary>
      /// Enter normal selection mode so that the user can choose a normal from the mesh
      /// to use for lighting on a layer per layer basis.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void EnterNormalSelectionMode(object sender, EventArgs e)
      {
         ToggleAppMenuEnabled(false);
         ToggleBuildingMenuEnabled(false);
         ToggleProjectorMenuEnabled(false);
         ToggleGizmoMenuEnabled(false);
         ToggleLayersMenuEnabled(false);
         ToggleSelectedFaceMenuEnabled(false);
         m_ProjectorPreview.RenderNormals = true;
      }

      /// <summary>
      /// Leave normal selection mode knowing that we selected a normal from the mesh.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void LeaveNormalSelectionMode(object sender, EventArgs e)
      {
         ToggleAppMenuEnabled(true);
         ToggleBuildingMenuEnabled(true);
         ToggleProjectorMenuEnabled(true);
         ToggleGizmoMenuEnabled(true);
         ToggleLayersMenuEnabled(true);
         ToggleSelectedFaceMenuEnabled(true);
         m_ProjectorPreview.RenderNormals = false;
         m_LayersScrollView.Layers[m_LayersScrollView.SelectedLayer].Normal = m_ProjectorPreview.SelectedNormal;
      }

      /// <summary>
      /// Cancel normal selection mode via right click and discard any normal selections.
      /// </summary>
      void CancelNormalSelectionMode()
      {
         ToggleAppMenuEnabled(true);
         ToggleBuildingMenuEnabled(true);
         ToggleProjectorMenuEnabled(true);
         ToggleGizmoMenuEnabled(true);
         ToggleLayersMenuEnabled(true);
         ToggleSelectedFaceMenuEnabled(true);
         m_ProjectorPreview.RenderNormals = false;
      }

      void Quad_OnSelected(object sender, EventArgs e)
      {
         m_SelectedFaceUVSpinBox[0].TexCoord = m_UVDualEdgeGraphEditor.SelectedQuadTexCoords[0];
         m_SelectedFaceUVSpinBox[1].TexCoord = m_UVDualEdgeGraphEditor.SelectedQuadTexCoords[1];
         m_SelectedFaceUVSpinBox[2].TexCoord = m_UVDualEdgeGraphEditor.SelectedQuadTexCoords[2];
         m_SelectedFaceUVSpinBox[3].TexCoord = m_UVDualEdgeGraphEditor.SelectedQuadTexCoords[3];
         m_SelectedFaceGameplayLayerSpinBox.Value = (m_UVDualEdgeGraphEditor.SelectedInputLayer + 1);
         m_SelectedFaceWallCheckbox.IsChecked = m_UVDualEdgeGraphEditor.SelectedQuadIsWall;
         m_SelectedFaceScoreboardCheckbox.IsChecked = m_UVDualEdgeGraphEditor.SelectedQuadIsScoreboard;

         for (int i = 0; i < 4; ++i)
         {
            m_SelectedFaceUVSpinBox[i].IsEnabled = m_UVDualEdgeGraphEditor.IsQuadSelected;
         }
         m_SelectedFaceGameplayLayerSpinBox.IsActive = m_UVDualEdgeGraphEditor.IsQuadSelected;
      }

      void Gizmo_OnDeSelect(object sender, EventArgs e)
      {
         if (!m_ProjectorPreview.IsProjectorID(m_ProjectorPreview.Gizmo.SelectedID))
         {
            ToggleProjectorMenuEnabled(false);
            m_UVGridEditor.RemoveGrid();
            m_UVDualEdgeGraphEditor.DumpEdgeGraph();
            m_UVDualEdgeGraphEditor.DeselectQuads();
         }
         if (!m_ProjectorPreview.IsBuildingID(m_ProjectorPreview.Gizmo.SelectedID))
         {
            ToggleBuildingMenuEnabled(false);
         }
      }

      void Gizmo_OnSelect(object sender, EventArgs e)
      {
         // Projector selection
         if (m_ProjectorPreview.IsProjectorID(m_ProjectorPreview.Gizmo.SelectedID))
         {
            ToggleProjectorMenuEnabled(true);
            m_ProjectorPreview.SelectProjector(m_ProjectorPreview.Gizmo.SelectedID);

            if (m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.UniformScale || 
                m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.NonUniformScale)
               m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;

            m_UVGridEditor.SetGrid(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Grid);
            m_UVDualEdgeGraphEditor.SetEdgeGraph(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].EdgeGraph);
            m_UVGridEditor.SetRenderTargetViewport(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Viewport);
            m_UVDualEdgeGraphEditor.SetRenderTargetViewport(m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Viewport, m_ProjectorPreview.SelectedProjector);

            if (m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].IsOn)
            {
               m_ProjectorResetButton.Text = "Turn Off";
            }
            else
            {
               m_ProjectorResetButton.Text = "Turn On";
            }
         }
         else
         {
            m_ProjectorPreview.DeSelectProjector();
            ToggleProjectorMenuEnabled(false);
         }

         // Building selection
         if (m_ProjectorPreview.IsBuildingID(m_ProjectorPreview.Gizmo.SelectedID))
         {
            ToggleBuildingMenuEnabled(true);
            m_ProjectorPreview.SelectBuilding(m_ProjectorPreview.Gizmo.SelectedID);
         }
         else
         {
            m_ProjectorPreview.DeSelectBuilding();
            ToggleBuildingMenuEnabled(false);
         }

         if (m_ProjectorPreview.Gizmo.SelectedID == m_ProjectorPreview.LightEntity.ID)
         {
            if (m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.UniformScale ||
                m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.NonUniformScale ||
                m_ProjectorPreview.Gizmo.ActiveMode == GizmoMode.Rotate)
            {
               m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;
            }
         }
      }

      void SelectedFaceWallCheckbox_OnClick(object sender, EventArgs e)
      {
         if (m_UVDualEdgeGraphEditor.IsQuadSelected)
         {
            m_UVDualEdgeGraphEditor.SelectedQuadIsWall = m_SelectedFaceWallCheckbox.IsChecked;
         }
      }

      void SelectedFaceScoreboardCheckbox_OnClick(object sender, EventArgs e)
      {
         if (m_UVDualEdgeGraphEditor.IsQuadSelected)
         {
            m_UVDualEdgeGraphEditor.SelectedQuadIsScoreboard = m_SelectedFaceScoreboardCheckbox.IsChecked;
         }
      }

      #region SpinBoxes

      private void SelectedFaceInputLayer_OnValueChanged(object sender, EventArgs e)
      {
         if (m_UVDualEdgeGraphEditor.IsQuadSelected)
         {
            m_UVDualEdgeGraphEditor.SelectedInputLayer = ((int)m_SelectedFaceGameplayLayerSpinBox.Value - 1);
         }
      }
      private void BuildingPositionX_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 pos = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position;
            pos.X = m_BuildingPositionXSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position = pos;
         }
      }
      private void BuildingPositionY_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 pos = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position;
            pos.Y = m_BuildingPositionYSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position = pos;
         }
      }
      private void BuildingPositionZ_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 pos = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position;
            pos.Z = m_BuildingPositionZSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Position = pos;
         }
      }
      private void BuildingRotationX_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotX = MathHelper.ToRadians(m_BuildingRotationXSpinBox.Value);
         }
      }
      private void BuildingRotationY_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotY = MathHelper.ToRadians(m_BuildingRotationYSpinBox.Value);
         }
      }
      private void BuildingRotationZ_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].RotZ = MathHelper.ToRadians(m_BuildingRotationZSpinBox.Value);
         }
      }
      private void BuildingScaleX_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 scale = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale;
            scale.X = m_BuildingScaleXSpinBox.Value;
            scale.Y = m_BuildingScaleYSpinBox.Value;
            scale.Z = m_BuildingScaleZSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale = scale;
         }
      }
      private void BuildingScaleY_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 scale = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale;
            scale.X = m_BuildingScaleXSpinBox.Value;
            scale.Y = m_BuildingScaleYSpinBox.Value;
            scale.Z = m_BuildingScaleZSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale = scale;
         }
      }
      private void BuildingScaleZ_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            Vector3 scale = m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale;
            scale.X = m_BuildingScaleXSpinBox.Value;
            scale.Y = m_BuildingScaleYSpinBox.Value;
            scale.Z = m_BuildingScaleZSpinBox.Value;
            m_ProjectorPreview.Buildings[m_ProjectorPreview.SelectedBuilding].Scale = scale;
         }
      }
      private void ProjectorPositionX_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            Vector3 pos = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position;
            pos.X = m_ProjectorPositionXSpinBox.Value;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position = pos;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.Position = pos;
         }
      }
      private void ProjectorPositionY_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            Vector3 pos = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position;
            pos.Y = m_ProjectorPositionYSpinBox.Value;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position = pos;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.Position = pos;
         }
      }
      private void ProjectorPositionZ_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            Vector3 pos = m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position;
            pos.Z = m_ProjectorPositionZSpinBox.Value;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Position = pos;
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.Position = pos;
         }
      }
      /*private void ProjectorRotationX_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotX = MathHelper.ToRadians(m_ProjectorRotationXSpinBox.Value);
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.RotX = MathHelper.ToRadians(m_ProjectorRotationXSpinBox.Value);
         }
      }
      private void ProjectorRotationY_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotY = MathHelper.ToRadians(m_ProjectorRotationYSpinBox.Value);
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.RotY = MathHelper.ToRadians(m_ProjectorRotationYSpinBox.Value);
         }
      }
      private void ProjectorRotationZ_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].RotZ = MathHelper.ToRadians(m_ProjectorRotationZSpinBox.Value);
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Entity.RotZ = MathHelper.ToRadians(m_ProjectorRotationZSpinBox.Value);
         }
      }*/

      private void ProjectorAspectRatio_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].AspectRatio = m_ProjectorAspectRatioSpinBox.Value;
         }
      }
      private void ProjectorFOV_OnValueChanged(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Fov = MathHelper.ToRadians(m_ProjectorFovSpinBox.Value);
         }
      }

      #endregion

      #region Buttons

      /*private void NewProjectorButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.AddProjector();

         int lastIndex = m_ProjectorPreview.Projectors.Count - 1;
         m_ProjectorPreview.Projectors[lastIndex].Grid.Reset(m_UVGridEditor.Viewport.Width, m_UVGridEditor.Viewport.Height);
         m_ProjectorPreview.Projectors[lastIndex].EdgeGraph.AssembleGraph(m_ProjectorPreview.Projectors[lastIndex].Grid.GetIntersectionPoints());
      }*/

      private void NewMeshButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.AddBuilding();
      }

      private void ToLocalCoordButton_OnClick(object sender, EventArgs e)
      {
         // Sync uv coords with selected quad
         if (m_UVDualEdgeGraphEditor.IsQuadSelected)
         {
            m_SelectedFaceUVSpinBox[0].TexCoord = new Vector2(0, 0);
            m_SelectedFaceUVSpinBox[1].TexCoord = new Vector2(0, 1);
            m_SelectedFaceUVSpinBox[2].TexCoord = new Vector2(1, 1);
            m_SelectedFaceUVSpinBox[3].TexCoord = new Vector2(1, 0);

            Vector2[] uvs = new Vector2[4];
            uvs[0] = m_SelectedFaceUVSpinBox[0].TexCoord;
            uvs[1] = m_SelectedFaceUVSpinBox[1].TexCoord;
            uvs[2] = m_SelectedFaceUVSpinBox[2].TexCoord;
            uvs[3] = m_SelectedFaceUVSpinBox[3].TexCoord;
            m_UVDualEdgeGraphEditor.SetUVs(uvs);
         }
      }

      private void NewGameplayLayerButton_OnClick(object sender, EventArgs e)
      {
         AddLayer(LayerType.Gameplay);
      }

      private void TrashLayerButton_OnClick(object sender, EventArgs e)
      {
         if (m_LayersScrollView.SelectedLayer > 0)
         {
            m_UVDualEdgeGraphEditor.DeleteLayer(m_LayersScrollView.SelectedLayer);
            m_LayersScrollView.DeleteSelectedLayer();
            m_SelectedFaceGameplayLayerSpinBox.Max -= 1;
            if (m_UVDualEdgeGraphEditor.IsQuadSelected)
               m_SelectedFaceGameplayLayerSpinBox.Value = (m_UVDualEdgeGraphEditor.SelectedInputLayer + 1);
         }
      }

      private void BuildingResetButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.IsBuildingSelected)
         {
            m_ProjectorPreview.ResetSelectedBuilding();
         }
      }

      private void BuildingDeleteButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Buildings.Count > 1 && m_ProjectorPreview.IsBuildingSelected)
         {
            m_ProjectorPreview.DeleteBuilding(m_ProjectorPreview.SelectedBuilding);

            ToggleBuildingMenuEnabled(false);
         }
      }

      /*
      private void BuildingColorButton_OnClick(object sender, EventArgs e)
      {
         if (m_BuildingColorPicker.IsActive)
         {
            m_BuildingColorPicker.HidePickerDialog();
            m_BuildingDeleteButton.Location += new Vector2(0, -80);
            m_BuildingResetButton.Location += new Vector2(0, -80);
            m_BuildingDeleteButton.TextPos += new Vector2(0, -80);
            m_BuildingResetButton.TextPos += new Vector2(0, -80);
            ShiftProjectionPanelContents(new Vector2(0, -80));
            ShiftLayerPanelContents(new Vector2(0, -80));
         }
         else
         {
            m_BuildingColorPicker.ShowPickerDialog();
            m_BuildingDeleteButton.Location += new Vector2(0, 80);
            m_BuildingResetButton.Location += new Vector2(0, 80);
            m_BuildingDeleteButton.TextPos += new Vector2(0, 80);
            m_BuildingResetButton.TextPos += new Vector2(0, 80);
            ShiftProjectionPanelContents(new Vector2(0, 80));
            ShiftLayerPanelContents(new Vector2(0, 80));
            
         }
      }
      */

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
         //m_ProjectorRotationLabel.Location += offset;
         //m_ProjectorRotationXLabel.Location += offset;
         //m_ProjectorRotationYLabel.Location += offset;
         //m_ProjectorRotationZLabel.Location += offset;
         //m_ProjectorRotationXSpinBox.Location += offset;
         //m_ProjectorRotationXSpinBox.Children[0].Location += offset;
         //m_ProjectorRotationXSpinBox.Children[1].Location += offset;
         //m_ProjectorRotationYSpinBox.Location += offset;
         //m_ProjectorRotationYSpinBox.Children[0].Location += offset;
         //m_ProjectorRotationYSpinBox.Children[1].Location += offset;
         //m_ProjectorRotationZSpinBox.Location += offset;
         //m_ProjectorRotationZSpinBox.Children[0].Location += offset;
         //m_ProjectorRotationZSpinBox.Children[1].Location += offset;
         m_ProjectorResetButton.Location += offset;
         m_ProjectorResetButton.TextPos += offset;
         m_ProjectorDeleteButton.Location += offset;
         m_ProjectorDeleteButton.TextPos += offset;
         m_ProjectorAlphaLabel.Location += offset;
         m_ProjectorAlphaSlider.Location += offset;
         m_ProjectorAlphaValueLabel.Location += offset;
      }

      private void ProjectorResetButton_OnClick(object sender, EventArgs e)
      {
      }

      private void ProjectorDeleteButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Projectors.Count > 1 && m_ProjectorPreview.IsProjectorSelected)
         {
            m_ProjectorPreview.DeleteProjector(m_ProjectorPreview.SelectedProjector);

            ToggleProjectorMenuEnabled(false);
            m_UVGridEditor.RemoveGrid();
            m_UVDualEdgeGraphEditor.DumpEdgeGraph();
            m_UVDualEdgeGraphEditor.DeselectQuads();
            
         }
      }
      
      private void TranslateButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Translate;
      }

      private void NonUniformScaleButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.NonUniformScale;
      }

      private void ScaleButton_OnClick(object sender, EventArgs e)
      {
         if (!m_ProjectorPreview.IsProjectorID(m_ProjectorPreview.Gizmo.SelectedID) &&
             m_ProjectorPreview.Gizmo.SelectedID != m_ProjectorPreview.LightEntity.ID)
            m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.UniformScale;
      }

      private void RotateButton_OnClick(object sender, EventArgs e)
      {
         if (m_ProjectorPreview.Gizmo.SelectedID != m_ProjectorPreview.LightEntity.ID)
            m_ProjectorPreview.Gizmo.ActiveMode = GizmoMode.Rotate;
      }

      /*private void ModeButton_OnClick(object sender, EventArgs e)
      {
         m_EditorMode = !m_EditorMode;
         if (m_EditorMode)
         {
            //m_ModeButton.Text = "Preview";
            m_ProjectorPreview.EditorMode = true;
         }
         else
         {
            //m_ModeButton.Text = "Editor";
            m_ProjectorPreview.EditorMode = false;
         }
      }*/

      private void PlayButton_OnClick(object sender, EventArgs e)
      {
         if (!m_DEM_GAMES)
         {
            // Play da gamez
            m_DEM_GAMES = true;
            m_PlayButton.Text = "Pause";

            FiniteStateMachine.GetInstance().StartGame();

            m_UVGridEditor.RemoveGrid();
            m_UVDualEdgeGraphEditor.DumpEdgeGraph();
            m_UVDualEdgeGraphEditor.DeselectAll();
            ToggleSceneMenuEnabled(false);
            ToggleAppMenuEnabled(false);
            ToggleSelectedFaceMenuEnabled(false);
            m_ProjectorPreview.DeSelectProjector();
            m_ProjectorPreview.DeSelectBuilding();
            m_ProjectorPreview.Gizmo.DeselectAll();
         }
         else
         {
            m_DEM_GAMES = false;
            m_ProjectorPreview.EditorMode = true;

            m_PlayButton.Text = "Play";
            ToggleAppMenuEnabled(true);
            ToggleSceneMenuEnabled(true);

            FiniteStateMachine.GetInstance().QuitGame();
         }
      }

      private void ResetButton_OnClick(object sender, EventArgs e)
      {
         Reset();
      }

      private void SaveButton_OnClick(object sender, EventArgs e)
      {
         System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
         save.Filter = "Xml|*.xml";
         save.Title = "Save editor configuration";
         save.ShowDialog();

         if (save.FileName != "")
         {
            SaveEditorConfig(save.FileName);
         }
      }

      private void OpenButton_OnClick(object sender, EventArgs e)
      {
         System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
         open.Filter = "Xml|*.xml";
         open.Title = "Open editor configuration";
         
         if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
         {
            LoadEditorConfig(open.FileName);
         }
      }

      private void QuitButton_OnClick(object sender, EventArgs e)
      {

         m_Game.GraphicsManager.PreferredBackBufferWidth = GameConstants.DEFAULT_WINDOW_WIDTH;
         m_Game.GraphicsManager.PreferredBackBufferHeight = GameConstants.DEFAULT_WINDOW_HEIGHT;
         m_Game.GraphicsManager.PreferMultiSampling = true;
         m_Game.GraphicsManager.ApplyChanges();

         FiniteStateMachine.GetInstance().ResizeGame(m_Game.GraphicsManager.PreferredBackBufferWidth, m_Game.GraphicsManager.PreferredBackBufferHeight);
   //      GameConstants.WindowWidth = m_Game.GraphicsManager.PreferredBackBufferWidth;
 //        GameConstants.WindowHeight = m_Game.GraphicsManager.PreferredBackBufferHeight;

         // Allow for exit to main menu
         FiniteStateMachine.GetInstance().SetState(StateType.MainMenu);
         FiniteStateMachine.GetInstance().QuitEditor();

      }

      #endregion

      #endregion

      #region Rendering

      public override void Draw(SpriteBatch spriteBatch)
      {
         Viewport defaultViewport = m_Game.GraphicsDevice.Viewport;

         UVDualEdgeGraph graph = m_UVDualEdgeGraphEditor.EdgeGraph;
         int selectedQuad = m_UVDualEdgeGraphEditor.SelectedQuadIndex;
         int selectedVert = m_UVDualEdgeGraphEditor.SelectedVertIndex;
         int selectedEdge = m_UVDualEdgeGraphEditor.SelectedEdgeIndex;
         int hoveredQuad = m_UVDualEdgeGraphEditor.HoveredQuadIndex;
         int hoveredVert = m_UVDualEdgeGraphEditor.HoveredVertIndex;
         int hoveredEdge = m_UVDualEdgeGraphEditor.HoveredEdgeIndex;
         m_UVDualEdgeGraphEditor.SelectedQuadIndex = -1;
         m_UVDualEdgeGraphEditor.SelectedVertIndex = -1;
         m_UVDualEdgeGraphEditor.SelectedEdgeIndex = -1;
         m_UVDualEdgeGraphEditor.HoveredQuadIndex = -1;
         m_UVDualEdgeGraphEditor.HoveredVertIndex = -1;
         m_UVDualEdgeGraphEditor.HoveredEdgeIndex = -1;
         for (int i = 0; i < m_ProjectorPreview.Projectors.Count; ++i)
         {
            m_UVDualEdgeGraphEditor.DumpEdgeGraph();
            m_UVDualEdgeGraphEditor.SetEdgeGraph(m_ProjectorPreview.Projectors[i].EdgeGraph);
            m_UVDualEdgeGraphEditor.SetRenderTargetViewport(m_ProjectorPreview.Projectors[i].Viewport, i);
            m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Projectors[i].Viewport;

            //m_UVDualEdgeGraphEditor.SetRenderTargetTexture(m_ProjectorPreview.Projectors[i].Texture);
            m_UVDualEdgeGraphEditor.DrawRenderTarget(spriteBatch, i, false);
            
            m_ProjectorPreview.Projectors[i].Texture = m_UVDualEdgeGraphEditor.RenderTargetTextures[i];
         }
         m_UVDualEdgeGraphEditor.DumpEdgeGraph();
         m_UVDualEdgeGraphEditor.SetEdgeGraph(graph);

         m_UVDualEdgeGraphEditor.SelectedQuadIndex = selectedQuad;
         m_UVDualEdgeGraphEditor.SelectedVertIndex = selectedVert;
         m_UVDualEdgeGraphEditor.SelectedEdgeIndex = selectedEdge;
         m_UVDualEdgeGraphEditor.HoveredQuadIndex = hoveredQuad;
         m_UVDualEdgeGraphEditor.HoveredVertIndex = hoveredVert;
         m_UVDualEdgeGraphEditor.HoveredEdgeIndex = hoveredEdge;

         if (m_ProjectorPreview.RenderNormals)
         {
            m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Viewport;
            m_ProjectorPreview.DrawRenderTarget(spriteBatch);
         }

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
         RenderApplicationMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_SCENE_Y - 2, GUI_LEFT_TOOLBAR_WIDTH, 2), Color.Black);
         RenderSceneMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_SELECTED_QUAD_Y - 2, GUI_LEFT_TOOLBAR_WIDTH, 2), Color.Black);
         RenderSelectedQuadMenu(spriteBatch);
         spriteBatch.End();

         // Render the UV editor
         m_Game.GraphicsDevice.Viewport = m_UVDualEdgeGraphEditor.Viewport;
         m_UVDualEdgeGraphEditor.Draw(spriteBatch);

         // Render the UV grid editor
         m_Game.GraphicsDevice.Viewport = m_UVGridEditor.Viewport;
         m_UVGridEditor.Draw(spriteBatch);

         // Right menu
         m_Game.GraphicsDevice.Viewport = m_RightMenuViewport;
         spriteBatch.Begin();
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, GUI_TOOLBAR_WIDTH, m_RightMenuViewport.Height), Color.Gray);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, 0, 2, m_RightMenuViewport.Height), Color.Black);
         RenderGizmoMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_GIZMO_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
         RenderBuildingMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_BUILDING_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
         RenderProjectorMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_PROJECTOR_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
         RenderLayersMenu(spriteBatch);
         spriteBatch.Draw(m_WhiteTexture, new Rectangle(0, GUI_LAYERS_Y - 2, GUI_TOOLBAR_WIDTH, 2), Color.Black);
         spriteBatch.End();

         // Render color picker if it is active
         //if (m_BuildingColorPicker.IsActive)
         //{
         //   m_Game.GraphicsDevice.Viewport = m_BuildingColorPicker.Viewport;
         //   m_BuildingColorPicker.Draw(m_Game.GraphicsDevice, spriteBatch);
         //}

         // Render the projector preview
         m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Viewport;
         m_ProjectorPreview.Draw(spriteBatch, m_DEM_GAMES);

         // Render each projector window
         for (int i = 0; i < m_ProjectorPreview.Projectors.Count; ++i)
         {
            m_Game.GraphicsDevice.Viewport = m_ProjectorPreview.Projectors[i].Viewport;
               
            if (m_ProjectorPreview.Projectors[i].Texture != null)
            {
               spriteBatch.Begin();
               spriteBatch.Draw(m_ProjectorPreview.Projectors[i].Texture, new Rectangle(0, 0, m_ProjectorPreview.Projectors[i].Viewport.Width, m_ProjectorPreview.Projectors[i].Viewport.Height), Color.White);
               spriteBatch.End();
            }
         }

         // Restore the default viewport
         m_Game.GraphicsDevice.Viewport = defaultViewport;
      }

      private void RenderSceneMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_SCENE_Y, GUI_LEFT_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_SceneHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_NewMeshButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_NewProjectorButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderSelectedQuadMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(GUI_LEFT_TOOLBAR_X + 2, GUI_SELECTED_QUAD_Y, GUI_LEFT_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_SelectedFaceHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceTexCoordLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP0Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP1Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP2Label.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceP3Label.Draw(m_Game.GraphicsDevice, spriteBatch);

         for (int i = 0; i < 4; ++i)
         {
            m_SelectedFaceUVSpinBox[i].Draw(m_Game.GraphicsDevice, spriteBatch);
         }
         if (m_UVDualEdgeGraphEditor.IsQuadSelected && m_UVDualEdgeGraphEditor.SelectedQuadIsWall)
            m_SelectedFaceGameplayLayerSpinBox.IsActive = false;
         m_SelectedFaceGameplayLayerSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceGameplayLayerLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceWallCheckbox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SelectedFaceScoreboardCheckbox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ToLocalCoordButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderApplicationMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_APP_Y, GUI_LEFT_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_AppHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ModeButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_PlayButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ResetButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_QuitButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_SaveButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_OpenButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderGizmoMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_GIZMO_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_GizmoHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_GizmoHeaderValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_TranslateButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_RotateButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ScaleButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_NonUniformScaleButton.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderBuildingMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_BUILDING_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
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
         //m_BuildingSnapToGroundLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_BuildingColorLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_BuildingColorButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingDeleteButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_BuildingResetButton.Draw(m_Game.GraphicsDevice, spriteBatch); 
         //spriteBatch.Draw(m_ColorPickerPreviewPadTexture, new Rectangle((int)m_BuildingColorButton.Location.X + 4, (int)m_BuildingColorButton.Location.Y + 4, m_BuildingColorButton.Width - 8, m_BuildingColorButton.Height - 8), m_BuildingColorPicker.SelectedColor);
         
      }

      private void RenderProjectorMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_PROJECTOR_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
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
         //m_ProjectorRotationLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationXLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationYLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationZLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationXSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationYSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ProjectorRotationZSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorResetButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorDeleteButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaSlider.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorAlphaValueLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
      }

      private void RenderLayersMenu(SpriteBatch spriteBatch)
      {
         RenderGradient(new Rectangle(2, GUI_LAYERS_Y, GUI_TOOLBAR_WIDTH - 2, 18), spriteBatch, Color.Gray, Color.LightGray);
         m_LayersHeaderLabel.Draw(m_Game.GraphicsDevice, spriteBatch);

         m_LayersScrollView.Draw(m_Game.GraphicsDevice, spriteBatch);

         m_NewGameplayLayerButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_NewParticleLayerButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ShiftLayerDownButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         //m_ShiftLayerUpButton.Draw(m_Game.GraphicsDevice, spriteBatch);
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

      #region Saving/Loading

      bool LoadEditorConfig(string filename)
      {
         try
         {
            // Open the file
            FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);

            // Read the data from the file
            XmlSerializer serializer = new XmlSerializer(typeof(EditorSaveData));
            EditorSaveData data = (EditorSaveData)serializer.Deserialize(stream);

            // Close the file
            stream.Close();

            //
            // OK, first of all, I'm pretty drunk.  THis is the end of the semester and 
            // the movie exec on Southpark just walked into the bathroom to find Jesus
            // waiting to bully him.  "My child, have you ever heard of a place called Hell?
            // It's eternal fire and it's gonna hurt real bad."  HA!!!
            // So................................................
            //
            // We need to verify that the config we are loading has the same amount of projectors
            // attached as it was saved from.  More than that, we need to make sure that they are
            // the same resolution.  No dice otherwise...
            //
            System.Windows.Forms.Screen[] screens = GetOrderedScreens();
            int numScreensInConfig = data.WindowData.Count;
            if (screens.Length != numScreensInConfig) return false;
            for (int i = 0; i < numScreensInConfig; ++i)
            {
               // Dude!  My mouse just died.  The battery I mean.  No worries.  And it's an MX revolution
               // so you get this sweet integrated lithium ion battery that you just recharge.  Win right?
               // But I have to wait now....another southpark?

               // WAIT!!! WE DON'T NEED A MOUSE TO TYPE!!!! HAHAHA.  That only took me like...3 lines of 
               // typing.  Lol
               if (screens[i].Bounds.Width != data.WindowData[i].Width ||
                   screens[i].Bounds.Height != data.WindowData[i].Height)
               {
                  return false;
               }
            }

            // OK so we passed the verification test.  Now we just need to load what the file
            // contained into the editor.
            Reset();
            m_ProjectorPreview.Projectors.Clear();
            m_ProjectorPreview.Buildings.Clear();

            // Load buildings
            int numBuildingsInConfig = data.BuildingData.Count;
            for (int i = 0; i < numBuildingsInConfig; ++i)
            {
               int id = data.BuildingData[i].ID;
               EntityType type = data.BuildingData[i].Type;
               string meshFilename = data.BuildingData[i].MeshFilename;
               Vector3 position = data.BuildingData[i].Position;
               Vector3 scale = data.BuildingData[i].Scale;
               float rotX = data.BuildingData[i].RotX;
               float rotY = data.BuildingData[i].RotY;
               float rotZ = data.BuildingData[i].RotZ;
               m_ProjectorPreview.AddBuilding(id, type, meshFilename, position, scale, rotX, rotY, rotZ);
            }

            // Load layers
            int numLayersInConfig = data.LayerData.Count;
            m_LayersScrollView.Layers = new List<Layer>();
            for (int i = 0; i < numLayersInConfig; ++i)
            {
               Label nameLabel = new Label("Gameplay " + (m_LayersScrollView.NumLayers + 1), Layer.NAME_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.LAYER_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               Label widthLabel = new Label("Width", Layer.WIDTH_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT, Layer.WIDTH_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               Label heightLabel = new Label("Height", Layer.HEIGHT_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               Label normalsLabel = new Label("Normal", Layer.NORMALS_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               Label linkLabel = new Label("Linked Layers:", Layer.LINKS_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               Label linkListLabel = new Label("None", Layer.LINKS_LIST_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
               NumUpDown widthSpinBox = new NumUpDown(new Rectangle(Layer.WIDTH_SPINBOX_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT - 2, 40, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, GameConstants.TILE_DIM, GameConstants.TILE_DIM * GameConstants.MAX_TILES_WIDE, GameConstants.TILE_DIM, "{0:0}", m_ScrollMouseInput);
               NumUpDown heightSpinBox = new NumUpDown(new Rectangle(Layer.HEIGHT_SPINBOX_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT - 2, 40, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, GameConstants.TILE_DIM, GameConstants.TILE_DIM * GameConstants.MAX_TILES_HIGH, GameConstants.TILE_DIM, "{0:0}", m_ScrollMouseInput);
               widthSpinBox.Value = GameConstants.TILE_DIM * GameConstants.DEFAULT_TILES_WIDE;
               heightSpinBox.Value = GameConstants.TILE_DIM * GameConstants.DEFAULT_TILES_HIGH;

               Button linkBtn = new Button(new Rectangle(Layer.LINK_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2 + 2, Layer.LINK_BUTTON_WIDTH, Layer.LINK_BUTTON_HEIGHT), m_LinkButtonTexture, m_ScrollMouseInput);
               linkBtn.SetImage(Button.ImageType.OVER, m_LinkButtonTextureOnHover);
               linkBtn.SetImage(Button.ImageType.CLICK, m_LinkButtonTextureOnPress);
               Button editBtn = new Button(new Rectangle(Layer.EDIT_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.EDIT_BUTTON_WIDTH, Layer.EDIT_BUTTON_HEIGHT), m_EditLayerButtonTexture, m_ScrollMouseInput);
               editBtn.SetImage(Button.ImageType.OVER, m_EditLayerButtonTextureOnHover);
               editBtn.SetImage(Button.ImageType.CLICK, m_EditLayerButtonTextureOnPress);

               GameplayLayer l = new GameplayLayer(nameLabel, widthLabel, heightLabel, normalsLabel, linkLabel, linkListLabel, widthSpinBox, heightSpinBox, editBtn, linkBtn, m_ColorPickerPreviewPadTexture);
               l.TYPE_ID = data.LayerData[i].TYPE_ID;
               l.Type = data.LayerData[i].Type;
               l.Name = data.LayerData[i].LayerName;
               l.LinkedLayers = data.LayerData[i].LinkedLayers;
               l.Normal = data.LayerData[i].Normal;
               m_LayersScrollView.Layers.Add(l);
            }

            // Load projectors
            int numProjectorsInConfig = data.ProjectorData.Count;
            for (int i = 0; i < numProjectorsInConfig; ++i)
            {
               float alpha = data.ProjectorData[i].Alpha;
               float aspectRatio = data.ProjectorData[i].AspectRatio;
               float far = data.ProjectorData[i].FarPlane;
               float near = data.ProjectorData[i].NearPlane;
               float fov = data.ProjectorData[i].Fov;
               UVDualEdgeGraph edgeGraph = new UVDualEdgeGraph(m_Game.Content.Load<Texture2D>("Textures/Layer0_2"));
               int numEdgesInConfig = data.ProjectorData[i].EdgeGraphData.EdgeData.Count;
               int numVerticesInConfig = data.ProjectorData[i].EdgeGraphData.VertexData.Count;
               int numQuadsInConfig = data.ProjectorData[i].EdgeGraphData.QuadData.Count;
               edgeGraph.Edges = new List<UVEdge>();
               edgeGraph.Quads = new List<UVQuad>();
               edgeGraph.Vertices = new List<UVVertex>();
               for (int j = 0; j < numEdgesInConfig; ++j)
               {
                  UVEdge e = new UVEdge(data.ProjectorData[i].EdgeGraphData.EdgeData[j].P1, data.ProjectorData[i].EdgeGraphData.EdgeData[j].P2);
                  e.NextEdge = data.ProjectorData[i].EdgeGraphData.EdgeData[j].NextEdge;
                  e.PrevEdge = data.ProjectorData[i].EdgeGraphData.EdgeData[j].PrevEdge;
                  e.TwinEdge = data.ProjectorData[i].EdgeGraphData.EdgeData[j].TwinEdge;
                  edgeGraph.Edges.Add(e);
               }
               for (int j = 0; j < numVerticesInConfig; ++j)
               {
                  UVVertex v = new UVVertex(data.ProjectorData[i].EdgeGraphData.VertexData[j].Vertex);
                  v.ConnectedEdges = data.ProjectorData[i].EdgeGraphData.VertexData[j].ConnectedEdges;
                  edgeGraph.Vertices.Add(v);
               }
               for (int j = 0; j < numQuadsInConfig; ++j)
               {
                  UVQuad q = new UVQuad(data.ProjectorData[i].EdgeGraphData.QuadData[j].OrthoQuadVertices, data.ProjectorData[i].EdgeGraphData.QuadData[j].P0, data.ProjectorData[i].EdgeGraphData.QuadData[j].P1, data.ProjectorData[i].EdgeGraphData.QuadData[j].P2, data.ProjectorData[i].EdgeGraphData.QuadData[j].P3, data.ProjectorData[i].EdgeGraphData.QuadData[j].InputLayer, m_Game.Content.Load<Texture2D>("Textures/Layer0_2"));
                  q.IsScoreboard = data.ProjectorData[i].EdgeGraphData.QuadData[j].IsScoreboard;
                  q.IsWall = data.ProjectorData[i].EdgeGraphData.QuadData[j].IsWall;
                  edgeGraph.Quads.Add(q);
               }
               UVGrid grid = new UVGrid(data.ProjectorData[i].GridData.Width, data.ProjectorData[i].GridData.Height);
               grid.Vertices = data.ProjectorData[i].GridData.Vertices;
               bool isOn = data.ProjectorData[i].IsOn;
               Viewport viewport = data.ProjectorData[i].Viewport;
               Vector3 up = data.ProjectorData[i].Up;
               Vector3 lookat = data.ProjectorData[i].LookAt;
               Vector3 position = data.ProjectorData[i].Position;
               float rotX = data.ProjectorData[i].RotX;
               float rotY = data.ProjectorData[i].RotY;
               float rotZ = data.ProjectorData[i].RotZ;
               m_ProjectorPreview.AddProjector(alpha, aspectRatio, fov, near, far, edgeGraph, grid, isOn, viewport, position, lookat, up, rotX, rotY, rotZ);
            }

            return true;
         }
         catch (IOException e)
         {
            Console.WriteLine(e.Message);
            return false;
         }
         /*catch (Exception e)
         {
            Console.WriteLine(e.Message);
            return false;
         }*/
      }

      private System.Windows.Forms.Screen[] GetOrderedScreens()
      {
         System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
         bool swapped = false;
         do
         {
            swapped = false;
            for (int i = 1; i < screens.Length; ++i)
            {
               if (screens[i - 1].Bounds.X > screens[i].Bounds.X)
               {
                  System.Windows.Forms.Screen temp = screens[i - 1];
                  screens[i - 1] = screens[i];
                  screens[i] = temp;
                  swapped = true;
               }
            }
         } while (swapped);
         return screens;
      }

      void SaveEditorConfig(string filename)
      {
         //
         // Prepare data packet for saving
         //

         EditorSaveData data = new EditorSaveData();
         data.BuildingData = new List<BuildingSaveData>();
         data.LayerData = new List<LayerSaveData>();
         data.WindowData = new List<WindowSaveData>();
         data.ProjectorData = new List<ProjectorSaveData>();

         // Save buildings
         for (int i = 0; i < m_ProjectorPreview.Buildings.Count; ++i)
         {
            BuildingSaveData bData = new BuildingSaveData();
            bData.ID = m_ProjectorPreview.Buildings[i].ID;
            bData.MeshFilename = m_ProjectorPreview.Buildings[i].MeshFilename;
            bData.Position = m_ProjectorPreview.Buildings[i].Position;
            bData.RotX = m_ProjectorPreview.Buildings[i].RotX;
            bData.RotY = m_ProjectorPreview.Buildings[i].RotY;
            bData.RotZ = m_ProjectorPreview.Buildings[i].RotZ;
            bData.Scale = m_ProjectorPreview.Buildings[i].Scale;
            bData.Type = m_ProjectorPreview.Buildings[i].Type;
            data.BuildingData.Add(bData);
         }

         // Save projectors
         for (int i = 0; i < m_ProjectorPreview.Projectors.Count; ++i)
         {
            GridSaveData gData = new GridSaveData();
            gData.Height = m_ProjectorPreview.Projectors[i].Grid.Height;
            gData.Width = m_ProjectorPreview.Projectors[i].Grid.Width;
            gData.Vertices = m_ProjectorPreview.Projectors[i].Grid.Vertices;
            EdgeGraphSaveData egData = new EdgeGraphSaveData();
            List<UVEdgeSaveData> edgeData = new List<UVEdgeSaveData>();
            List<UVQuadSaveData> quadData = new List<UVQuadSaveData>();
            List<UVVertexSaveData> vertData = new List<UVVertexSaveData>();
            for (int j = 0; j < m_ProjectorPreview.Projectors[i].EdgeGraph.Edges.Count; ++j)
            {
               UVEdgeSaveData tempEData = new UVEdgeSaveData();
               tempEData.P1 = m_ProjectorPreview.Projectors[i].EdgeGraph.Edges[j].P1;
               tempEData.P2 = m_ProjectorPreview.Projectors[i].EdgeGraph.Edges[j].P2;
               tempEData.NextEdge = m_ProjectorPreview.Projectors[i].EdgeGraph.Edges[j].NextEdge;
               tempEData.PrevEdge = m_ProjectorPreview.Projectors[i].EdgeGraph.Edges[j].PrevEdge;
               tempEData.TwinEdge = m_ProjectorPreview.Projectors[i].EdgeGraph.Edges[j].TwinEdge;
               edgeData.Add(tempEData);
            }
            for (int j = 0; j < m_ProjectorPreview.Projectors[i].EdgeGraph.Quads.Count; ++j)
            {
               UVQuadSaveData tempQData = new UVQuadSaveData();
               tempQData.P0 = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].P0;
               tempQData.P1 = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].P1;
               tempQData.P2 = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].P2;
               tempQData.P3 = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].P3;
               tempQData.IsWall = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].IsWall;
               tempQData.IsScoreboard = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].IsScoreboard;
               tempQData.InputLayer = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].InputLayer;
               tempQData.OrthoQuadVertices = m_ProjectorPreview.Projectors[i].EdgeGraph.Quads[j].Vertices;
               quadData.Add(tempQData);
            }
            for (int j = 0; j < m_ProjectorPreview.Projectors[i].EdgeGraph.Vertices.Count; ++j)
            {
               UVVertexSaveData tempVData = new UVVertexSaveData();
               tempVData.ConnectedEdges = m_ProjectorPreview.Projectors[i].EdgeGraph.Vertices[j].ConnectedEdges;
               tempVData.Vertex = m_ProjectorPreview.Projectors[i].EdgeGraph.Vertices[j].Vertex;
               vertData.Add(tempVData);
            }
            egData.EdgeData = edgeData;
            egData.QuadData = quadData;
            egData.VertexData = vertData;
            ProjectorSaveData pData = new ProjectorSaveData();
            pData.Alpha = m_ProjectorPreview.Projectors[i].Alpha;
            pData.AspectRatio = m_ProjectorPreview.Projectors[i].AspectRatio;
            pData.FarPlane = m_ProjectorPreview.Projectors[i].FarPlane;
            pData.NearPlane = m_ProjectorPreview.Projectors[i].NearPlane;
            pData.Fov = m_ProjectorPreview.Projectors[i].Fov;
            pData.IsOn = m_ProjectorPreview.Projectors[i].IsOn;
            pData.LookAt = m_ProjectorPreview.Projectors[i].LookAt;
            pData.Position = m_ProjectorPreview.Projectors[i].Position;
            pData.RotX = m_ProjectorPreview.Projectors[i].RotX;
            pData.RotY = m_ProjectorPreview.Projectors[i].RotY;
            pData.RotZ = m_ProjectorPreview.Projectors[i].RotZ;
            pData.Up = m_ProjectorPreview.Projectors[i].Up;
            pData.Viewport = m_ProjectorPreview.Projectors[i].Viewport;
            pData.GridData = gData;
            pData.EdgeGraphData = egData;
            data.ProjectorData.Add(pData);
         }

         // Save layers
         for (int i = 0; i < m_LayersScrollView.Layers.Count; ++i)
         {
            LayerSaveData lData = new LayerSaveData();
            lData.ID = m_LayersScrollView.Layers[i].ID;
            lData.TYPE_ID = m_LayersScrollView.Layers[i].TYPE_ID;
            lData.Width = m_LayersScrollView.Layers[i].Width;
            lData.Height = m_LayersScrollView.Layers[i].Height;
            lData.Type = m_LayersScrollView.Layers[i].Type;
            lData.Normal = m_LayersScrollView.Layers[i].Normal;
            lData.LinkedLayers = m_LayersScrollView.Layers[i].LinkedLayers;
            lData.LayerName = m_LayersScrollView.Layers[i].NameLabel.Text;
            data.LayerData.Add(lData);
         }

         // Save windows
         System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
         bool swapped = false;
         do {
            swapped = false;
            for (int i = 1; i < screens.Length; ++i)
            {
               if (screens[i - 1].Bounds.X > screens[i].Bounds.X)
               {
                  System.Windows.Forms.Screen temp = screens[i - 1];
                  screens[i - 1] = screens[i];
                  screens[i] = temp;
                  swapped = true;
               }
            }
         } while (swapped);
         for (int i = 0; i < screens.Length; ++i)
         {
            WindowSaveData wData = new WindowSaveData();
            wData.Width = screens[i].Bounds.Width;
            wData.Height = screens[i].Bounds.Height;
            wData.WindowIndex = i;
            wData.IsProjector = (i > 0);
            data.WindowData.Add(wData);
         }

         //
         // Actually write file out
         //

         XmlSerializer writer = new XmlSerializer(typeof(EditorSaveData));
         StreamWriter file = new StreamWriter(@filename);
         writer.Serialize(file, data);
         file.Close();
      }

      #endregion

      #region Utility

      public void AddProjector(Rectangle bounds)
      {
         m_ProjectorPreview.AddProjector(bounds);

         int lastIndex = m_ProjectorPreview.Projectors.Count - 1;
         float aspectRatio = (float)m_ProjectorPreview.Projectors[lastIndex].Viewport.Height / (float)m_ProjectorPreview.Projectors[lastIndex].Viewport.Width;
         m_ProjectorPreview.Projectors[lastIndex].Grid.Reset(m_UVGridEditor.Viewport.Width, m_UVGridEditor.Viewport.Width * aspectRatio);
         m_ProjectorPreview.Projectors[lastIndex].EdgeGraph.AssembleGraph(m_ProjectorPreview.Projectors[lastIndex].Grid.GetIntersectionPoints());
         m_UVDualEdgeGraphEditor.SetNumProjectors(m_ProjectorPreview.Projectors.Count);
      }

      public override void Resize(int dx, int dy)
      {
         int windowWidth = GameConstants.WindowWidth;
         if (dx > 0)
            dx -= (windowWidth - GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
         else
            dx = 1280 - GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
         float rightMenuAs = (float)m_RightMenuViewport.Width / (float)m_RightMenuViewport.Height;
         m_RightMenuViewport.X += dx;
         m_RightMenuViewport.Height += dy;
         //m_RightMenuViewport.Width = (int)(m_RightMenuViewport.Height * rightMenuAs);
         m_RightMenuMouseInput.Offset = new Vector2(-m_RightMenuViewport.X, 0);

         //Viewport v = m_BuildingColorPicker.Viewport;
         //v.X = m_RightMenuViewport.X + GUI_BUILDING_COLOR_PICKER_X;
         //m_BuildingColorPicker.Viewport = v;

         m_ProjectionEditorPanel.Width += dx;
         m_ProjectionEditorPanel.Height += dy;
         m_ProjectorPreview.Resize(dx, dy);

         Vector2 btnLoc = m_NewGameplayLayerButton.Location;
         btnLoc.Y += dy;
         m_NewGameplayLayerButton.Location = btnLoc;
         btnLoc = m_TrashLayerButton.Location;
         btnLoc.Y += dy;
         m_TrashLayerButton.Location = btnLoc;
         Rectangle displayBounds = m_LayersScrollView.DisplayBounds;
         displayBounds.X += dx;
         displayBounds.Height += dy;
         m_LayersScrollView.DisplayBounds = displayBounds;
         m_LayersScrollView.UpdateProjection();

         m_ScrollMouseInput.Offset = new Vector2(-displayBounds.X, -displayBounds.Y);
      }

      private void AssembleGameplayLayer()
      {
         Label defaultGameplayLabel = new Label("Gameplay " + (m_LayersScrollView.NumLayers + 1), Layer.NAME_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.LAYER_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         Label defaultWidthLabel = new Label("Width", Layer.WIDTH_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT, Layer.WIDTH_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         Label defaultHeightLabel = new Label("Height", Layer.HEIGHT_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         Label defaultNormalsLabel = new Label("Normal", Layer.NORMALS_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         Label linkLabel = new Label("Linked Layers:", Layer.LINKS_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         Label linkListLabel = new Label("None", Layer.LINKS_LIST_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2, Layer.HEIGHT_WIDTH, Layer.LABEL_HEIGHT, Color.Black, m_ArialFont10);
         NumUpDown widthSpinBox = new NumUpDown(new Rectangle(Layer.WIDTH_SPINBOX_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT - 2, 40, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, GameConstants.TILE_DIM, GameConstants.TILE_DIM * GameConstants.MAX_TILES_WIDE, GameConstants.TILE_DIM, "{0:0}", m_ScrollMouseInput);
         NumUpDown heightSpinBox = new NumUpDown(new Rectangle(Layer.HEIGHT_SPINBOX_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT - 2, 40, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_ArialFont10, Color.Black, GameConstants.TILE_DIM, GameConstants.TILE_DIM * GameConstants.MAX_TILES_HIGH, GameConstants.TILE_DIM, "{0:0}", m_ScrollMouseInput);
         widthSpinBox.Value = GameConstants.TILE_DIM * GameConstants.DEFAULT_TILES_WIDE;
         heightSpinBox.Value = GameConstants.TILE_DIM * GameConstants.DEFAULT_TILES_HIGH;

         Button linkBtn = new Button(new Rectangle(Layer.LINK_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT + Layer.LABEL_HEIGHT * 2 + 2, Layer.LINK_BUTTON_WIDTH, Layer.LINK_BUTTON_HEIGHT), m_LinkButtonTexture, m_ScrollMouseInput);
         linkBtn.SetImage(Button.ImageType.OVER, m_LinkButtonTextureOnHover);
         linkBtn.SetImage(Button.ImageType.CLICK, m_LinkButtonTextureOnPress);
         Button editBtn = new Button(new Rectangle(Layer.EDIT_BUTTON_X, Layer.LAYER_PADDING_Y + (Layer.LAYER_PADDING_Y * m_LayersScrollView.NumLayers) + m_LayersScrollView.NumLayers * Layer.LAYER_HEIGHT, Layer.EDIT_BUTTON_WIDTH, Layer.EDIT_BUTTON_HEIGHT), m_EditLayerButtonTexture, m_ScrollMouseInput);
         editBtn.SetImage(Button.ImageType.OVER, m_EditLayerButtonTextureOnHover);
         editBtn.SetImage(Button.ImageType.CLICK, m_EditLayerButtonTextureOnPress);
         m_LayersScrollView.AddLayer(new GameplayLayer(defaultGameplayLabel, defaultWidthLabel, defaultHeightLabel, defaultNormalsLabel, linkLabel, linkListLabel, widthSpinBox, heightSpinBox, editBtn, linkBtn, m_ColorPickerPreviewPadTexture));
      }

      #endregion

      #region Public Access TV

      public Texture2D ScoreboardInput
      {
          set
          {
              for (int p = 0; p < m_ProjectorPreview.Projectors.Count; ++p)
              {
                  for (int i = 0; i < m_ProjectorPreview.Projectors[p].EdgeGraph.Quads.Count; ++i)
                  {
                      if (m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].IsScoreboard)
                      {
                          m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].Texture = value;
                      }
                  }
              }
          }
      }

      public Texture2D ProjectorInput
      {
         set
         {
            // Assign the render target to the editors
            m_UVGridEditor.RenderTarget = value;

            if (m_ProjectorPreview.IsProjectorSelected)
            {
               m_ProjectorPreview.Projectors[m_ProjectorPreview.SelectedProjector].Texture = value;
            }
         }
      }

      public PointLightComponent Light
      {
          get { return m_ProjectorPreview.Light; }
      }

      public List<Layer> Layers
      {
         get
         {
            return m_LayersScrollView.Layers;
         }
      }

      public Texture2D[] GameplayRenderTargets
      {
         set
         {
            for (int p = 0; p < m_ProjectorPreview.Projectors.Count; ++p)
            {
               for (int i = 0; i < m_ProjectorPreview.Projectors[p].EdgeGraph.Quads.Count; ++i)
               {
                  if (m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].IsWall)
                  {
                     m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].Texture = m_WallTexture;
                  }
                  else
                  {
                     m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].Texture = value[m_ProjectorPreview.Projectors[p].EdgeGraph.Quads[i].InputLayer];
                  }
               }
            }

            //m_UVDualEdgeGraphEditor.SyncRenderTargets(value);
         }
      }

      #endregion

   }
}
