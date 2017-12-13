using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;

namespace NPS
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        void LoadSettings()
        {
			// Settings
			textDownload.Text = Settings.Instance.downloadDir;
			textPKGPath.Text = Settings.Instance.pkgPath;
			textParams.Text = Settings.Instance.pkgParams;
			checkBox1.Checked = Settings.Instance.deleteAfterUnpack;
			numericUpDown1.Value = Settings.Instance.simultaneousDl;

			// Game URIs
			tb_ps3uri.Text = Settings.Instance.PS3Uri;
			tb_psmuri.Text = Settings.Instance.PSMUri;
			tb_pspuri.Text = Settings.Instance.PSPUri;
			tb_psvuri.Text = Settings.Instance.GamesUri;
			tb_psxuri.Text = Settings.Instance.PSXUri;

			// DLC URIs
			tb_ps3dlcuri.Text = Settings.Instance.PS3DLCUri;
			tb_pspdlcuri.Text = Settings.Instance.PSPDLCUri;
			tb_psvdlcuri.Text = Settings.Instance.DLCUri;

			// Theme URIs
			tb_ps3thmuri.Text = Settings.Instance.PS3ThemeUri;
			tb_pspthmuri.Text = Settings.Instance.PSPThemeUri;
			tb_psvthmuri.Text = Settings.Instance.ThemeUri;

			// Update URIs
			tb_psvupduri.Text = Settings.Instance.UpdateUri;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textDownload.Text = fbd.SelectedPath;
                    Settings.Instance.downloadDir = textDownload.Text;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new OpenFileDialog())
            {
                if (Type.GetType("Mono.Runtime") != null)
                {
                    fbd.Filter = "|*";
                }
                else
                {
                    fbd.Filter = "|*.exe";
                }

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    textPKGPath.Text = fbd.FileName;
                    Settings.Instance.pkgPath = textPKGPath.Text;
                }
            }
        }

        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateSettings(true);
        }

        void UpdateSettings(bool withStoring)
        {
			// Settings
			Settings.Instance.downloadDir = textDownload.Text;
			Settings.Instance.pkgPath = textPKGPath.Text;
			Settings.Instance.pkgParams = textParams.Text;
			Settings.Instance.deleteAfterUnpack = checkBox1.Checked;
			Settings.Instance.simultaneousDl = (int)numericUpDown1.Value;

			// Game URIs
			Settings.Instance.PS3Uri = tb_ps3uri.Text;
			Settings.Instance.PSMUri = tb_psmuri.Text;
			Settings.Instance.PSPUri = tb_pspuri.Text;
			Settings.Instance.GamesUri = tb_psvuri.Text;
			Settings.Instance.PSXUri = tb_psxuri.Text;

			// DLC URIs
			Settings.Instance.PS3DLCUri = tb_ps3dlcuri.Text;
			Settings.Instance.PSPDLCUri = tb_pspdlcuri.Text;
			Settings.Instance.DLCUri = tb_psvdlcuri.Text;

			// Theme URIs
			Settings.Instance.PS3ThemeUri = tb_ps3thmuri.Text;
			Settings.Instance.PSPThemeUri = tb_pspthmuri.Text;
			Settings.Instance.ThemeUri = tb_psvthmuri.Text;

			// Update URIs
			Settings.Instance.UpdateUri = tb_psvupduri.Text;

			if (withStoring)
                Settings.Instance.Store();
        }

		private void btn_psvuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_psvuri);
		}

		private void btn_psmuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_psmuri);
		}

		private void btn_psxuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_psxuri);
		}

		private void btn_ps3uri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_ps3uri);
		}

		private void btn_pspuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_pspuri);
		}

		private void btn_psvdlcuri_Click(object sender, EventArgs e)
        {
            ShowOpenFileWindow(tb_psvdlcuri);
        }

		private void btn_ps3dlcuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_ps3dlcuri);
		}

		private void btn_pspdlcuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_pspdlcuri);
		}

		private void btn_psvthmuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_psvthmuri);
		}

		private void btn_ps3thmuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_ps3thmuri);
		}

		private void btn_pspthmuri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_pspthmuri);
		}

		private void btn_psvupduri_Click(object sender, EventArgs e)
		{
			ShowOpenFileWindow(tb_psvupduri);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.deleteAfterUnpack = checkBox1.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance.simultaneousDl = (int)numericUpDown1.Value;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"Here you can give parameters to pass to your pkg dec tool. Available variables are: 
- {zRifKey}
- {pkgFile}
- {gameTitle}
- {region}
- {titleID}");
        }

        void ShowOpenFileWindow(TextBox tb)
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "|*.tsv";

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    tb.Text = fbd.FileName;
                }
            }

        }

        private void button_export(object sender, EventArgs e)
        {
            try
            {
                using (var fsd = new SaveFileDialog())
                {
                    fsd.Filter = "|*.npsSettings";
                    DialogResult result = fsd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        UpdateSettings(false);
                        FileStream stream = File.Create(fsd.FileName);
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, Settings.Instance);
                        stream.Close();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        private void button_import(object sender, EventArgs e)
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "|*.npsSettings";

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (System.IO.File.Exists(fbd.FileName))
                    {
                        var stream = File.OpenRead(fbd.FileName);
                        var formatter = new BinaryFormatter();
                        Settings.Instance = (Settings)formatter.Deserialize(stream);
                        stream.Close();
                        LoadSettings();
                    }
                }
            }
        }
    }
}
