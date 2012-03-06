
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

// Microsoft includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local includes
using ProjectionMappingGame.StateMachine;

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

      // Rendering members
      GraphicsDeviceManager m_GraphicsManager;  // Handle to active XNA graphics device controller
      SpriteBatch m_SpriteBatch;                // Sprite renderer for 2D

      #region Initialization

      /// <summary>
      /// Default constructor for type GameDriver initializes base XNA
      /// components such as content and the graphics device.
      /// </summary>
      public GameDriver()
      {
         // Initialize graphics device
         m_GraphicsManager = new GraphicsDeviceManager(this);
         m_GraphicsManager.PreferredBackBufferWidth = GameConstants.DEFAULT_WINDOW_WIDTH;
         m_GraphicsManager.PreferredBackBufferHeight = GameConstants.DEFAULT_WINDOW_HEIGHT;
         m_GraphicsManager.PreferMultiSampling = true;
         m_GraphicsManager.ApplyChanges();
         Window.AllowUserResizing = true;
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
   }
}
