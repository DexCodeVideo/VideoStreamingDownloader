using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static VideoStreamingDownloader.UserSettings;

namespace VideoStreamingDownloader
{
    internal partial class UserSettingsForm : Form
    {
        internal UserSettingsForm()
        {
            InitializeComponent();
            SetTexts();
            LoadData();
            SetData();
        }

        private void SetTexts()
        {
            //TempDirLabel.Text = string.Empty;
            SaveButton.Text = Resources.Translations.Strings.SaveClose;
            LanguageLabel.Text = Resources.Translations.Strings.Language;
            this.Text = Resources.Translations.Strings.Preferences;
        }

        private void LoadData()
        {
            var languages = new Dictionary<string, string>
            {
                { "en", Resources.Translations.Strings.eng },
                { "ca", Resources.Translations.Strings.cat }
            };

            LanguageComboBox.DataSource = new BindingSource(languages, null);
            LanguageComboBox.DisplayMember = "Value";
            LanguageComboBox.ValueMember = "Key";
        }

        private void SetData()
        {
            tbDefaultPath.Text = Program.UserSettings.DefaultDownloadPath;
            tbTempPath.Text = Program.UserSettings.TempPath;
            nudSimultaneousDownloads.Value = Program.UserSettings.SimultaneousDownloads;
            LanguageComboBox.SelectedValue = Program.UserSettings.Language;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var dto = new UserSettingsDto()
            {
                TempPath = tbTempPath.Text,
                DefaultDownloadPath = tbDefaultPath.Text,
                SimultaneousDownloads = (int)nudSimultaneousDownloads.Value,
                Language = LanguageComboBox.SelectedValue.ToString()
            };

            bool languageChanged = Program.UserSettings.Language != dto.Language;

            Program.UserSettings.Save(dto);
            if (languageChanged)
            {
                Program.SetLanguage(Program.UserSettings.Language);

            }

            Close();
        }

        private void btDefaultFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder(tbDefaultPath);
        }

        private void btTempFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder(tbTempPath);
        }

        private void ChooseFolder(TextBox tb)
        {
            folderBrowserDialog1.SelectedPath = tb.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                tb.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
