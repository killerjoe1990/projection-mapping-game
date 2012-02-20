using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.GUI
{
   public class Label : UIElement
   {
      string m_Text;
      SpriteFont m_Font;
      Color m_Color;

      public Label(string text, int x, int y, int w, int h, Color color)
         : base()
      {
         m_Color = color;
         m_Text = text;
         Location = new Vector2(x, y);
         Width = w;
         Height = h;
      }

      public override void Update(float deltaTime)
      {

      }

      public override void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         spriteBatch.DrawString(m_Font, m_Text, Location + Vector2.One, Color.Black);
         spriteBatch.DrawString(m_Font, m_Text, Location, m_Color);
      }

      #region Public Access TV

      /// <summary>
      /// Accessor/Mutator for the current label font.
      /// </summary>
      public SpriteFont Font
      {
         set { m_Font = value; }
         get { return m_Font; }
      }

      /// <summary>
      /// Accessor/Mutator for the current label color.
      /// </summary>
      public Color FillColor
      {
         set { m_Color = value; }
         get { return m_Color; }
      }

      #endregion

   }
}
