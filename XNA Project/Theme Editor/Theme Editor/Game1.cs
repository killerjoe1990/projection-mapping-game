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

        bool m_EditorOpen;

        public Game1()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

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

        protected void OpenEditor()
        {
           m_EditorOpen = true;
           DialogResult r = m_ThemeForm.ShowDialog();

           if (r == DialogResult.OK)
           {
               CreatePreview();
           }

           m_EditorOpen = false;
        }

        protected void CreatePreview()
        {
            Texture2D[] backgroundFrames = new Texture2D[m_ThemeForm.m_Backgrounds.Length];

            m_Platforms = new List<Platform>();

            for(int i = 0; i < backgroundFrames.Length; ++i)
            {
                backgroundFrames[i] = Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(m_ThemeForm.m_Backgrounds[i]));
            }

            m_Background = new AnimatedBackground(backgroundFrames, m_ThemeForm.m_BackgroundRate, GameConstants.DEFAULT_WINDOW_WIDTH, GameConstants.DEFAULT_WINDOW_HEIGHT);

            Texture2D[][] platTextures = new Texture2D[m_ThemeForm.m_Plats.Count][];

            for (int i = 0; i < platTextures.Length; ++i)
            {
                PlatformValues pv = m_ThemeForm.m_Plats.ElementAt(i).Value;
                List<Texture2D> plats = new List<Texture2D>();

                if (pv.LeftImage != null)
                {
                    plats.Add(Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(pv.LeftImage)));
                }

                foreach (string s in pv.CenterImages)
                {
                    plats.Add(Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(s)));
                }

                if(pv.RightImage != null)
                {
                    plats.Add(Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(pv.RightImage)));
                }
                
                platTextures[i] = plats.ToArray();
                
            }

            m_PlatSpawn = new PlatformSpawner(platTextures, GameConstants.DEFAULT_WINDOW_WIDTH);

            m_Objects = new List<MoveableObject>();

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

                    obj.Animation = new Animation(Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(m_ThemeForm.m_MSprites.ElementAt(texIndex).Key)), sv.Frames, sv.Rate, true);

                    m_Objects.Add(obj);
                }
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

                    int texIndex = GameConstants.RANDOM.Next(m_ThemeForm.m_SSprites.Count);

                    SpriteValues sv = m_ThemeForm.m_SSprites.ElementAt(texIndex).Value;
                    string key = m_ThemeForm.m_SSprites.ElementAt(texIndex).Key;

                    obj.Animation = new Animation(Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(key)), sv.Frames, sv.Rate, false);
                    obj.Animation.RegisterAnimationEnd(StaticObjectDone);

                    m_Objects.Add(obj);

                    m_StaticTimer = (float)GameConstants.RANDOM.NextDouble() * (m_ThemeForm.m_TimeRange) + m_ThemeForm.m_MinTime;
                }
            }

            if (!m_EditorOpen && Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                OpenEditor();
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

            base.Draw(gameTime);
        }
    }
}
