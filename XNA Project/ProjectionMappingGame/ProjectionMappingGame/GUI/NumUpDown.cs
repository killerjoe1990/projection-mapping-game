using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.GUI
{
    public class NumUpDown : ClickableElement
    {
        const float NUM_BUTTON_RATIO = 0.8f;

        float m_Max;
        float m_Min;
        float m_Current;
        float m_StepSize;
        string m_Precision;

        Button m_Up;
        Button m_Down;

        SpriteFont m_Font;
        Texture2D m_BackgroundImg;
        Texture2D m_WhiteTexture;
        Color m_Color;

        string m_EnteredNumber;

        KeyboardInput m_Keyboard;
        protected event EventHandler m_OnValueChanged;

        public NumUpDown(Rectangle bounds, Texture2D background, Texture2D white, Texture2D upButton, Texture2D downButton, SpriteFont font, Color color, float min, float max, float step, string precision, MouseInput mouse)
        {
            m_Keyboard = new KeyboardInput();

            m_WhiteTexture = white;
            m_Precision = precision;
            m_Bounds = bounds;
            m_BackgroundImg = background;

            m_Min = min;
            m_Max = max;
            m_StepSize = step;
            m_Current = min;

            m_Font = font;
            m_Color = color;

            Rectangle num = new Rectangle(m_Bounds.X, m_Bounds.Y, (int)(m_Bounds.Width * NUM_BUTTON_RATIO), m_Bounds.Height);
            Rectangle up = new Rectangle(m_Bounds.X + num.Width, m_Bounds.Y, m_Bounds.Width - num.Width, m_Bounds.Height / 2);
            Rectangle down = new Rectangle(m_Bounds.X + num.Width, m_Bounds.Y + up.Height, up.Width, up.Height);

            m_Up = new Button(up, upButton, mouse);
            m_Down = new Button(down, downButton, mouse);

            m_Up.RegisterOnClick(OnUpClick);
            m_Down.RegisterOnClick(OnDownClick);

            AddChild(m_Up);
            AddChild(m_Down);
            mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);


            m_Keyboard = new KeyboardInput();
            m_Keyboard.RegisterKeyPress(OnKeyPressed);

            m_EnteredNumber = "";
        }

        protected override void OnLeftClick(object sender, MouseEventArgs args)
        {
           if (m_IsActive)
           {
                 base.SetContext(IsOver(args.X, args.Y));
           }

        }

        public void RegisterOnValueChanged(EventHandler handler)
        {
           m_OnValueChanged += handler;
        }

        public float Value
        {
            get
            {
                return m_Current;
            }
            set
            {
                m_Current = value;
            }
        }

        public float Max
        {
           get { return m_Max; }
           set { m_Max = value; }
        }

        public float Min
        {
           get { return m_Min; }
           set { m_Min = value; }
        }

        public override void SetContext(bool isInContext)
        {
            if (isInContext)
            {
                m_EnteredNumber = "";
            }
            else
            {
                if (m_EnteredNumber != "")
                {
                    m_Current = float.Parse(m_EnteredNumber);
                }
            }
            base.SetContext(isInContext);
        }
        
        public void OnUpClick(Object sender, EventArgs args)
        {
           if (m_IsActive)
           {
              m_Current = MathHelper.Clamp(m_Current + m_StepSize, m_Min, m_Max);
              if (m_OnValueChanged != null)
                 m_OnValueChanged(this, new EventArgs());
           }
            
        }
        public void OnDownClick(Object sender, EventArgs args)
        {
           if (m_IsActive)
           {
              m_Current = MathHelper.Clamp(m_Current - m_StepSize, m_Min, m_Max);
              if (m_OnValueChanged != null)
                 m_OnValueChanged(this, new EventArgs());
           }
            
        }

        public void OnKeyPressed(Object sender, Keys[] args)
        {
           if (m_IsActive)
           {
              if (m_Context)
              {
                 foreach (Keys k in args)
                 {
                    string s = k.ToString();
                    char x = s[s.Length - 1];
                    if (Char.IsDigit(x))
                    {
                       m_EnteredNumber += x;
                    }
                    else
                    {
                       if (k == Keys.Decimal && m_EnteredNumber.IndexOf('.') < 0)
                       {
                          m_EnteredNumber += '.';
                       }
                    }
                 }
              }
           }
            
        }

        public override void Update(float deltaTime)
        {
            if (m_EnteredNumber != "")
            {
                float.TryParse(m_EnteredNumber, out m_Current);
                m_Current = MathHelper.Min(m_Current, m_Max);
            }
            m_Keyboard.HandleInput(PlayerIndex.One);
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
           if (m_IsVisible)
           {
              Rectangle num = new Rectangle(m_Bounds.X, m_Bounds.Y, m_Bounds.Width, m_Bounds.Height);

              if (m_Context)
              {
                 sprite.Draw(m_BackgroundImg, num, Color.LightGray);
              }
              else
              {
                 sprite.Draw(m_BackgroundImg, num, Color.White);
              }
              m_Up.Draw(graphics, sprite);
              m_Down.Draw(graphics, sprite);

              int x, y;
              string val = String.Format(m_Precision, m_Current);
              Vector2 dim = m_Font.MeasureString(val);
              x = (int)(num.X + 4);
              y = (int)(num.Y + 2);

              sprite.DrawString(m_Font, val, new Vector2(x, y), m_Color);

              if (!m_IsActive)
              {
                 Color shade = Color.Black;
                 shade.A = 128;
                 sprite.Draw(m_WhiteTexture, m_Bounds, shade);
              }
           }
        }

    }
}
