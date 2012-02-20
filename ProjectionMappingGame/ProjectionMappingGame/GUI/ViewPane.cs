using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectionMappingGame.Input;

namespace ProjectionMappingGame.GUI
{
    /// <summary>
    /// A class that can keep track of multiple ViewPane instances. This will keep track of which
    /// ViewPane is in context for input handling purposes.
    /// </summary>
    public class ViewPaneController : UIElement
    {
        protected List<ViewPane> m_Views;
        protected ViewPane m_CurrentContext;

        protected Texture2D m_BorderImage;

        protected int m_Width;
        protected int m_Height;

        public ViewPaneController(int width, int height)
        {
            m_Width = width;
            m_Height = height;

            m_Views = new List<ViewPane>();
            m_CurrentContext = null;
        }

        /// <summary>
        /// Adds a ViewPane object to be controlled
        /// </summary>
        /// <param name="view">The object to add</param>
        public void AddViewPane(ViewPane view)
        {
            m_Views.Add(view);

            // make positioning relative to upper left corner of this element
            Vector2 l = view.Location;
            l.X += m_Bounds.X;
            l.Y += m_Bounds.Y;
            view.Location = l;

            view.SetController(this);

            if (m_CurrentContext == null)
            {
                m_CurrentContext = view;
            }
        }

        /// <summary>
        /// Sets the current context pane of the controller to the provided ViewPane.
        /// If the ViewPane has not been added to the controller it will be added here.
        /// </summary>
        /// <param name="view">The object to set.</param>
        public void SetContextPane(ViewPane view)
        {
            m_CurrentContext = view;

            if (!m_Views.Contains(view))
            {
                m_Views.Add(view);
            }
        }

        /// <summary>
        /// Update each ViewPane controlled
        /// </summary>
        /// <param name="deltaTime">The time from last frame</param>
        public override void Update(float deltaTime)
        {
            foreach (ViewPane v in m_Views)
            {
                v.Update(deltaTime);
            }
        }

        /// <summary>
        /// Displays each ViewPane controlled
        /// </summary>
        /// <param name="graphics">game's Graphics Device</param>
        /// <param name="sprite">game's Sprite Batch</param>
        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
            foreach (ViewPane v in m_Views)
            {
                v.Draw(graphics, sprite);
            }
        }

        /// <summary>
        /// Handle input only on the ViewPane in context.
        /// </summary>
        /// <param name="player">The player taking action.</param>
        /// <returns></returns>
        public bool HandleInput(int player)
        {
            return m_CurrentContext.HandleInput(player);
        }

    }


    

    /// <summary>
    /// An element that can be used as a background to contain multiple other UIElements.
    /// The area can be set to a static image, or a game render target. 
    /// </summary>
    public class ViewPane : ClickableElement
    {
        /// <summary>
        /// How the Title will be displayed
        /// </summary>
        public enum TitleFormat
        {
            LEFT,
            RIGHT,
            CENTERED,
            NONE
        }

        private const int TITLE_BUFFER = 0;

        protected Texture2D m_RenderTarget;
        protected Texture2D m_BorderImage;
        protected int m_BorderThickness;

        protected string m_Title;
        protected SpriteFont m_TitleFont;
        protected TitleFormat m_TitleFormat;
        protected Color m_TitleColor;
        protected Texture2D m_TitleImage;

        protected InputController m_Input;
        protected List<ClickableElement> m_Clickables;

        protected ViewPaneController m_Controller;

        public ViewPane(Rectangle bounds, ref Texture2D target, InputController input, MouseInput mouse)
        {
            m_Bounds = bounds;

            m_Title = "";
            m_TitleFont = null;
            m_TitleFormat = TitleFormat.NONE;
            m_TitleColor = Color.Black;
            m_TitleImage = null;

            m_BorderImage = null;
            m_BorderThickness = 0;
            m_RenderTarget = target;
            m_Input = input;

            m_Clickables = new List<ClickableElement>();

            m_Controller = null;
            mouse.RegisterMouseEvent(MouseEventType.LeftClick, OnLeftClick);
            mouse.RegisterMouseEvent(MouseEventType.RightClick, OnRightClick);
        }

        /// <summary>
        /// Sets the ViewPaneController that will handle this pane. Will only be called by the controller itself.
        /// </summary>
        /// <param name="paneControll">The controller to set</param>
        public void SetController(ViewPaneController paneControll)
        {
            m_Controller = paneControll;
        }

        /// <summary>
        /// Add a clickable element to the list of elements handled by this pane.
        /// </summary>
        /// <param name="element">Element to add</param>
        public void AddClickableElement(ClickableElement element)
        {
            m_Clickables.Add(element);
            AddChild(element);
        }

        /// <summary>
        /// Adds a border image to the viewpane. 
        /// </summary>
        /// <param name="image">The image to add</param>
        public void AddBorder(Texture2D image, int thickness)
        {
            m_BorderImage = image;
            m_BorderThickness = thickness;
        }

        /// <summary>
        /// Adds a title to the pane.
        /// </summary>
        /// <param name="title">The title of the pane</param>
        /// <param name="format">The format to display the title</param>
        /// <param name="font">The font of the title</param>
        /// <param name="image">The image displayed behind the title</param>
        /// <param name="color">The color of the text</param>
        public void AddTitle(string title, TitleFormat format, SpriteFont font, Texture2D image, Color color)
        {
            m_Title = title;
            m_TitleFormat = format;
            m_TitleFont = font;
            m_TitleImage = image;
            m_TitleColor = color;
        }

        public bool HandleInput(int player)
        {
            if (m_Input != null)
            {
                m_Input.HandleInput(player);

                return true;
            }

            return false;
        }

        public override void OnLeftClick(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X,args.Y) && m_Controller != null)
            {
                m_Controller.SetContextPane(this);
            }
        }

        public override void OnRightClick(object sender, MouseEventArgs args)
        {
            if (IsOver(args.X, args.Y) && m_Controller != null)
            {
                m_Controller.SetContextPane(this);
            }
        }

        public override void Update(float deltaTime)
        {
            foreach (UIElement child in m_Children)
            {
                child.Update(deltaTime);
            }
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite)
        {
            Vector2 titleSize = m_TitleFont.MeasureString(m_Title);

            Rectangle view = new Rectangle(m_Bounds.X + m_BorderThickness, m_Bounds.Y + m_BorderThickness + (int)titleSize.Y + TITLE_BUFFER, m_Bounds.Width - 2 * m_BorderThickness, m_Bounds.Height - (2 * m_BorderThickness + (int)titleSize.Y + TITLE_BUFFER));

            if (m_RenderTarget != null)
            {
                sprite.Draw(m_RenderTarget, view, Color.White);
            }

            foreach (UIElement child in m_Children)
            {
                child.Draw(graphics, sprite);
            }

            switch (m_TitleFormat)
            {
                case TitleFormat.CENTERED:
                    float x = m_Bounds.X + ((m_Bounds.Width / 2) - (titleSize.X / 2));
                    sprite.Draw(m_TitleImage, new Rectangle(m_Bounds.X + m_BorderThickness, m_Bounds.Y + m_BorderThickness, m_Bounds.Width - 2 * m_BorderThickness, (int)titleSize.Y + TITLE_BUFFER), Color.White);
                    sprite.DrawString(m_TitleFont, m_Title, new Vector2(x, m_Bounds.Y + m_BorderThickness), m_TitleColor);
                    break;
                case TitleFormat.LEFT:
                    sprite.Draw(m_TitleImage, new Rectangle(m_Bounds.X + m_BorderThickness, m_Bounds.Y + m_BorderThickness, m_Bounds.Width - 2 * m_BorderThickness, (int)titleSize.Y + TITLE_BUFFER), Color.White);
                    sprite.DrawString(m_TitleFont, m_Title, new Vector2(m_Bounds.X + m_BorderThickness, m_Bounds.Y + m_BorderThickness), m_TitleColor);
                    break;
                case TitleFormat.RIGHT:
                    sprite.Draw(m_TitleImage, new Rectangle(m_Bounds.X + m_BorderThickness, m_Bounds.Y + m_BorderThickness, m_Bounds.Width - 2 * m_BorderThickness, (int)titleSize.Y + TITLE_BUFFER), Color.White);
                    sprite.DrawString(m_TitleFont, m_Title, new Vector2(m_Bounds.X + (m_Bounds.Width - titleSize.X - m_BorderThickness), m_Bounds.Y + m_BorderThickness), m_TitleColor);
                    break;
            }

            if (m_BorderImage != null)
            {
                Rectangle border = new Rectangle(m_Bounds.X, m_Bounds.Y, m_Bounds.Width, m_BorderThickness);
                sprite.Draw(m_BorderImage, border, Color.White);

                border.Y = m_Bounds.Y + (m_Bounds.Height - m_BorderThickness);
                sprite.Draw(m_BorderImage, border, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);

                border.Y = m_Bounds.Y + m_BorderThickness;
                border.X += m_BorderThickness;
                //border.Width = m_BorderThickness;
                border.Width = m_Bounds.Height - 2 * m_BorderThickness;
                sprite.Draw(m_BorderImage, border, null, Color.White, (float)Math.PI / 2.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);

                //border.X = m_Bounds.X + m_Bounds.Width - m_BorderThickness;
                border.X = m_Bounds.Right;
                sprite.Draw(m_BorderImage, border, null, Color.White, (float)Math.PI / 2.0f, Vector2.Zero, SpriteEffects.None, 0);
            }

            
        }
    }
}
