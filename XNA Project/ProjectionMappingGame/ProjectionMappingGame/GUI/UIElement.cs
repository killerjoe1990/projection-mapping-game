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

namespace ProjectionMappingGame.GUI
{
    /// <summary>
    /// The base class for all User Interface Elements in this library.
    /// Almost all classes derive from UIELement. This provides the basic 
    /// attributes and functionality for an element that will be displayed on 
    /// screen.
    /// </summary>
    public abstract class UIElement
    {
        protected List<UIElement> m_Children;
        protected Rectangle m_Bounds;
        protected bool m_IsActive;
        protected bool m_IsVisible;

        public UIElement()
        {
           m_IsActive = true;
           m_IsVisible = true;
            m_Children = new List<UIElement>();
            m_Bounds = Rectangle.Empty;
        }

        /// <summary>
        /// The screen location of the element in pixels.
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return new Vector2(m_Bounds.X, m_Bounds.Y);
            }
            set
            {
                m_Bounds.X = (int)value.X;
                m_Bounds.Y = (int)value.Y;
            }
        }

        public bool IsActive
        {
           get { return m_IsActive; }
           set { m_IsActive = value; }
        }

        public bool IsVisible
        {
           get { return m_IsVisible; }
           set { m_IsVisible = value; }
        }

        /// <summary>
        /// The width of the element in pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return m_Bounds.Width;
            }
            set
            {
                m_Bounds.Width = value;
            }
        }

        /// <summary>
        /// THe height of the element in pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return m_Bounds.Height;
            }
            set
            {
                m_Bounds.Height = value;
            }
        }

        /// <summary>
        /// A list of children associated with this element
        /// </summary>
        public List<UIElement> Children
        {
            get
            {
                return m_Children;
            }
        }

        /// <summary>
        /// Add a child element
        /// </summary>
        /// <param name="child">The UIElement to be added</param>
        public void AddChild(UIElement child)
        {
            m_Children.Add(child);
        }

        /// <summary>
        /// Performs any frame based calculations.
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(float deltaTime)
        {
            foreach (UIElement child in m_Children)
            {
                child.Update(deltaTime);
            }
        }
        /// <summary>
        /// Displays the element on the screen
        /// </summary>
        /// <param name="graphics">Graphics Device of the game</param>
        /// <param name="sprite">Sprite Batch used by the game</param>
        public abstract void Draw(GraphicsDevice graphics, SpriteBatch sprite);
    }

    /// <summary>
    /// The base class for any UIElement that has mouse interaction.
    /// Sliders, buttons, etc. all derive from this class.
    /// </summary>
    public abstract class ClickableElement : UIElement
    {

        protected bool m_Context;

        protected virtual void OnLeftClick(object sender, MouseEventArgs args)
        {

        }

        protected virtual void OnRightClick(object sender, MouseEventArgs args)
        {

        }

        protected virtual void OnLeftDrag(object sender, MouseEventArgs args)
        {

        }

        protected virtual void OnRightDrag(object sender, MouseEventArgs args)
        {

        }

        /// <summary>
        /// Returns true if the given coordinates are over the clickable object.
        /// </summary>
        /// <param name="x">X coordinate in pixels</param>
        /// <param name="y">Y coordinate in pixels</param>
        /// <returns></returns>
        public bool IsOver(int x, int y)
        {
            if (m_Bounds.Intersects(new Rectangle(x, y, 1, 1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void SetContext(bool isInContext)
        {
            m_Context = isInContext;
        }
    }
}
