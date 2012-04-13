using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectionMappingGame.FileSystem;
using ProjectionMappingGame.StateMachine;

namespace ProjectionMappingGame.Game.Themes
{
    public abstract class Theme
    {
        // Contains all Texture2D data needed.
        protected ThemeTextures m_Textures;
        protected Point m_WindowSize;
        protected AnimatedBackground m_Background;

        public Theme(ThemeTextures textures, Point windowSize)
        {
            m_Textures = textures;
            m_WindowSize = windowSize;
        }

        public abstract void Update(float deltaTime);

        public abstract void Draw(SpriteBatch batch, Color color);
    }
}
