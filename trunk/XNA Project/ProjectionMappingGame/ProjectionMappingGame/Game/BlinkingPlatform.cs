using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectionMappingGame.Game
{
    class BlinkingPlatform : Platform
    {
        public bool m_IsBlinking;

        private float m_TimerToDestroyItself;

        private Texture2D m_White;

        public BlinkingPlatform(Vector2 position, Vector2 velocity, int tilesWide, Texture2D[] images)
            : base(position, velocity, tilesWide, images)
        {
            m_IsBlinking = false;

            m_TimerToDestroyItself = 2.0f;

            Blink = true;

            m_White = GameConstants.WHITE_TEXTURE;
        }

        public override bool Collide(MoveableObject obj, CollisionDirections direction)
        {
            if (direction == CollisionDirections.Bot)
            {

                int chanceToBlink = GameConstants.RANDOM.Next(0, 100);
                if (!m_IsBlinking && chanceToBlink < GameConstants.BLINKPLAT_CHANCE_TO_BLINK_ON_JUMP)
                {
                    m_IsBlinking = true;
                    m_TimerToDestroyItself = 2.0f;
                }
                return true;
            }
            return false;
        }

        public override void Update(float deltaTime)
        {
            if (m_IsBlinking)
            {
                m_TimerToDestroyItself -= deltaTime;

                if (m_TimerToDestroyItself <= 0)
                {
                    SetToDestroyEvent(this, null);
                    m_IsBlinking = false;
                }
            }
            base.Update(deltaTime);  
        }
        public override void Draw(SpriteBatch batch, Color color)
        {
            if (this.m_Status != PlatformStatus.Dead)
            {
                float intensity = ((float)Math.Cos(15 * m_TimerToDestroyItself) + 1) / 2.0f;

                Color blinkingColor = Color.White;
                
                float fade = color.R / 255.0f;

                blinkingColor *= fade;

                if (m_IsBlinking)
                {
                    blinkingColor.A = (byte)(255 * intensity);
                }
                else
                {
                    blinkingColor.A = 255;
                }

                

                base.Draw(batch, blinkingColor);
            }

            
        }
        private void SetToDestroyEvent(object source, ElapsedEventArgs e)
        {
            m_Status = PlatformStatus.Dead;
        }
    }
}
