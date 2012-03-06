using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Local imports
using ProjectionMappingGame.GUI;

namespace ProjectionMappingGame.Editor
{
   public enum LayerType
   {
      Gameplay,
      Texture,
      Particle
   };

   public abstract class Layer
   {
      public const int LAYER_HEIGHT = 20;
      public const int LAYER_WIDTH = 200;
      public const int LAYER_PADDING_Y = 5;
      public const int NAME_X = VISIBILITY_BUTTON_X + VISIBILITY_BUTTON_WIDTH + 2;
      public const int VISIBILITY_BUTTON_X = 2;
      public const int VISIBILITY_BUTTON_WIDTH = 16;
      public const int VISIBILITY_BUTTON_HEIGHT = 16;
      public const int EDIT_BUTTON_X = LAYER_WIDTH - EDIT_BUTTON_WIDTH - 12;
      public const int EDIT_BUTTON_WIDTH = 16;
      public const int EDIT_BUTTON_HEIGHT = 16;

      // Static Identification
      public static int LAYER_COUNTER = 0;
      public int ID;
      public int TYPE_ID;

      protected string m_LayerName;
      protected bool m_IsEditable;
      protected bool m_IsSelected;
      protected bool m_IsVisible;
      protected LayerType m_Type;

      protected Label m_NameLabel;
      protected Button m_VisibleButton;
      protected Button m_EditButton;

      public Layer(LayerType type, Label nameLabel, Button visibleBtn, Button editBtn)
      {
         // ID the layer
         ID = LAYER_COUNTER++;

         // Store settings
         m_Type = type;
         m_NameLabel = nameLabel;
         m_VisibleButton = visibleBtn;
         m_EditButton = editBtn;
         m_VisibleButton.RegisterOnClick(VisibleButton_OnClicked);
         m_EditButton.RegisterOnClick(EditButton_OnClicked);

         // Defaults
         m_IsSelected = false;
         m_IsVisible = true;
      }

      private void VisibleButton_OnClicked(object sender, EventArgs e)
      {

      }

      private void EditButton_OnClicked(object sender, EventArgs e)
      {

      }

      #region Public Access TV

      public Label NameLabel
      {
         get { return m_NameLabel; }
      }

      public Button VisibleButton
      {
         get { return m_VisibleButton; }
      }

      public Button EditButton
      {
         get { return m_EditButton; }
      }

      public LayerType Type
      {
         get { return m_Type; }
         set { m_Type = value; }
      }

      public bool Visible
      {
         get { return m_IsVisible; }
         set { m_IsVisible = value; }
      }

      public bool Selected
      {
         get { return m_IsSelected; }
         set { m_IsSelected = value; }
      }

      public bool Editable
      {
         get { return m_IsEditable; }
         set { m_IsEditable = value; }
      }

      public string Name
      {
         get { return m_LayerName; }
         set { m_LayerName = value; }
      }

      #endregion

   }

   public class GameplayLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public GameplayLayer(Label nameLabel, Button visibleBtn, Button editBtn)
         : base(LayerType.Gameplay, nameLabel, visibleBtn, editBtn)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = false;
         m_LayerName = LayerType.Gameplay.ToString() + " " + (TYPE_ID + 1).ToString();
         m_NameLabel.Text = m_LayerName;
      }
   }

   public class TextureLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public TextureLayer(Label nameLabel, Button visibleBtn, Button editBtn)
         : base(LayerType.Texture, nameLabel, visibleBtn, editBtn)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = true;
         m_LayerName = LayerType.Texture.ToString() + " " + TYPE_ID.ToString();
      }
   }

   public class ParticleLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public ParticleLayer(Label nameLabel, Button visibleBtn, Button editBtn)
         : base(LayerType.Particle, nameLabel, visibleBtn, editBtn)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = true;
         m_LayerName = LayerType.Particle.ToString() + " " + TYPE_ID.ToString();
      }
   }
}
