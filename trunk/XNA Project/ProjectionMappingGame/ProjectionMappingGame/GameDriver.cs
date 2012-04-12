
#region File Description

/******************************************************************
 * Filename:        GameDriver.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         1/24/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

// Microsoft includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local includes
using ProjectionMappingGame.StateMachine;
using ProjectionMappingGame.FileSystem;

#endregion

namespace ProjectionMappingGame
{
   /// <summary>
   /// GameDriver handles the basic delegation of updating and drawing
   /// to all active XNA game components.
   /// </summary>
   public class GameDriver : Microsoft.Xna.Framework.Game
   {
      // Timing/Performance monitoring fields
      Stopwatch m_Watch;
      int m_FrameCount;
      float m_FramesPerSecond;
      float m_FramesPerMillisecond;
      float m_PrevMilliseconds;
      float m_ElapsedMilliseconds;

      // State machine
      FiniteStateMachine m_FSM;

      // Configuration
      AppConfig m_AppConfig;
      List<Theme> m_Themes;
      public AppConfig Config
      {
         get { return m_AppConfig; }
      }

      // Rendering members
      GraphicsDeviceManager m_GraphicsManager;  // Handle to active XNA graphics device controller
      SpriteBatch m_SpriteBatch;                // Sprite renderer for 2D
      public GraphicsDeviceManager GraphicsManager
      {
         get { return m_GraphicsManager; }
      }
      #region Initialization

      /// <summary>
      /// Default constructor for type GameDriver initializes base XNA
      /// components such as content and the graphics device.
      /// </summary>
      public GameDriver()
      {
         // Load application configuration if it exists
         if (!LoadAppConfig("config.xml"))
         {
            CreateDefaultAppConfig("config.xml");
         }

         // Initialize graphics device
         m_GraphicsManager = new GraphicsDeviceManager(this);
         m_GraphicsManager.PreferredBackBufferWidth = m_AppConfig.DefaultScreenWidth;
         m_GraphicsManager.PreferredBackBufferHeight = m_AppConfig.DefaultScreenHeight;
         m_GraphicsManager.PreferMultiSampling = true;
         m_GraphicsManager.ApplyChanges();
         //Window.AllowUserResizing = true;
         Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

         // Set content manager's root directory for asset loading.
         // This should only be set once; right here.
         Content.RootDirectory = "Content";

         // Show mouse
         this.IsMouseVisible = true;

         // Load themes - This is where I read texture filenames in the provided directory
         // and their textures into my Theme[] array.
         LoadThemes();
      }

      void Window_ClientSizeChanged(object sender, EventArgs e)
      {
         FiniteStateMachine.GetInstance().ResizeGame(Window.ClientBounds.Width, Window.ClientBounds.Height); 
      }

      /// <summary>
      /// Initialize game components before Update and LoadContent().
      /// </summary>
      protected override void Initialize()
      {
         // Initialize and start stopwatch to monitor fps
         m_Watch = new Stopwatch();
         m_Watch.Start();

         // Store singleton reference to finite state machine and initialize
         m_FSM = FiniteStateMachine.GetInstance();
         m_FSM.Configure(this, GameConstants.DEFAULT_STATE);

         // Enumerate initialization to XNA attached components. aka States in the state machine.
         base.Initialize();
      }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw in 2D.
         m_SpriteBatch = new SpriteBatch(GraphicsDevice);

         // Load all states
         m_FSM.LoadContent(Content);
      }

      #endregion

      #region Destruction

      /// <summary>
      /// Unload game content on final game destruction.  We shouldn't have
      /// to do anything in here.  Though we should destroy content in 
      /// GameState instances.
      /// </summary>
      protected override void UnloadContent()
      {
         for (int i = 0; i < m_Themes.Count; ++i)
         {
            for (int j = 0; j < m_Themes[i].Background.Length; ++j)
            {
               m_Themes[i].Background[j].Dispose();
            }
            for (int j = 0; j < m_Themes[i].Platforms.Length; ++j)
            {
               for (int n = 0; n < m_Themes[i].Platforms[j].Length; ++n)
               {
                  m_Themes[i].Platforms[j][n].Dispose();
               }
            }
            for (int j = 0; j < m_Themes[i].SpriteSheets.Length; ++j)
            {
               m_Themes[i].SpriteSheets[i].Dispose();
            }
         }
      }

      #endregion

      #region Updating

      /// <summary>
      /// Update the game BEFORE Draw() each frame.  All physics/collisions/parameter
      /// maintaining/input/etc should be funneled through here.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

         // Update all active states
         m_FSM.Update(elapsedTime);
            
         // Enumerate update to XNA attached components. aka States in the state machine
         base.Update(gameTime);
      }

      #endregion

      #region Drawing

      /// <summary>
      /// Render the game each frame.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         // Clear the back buffer
         GraphicsDevice.Clear(Color.CornflowerBlue);

         // Draw all active states
         m_FSM.Draw(m_SpriteBatch);

         // Enumerate draw to XNA attached components. aka State in the state machine
         base.Draw(gameTime);

         // Measure performance in fps and fpms
         if (m_Watch.IsRunning)
         {
            m_FrameCount++;

            // Calculate Frames per Millisecond
            float frameMilliseconds = m_Watch.ElapsedMilliseconds;
            float deltaMilliseconds = frameMilliseconds - m_PrevMilliseconds;
            m_ElapsedMilliseconds += deltaMilliseconds;
            m_FramesPerMillisecond = (float)m_FrameCount / m_ElapsedMilliseconds;

            // Calculate Frames per second
            if (m_ElapsedMilliseconds >= 1000)
            {
               m_FramesPerSecond = (float)m_FrameCount / (m_ElapsedMilliseconds / 1000);

               m_FrameCount = 0;
               m_ElapsedMilliseconds = 0;
            }

            m_PrevMilliseconds = frameMilliseconds;
         }

         // If in debug mode, display FPS in window title
         if (GameConstants.DEBUG_MODE)
         {
            Window.Title = "FPS: " + m_FramesPerSecond;
         }
      }

      #endregion

      #region Configuration

      void CreateDefaultAppConfig(string filename)
      {
         AppConfig data = new AppConfig();
         data.DefaultScreenWidth = GameConstants.DEFAULT_WINDOW_WIDTH;
         data.DefaultScreenHeight = GameConstants.DEFAULT_WINDOW_HEIGHT;
         data.RootThemePath = GameConstants.DEFAULT_THEME_ROOT_PATH;
         data.Loaded = true;

         XmlSerializer writer = new XmlSerializer(typeof(AppConfig));
         StreamWriter file = new StreamWriter(@filename);
         writer.Serialize(file, data);
         file.Close();
      }

      bool LoadAppConfig(string filename)
      {
         // Set defaults in case there is no config file
         m_AppConfig = new AppConfig();
         m_AppConfig.Loaded = false;
         m_AppConfig.RootThemePath = GameConstants.DEFAULT_THEME_ROOT_PATH;
         m_AppConfig.DefaultScreenWidth = GameConstants.DEFAULT_WINDOW_WIDTH;
         m_AppConfig.DefaultScreenHeight = GameConstants.DEFAULT_WINDOW_HEIGHT;

         try
         {
            // Open the file
            FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);

            // Read the data from the file
            XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
            AppConfig data = (AppConfig)serializer.Deserialize(stream);

            // Close the file
            stream.Close();

            // Transfer data
            m_AppConfig.DefaultScreenWidth = data.DefaultScreenWidth;
            m_AppConfig.DefaultScreenHeight = data.DefaultScreenHeight;
            m_AppConfig.RootThemePath = data.RootThemePath;
            m_AppConfig.Loaded = data.Loaded;
            return true;
         }
         catch (IOException e)
         {
            return false;
         }
      }

      void LoadThemes()
      {
         // Load all themes in the provided root theme path
         List<ThemeConfig> themeConfigs = new List<ThemeConfig>();
         try
         {
            // Get all theme folders.  Assume that these directories contain proper themes regardless
            // of any sort of naming convention at this level.
            string[] themes = Directory.GetDirectories(m_AppConfig.RootThemePath);

            // Create and load each theme
            for (int i = 0; i < themes.Length; ++i)
            {
               // Create a theme and configure it
               ThemeConfig theme = new ThemeConfig();
               theme.Name = themes[i];

               // Load backgrounds
               string backgroundDir = theme.Name + "\\" + ThemeConfig.BACKGROUND_DIRECTORY;
               string[] backgrounds = Directory.GetFiles(backgroundDir, ThemeConfig.BACKGROUND_FILE_EXT);
               theme.Backgrounds = new List<string>();
               for (int j = 0; j < backgrounds.Length; ++j)
               {
                  theme.Backgrounds.Add(backgrounds[j]);
               }

               // Load spritesheets
               /*string spritesheetDir = theme.Name + "\\" + ThemeConfig.SPRITESHEET_DIRECTORY;
               string[] spritesheets = Directory.GetFiles(spritesheetDir, ThemeConfig.SPRITESHEET_FILE_EXT);
               theme.SpriteSheets = new List<string>();
               for (int j = 0; j < spritesheets.Length; ++j)
               {
                  theme.SpriteSheets.Add(spritesheets[j]);
               }*/

               // Load platforms
               string platformDir = theme.Name + "\\" + ThemeConfig.PLATFORM_DIRECTORY;
               string[] platformDirs = Directory.GetDirectories(platformDir);
               theme.Platforms = new List<List<string>>();
               for (int j = 0; j < platformDirs.Length; ++j)
               {
                  theme.Platforms.Add(new List<string>());

                  string d = platformDirs[j];
                  string[] platforms = Directory.GetFiles(d, ThemeConfig.PLATFORM_FILE_EXT);
                  for (int n = 0; n < platforms.Length; ++n)
                  {
                     theme.Platforms[j].Add(platforms[n]);
                  }
               }

               // Store the configured theme in our list
               themeConfigs.Add(theme);
            }

            // Now that we know the theme configuration, we can actually load the textures we need.
            int numThemes = themeConfigs.Count;
            m_Themes = new List<Theme>();
            for (int i = 0; i < numThemes; ++i)
            {
               m_Themes.Add(LoadTheme(themeConfigs[i]));
            }
         }
         // Damn IO errors
         catch (UnauthorizedAccessException e)
         {
            Console.WriteLine(e.Message);
         }
      }

      Theme LoadTheme(ThemeConfig themeConfig)
      {
         Theme t = new Theme();
         t.Background = new Texture2D[themeConfig.Backgrounds.Count];
         t.Platforms = new Texture2D[themeConfig.Platforms.Count][];
         //t.SpriteSheets = new Texture2D[themeConfig.SpriteSheets.Count];
         for (int j = 0; j < themeConfig.Backgrounds.Count; ++j)
         {
            t.Background[j] = Texture2D.FromStream(GraphicsDevice, File.OpenRead(themeConfig.Backgrounds[j]));
         }
         for (int j = 0; j < themeConfig.Platforms.Count; ++j)
         {
            t.Platforms[j] = new Texture2D[themeConfig.Platforms[j].Count];
            for (int i = 0; i < themeConfig.Platforms[j].Count; ++i)
            {
               t.Platforms[j][i] = Texture2D.FromStream(GraphicsDevice, File.OpenRead(themeConfig.Platforms[j][i]));
            }
         }
         /*for (int j = 0; j < themeConfig.SpriteSheets.Count; ++j)
         {
            t.SpriteSheets[j] = Texture2D.FromStream(GraphicsDevice, File.OpenRead(themeConfig.SpriteSheets[j]));
         }*/
         return t;
      }

      #endregion

   }
}
