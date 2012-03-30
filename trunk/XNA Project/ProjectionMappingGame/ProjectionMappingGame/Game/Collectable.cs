using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.Game
{
    public class Collectable : MoveableObject
    {
        const float SPEED = 120;

        float m_SpawnTimer;

        public Collectable(Rectangle bounds, Vector2 velocity, Texture2D image)
            : base(bounds, velocity, image)
        {
            Active = false;

            m_SpawnTimer = (float)GameConstants.RANDOM.NextDouble() * (GameConstants.POWERUP_TIME_MAX - GameConstants.POWERUP_TIME_MIN) + GameConstants.POWERUP_TIME_MIN;
        }

        public bool Active
        {
            get;
            protected set;
        }

        public bool SpawnReady
        {
            get;
            protected set;
        }

        public void SetAnimation(Texture2D spriteSheet, float rate, int frames)
        {
            m_CurrentAnimation = new Animation(spriteSheet, frames, rate, true);
        }

        public void Activate(Level lvl)
        {
            lvl.AddObject(this);
            m_Position.X = lvl.WindowSize.X / 2;
            m_Position.Y = lvl.WindowSize.Y / 2;

            float rotation = (float)GameConstants.RANDOM.NextDouble() * MathHelper.TwoPi;

            Vector2 velocity = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(rotation));
            velocity.Normalize();
            m_Velocity = velocity * SPEED;

            Active = true;
            SpawnReady = false;
            m_SpawnTimer = (float)GameConstants.RANDOM.NextDouble() * (GameConstants.POWERUP_TIME_MAX - GameConstants.POWERUP_TIME_MIN) + GameConstants.POWERUP_TIME_MIN;
        }

        public override void Update(float deltaTime)
        {
            if (Active)
            {
                base.Update(deltaTime);
            }
            else
            {
                m_SpawnTimer -= deltaTime;

                if (m_SpawnTimer <= 0)
                {
                    SpawnReady = true;
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (Active)
            {
                base.Draw(batch);
            }
        }
    }

    public class InvinciblePowerup : Collectable
    {
        const float DURATION = 10.0f;
        const float EFFECT = 1.5f;

        public InvinciblePowerup(Rectangle bounds, Vector2 velocity, Texture2D image)
            : base(bounds, velocity, image)
        {
        }

        public override void Collide(MoveableObject obj)
        {
            if (Active)
            {
                Player p = (Player)obj;

                if (p.ChangeStatus(Player.Bonus.INVINCIBLE, DURATION))
                {
                    p.AddSpeedMultiplier(EFFECT, DURATION);
                    Active = false;

                    base.Collide(obj);
                }
            }
            
        }
    }

    public class SpeedBoost : Collectable
    {
        const float DURATION = 15.0f;
        const float EFFECT = 2.0f;

        public SpeedBoost(Rectangle bounds, Vector2 velocity, Texture2D image)
            : base(bounds, velocity, image)
        {
        }

        public override void Collide(MoveableObject obj)
        {
            if (Active)
            {
                Player p = (Player)obj;

                p.AddSpeedMultiplier(EFFECT, DURATION);

                Active = false;

                base.Collide(obj);
            }
        }
    }
}
