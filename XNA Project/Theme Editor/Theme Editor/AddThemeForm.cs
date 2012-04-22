using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Theme_Editor
{
    public struct PlatformValues
    {
        public String LeftImage;
        public List<String> CenterImages;
        public String RightImage;
        public bool Blinking;
    }

    public struct SpriteValues
    {
        public int Frames;
        public float Rate;
    }

    public partial class AddThemeForm : Form
    {
        public float m_BackgroundRate = 1;

        public float m_MinSpeed = 1;
        public float m_SpeedRange = 1;
        public int m_MSminSize = 1;
        public int m_MSsizeRange = 1;
        public int m_NumObjects = 1;

        public float m_MinTime = 1;
        public float m_TimeRange = 1;
        public int m_SSminSize = 1;
        public int m_SSsizeRange = 1;

        int m_PlatNumber = 0;

        public Dictionary<String, PlatformValues> m_Plats;
        public Dictionary<String, SpriteValues> m_MSprites;
        public Dictionary<String, SpriteValues> m_SSprites;

        public string[] m_Backgrounds;

        public AddThemeForm()
        {
            InitializeComponent();

            m_Plats = new Dictionary<string, PlatformValues>();
            m_MSprites = new Dictionary<string, SpriteValues>();
            m_SSprites = new Dictionary<string, SpriteValues>();
        }

        private void btn_AddBackground_Click(object sender, EventArgs e)
        {
            DialogResult r = file_Open.ShowDialog();

            if (r == DialogResult.OK)
            {
                lst_Backgrounds.Items.AddRange(file_Open.FileNames);
            }
        }

        private void lst_Backgrounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_Backgrounds.Items.Count != 0)
            {
                if (lst_Backgrounds.SelectedItem != null)
                {
                    pic_Background.Image = Bitmap.FromFile((string)lst_Backgrounds.SelectedItem);
                }
                else
                {
                    pic_Background.Image = null;
                }
            }
        }

        private void btn_RemoveBackground_Click(object sender, EventArgs e)
        {
            if (lst_Backgrounds.SelectedItem != null)
            {
                pic_Background.Image = null;
                lst_Backgrounds.Items.Remove(lst_Backgrounds.SelectedItem);
            }
        }

        private void btn_BackgroundUp_Click(object sender, EventArgs e)
        {
            if (lst_Backgrounds.SelectedIndex > 0)
            {
                object temp = lst_Backgrounds.SelectedItem;
                int i = lst_Backgrounds.SelectedIndex;
                lst_Backgrounds.Items.Remove(lst_Backgrounds.SelectedItem);
                lst_Backgrounds.Items.Insert(i - 1, temp);
                lst_Backgrounds.SelectedIndex = i - 1;
            }
        }

        private void btn_BackgroundDown_Click(object sender, EventArgs e)
        {
            if (lst_Backgrounds.SelectedIndex < lst_Backgrounds.Items.Count - 1)
            {
                object temp = lst_Backgrounds.SelectedItem;
                int i = lst_Backgrounds.SelectedIndex;
                lst_Backgrounds.Items.Remove(lst_Backgrounds.SelectedItem);
                lst_Backgrounds.Items.Insert(i + 1, temp);
                lst_Backgrounds.SelectedIndex = i + 1;
            }
        }

        private void num_BackgroundRate_ValueChanged(object sender, EventArgs e)
        {
            m_BackgroundRate = (float)num_BackgroundRate.Value;
        }

        private void lst_PlatformLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_PlatformLeft.SelectedItem != null)
            {
                pic_PlatformLeft.Image = Bitmap.FromFile((string)lst_PlatformLeft.SelectedItem);
            }
            else
            {
                pic_PlatformLeft.Image = null;
            }
        }

        private void lst_PlatformCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_PlatformCenter.SelectedItem != null)
            {
                pic_PlatformCenter.Image = Bitmap.FromFile((string)lst_PlatformCenter.SelectedItem);
            }
            else
            {
                pic_PlatformCenter.Image = null;
            }
        }

        private void lst_PlatformRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_PlatformRight.SelectedItem != null)
            {
                pic_PlatformRight.Image = Bitmap.FromFile((string)lst_PlatformRight.SelectedItem);
            }
            else
            {
                pic_PlatformRight.Image = null;
            }
        }

        private void lst_Platforms_SelectedIndexChanged(object sender, EventArgs e)
        {
            lst_PlatformLeft.Items.Clear();
            lst_PlatformCenter.Items.Clear();
            lst_PlatformRight.Items.Clear();

            if (lst_Platforms.SelectedItem != null)
            {
                string key = (string)lst_Platforms.SelectedItem;

                chck_PlatformBlink.Checked = m_Plats[key].Blinking;

                pic_PlatformCenter.Image = null;
                pic_PlatformLeft.Image = null;
                pic_PlatformRight.Image = null;

                lst_PlatformLeft.Items.Add(m_Plats[key].LeftImage);
                lst_PlatformCenter.Items.AddRange(m_Plats[key].CenterImages.ToArray());
                lst_PlatformRight.Items.Add(m_Plats[key].RightImage);
            }
        }

        private void btn_PlatformAdd_Click(object sender, EventArgs e)
        {
            string platName = "Platform" + m_PlatNumber;
            m_PlatNumber++;

            lst_Platforms.Items.Add(platName);

            PlatformValues pv = new PlatformValues();
            pv.CenterImages = new List<string>();
            pv.Blinking = chck_PlatformBlink.Checked;

            if(chck_PlatformBlink.Checked)
            {
                EnsureOneBlinking(platName);
            }

            m_Plats.Add(platName, pv);
        }

        private void EnsureOneBlinking(string platName)
        {
            foreach (object obj in lst_Platforms.Items)
            {
                string name = (string)obj;
                if (name != platName)
                {
                    PlatformValues otherPlat = m_Plats[name];
                    if (otherPlat.Blinking)
                    {
                        otherPlat.Blinking = false;
                        m_Plats[name] = otherPlat;
                    }
                }
            }
        }

        private void chck_PlatformBlink_CheckedChanged(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null)
            {
                PlatformValues p = m_Plats[(string)lst_Platforms.SelectedItem];
                p.Blinking = chck_PlatformBlink.Checked;
                m_Plats[(string)lst_Platforms.SelectedItem] = p;

                if (chck_PlatformBlink.Checked)
                {
                    EnsureOneBlinking((string)lst_Platforms.SelectedItem);
                }
            }
        }

        private void btn_PlatformRemove_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null)
            {
                m_Plats.Remove((string)lst_Platforms.SelectedItem);
                lst_Platforms.Items.Remove(lst_Platforms.SelectedItem);
            }
        }

        private void btn_PlatAddLeft_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null)
            {
                DialogResult r = file_Open.ShowDialog();

                if (r == System.Windows.Forms.DialogResult.OK)
                {
                    string file = file_Open.FileName;
                    lst_PlatformLeft.Items.Clear();
                    lst_PlatformLeft.Items.Add(file);

                    string key = (string)lst_Platforms.SelectedItem;
                    PlatformValues pv = m_Plats[key];
                    pv.LeftImage = file;
                    m_Plats[key] = pv;
                }
            }
        }

        private void btn_PlatRemoveLeft_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null && lst_PlatformLeft.SelectedItem != null)
            {
                lst_PlatformLeft.Items.Clear();

                string key = (string)lst_Platforms.SelectedItem;
                PlatformValues pv = m_Plats[key];
                pv.LeftImage = null;
                m_Plats[key] = pv;
            }
        }

        private void btn_PlatAddCenter_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null)
            {
                DialogResult r = file_Open.ShowDialog();

                if (r == System.Windows.Forms.DialogResult.OK)
                {
                    string[] files = file_Open.FileNames;
                    lst_PlatformCenter.Items.AddRange(files);

                    string key = (string)lst_Platforms.SelectedItem;
                    PlatformValues pv = m_Plats[key];
                    pv.CenterImages.AddRange(files);
                    m_Plats[key] = pv;
                }
            }
        }

        private void btn_PlatRemoveCenter_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null && lst_PlatformCenter.SelectedItem != null)
            {
                string key = (string)lst_Platforms.SelectedItem;
                PlatformValues pv = m_Plats[key];
                pv.CenterImages.Remove((string)lst_PlatformCenter.SelectedItem);
                m_Plats[key] = pv;

                lst_PlatformCenter.Items.Remove((string)lst_PlatformCenter.SelectedItem);
            }
        }

        private void btn_PlatAddRight_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null)
            {
                DialogResult r = file_Open.ShowDialog();

                if (r == System.Windows.Forms.DialogResult.OK)
                {
                    string file = file_Open.FileName;
                    lst_PlatformRight.Items.Clear();
                    lst_PlatformRight.Items.Add(file);

                    string key = (string)lst_Platforms.SelectedItem;
                    PlatformValues pv = m_Plats[key];
                    pv.RightImage = file;
                    m_Plats[key] = pv;
                }
            }
        }

        private void m_PlatRemoveRight_Click(object sender, EventArgs e)
        {
            if (lst_Platforms.SelectedItem != null && lst_PlatformRight.SelectedItem != null)
            {
                lst_PlatformRight.Items.Clear();

                string key = (string)lst_Platforms.SelectedItem;
                PlatformValues pv = m_Plats[key];
                pv.RightImage = null;
                m_Plats[key] = pv;
            }
        }

        private void num_MSpriteMinSize_ValueChanged(object sender, EventArgs e)
        {
            m_MSminSize = (int)num_MSpriteMinSize.Value;
        }

        private void num_MSpriteSizeRange_ValueChanged(object sender, EventArgs e)
        {
            m_MSsizeRange = (int)num_MSpriteSizeRange.Value;
        }

        private void num_MSpriteMinSpeed_ValueChanged(object sender, EventArgs e)
        {
            m_MinSpeed = (float)num_MSpriteMinSpeed.Value;
        }

        private void num_MSpriteSpeedRange_ValueChanged(object sender, EventArgs e)
        {
            m_SpeedRange = (float)num_MSpriteSpeedRange.Value;
        }

        private void num_MSpriteNumber_ValueChanged(object sender, EventArgs e)
        {
            m_NumObjects = (int)num_MSpriteNumber.Value;
        }

        private void lst_MovingSprites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_MovingSprites.SelectedItem != null)
            {
                string key = (string)lst_MovingSprites.SelectedItem;
                pic_MovingSprite.Image = Bitmap.FromFile(key);

                num_MSpriteRate.Value = (decimal)m_MSprites[key].Rate;
                num_MSpriteFrames.Value = m_MSprites[key].Frames;
            }
        }

        private void btn_MSpriteAdd_Click(object sender, EventArgs e)
        {
            DialogResult r = file_Open.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = file_Open.FileNames;
                lst_MovingSprites.Items.AddRange(files);

                foreach (string s in files)
                {
                    SpriteValues sv = new SpriteValues();
                    sv.Frames = (int)num_MSpriteFrames.Value;
                    sv.Rate = (float)num_MSpriteRate.Value;

                    m_MSprites.Add(s, sv);
                }
            }
        }

        private void btn_MSpriteRemove_Click(object sender, EventArgs e)
        {
            if (lst_MovingSprites.SelectedItem != null)
            {
                pic_MovingSprite.Image = null;

                string key = (string)lst_MovingSprites.SelectedItem;
                m_MSprites.Remove(key);

                lst_MovingSprites.Items.Remove(lst_MovingSprites.SelectedItem);
            }
        }

        private void num_MSpriteFrames_ValueChanged(object sender, EventArgs e)
        {
            if (lst_MovingSprites.SelectedItem != null)
            {
                string key = (string)lst_MovingSprites.SelectedItem;

                SpriteValues sv = m_MSprites[key];
                sv.Frames = (int)num_MSpriteFrames.Value;
                m_MSprites[key] = sv;
            }
        }

        private void num_MSpriteRate_ValueChanged(object sender, EventArgs e)
        {
            if (lst_MovingSprites.SelectedItem != null)
            {
                string key = (string)lst_MovingSprites.SelectedItem;

                SpriteValues sv = m_MSprites[key];
                sv.Rate = (float)num_MSpriteRate.Value;
                m_MSprites[key] = sv;
            }
        }

        private void num_SSpriteMinSize_ValueChanged(object sender, EventArgs e)
        {
            m_SSminSize = (int)num_SSpriteMinSize.Value;
        }

        private void num_SSpriteSizeRange_ValueChanged(object sender, EventArgs e)
        {
            m_SSsizeRange = (int)num_SSpriteSizeRange.Value;
        }

        private void num_SSpriteMinTime_ValueChanged(object sender, EventArgs e)
        {
            m_MinTime = (float)num_SSpriteMinTime.Value;
        }

        private void num_SSpriteTimeRange_ValueChanged(object sender, EventArgs e)
        {
            m_TimeRange = (float)num_SSpriteTimeRange.Value;
        }

        private void lst_StaticSprites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_StaticSprites.SelectedItem != null)
            {
                string key = (string)lst_StaticSprites.SelectedItem;
                pic_StaticSprite.Image = Bitmap.FromFile(key);

                num_SSpriteRate.Value = (decimal)m_SSprites[key].Rate;
                num_SSpriteFrames.Value = m_SSprites[key].Frames;
            }
        }

        private void btn_SSpriteAdd_Click(object sender, EventArgs e)
        {
            DialogResult r = file_Open.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = file_Open.FileNames;
                lst_StaticSprites.Items.AddRange(files);

                foreach (string s in files)
                {
                    SpriteValues sv = new SpriteValues();
                    sv.Frames = (int)num_SSpriteFrames.Value;
                    sv.Rate = (float)num_SSpriteRate.Value;

                    m_SSprites.Add(s, sv);
                }
            }
        }

        private void btn_SSpriteRemove_Click(object sender, EventArgs e)
        {
            if (lst_StaticSprites.SelectedItem != null)
            {
                pic_StaticSprite.Image = null;

                string key = (string)lst_StaticSprites.SelectedItem;
                m_SSprites.Remove(key);

                lst_StaticSprites.Items.Remove(lst_StaticSprites.SelectedItem);
            }
        }

        private void num_SSpriteFrames_ValueChanged(object sender, EventArgs e)
        {
            if (lst_StaticSprites.SelectedItem != null)
            {
                string key = (string)lst_StaticSprites.SelectedItem;

                SpriteValues sv = m_SSprites[key];
                sv.Frames = (int)num_SSpriteFrames.Value;
                m_SSprites[key] = sv;
            }
        }

        private void num_SSpriteRate_ValueChanged(object sender, EventArgs e)
        {
            if (lst_StaticSprites.SelectedItem != null)
            {
                string key = (string)lst_StaticSprites.SelectedItem;

                SpriteValues sv = m_SSprites[key];
                sv.Rate = (float)num_SSpriteRate.Value;
                m_SSprites[key] = sv;
            }
        }

        private void btn_Preview_Click(object sender, EventArgs e)
        {
            m_Backgrounds = new string[lst_Backgrounds.Items.Count];

            for (int i = 0; i < m_Backgrounds.Length; ++i)
            {
                m_Backgrounds[i] = (string)lst_Backgrounds.Items[i];
            }
        }

        
    }
}
