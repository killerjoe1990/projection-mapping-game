using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;

namespace Theme_Editor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        SpriteBatch m_SpriteBatch;

        PlatformSpawner m_PlatSpawn;

        AnimatedBackground m_Background;
        List<MoveableObject> m_Objects;
        List<Platform> m_Platforms;
        float m_StaticTimer;

        AddThemeForm m_ThemeForm;

        Point m_WindowSize;

        Texture2D[] m_BackgroundFrames;
        Texture2D[][] m_PlatTextures;
        Texture2D[] m_MovingTextures;
        Texture2D[] m_StaticTextures;

        bool m_EditorOpen;
        bool m_Quit;

        public Game1()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_Quit = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_ThemeForm = new AddThemeForm();

            m_WindowSize = new Point(GameConstants.DEFAULT_WINDOW_WIDTH, GameConstants.DEFAULT_WINDOW_HEIGHT);

            m_Graphics.PreferredBackBufferWidth = m_WindowSize.X;
            m_Graphics.PreferredBackBufferHeight = m_WindowSize.Y;
            m_Graphics.ApplyChanges();

            IsFixedTimeStep = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            OpenEditor();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void Reset()
        {
            m_SpriteBatch.Dispose();

            m_Background = null;
            m_Objects = null;
            m_Platforms = null;
            m_PlatSpawn = null;
            
            foreach(Texture2D t in m_BackgroundFrames)
            {
                t.Dispose();
            }

            m_BackgroundFrames = null;

            foreach (Texture2D[] ta in m_PlatTextures)
            {
                foreach (Texture2D t in ta)
                {
                    t.Dispose();
                }
            }

            m_PlatTextures = null;

            foreach (Texture2D t in m_MovingTextures)
            {
                t.Dispose();
            }

            m_MovingTextures = null;

            foreach (Texture2D t in m_StaticTextures)
            {
                t.Dispose();
            }

            m_StaticTextures = null;
        }

        protected void OpenEditor()
        {
           m_EditorOpen = true;
           DialogResult r = m_ThemeForm.ShowDialog();

           if (r == DialogResult.OK)
           {
               CreatePreview();
           }
           else
           {
               m_Quit = true;
               Exit();
           }

           m_EditorOpen = false;
        }

        protected void CreatePreview()
        {
            System.IO.Stream stream;
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            m_BackgroundFrames = new Texture2D[m_ThemeForm.m_Backgrounds.Length];

            m_Platforms = new List<Platform>();

            for(int i = 0; i < m_BackgroundFrames.Length; ++i)
            {
                stream = System.IO.File.OpenRead(m_ThemeForm.m_Backgrounds[i]);
                m_BackgroundFrames[i] = Texture2D.FromStream(GraphicsDevice, stream);
                stream.Close();
            }

            m_Background = new AnimatedBackground(m_BackgroundFrames, m_ThemeForm.m_BackgroundRate, GameConstants.DEFAULT_WINDOW_WIDTH, GameConstants.DEFAULT_WINDOW_HEIGHT);

            m_PlatTextures = new Texture2D[m_ThemeForm.m_Plats.Count][];

            for (int i = 0; i < m_PlatTextures.Length; ++i)
            {
                PlatformValues pv = m_ThemeForm.m_Plats.ElementAt(i).Value;
                List<Texture2D> plats = new List<Texture2D>();

                if (pv.LeftImage != null)
                {
                    stream = System.IO.File.OpenRead(pv.LeftImage);
                    plats.Add(Texture2D.FromStream(GraphicsDevice, stream));
                    stream.Close();
                }

                foreach (string s in pv.CenterImages)
                {
                    stream = System.IO.File.OpenRead(s);
                    plats.Add(Texture2D.FromStream(GraphicsDevice, stream));
                    stream.Close();
                }

                if(pv.RightImage != null)
                {
                    stream = System.IO.File.OpenRead(pv.RightImage);
                    plats.Add(Texture2D.FromStream(GraphicsDevice, stream));
                    stream.Close();
                }
                
                m_PlatTextures[i] = plats.ToArray();
                
            }

            m_PlatSpawn = new PlatformSpawner(m_PlatTextures, GameConstants.DEFAULT_WINDOW_WIDTH);

            m_Objects = new List<MoveableObject>();

            m_MovingTextures = new Texture2D[m_ThemeForm.m_MSprites.Count];

            for (int i = 0; i < m_MovingTextures.Length; ++i)
            {
                stream = System.IO.File.OpenRead((string)m_ThemeForm.m_MSprites.Keys.ElementAt(i));
                m_MovingTextures[i] = Texture2D.FromStream(GraphicsDevice, stream);
                stream.Close();
            }

            if (m_ThemeForm.m_MSprites.Count > 0)
            {
                for (int i = 0; i < m_ThemeForm.m_NumObjects; ++i)
                {
                    int texIndex = GameConstants.RANDOM.Next(m_ThemeForm.m_MSprites.Count);
                    SpriteValues sv = m_ThemeForm.m_MSprites.ElementAt(texIndex).Value;

                    int size = GameConstants.RANDOM.Next(m_ThemeForm.m_MSsizeRange) + m_ThemeForm.m_MSminSize;
                    float speed = (float)GameConstants.RANDOM.NextDouble() * (m_ThemeForm.m_SpeedRange) + m_ThemeForm.m_MinSpeed;
                    Vector2 position = new Vector2();
                    position.X = (float)GameConstants.RANDOM.NextDouble() * GameConstants.DEFAULT_WINDOW_WIDTH;
                    position.Y = (float)GameConstants.RANDOM.NextDouble() * GameConstants.DEFAULT_WINDOW_HEIGHT;
                    Vector2 direction = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ((float)GameConstants.RANDOM.NextDouble() * MathHelper.TwoPi));

                    MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), speed * direction);

                    obj.Animation = new Animation(m_MovingTextures[texIndex], sv.Frames, sv.Rate, true);

                    m_Objects.Add(obj);
                }
            }

            m_StaticTextures = new Texture2D[m_ThemeForm.m_SSprites.Count];

            for (int i = 0; i < m_StaticTextures.Length; ++i)
            {
                stream = System.IO.File.OpenRead((string)m_ThemeForm.m_SSprites.Keys.ElementAt(i));
                m_StaticTextures[i] = Texture2D.FromStream(GraphicsDevice,stream);
                stream.Close();
            }

            m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (m_ThemeForm.m_TimeRange) + m_ThemeForm.m_MinTime;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!m_Quit && !m_EditorOpen)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.Ticks / (float)System.TimeSpan.TicksPerSecond;

                m_Background.Update(deltaTime);

                List<Platform> newPlats = m_PlatSpawn.SpawnPlatforms(deltaTime);

                if (newPlats != null)
                {
                    m_Platforms.AddRange(newPlats);
                }

                for (int i = m_Platforms.Count - 1; i >= 0; --i)
                {
                    m_Platforms[i].Update(deltaTime);

                    if (m_Platforms[i].Status == PlatformStatus.Dead)
                    {
                        m_Platforms.RemoveAt(i);
                    }
                }

                for (int i = m_Objects.Count - 1; i >= 0; --i)
                {
                    m_Objects[i].Update(deltaTime);
                }

                foreach (MoveableObject obj in m_Objects)
                {
                    obj.ScreenBounce(obj.CheckBounds(m_WindowSize.X, m_WindowSize.Y), m_WindowSize);
                }

                if (m_ThemeForm.m_SSprites.Count > 0)
                {
                    m_StaticTimer -= deltaTime;

                    if (m_StaticTimer < 0)
                    {
                        int size = GameConstants.RANDOM.Next(m_ThemeForm.m_SSsizeRange) + m_ThemeForm.m_SSminSize;
                        Vector2 position = new Vector2();
                        position.X = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.X;
                        position.Y = (float)GameConstants.RANDOM.NextDouble() * m_WindowSize.Y;

                        MoveableObject obj = new MoveableObject(new Rectangle((int)position.X, (int)position.Y, size, size), Vector2.Zero);

                        int texIndex = GameConstants.RANDOM.Next(m_StaticTextures.Length);

                        SpriteValues sv = m_ThemeForm.m_SSprites.ElementAt(texIndex).Value;

                        obj.Animation = new Animation(m_StaticTextures[texIndex], sv.Frames, sv.Rate, false);
                        obj.Animation.RegisterAnimationEnd(StaticObjectDone);

                        m_Objects.Add(obj);

                        m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (m_ThemeForm.m_TimeRange) + m_ThemeForm.m_MinTime;
                    }
                }

                if (!m_EditorOpen && Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    Reset();
                    OpenEditor();
                }
            }
            base.Update(gameTime);
        }

        public void StaticObjectDone(object sender, EventArgs args)
        {
            for (int i = m_Objects.Count - 1; i >= 0; --i)
            {
                if (m_Objects[i].Animation == sender)
                {
                    m_Objects.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (!m_Quit && !m_EditorOpen)
            {
                m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                m_Background.Draw(m_SpriteBatch, SpriteEffects.None);

                foreach (MoveableObject obj in m_Objects)
                {
                    obj.Draw(m_SpriteBatch);
                }

                foreach (Platform p in m_Platforms)
                {
                    p.Draw(m_SpriteBatch, Color.White);
                }

                m_SpriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
