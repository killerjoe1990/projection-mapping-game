using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

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

                if (m_Plats[key].LeftImage != null)
                {
                    lst_PlatformLeft.Items.Add(m_Plats[key].LeftImage);
                }

                lst_PlatformCenter.Items.AddRange(m_Plats[key].CenterImages.ToArray());

                if (m_Plats[key].RightImage != null)
                {
                    lst_PlatformRight.Items.Add(m_Plats[key].RightImage);
                }
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

            if (chck_PlatformBlink.Checked)
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
            string check = CheckDependencies();

            if (check != "")
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                MessageBox.Show(check, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                m_Backgrounds = new string[lst_Backgrounds.Items.Count];

                for (int i = 0; i < m_Backgrounds.Length; ++i)
                {
                    m_Backgrounds[i] = (string)lst_Backgrounds.Items[i];
                }
            }
        }

        private string CheckDependencies()
        {
            string errors = "";

            if (lst_Backgrounds.Items.Count < 1)
            {
                errors += "At least 1 background frame required. \n";
            }

            bool hasChecked = false;

            foreach (KeyValuePair<string, PlatformValues> pv in m_Plats)
            {
                if (pv.Value.Blinking)
                {
                    hasChecked = true;
                }
            }

            if (!hasChecked || m_Plats.Count < 2)
            {
                errors += "Only 1 blinking platform and at least 1 normal platform required. \n";
            }

            return errors;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string check = CheckDependencies();

            if (check != "")
            {
                MessageBox.Show(check, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                SaveForm form = new SaveForm();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string error = SaveTheme(form.Name);

                    if (error != "")
                    {
                        MessageBox.Show(error, "SAVE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
        }

        private string SaveTheme(string name)
        {
            string error = "";

            try
            {

                pic_Background.Image = null;
                pic_Background.Update();

                XElement root = new XElement("Theme",
                    new XAttribute("Name", name),
                    new XElement("Background",
                        new XAttribute("Rate", m_BackgroundRate)));

                string tempName = name;
                bool overrideTheme = false;

                if (System.IO.Directory.Exists("Themes\\" + name))
                {
                    if (MessageBox.Show("Theme name already exists. Override Theme?", "Save", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        overrideTheme = true;
                        name = "XX" + name + "XX";
                    }
                    else
                    {
                        return "Please Select a Different Theme Name.";
                    }
                }

                System.IO.Directory.CreateDirectory("Themes\\" + name + "\\Background");

                for (int i = 0; i < lst_Backgrounds.Items.Count; ++i)
                {
                    string file = (string)lst_Backgrounds.Items[i];
                    string extension = Regex.Match(file, @"\.\w{3}").ToString();
                    System.IO.File.Copy(file, "Themes\\" + name + "\\Background\\" + tempName + "Background" + i + extension, true);
                }

                System.IO.Directory.CreateDirectory("Themes\\" + name + "\\Platforms");

                int platNumber = 1;

                for (int i = 0; i < m_Plats.Count; ++i)
                {
                    KeyValuePair<string, PlatformValues> element = m_Plats.ElementAt(i);

                    if (element.Value.Blinking)
                    {
                        SavePlatforms(element, "Themes\\" + name, tempName, 0);
                    }
                    else
                    {
                        SavePlatforms(element, "Themes\\" + name, tempName, platNumber);
                        platNumber++;
                    }
                }

                XElement msprite = new XElement("MovingSprites",
                    new XAttribute("Number", m_NumObjects),
                    new XAttribute("MinSize", m_MSminSize),
                    new XAttribute("MaxSize", m_MSminSize + m_MSsizeRange),
                    new XAttribute("MinSpeed", m_MinSpeed),
                    new XAttribute("MaxSpeed", m_MinSpeed + m_SpeedRange));

                System.IO.Directory.CreateDirectory("Themes\\" + name + "\\MovingSprites");

                for (int i = 0; i < m_MSprites.Count; ++i)
                {
                    KeyValuePair<string, SpriteValues> element = m_MSprites.ElementAt(i);
                    string file = element.Key;
                    string extension = Regex.Match(file, @"\.\w{3}").ToString();
                    System.IO.File.Copy(file, "Themes\\" + name + "\\MovingSprites\\" + tempName + "Sprite" + i + extension, true);

                    msprite.Add(new XElement("Sprite",
                        new XAttribute("Frames", element.Value.Frames),
                        new XAttribute("Rate", element.Value.Rate)));
                }

                root.Add(msprite);

                System.IO.Directory.CreateDirectory("Themes\\" + name + "\\StaticSprites");

                XElement ssprite = new XElement("StaticSprites",
                    new XAttribute("MinSize", m_SSminSize),
                    new XAttribute("MaxSize", m_SSminSize + m_SSsizeRange),
                    new XAttribute("MinTime", m_MinTime),
                    new XAttribute("MaxTime", m_MinTime + m_TimeRange));

                for (int i = 0; i < m_SSprites.Count; ++i)
                {
                    KeyValuePair<string, SpriteValues> element = m_SSprites.ElementAt(i);
                    string file = element.Key;
                    string extension = Regex.Match(file, @"\.\w{3}").ToString();
                    System.IO.File.Copy(file, "Themes\\" + name + "\\StaticSprites\\" + tempName + "Sprite" + i + extension, true);

                    ssprite.Add(new XElement("Sprite",
                        new XAttribute("Frames", element.Value.Frames),
                        new XAttribute("Rate", element.Value.Rate)));
                }

                root.Add(ssprite);

                root.Save("Themes\\" + name + "\\ThemeConfig.xml");

                if (overrideTheme)
                {
                    System.IO.Directory.Delete("Themes\\" + tempName, true);
                    System.IO.Directory.Move("Themes\\" + name, "Themes\\" + tempName);

                    LoadTheme("Themes\\" + tempName);
                }
            }
            catch (Exception e)
            {
                error += "File System Error\n";
                error += "Please Try Again";
            }

            return error;
        }

        private void SavePlatforms(KeyValuePair<string, PlatformValues> element, string root, string name, int index)
        {
            System.IO.Directory.CreateDirectory(root + "\\Platforms\\" + index);

            int platIndex = 0;

            if (element.Value.LeftImage != null)
            {
                string file = element.Value.LeftImage;
                string extension = Regex.Match(file, @"\.\w{3}").ToString();
                System.IO.File.Copy(file, root + "\\Platforms\\" + index + "\\" + name + "Platform" + platIndex + extension, true);
                platIndex++;
            }
            foreach (string s in element.Value.CenterImages)
            {
                string file = s;
                string extension = Regex.Match(file, @"\.\w{3}").ToString();
                System.IO.File.Copy(file, root + "\\Platforms\\" + index + "\\" + name + "Platform" + platIndex + extension, true);
                platIndex++;
            }
            if (element.Value.RightImage != null)
            {
                string file = element.Value.RightImage;
                string extension = Regex.Match(file, @"\.\w{3}").ToString();
                System.IO.File.Copy(file, root + "\\Platforms\\" + index + "\\" + name + "Platform" + platIndex + extension, true);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
            }
            base.OnClosed(e);
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            if (fldr_Directories.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(fldr_Directories.SelectedPath + "\\ThemeConfig.xml"))
                {
                    LoadTheme(fldr_Directories.SelectedPath);
                }
                else
                {
                    MessageBox.Show("No Theme Detected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTheme(string path)
        {
            XmlDocument config = new XmlDocument();
            config.Load(path + "\\ThemeConfig.xml");

            string[] files = System.IO.Directory.GetFiles(path + "\\Background");

            lst_Backgrounds.Items.Clear();
            lst_Backgrounds.Items.AddRange(files);

            num_BackgroundRate.Value = decimal.Parse(config.SelectSingleNode("Theme/Background").Attributes["Rate"].Value);

            files = System.IO.Directory.GetFiles(path + "\\MovingSprites");

            lst_MovingSprites.Items.Clear();
            lst_MovingSprites.Items.AddRange(files);
            XmlNode nodes = config.SelectSingleNode("Theme/MovingSprites");
            num_MSpriteMinSize.Value = decimal.Parse(nodes.Attributes["MinSize"].Value);
            num_MSpriteSizeRange.Value = decimal.Parse(nodes.Attributes["MaxSize"].Value) - num_MSpriteMinSize.Value;
            num_MSpriteNumber.Value = decimal.Parse(nodes.Attributes["Number"].Value);
            num_MSpriteMinSpeed.Value = decimal.Parse(nodes.Attributes["MinSpeed"].Value);
            num_MSpriteSpeedRange.Value = decimal.Parse(nodes.Attributes["MaxSpeed"].Value) - num_MSpriteMinSpeed.Value;
            m_MSprites.Clear();
            for (int i = 0; i < files.Length; ++i)
            {
                SpriteValues sv = new SpriteValues();

                sv.Frames = Int32.Parse(nodes.ChildNodes[i].Attributes["Frames"].Value);
                sv.Rate = float.Parse(nodes.ChildNodes[i].Attributes["Rate"].Value);

                m_MSprites.Add(files[i], sv);
            }

            files = System.IO.Directory.GetFiles(path + "\\StaticSprites");

            lst_StaticSprites.Items.Clear();
            lst_StaticSprites.Items.AddRange(files);
            nodes = config.SelectSingleNode("Theme/StaticSprites");
            num_SSpriteMinSize.Value = decimal.Parse(nodes.Attributes["MinSize"].Value);
            num_SSpriteSizeRange.Value = decimal.Parse(nodes.Attributes["MaxSize"].Value) - num_SSpriteMinSize.Value;
            num_SSpriteMinTime.Value = decimal.Parse(nodes.Attributes["MinTime"].Value);
            num_SSpriteTimeRange.Value = decimal.Parse(nodes.Attributes["MaxTime"].Value) - num_SSpriteMinTime.Value;
            m_SSprites.Clear();
            for (int i = 0; i < files.Length; ++i)
            {
                SpriteValues sv = new SpriteValues();

                sv.Frames = Int32.Parse(nodes.ChildNodes[i].Attributes["Frames"].Value);
                sv.Rate = float.Parse(nodes.ChildNodes[i].Attributes["Rate"].Value);

                m_SSprites.Add(files[i], sv);
            }

            string[] directories = System.IO.Directory.GetDirectories(path + "\\Platforms");

            files = System.IO.Directory.GetFiles(directories[0]);
            m_Plats.Clear();
            PlatformValues pv = new PlatformValues();
            pv.Blinking = true;
            pv.LeftImage = files[0];
            pv.CenterImages = new List<string>();
            if (files.Length > 1)
            {
                for (int i = 1; i < files.Length - 1; ++i)
                {
                    pv.CenterImages.Add(files[i]);
                }
                pv.RightImage = files[files.Length - 1];
            }
            m_Plats.Add("Platform0", pv);
            lst_Platforms.Items.Add("Platform0");

            for (int i = 1; i < directories.Length; ++i)
            {
                files = System.IO.Directory.GetFiles(directories[1]);

                pv = new PlatformValues();
                pv.Blinking = false;
                pv.LeftImage = files[0];
                pv.CenterImages = new List<string>();

                if (files.Length > 1)
                {
                    for (int j = 1; j < files.Length - 1; ++j)
                    {
                        pv.CenterImages.Add(files[j]);
                    }
                    pv.RightImage = files[files.Length - 1];
                }
                m_Plats.Add("Platform" + i, pv);
                lst_Platforms.Items.Add("Platform" + i);
            }

            m_PlatNumber = directories.Length;
        }
    }

}
