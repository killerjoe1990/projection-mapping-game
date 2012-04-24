using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.Game
{
    public class Portal : MoveableObject
    {
        Color m_Color;

        public Portal(Rectangle bounds, int destination, Texture2D texture, Color c)
            : base(bounds, Vector2.Zero, texture)
        {
            Destination = destination;
            m_CurrentAnimation = new Animation(texture, GameConstants.PORTAL_FRAMES, GameConstants.PORTAL_FRAMERATE, true);
            m_CurrentAnimation.SetColor(c);
            m_Color = c;

        }

        public int Destination
        {
            get;
            set;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch, m_Color);
        }
    }
}
