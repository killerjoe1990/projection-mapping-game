
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
using System.Xml;

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
      List<ThemeTextures> m_Themes;
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

      public List<ThemeTextures> Themes
      {
          get
          {
              return m_Themes;
          }
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

         // Load themes - This is where I read texture filenames in the provided directory
         // and their textures into my Theme[] array.
         LoadThemes();

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
            for (int j = 0; j < m_Themes[i].StaticSprites.Length; ++j)
            {
               m_Themes[i].StaticSprites[j].Dispose();
            }
            for (int j = 0; j < m_Themes[i].MovingSprites.Length; ++j)
            {
                m_Themes[i].MovingSprites[j].Dispose();
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
            Console.WriteLine(e.Message);
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
               if (themes[i].Contains("svn")) continue;

               // Create a theme and configure it
               ThemeConfig theme = new ThemeConfig();
               theme.Name = themes[i];

               // Load backgrounds
               string backgroundDir = theme.Name + "\\" + ThemeConfig.BACKGROUND_DIRECTORY;
               string[] backgrounds = Directory.GetFiles(backgroundDir, ThemeConfig.BACKGROUND_FILE_EXT);
               theme.Backgrounds = new List<string>();
               for (int j = 0; j < backgrounds.Length; ++j)
               {
                  if (backgrounds[j].Contains("svn")) continue;
                  theme.Backgrounds.Add(backgrounds[j]);
               }

               // Load spritesheets
               string spritesheetDir = theme.Name + "\\" + ThemeConfig.MOVINGSPRITE_DIRECTORY;
               string[] moveSpritesheets = Directory.GetFiles(spritesheetDir, ThemeConfig.SPRITESHEET_FILE_EXT);
               theme.MovingSprites = new List<string>();
               for (int j = 0; j < moveSpritesheets.Length; ++j)
               {
                  if (moveSpritesheets[j].Contains("svn")) continue;
                  theme.MovingSprites.Add(moveSpritesheets[j]);
               }

               // Load spritesheets
               string spritesheetDir2 = theme.Name + "\\" + ThemeConfig.STATICSPRITE_DIRECTORY;
               string[] staticSpritesheets = Directory.GetFiles(spritesheetDir2, ThemeConfig.SPRITESHEET_FILE_EXT);
               theme.StaticSprites = new List<string>();
               for (int j = 0; j < staticSpritesheets.Length; ++j)
               {
                  if (staticSpritesheets[j].Contains("svn")) continue;
                   theme.StaticSprites.Add(staticSpritesheets[j]);
               }

               // Load platforms
               string platformDir = theme.Name + "\\" + ThemeConfig.PLATFORM_DIRECTORY;
               string[] platformDirs = Directory.GetDirectories(platformDir);
               theme.Platforms = new List<List<string>>();
               for (int j = 0; j < platformDirs.Length; ++j)
               {
                  if (platformDirs[j].Contains("svn")) continue;

                  theme.Platforms.Add(new List<string>());

                  string d = platformDirs[j];
                  string[] platforms = Directory.GetFiles(d, ThemeConfig.PLATFORM_FILE_EXT);
                  for (int n = 0; n < platforms.Length; ++n)
                  {
                     if (platforms[n].Contains("svn")) continue;
                     theme.Platforms[theme.Platforms.Count - 1].Add(platforms[n]);
                  }
               }

               // Store the configured theme in our list
               themeConfigs.Add(theme);
            }

            // Now that we know the theme configuration, we can actually load the textures we need.
            int numThemes = themeConfigs.Count;
            m_Themes = new List<ThemeTextures>();
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

      ThemeTextures LoadTheme(ThemeConfig themeConfig)
      {
         ThemeTextures t = new ThemeTextures();
         
         t.Background = new Texture2D[themeConfig.Backgrounds.Count];
         t.Platforms = new Texture2D[themeConfig.Platforms.Count][];
         t.MovingSprites = new Texture2D[themeConfig.MovingSprites.Count];
         t.MovingSpriteInfo = new Vector2[themeConfig.MovingSprites.Count];
         t.StaticSprites = new Texture2D[themeConfig.StaticSprites.Count];
         t.StaticSpriteInfo = new Vector2[themeConfig.StaticSprites.Count];

         XmlDocument config = new XmlDocument();
         config.Load(themeConfig.Name + "\\" + ThemeConfig.CONFIGURATION_XML);
          XmlNode root = config.SelectSingleNode("Theme");
         t.Name = root.Attributes["Name"].Value;

         XmlNode node = root.SelectSingleNode("Background");
         t.BackgroundRate = float.Parse(node.Attributes["Rate"].Value);

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

         node = config.SelectSingleNode("Theme/MovingSprites");
         t.NumMovingSprites = Int32.Parse(node.Attributes["Number"].Value);
         t.MovingSpriteSize = new Point(Int32.Parse(node.Attributes["MinSize"].Value), Int32.Parse(node.Attributes["MaxSize"].Value));
         t.MovingSpriteSpeed = new Vector2(float.Parse(node.Attributes["MinSpeed"].Value), float.Parse(node.Attributes["MaxSpeed"].Value));

         for (int j = 0; j < themeConfig.MovingSprites.Count; ++j)
         {
            t.MovingSpriteInfo[j] = new Vector2(float.Parse(node.ChildNodes[j].Attributes["Frames"].Value), float.Parse(node.ChildNodes[j].Attributes["Rate"].Value));
            t.MovingSprites[j] = Texture2D.FromStream(GraphicsDevice, File.OpenRead(themeConfig.MovingSprites[j]));
         }

         node = config.SelectSingleNode("Theme/StaticSprites");
         t.StaticSpriteSize = new Point(Int32.Parse(node.Attributes["MinSize"].Value), Int32.Parse(node.Attributes["MaxSize"].Value));
         t.StaticSpriteTime = new Vector2(float.Parse(node.Attributes["MinTime"].Value), float.Parse(node.Attributes["MaxTime"].Value));

         for (int j = 0; j < themeConfig.StaticSprites.Count; ++j)
         {
             t.StaticSpriteInfo[j] = new Vector2(float.Parse(node.ChildNodes[j].Attributes["Frames"].Value), float.Parse(node.ChildNodes[j].Attributes["Rate"].Value));
             t.StaticSprites[j] = Texture2D.FromStream(GraphicsDevice, File.OpenRead(themeConfig.StaticSprites[j]));
         }
         return t;
      }

      #endregion

   }
}
