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
    class RedBlueTheme : Theme
    {
        const float BACKGROUND_RATE = 1;

        public RedBlueTheme(ThemeTextures textures, Point windowSize)
            : base(textures, windowSize)
        {
            m_Background = new AnimatedBackground(textures.Background, BACKGROUND_RATE, windowSize.X, windowSize.Y);
        }

        public override void Update(float deltaTime)
        {
            m_Background.Update(deltaTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Microsoft.Xna.Framework.Color color)
        {
            m_Background.SetColor(color);
            m_Background.Draw(batch, SpriteEffects.None);
        }
    }
}
