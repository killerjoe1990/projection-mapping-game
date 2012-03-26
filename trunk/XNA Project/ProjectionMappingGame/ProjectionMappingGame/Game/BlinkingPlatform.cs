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
        public bool isBlinking;

        private bool isLoweringNumber;
        private Color colorForAlpha = Color.White;
        private Timer timerToDestroyItself;

        public BlinkingPlatform(Vector2 position, Vector2 velocity, int tilesWide, Texture2D[] images)
            : base(position, velocity, tilesWide, images)
        {
            isBlinking = false;
            isLoweringNumber = true;
            timerToDestroyItself = new Timer(2000);
            timerToDestroyItself.Elapsed += new ElapsedEventHandler(SetToDestroyEvent);
            
        }

        public override bool Collide(MoveableObject obj, CollisionDirections direction)
        {
            if (direction == CollisionDirections.Bot)
            {

                int chanceToBlink = GameConstants.RANDOM.Next(0, 100);
                if (chanceToBlink < GameConstants.BLINKPLAT_CHANCE_TO_BLINK_ON_JUMP)
                {
                    isBlinking = true;
                    timerToDestroyItself.Enabled = true;
                }
                return true;
            }
            return false;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
           
        }
        public override void Draw(SpriteBatch batch)
        {
            if (this.m_Status != PlatformStatus.Dead)
            {

                base.Draw(batch);
                if (isBlinking == true && this.colorForAlpha.A > 0 && isLoweringNumber == true)
                {
                    colorForAlpha.A-= 5;
                    if (colorForAlpha.A <= 0)
                    {
                        colorForAlpha.A = 0;
                        isLoweringNumber = false;
                    }
                    foreach (MoveableObject o in Tiles)
                    {
                        o.SetColor(colorForAlpha);
                    }
                }
                else if (isBlinking == true && this.colorForAlpha.A < 255 && isLoweringNumber == false)
                {
                    colorForAlpha.A+= 5;
                    if (colorForAlpha.A >= 255)
                    {
                        colorForAlpha.A = 255;
                        isLoweringNumber = true;
                    }
                    foreach (MoveableObject o in Tiles)
                    {
                        o.SetColor(colorForAlpha);
                    }
                }
            }
        }
        private void SetToDestroyEvent(object source, ElapsedEventArgs e)
        {
            this.m_Status = PlatformStatus.Dead;
        }
    }
}
