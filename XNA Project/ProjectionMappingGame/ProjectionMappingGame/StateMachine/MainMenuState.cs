
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local imports
using ProjectionMappingGame.GUI;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   class MainMenuState : GameState
   {
      // Fonts
      SpriteFont m_ArialFont;
      SpriteFont m_Arial10Font;

      // Textures
      Texture2D m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_SpinBoxFillTexture;
      Texture2D m_ButtonTexture, m_ButtonTextureOnHover, m_ButtonTextureOnPress;
      Texture2D m_WindowSizerBackgroundTexture, m_WindowSizerBorderTexture;
      Texture2D m_WhiteTexture;

      // Input handling
      MouseInput m_MouseInput;

      // GUI objects
      List<WindowSizerControl> m_WindowSizers;
      GUI.Label m_InstructionsLabel;
      GUI.Label m_ProjectorsLabel;
      NumUpDown m_ProjectorsSpinBox;
      GUI.Button m_DoneButton;
      GUI.Button m_ResetButton;
      GUI.Button m_QuitButton;
      GUI.Button m_PlayButton;
      int m_LastSpinBoxValue;

      // GUI constants
      const int GUI_BUTTON_WIDTH = 80;
      const int GUI_BUTTON_HEIGHT = 24;
      const int GUI_BUTTON_PADDING_X = 5;
      const int GUI_BUTTON_Y = 50;
      const int GUI_NUM_PROJECTORS_Y = 25;
      const int GUI_SPINBOX_WIDTH = 50;
      const int GUI_SPINBOX_HEIGHT = 20;
      const float WINDOW_SIZER_SCALE = 0.15f;
      const int WINDOW_PADDING_X = 10;
      const int WINDOW_CENTER_Y = -(WINDOW_HEIGHT / 2) - 30;
      const int WINDOW_WIDTH = 300;
      const int WINDOW_HEIGHT = 300;

      public MainMenuState(GameDriver game)
         : base(game, StateType.MainMenu)
      {
         m_MouseInput = new MouseInput();
      }

      public override void Reset()
      {
      }

      public override void Resize(int dx, int dy)
      {
      }

      public override void LoadContent(ContentManager content)
      {
         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");
         m_Arial10Font = content.Load<SpriteFont>("Fonts/Arial10");

         // Load textures
         m_WhiteTexture = content.Load<Texture2D>("Textures/white");
         m_ButtonTexture = content.Load<Texture2D>("Textures/GUI/button");
         m_ButtonTextureOnHover = content.Load<Texture2D>("Textures/GUI/button_on_hover");
         m_ButtonTextureOnPress = content.Load<Texture2D>("Textures/GUI/button_on_hover");
         m_SpinBoxFillTexture = content.Load<Texture2D>("Textures/GUI/spinbox_fill");
         m_SpinBoxUpTexture = content.Load<Texture2D>("Textures/GUI/spinbox_up");
         m_SpinBoxDownTexture = content.Load<Texture2D>("Textures/GUI/spinbox_down");
         m_WindowSizerBackgroundTexture = content.Load<Texture2D>("Textures/GUI/window_sizer_background");
         m_WindowSizerBorderTexture = content.Load<Texture2D>("Textures/GUI/window_sizer_border");

         int numScreens = System.Windows.Forms.Screen.AllScreens.Length;

         // Load gui
         m_InstructionsLabel = new GUI.Label("", 0, 0, 0, 0, Microsoft.Xna.Framework.Color.White, m_ArialFont);
         m_ProjectorsLabel = new GUI.Label("# Projectors", GameConstants.WindowWidth / 2 - (int)m_Arial10Font.MeasureString("# Projectors").Length(), GameConstants.WindowHeight / 2 + GUI_NUM_PROJECTORS_Y, (int)m_Arial10Font.MeasureString("# Projectors").Length(), 20, Microsoft.Xna.Framework.Color.White, m_Arial10Font);
         m_ProjectorsSpinBox = new NumUpDown(
            new Microsoft.Xna.Framework.Rectangle(
               GameConstants.WindowWidth / 2 + GUI_BUTTON_PADDING_X,
               GameConstants.WindowHeight / 2 + GUI_NUM_PROJECTORS_Y,
               GUI_SPINBOX_WIDTH,
               GUI_SPINBOX_HEIGHT
            ),
            m_SpinBoxFillTexture,
            m_WhiteTexture,
            m_SpinBoxUpTexture,
            m_SpinBoxDownTexture,
            m_Arial10Font,
            Microsoft.Xna.Framework.Color.Black,
            1,
            numScreens - 1,
            1,
            "{0:0}",
            m_MouseInput
         );
         m_LastSpinBoxValue = (int)m_ProjectorsSpinBox.Value;
         m_ProjectorsSpinBox.RegisterOnValueChanged(ProjectorsSpinBox_OnValueChanged);

         m_DoneButton = new GUI.Button(new Microsoft.Xna.Framework.Rectangle(GameConstants.WindowWidth / 2 - GUI_BUTTON_WIDTH / 2 - GUI_BUTTON_WIDTH - GUI_BUTTON_PADDING_X, GameConstants.WindowHeight / 2 + GUI_BUTTON_Y, GUI_BUTTON_WIDTH, GUI_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_Arial10Font, "Done", Microsoft.Xna.Framework.Color.Black);
         m_DoneButton.SetImage(GUI.Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_DoneButton.SetImage(GUI.Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_DoneButton.RegisterOnClick(DoneButton_OnClick);

         m_ResetButton = new GUI.Button(new Microsoft.Xna.Framework.Rectangle(GameConstants.WindowWidth / 2 - GUI_BUTTON_WIDTH / 2, GameConstants.WindowHeight / 2 + GUI_BUTTON_Y, GUI_BUTTON_WIDTH, GUI_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_Arial10Font, "Reset", Microsoft.Xna.Framework.Color.Black);
         m_ResetButton.SetImage(GUI.Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_ResetButton.SetImage(GUI.Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_ResetButton.RegisterOnClick(ResetButton_OnClick);

         m_QuitButton = new GUI.Button(new Microsoft.Xna.Framework.Rectangle(GameConstants.WindowWidth / 2 + GUI_BUTTON_WIDTH / 2 + GUI_BUTTON_PADDING_X, GameConstants.WindowHeight / 2 + GUI_BUTTON_Y, GUI_BUTTON_WIDTH, GUI_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_Arial10Font, "Quit", Microsoft.Xna.Framework.Color.Black);
         m_QuitButton.SetImage(GUI.Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_QuitButton.SetImage(GUI.Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_QuitButton.RegisterOnClick(QuitButton_OnClick);

         m_PlayButton = new GUI.Button(new Microsoft.Xna.Framework.Rectangle(GameConstants.WindowWidth / 2 + GUI_BUTTON_WIDTH * 2 + GUI_BUTTON_PADDING_X, GameConstants.WindowHeight / 2 + GUI_BUTTON_Y, GUI_BUTTON_WIDTH, GUI_BUTTON_HEIGHT), m_ButtonTexture, m_MouseInput, m_Arial10Font, "Play", Microsoft.Xna.Framework.Color.Black);
         m_PlayButton.SetImage(GUI.Button.ImageType.OVER, m_ButtonTextureOnHover);
         m_PlayButton.SetImage(GUI.Button.ImageType.CLICK, m_ButtonTextureOnPress);
         m_PlayButton.RegisterOnClick(PlayButton_OnClick);

         // Add basic monitor and additional projector outputs
         m_WindowSizers = new List<WindowSizerControl>();
         AddWindowSizer("Monitor");
         if (GetOrderedScreens().Length > 1)
         {
             AddWindowSizer("Projector 1");
         }
      }

      private Screen[] GetOrderedScreens()
      {
         Screen[] screens = System.Windows.Forms.Screen.AllScreens;
         bool swapped = false;
         do
         {
            swapped = false;
            for (int i = 1; i < screens.Length; ++i)
            {
               if (screens[i - 1].Bounds.X > screens[i].Bounds.X)
               {
                  Screen temp = screens[i - 1];
                  screens[i - 1] = screens[i];
                  screens[i] = temp;
                  swapped = true;
               }
            }
         } while (swapped);
         return screens;
      }

      private void AddWindowSizer(string name)
      {
         int numScreens = m_WindowSizers.Count;
         Screen[] screens = GetOrderedScreens();

         Microsoft.Xna.Framework.Rectangle sizerBounds = new Microsoft.Xna.Framework.Rectangle();
         sizerBounds.Width = screens[numScreens].Bounds.Width;
         sizerBounds.Height = screens[numScreens].Bounds.Height;

         int totalWidth = WINDOW_WIDTH * m_WindowSizers.Count + WINDOW_PADDING_X * m_WindowSizers.Count;
         totalWidth += (int)(sizerBounds.Width * WINDOW_SIZER_SCALE);
         int x = GameConstants.WindowWidth / 2 - totalWidth / 2;
         for (int i = 0; i < m_WindowSizers.Count; ++i)
         {
            Vector2 loc = m_WindowSizers[i].Location;
            loc.X = x;
            m_WindowSizers[i].Location = loc;
            x += WINDOW_WIDTH + WINDOW_PADDING_X;
         }
         sizerBounds.X = x;
         sizerBounds.Y = GameConstants.WindowHeight / 2 + WINDOW_CENTER_Y;

         Viewport viewport = new Viewport(sizerBounds.X, sizerBounds.Y, WINDOW_WIDTH, WINDOW_HEIGHT);

         MouseInput windowMouse = new MouseInput(new Vector2(-sizerBounds.X, -sizerBounds.Y));
         NumUpDown widthSpinBox = new NumUpDown(new Microsoft.Xna.Framework.Rectangle(WindowSizerControl.WIDTH_SPINBOX_X, WindowSizerControl.WIDTH_LABEL_Y, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_Arial10Font, Microsoft.Xna.Framework.Color.Black, 1024, 1920, 10, "{0:0}", windowMouse);
         NumUpDown heightSpinBox = new NumUpDown(new Microsoft.Xna.Framework.Rectangle(WindowSizerControl.HEIGHT_SPINBOX_X, WindowSizerControl.HEIGHT_LABEL_Y, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), m_SpinBoxFillTexture, m_WhiteTexture, m_SpinBoxUpTexture, m_SpinBoxDownTexture, m_Arial10Font, Microsoft.Xna.Framework.Color.Black, 768, 1080, 10, "{0:0}", windowMouse);
         GUI.Label widthLabel = new GUI.Label("Width", WindowSizerControl.WIDTH_LABEL_X, WindowSizerControl.WIDTH_LABEL_Y + 2, (int)m_Arial10Font.MeasureString("Width").Length(), 20, Microsoft.Xna.Framework.Color.Black, m_Arial10Font);
         GUI.Label heightLabel = new GUI.Label("Height", WindowSizerControl.HEIGHT_LABEL_X, WindowSizerControl.HEIGHT_LABEL_Y + 2, (int)m_Arial10Font.MeasureString("Height").Length(), 20, Microsoft.Xna.Framework.Color.Black, m_Arial10Font);
         m_WindowSizers.Add(
            new WindowSizerControl(
               name,
               m_Arial10Font,
               windowMouse,
               viewport,
               sizerBounds,
               m_WindowSizerBackgroundTexture,
               m_WindowSizerBorderTexture,
               widthLabel,
               heightLabel,
               widthSpinBox,
               heightSpinBox,
               WINDOW_SIZER_SCALE
            ));
      }

      private void RemoveWindowSizer()
      {
         m_WindowSizers.RemoveAt(m_WindowSizers.Count - 1);

         int totalWidth = WINDOW_WIDTH * m_WindowSizers.Count + WINDOW_PADDING_X * m_WindowSizers.Count;
         int x = GameConstants.WindowWidth / 2 - totalWidth / 2;
         for (int i = 0; i < m_WindowSizers.Count; ++i)
         {
            Vector2 loc = m_WindowSizers[i].Location;
            loc.X = x;
            m_WindowSizers[i].Location = loc;
            x += WINDOW_WIDTH + WINDOW_PADDING_X;
         }
      }

      private void AddProjectorSizer()
      {
         AddWindowSizer("Projector " + m_ProjectorsSpinBox.Value);
      }

      #region Updating

      public override void Update(float elapsedTime)
      {
      }

      #endregion

      #region Input Handling

      public override void HandleInput(float elapsedTime)
      {
         // Update all input devices
         m_MouseInput.HandleInput(PlayerIndex.One);
         for (int i = 0; i < m_WindowSizers.Count; ++i)
         {
            m_WindowSizers[i].HandleInput(elapsedTime);
         }
      }

      private void QuitButton_OnClick(object sender, EventArgs e)
      {
         Application.Exit();
      }

      private void ResetButton_OnClick(object sender, EventArgs e)
      {
         m_ProjectorsSpinBox.Value = 1;
         m_LastSpinBoxValue = (int)m_ProjectorsSpinBox.Value;
         m_WindowSizers.Clear();
         AddWindowSizer("Monitor");
         AddProjectorSizer();
      }

      private void DoneButton_OnClick(object sender, EventArgs e)
      {
         int maxHeight = int.MinValue;
         int totalWidth = 0;
         for (int i = 0; i < m_WindowSizers.Count; ++i)
         {
            totalWidth += m_WindowSizers[i].Width;
            if (m_WindowSizers[i].Height > maxHeight)
               maxHeight = m_WindowSizers[i].Height;
         }

         int w = totalWidth;
         int h = maxHeight;

         m_Game.GraphicsManager.PreferredBackBufferWidth = w;
         m_Game.GraphicsManager.PreferredBackBufferHeight = h;
         m_Game.GraphicsManager.PreferMultiSampling = true;
         m_Game.GraphicsManager.ApplyChanges();
         
         FiniteStateMachine.GetInstance().ResizeGame(m_Game.Window.ClientBounds.Width, m_Game.Window.ClientBounds.Height); 
         FiniteStateMachine.GetInstance().SetState(StateMachine.StateType.ProjectionEditor);
         FiniteStateMachine.GetInstance().StartEditor();

         Form form = (Form)Form.FromHandle(m_Game.Window.Handle);
         form.Location = new System.Drawing.Point(0, 0);
      }

      private void PlayButton_OnClick(object sender, EventArgs e)
      {
          FiniteStateMachine.GetInstance().StartGameWithoutEditor();
      }

      private void ProjectorsSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         if ((int)m_ProjectorsSpinBox.Value > m_LastSpinBoxValue)
         {
            AddProjectorSizer();
         }
         else if ((int)m_ProjectorsSpinBox.Value < m_LastSpinBoxValue)
         {
            RemoveWindowSizer();
         }
         m_LastSpinBoxValue = (int)m_ProjectorsSpinBox.Value;
      }

      #endregion

      #region Rendering

      public override void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();
         m_InstructionsLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorsLabel.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ProjectorsSpinBox.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_DoneButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_ResetButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_QuitButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         m_PlayButton.Draw(m_Game.GraphicsDevice, spriteBatch);
         spriteBatch.End();

         // Draw window sizers
         for (int i = 0; i < m_WindowSizers.Count; ++i)
         {
            m_WindowSizers[i].Draw(m_Game.GraphicsDevice, spriteBatch);
         }
      }

      #endregion;

      #region Public Access TV

      public List<WindowSizerControl> Windows
      {
         get { return m_WindowSizers; }
      }
      
      #endregion

   }
}
