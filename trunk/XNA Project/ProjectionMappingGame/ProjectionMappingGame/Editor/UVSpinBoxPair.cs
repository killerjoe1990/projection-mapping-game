#region File Description

//-----------------------------------------------------------------------------
// UVSpinBoxPair.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/27/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local imports
using ProjectionMappingGame.GUI;

#endregion

namespace ProjectionMappingGame.Editor
{
   class UVSpinBoxPair
   {
      Vector2 m_Location;
      Label m_ULabel;
      Label m_VLabel;
      NumUpDown m_USpinBox;
      NumUpDown m_VSpinBox;
      Texture2D m_WhiteTexture;

      const int GUI_SPINBOX_WIDTH = 40;
      const int GUI_SPINBOX_HEIGHT = 20;

      public UVSpinBoxPair(Vector2 location, Texture2D whiteTexture, Texture2D spinboxFill, Texture2D spinboxUp, Texture2D spinboxDown, SpriteFont font, MouseInput mouseInput)
      {
         m_Location = location;
         m_WhiteTexture = whiteTexture;

         m_ULabel = new Label("U", (int)location.X + 4, (int)location.Y + 4, 10, GUI_SPINBOX_HEIGHT, Color.Black);
         m_VLabel = new Label("V", (int)location.X + 24 + GUI_SPINBOX_WIDTH, (int)location.Y + 4, 10, GUI_SPINBOX_HEIGHT, Color.Black);
         m_ULabel.Font = font;
         m_VLabel.Font = font;

         m_USpinBox = new NumUpDown(new Rectangle((int)location.X + 17, (int)location.Y + 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), spinboxFill, spinboxUp, spinboxDown, font, Color.Black, 0.0f, 1.0f, 0.025f, "{0:0.00}", mouseInput);
         m_USpinBox.RegisterOnValueChanged(U_OnValueChanged);
         m_VSpinBox = new NumUpDown(new Rectangle((int)location.X + 37 + GUI_SPINBOX_WIDTH, (int)location.Y + 2, GUI_SPINBOX_WIDTH, GUI_SPINBOX_HEIGHT), spinboxFill, spinboxUp, spinboxDown, font, Color.Black, 0.0f, 1.0f, 0.025f, "{0:0.00}", mouseInput);
         m_VSpinBox.RegisterOnValueChanged(V_OnValueChanged);
      }

      #region Input Handling

      private void U_OnValueChanged(object sender, EventArgs e)
      {
      }

      private void V_OnValueChanged(object sender, EventArgs e)
      {
      }

      #endregion

      #region Rendering

      public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         spriteBatch.Draw(m_WhiteTexture, new Rectangle((int)m_Location.X, (int)m_Location.Y, 60, 44), Color.Gray);
         m_ULabel.Draw(graphics, spriteBatch);
         m_VLabel.Draw(graphics, spriteBatch);
         m_USpinBox.Draw(graphics, spriteBatch);
         m_VSpinBox.Draw(graphics, spriteBatch);
      }

      #endregion

      public void Shift(Vector2 delta)
      {
         // Shift location
         m_Location += delta;

         // Shift all GUI objects
         m_ULabel.Location += delta;
         m_VLabel.Location += delta;
         m_USpinBox.Location += delta;
         m_VSpinBox.Location += delta;
         m_USpinBox.Children[0].Location += delta;
         m_USpinBox.Children[1].Location += delta;
         m_VSpinBox.Children[0].Location += delta;
         m_VSpinBox.Children[1].Location += delta;
      }

      public Vector2 TexCoord
      {
         set
         {
            m_USpinBox.Value = value.X;
            m_VSpinBox.Value = value.Y;
         }
         get
         {
            return new Vector2(m_USpinBox.Value, m_VSpinBox.Value);
         }
      }


      public Vector2 Location
      {
         set
         {
            Vector2 offset = value - m_Location;
            m_Location = value;

            m_ULabel.Location = m_Location + new Vector2(4, 4);
            m_VLabel.Location = m_Location + new Vector2(4, GUI_SPINBOX_HEIGHT + 4);

            m_USpinBox.Location = m_Location + new Vector2(17, 2);
            m_VSpinBox.Location = m_Location + new Vector2(17, GUI_SPINBOX_HEIGHT + 2);
            m_USpinBox.Children[0].Location = m_Location + new Vector2(48, 2);
            m_USpinBox.Children[1].Location = m_Location + new Vector2(48, 12);
            m_VSpinBox.Children[0].Location = m_Location + new Vector2(48, GUI_SPINBOX_HEIGHT + 2);
            m_VSpinBox.Children[1].Location = m_Location + new Vector2(48, GUI_SPINBOX_HEIGHT + 12);
         }
      }

   }
}
