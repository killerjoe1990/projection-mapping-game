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
        Color m_Color;

        string m_EnteredNumber;

        KeyboardInput m_Keyboard;
        protected event EventHandler m_OnValueChanged;

        public NumUpDown(Rectangle bounds, Texture2D background, Texture2D upButton, Texture2D downButton, SpriteFont font, Color color, float min, float max, float step, string precision, MouseInput mouse)
        {
            m_Keyboard = new KeyboardInput();

            m_Precision = precision;
            m_Bounds = bounds;
            m_BackgroundImg = background;

            m_Min = min;
            m_Max = max;
            m_StepSize = step;
            m_Current = 0;

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


            m_Keyboard = new KeyboardInput();
            m_Keyboard.RegisterKeyPress(OnKeyPressed);

            m_EnteredNumber = "";
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
            m_Current = MathHelper.Clamp(m_Current + m_StepSize, m_Min, m_Max);
            m_OnValueChanged(this, new EventArgs());
        }
        public void OnDownClick(Object sender, EventArgs args)
        {
            m_Current = MathHelper.Clamp(m_Current - m_StepSize, m_Min, m_Max);
            m_OnValueChanged(this, new EventArgs());
        }

        public void OnKeyPressed(Object sender, Keys[] args)
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
                    /*switch (k)
                    {
                        case Keys.D1:
                            m_EnteredNumber += '1';
                            break;
                        case Keys.D2:
                            m_EnteredNumber += '2';
                            break;
                        case Keys.D3:
                            m_EnteredNumber += '3';
                            break;
                        case Keys.D4:
                            m_EnteredNumber += '4';
                            break;
                        case Keys.D5:
                            m_EnteredNumber += '5';
                            break;
                        case Keys.D6:
                            m_EnteredNumber += '6';
                            break;
                        case Keys.D7:
                            m_EnteredNumber += '7';
                            break;
                        case Keys.D8:
                            m_EnteredNumber += '8';
                            break;
                        case Keys.D9:
                            m_EnteredNumber += '9';
                            break;
                        case Keys.D0:
                            m_EnteredNumber += '0';
                            break;
                        case Keys.Decimal:
                            m_EnteredNumber += '.';
                            break;
                    }
                     * */
                }
            }
        }

        public override void Update(float deltaTime)
        {
            if (m_EnteredNumber != "")
            {
                float.TryParse(m_EnteredNumber, out m_Current);
            }
            m_Keyboard.HandleInput(PlayerIndex.One);
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
            Rectangle num = new Rectangle(m_Bounds.X, m_Bounds.Y, m_Bounds.Width, m_Bounds.Height);
            sprite.Draw(m_BackgroundImg, num, Color.White);

            m_Up.Draw(graphics, sprite);
            m_Down.Draw(graphics, sprite);

            int x, y;
            string val = String.Format(m_Precision, m_Current);
            Vector2 dim = m_Font.MeasureString(val);
            x = (int)(num.X + 4);
            y = (int)(num.Y + 2);

            sprite.DrawString(m_Font, val, new Vector2(x, y), m_Color);
        }
    }
}
