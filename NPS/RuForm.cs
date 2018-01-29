using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleJson;

namespace NPS
{
    public partial class NPSBrowser : Form
    {
        public const string version = "0.77_beta3";
        List<Item> currentDatabase = new List<Item>();

        List<Item> gamesDbs = new List<Item>();
		List<Item> avatarsDbs = new List<Item>();
		List<Item> dlcsDbs = new List<Item>();
		List<Item> themesDbs = new List<Item>();
		List<Item> updatesDbs = new List<Item>();

		HashSet<string> types = new HashSet<string>();
		HashSet<string> regions = new HashSet<string>();
        int currentOrderColumn = 0;
        bool currentOrderInverted = false;

        List<DownloadWorker> downloads = new List<DownloadWorker>();
        Release[] releases = null;

        public NPSBrowser()
        {
            InitializeComponent();
            this.Text += " " + version;
            this.Icon = Properties.Resources._8_512;
            new Settings();

            if (string.IsNullOrEmpty(Settings.Instance.PSVUri) && string.IsNullOrEmpty(Settings.Instance.PSVDLCUri))
            {
                MessageBox.Show("Application did not provide any links to external files or decrypt mechanism.\r\nYou need to specify tsv (tab splitted text) file with your personal links to pkg files on your own.\r\n\r\nFormat: TitleId Region Name Pkg Key", "Disclaimer!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Options o = new Options();
                o.ShowDialog();
            }
            else if (!File.Exists(Settings.Instance.pkgPath))
            {
                MessageBox.Show("You are missing your pkg decryptor exe", "Whops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Options o = new Options();
                o.ShowDialog();
            }

            NewVersionCheck();
        }

        private void NoPayStationBrowser_Load(object sender, EventArgs e)
        {
            foreach (var hi in History.I.currentlyDownloading)
            {
                DownloadWorker dw = hi;
                dw.Recreate(this);
                lstDownloadStatus.Items.Add(dw.lvi);
                lstDownloadStatus.AddEmbeddedControl(dw.progress, 3, lstDownloadStatus.Items.Count - 1);
                downloads.Add(dw);
            }

            ServicePointManager.DefaultConnectionLimit = 30;
            LoadAllDatabases(null, null);
        }

        private void LoadAllDatabases(object sender, EventArgs e)
        {
			avatarsDbs.Clear();
            dlcsDbs.Clear();
            gamesDbs.Clear();
			themesDbs.Clear();
			updatesDbs.Clear();

			// Update DBs
			LoadDatabase(Settings.Instance.PSVUpdateUri, (psvupd) =>
			{
			updatesDbs.AddRange(psvupd);

			LoadDatabase(Settings.Instance.PS4UpdateUri, (ps4upd) =>
			{
			updatesDbs.AddRange(ps4upd);

			// Theme DBs
			LoadDatabase(Settings.Instance.PSVThemeUri, (psvthm) =>
			{
			themesDbs.AddRange(psvthm);

			LoadDatabase(Settings.Instance.PSPThemeUri, (pspthm) =>
			{
			themesDbs.AddRange(pspthm);

			LoadDatabase(Settings.Instance.PS3ThemeUri, (ps3thm) =>
			{
			themesDbs.AddRange(ps3thm);

			LoadDatabase(Settings.Instance.PS4ThemeUri, (ps4thm) =>
			{
			themesDbs.AddRange(ps4thm);

			// DLC DBs
			LoadDatabase(Settings.Instance.PSVDLCUri, (db) =>
			{
			dlcsDbs.AddRange(db);

			LoadDatabase(Settings.Instance.PSPDLCUri, (pspdlc) =>
			{
			dlcsDbs.AddRange(pspdlc);

			LoadDatabase(Settings.Instance.PS3DLCUri, (ps3dlc) =>
			{
			dlcsDbs.AddRange(ps3dlc);

			LoadDatabase(Settings.Instance.PS4DLCUri, (ps4dlc) =>
			{
			dlcsDbs.AddRange(ps4dlc);

			// Avatar DBs
			LoadDatabase(Settings.Instance.PS3AvatarUri, (ps3avatar) =>
			{
			avatarsDbs.AddRange(ps3avatar);

			// Game DBs
			LoadDatabase(Settings.Instance.PSVUri, (vita) =>
			{
			gamesDbs.AddRange(vita);

			LoadDatabase(Settings.Instance.PSMUri, (psm) =>
			{
			gamesDbs.AddRange(psm);

			LoadDatabase(Settings.Instance.PSXUri, (psx) =>
			{
			gamesDbs.AddRange(psx);

			LoadDatabase(Settings.Instance.PSPUri, (psp) =>
			{
			gamesDbs.AddRange(psp);

			LoadDatabase(Settings.Instance.PS3Uri, (ps3) =>
			{
			gamesDbs.AddRange(ps3);

			LoadDatabase(Settings.Instance.PS4Uri, (ps4) =>
			{
			gamesDbs.AddRange(ps4);

			Invoke(new Action(() =>
			{
				if (gamesDbs.Count > 0)
					rbnGames.Enabled = true;
				else rbnGames.Enabled = false;

				if (avatarsDbs.Count > 0)
					rbnAvatars.Enabled = true;
				else rbnAvatars.Enabled = false;

				if (dlcsDbs.Count > 0)
					rbnDLC.Enabled = true;
				else rbnDLC.Enabled = false;

				if (themesDbs.Count > 0)
					rbnThemes.Enabled = true;
				else rbnThemes.Enabled = false;

				if (updatesDbs.Count > 0)
					rbnUpdates.Enabled = true;
				else rbnUpdates.Enabled = false;

				rbnGames.Checked = true;
				currentDatabase = gamesDbs;

				cmbType.Items.Clear();
				cmbRegion.Items.Clear();

				foreach (string s in types)
					cmbType.Items.Add(s);

				foreach (string s in regions)
					cmbRegion.Items.Add(s);

				foreach (var a in cmbRegion.CheckBoxItems)
					a.Checked = true;

				foreach (var a in cmbType.CheckBoxItems)
					a.Checked = true;

				// Populate DLC Parent Titles
				foreach (var item in dlcsDbs)
				{
					var result = gamesDbs.FirstOrDefault(i => i.TitleId.StartsWith(item.TitleId.Substring(0, 9)))?.TitleName;
					item.ParentGameTitle = result ?? string.Empty;
				}

				cmbRegion.CheckBoxCheckedChanged += txtSearch_TextChanged;
				cmbType.CheckBoxCheckedChanged += txtSearch_TextChanged;
				txtSearch_TextChanged(null, null);
			}));

			// Game DBs
			}, DatabaseType.PS4);
			}, DatabaseType.PS3);
			}, DatabaseType.PSP);
			}, DatabaseType.ItsPSX);
			}, DatabaseType.ItsPsm);
			}, DatabaseType.Vita);

			// Avatar DBs
			}, DatabaseType.PS3Avatar);

			// DLC DBs
			}, DatabaseType.PS4DLC);
			}, DatabaseType.PS3DLC);
			}, DatabaseType.PSPDLC);
			}, DatabaseType.VitaDLC);

			// Theme DBs
			}, DatabaseType.PS4Theme);
			}, DatabaseType.PS3Theme);
			}, DatabaseType.PSPTheme);
			}, DatabaseType.VitaTheme);

			// Update DBs
			}, DatabaseType.PS4Update);
			}, DatabaseType.VitaUpdate);
		}

        void SetCheckboxState(List<Item> list, int id)
        {
            if (list.Count == 0)
            {
                cmbType.CheckBoxItems[id].Enabled = false;
                cmbType.CheckBoxItems[id].Checked = false;
            }
            else
            {
                cmbType.CheckBoxItems[id].Enabled = true;
                cmbType.CheckBoxItems[id].Checked = true;
            }
        }
        private void CmbRegion_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            txtSearch_TextChanged(null, null);

        }

        private void NewVersionCheck()
        {
            if (version.Contains("beta")) return;

            Task.Run(() =>
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.Credentials = CredentialCache.DefaultCredentials;
                    wc.Headers.Add("user-agent", "MyPersonalApp :)");
                    string content = wc.DownloadString("https://api.github.com/repos/jhonhenry10/NPS_Browser/releases");
                    wc.Dispose();

                    //dynamic test = JsonConvert.DeserializeObject<dynamic>(content);
                    releases = SimpleJson.SimpleJson.DeserializeObject<Release[]>(content);

                    string newVer = releases[0].tag_name;
                    if (version != newVer)
                    {
                        Invoke(new Action(() =>
                        {
                            downloadUpdateToolStripMenuItem.Visible = true;
                            this.Text += string.Format("         (!! new version {0} available !!)", newVer);
                        }));
                    }
                }
                catch { }
            });
        }

        private void LoadDatabase(string path, Action<List<Item>> result, DatabaseType dbType)
        {
            List<Item> dbs = new List<Item>();
            if (string.IsNullOrEmpty(path))
                result.Invoke(dbs);
            else
            {
                Task.Run(() =>
                {
                    path = new Uri(path).ToString();

                    try
                    {
                        WebClient wc = new WebClient();
                        string content = wc.DownloadString(new Uri(path));
                        wc.Dispose();
                        content = Encoding.UTF8.GetString(Encoding.Default.GetBytes(content));

                        string[] lines = content.Split(new string[] { "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);

                        for (int i = 1; i < lines.Length; i++)
                        {
                            var a = lines[i].Split('\t');

                            if (a.Length < 2)
                            {
                                continue;
                            }

							var itm = new Item()
							{
								TitleId = a[0],
								Region = a[1],
								TitleName = a[2],
								pkg = a[3],
								zRif = a[4],
								ContentId = a[5],
							};

							// PSV
							if (dbType == DatabaseType.Vita)
							{
								itm.contentType = "VITA";

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.VitaDLC)
							{
								itm.contentType = "VITA";
								itm.IsDLC = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.VitaTheme)
							{
								itm.contentType = "VITA";
								itm.IsTheme = true;
							}
							else if (dbType == DatabaseType.VitaUpdate)
							{
								itm.contentType = "VITA";
								itm.IsUpdate = true;

								itm.ContentId = null;
								itm.zRif = "";
								itm.TitleName = a[2] + " (" + a[3] + ")";
								itm.pkg = a[5];
								DateTime.TryParse(a[7], out itm.lastModifyDate);
							}

							// PSP
							else if (dbType == DatabaseType.PSP)
							{
								itm.ItsPsp = true;
								itm.contentType = "PSP";

								itm.contentType = a[2];
								itm.TitleName = a[3];
								itm.pkg = a[4];
								itm.ContentId = a[5];
								DateTime.TryParse(a[6], out itm.lastModifyDate);
								itm.zRif = a[7];
							}
							else if (dbType == DatabaseType.PSPDLC)
							{
								itm.ItsPsp = true;
								itm.contentType = "PSP";
								itm.IsDLC = true;
								
								itm.ContentId = a[4];
								DateTime.TryParse(a[5], out itm.lastModifyDate);
								itm.zRif = a[6];
							}
							else if (dbType == DatabaseType.PSPTheme)
							{
								itm.ItsPsp = true;
								itm.contentType = "PSP";
								itm.IsTheme = true;

								itm.zRif = "";
								itm.ContentId = a[4];
								DateTime.TryParse(a[5], out itm.lastModifyDate);
							}

							// PS3
							else if (dbType == DatabaseType.PS3)
                            {
								itm.contentType = "PS3";
                                itm.ItsPS3 = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS3Avatar)
							{
								itm.ItsPS3 = true;
								itm.contentType = "PS3";
								itm.IsAvatar = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS3DLC)
							{
								itm.ItsPS3 = true;
								itm.contentType = "PS3";
								itm.IsDLC = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS3Theme)
							{
								itm.ItsPS3 = true;
								itm.contentType = "PS3";
								itm.IsTheme = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}

							// PS4
							else if (dbType == DatabaseType.PS4)
							{
								itm.ItsPS4 = true;
								itm.contentType = "PS4";

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS4DLC)
							{
								itm.ItsPS4 = true;
								itm.contentType = "PS4";
								itm.IsDLC = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS4Theme)
							{
								itm.ItsPS4 = true;
								itm.contentType = "PS4";
								itm.IsTheme = true;

								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}
							else if (dbType == DatabaseType.PS4Update)
							{
								itm.ItsPS4 = true;
								itm.contentType = "PS4";
								itm.IsUpdate = true;

								itm.ContentId = null;
								itm.zRif = "";
								itm.TitleName = a[2] + " (" + a[3] + ")";
								itm.pkg = a[5];
								DateTime.TryParse(a[6], out itm.lastModifyDate);
							}

							// Others
							else if (dbType == DatabaseType.ItsPsm)
                            {
								itm.contentType = "PSM";

								itm.ContentId = null;
								DateTime.TryParse(a[5], out itm.lastModifyDate);
							}
                            else if (dbType == DatabaseType.ItsPSX)
                            {
								itm.contentType = "PSX";
								itm.ItsPsx = true;

								itm.zRif = "";
                                itm.ContentId = a[4];
								DateTime.TryParse(a[5], out itm.lastModifyDate);
							}

							// If the pkg is really a link to a JSON
							if (itm.pkg.EndsWith(".json"))
							{
								try
								{
									WebClient p4client = new WebClient();
									p4client.Credentials = CredentialCache.DefaultCredentials;
									p4client.Headers.Add("user-agent", "MyPersonalApp :)");
									string json = p4client.DownloadString(itm.pkg);
									wc.Dispose();

									JsonObject fields = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(json);
									JsonArray pieces = fields["pieces"] as JsonArray;
									foreach (JsonObject piece in pieces)
									{
										Item inneritm = new Item()
										{
											TitleId = itm.TitleId,
											Region = itm.Region,
											TitleName = itm.TitleName + " (Offset " + piece["fileOffset"].ToString() + ")",
											pkg = piece["url"].ToString(),
											zRif = itm.zRif,
											ContentId = itm.ContentId,
											lastModifyDate = itm.lastModifyDate,

											ItsPsp = itm.ItsPsp,
											ItsPS3 = itm.ItsPS3,
											ItsPS4 = itm.ItsPS4,
											ItsPsx = itm.ItsPsx,
											contentType = itm.contentType,

											IsAvatar = itm.IsAvatar,
											IsDLC = itm.IsDLC,
											IsTheme = itm.IsTheme,
											IsUpdate = itm.IsUpdate,

											DlcItm = itm.DlcItm,
											ParentGameTitle = itm.ParentGameTitle,
										};

										AddToDbHelper(inneritm, dbType, dbs);
										continue;
									}
								}
								catch { }
							}

							AddToDbHelper(itm, dbType, dbs);
                        }
                    }
                    catch (Exception err) { }
                    result.Invoke(dbs);
                });
            }
        }

		private void AddToDbHelper(Item itm, DatabaseType dbType, List<Item> dbs)
		{
			if (itm.pkg.ToLower().Contains("http://") || itm.pkg.ToLower().Contains("https://")) // (!itm.zRif.ToLower().Contains("missing"))
			{
				if (itm.zRif.ToLower().Contains("not required")) itm.zRif = "";

				if (dbType == DatabaseType.Vita || dbType == DatabaseType.PSP || dbType == DatabaseType.PS3 || dbType == DatabaseType.PS4)
					itm.CalculateDlCs(dlcsDbs.ToArray());

				dbs.Add(itm);
				types.Add(itm.contentType);
				regions.Add(itm.Region.Replace(" ", ""));
			}
		}

        private void RefreshList(List<Item> items)
        {
            List<ListViewItem> list = new List<ListViewItem>();

            foreach (var item in items)
            {
                var a = new ListViewItem(item.TitleId);
                if (History.I.completedDownloading.Contains(item))
                    a.BackColor = ColorTranslator.FromHtml("#B7FF7C");

                a.SubItems.Add(item.Region);
                a.SubItems.Add(item.TitleName);
                a.SubItems.Add(item.contentType);
                if (item.DLCs > 0)
                    a.SubItems.Add(item.DLCs.ToString());
                else a.SubItems.Add("");
                if (item.lastModifyDate != DateTime.MinValue)
                    a.SubItems.Add(item.lastModifyDate.ToString());
                else a.SubItems.Add("");
                a.Tag = item;
                list.Add(a);
            }

            lstTitles.BeginUpdate();
            if (rbnDLC.Checked) lstTitles.Columns[4].Width = 0;
            else lstTitles.Columns[4].Width = 60;
            lstTitles.Items.Clear();
            lstTitles.Items.AddRange(list.ToArray());

            lstTitles.ListViewItemSorter = new ListViewItemComparer(2, false);
            lstTitles.Sort();

            lstTitles.EndUpdate();

            string type = "";
			if (rbnGames.Checked) type = "Games";
			else if (rbnAvatars.Checked) type = "Avatars";
			else if (rbnDLC.Checked) type = "DLCs";
			else if (rbnThemes.Checked) type = "Themes";
			else if (rbnUpdates.Checked) type = "Updates";
			//else if (rbnPSM.Checked) type = "PSM Games";
			//else if (rbnPSX.Checked) type = "PSX Games";

			lblCount.Text = $"{list.Count}/{currentDatabase.Count} {type}";
        }

        // Form
        private void NPSBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Instance.Store();
            History.I.currentlyDownloading.Clear();

            foreach (var lstItm in lstDownloadStatus.Items)
            {
                DownloadWorker dw = ((lstItm as ListViewItem).Tag as DownloadWorker);

                History.I.currentlyDownloading.Add(dw);
            }

            History.I.Save();
        }

        // Menu
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options o = new Options();
            o.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void downloadUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = releases?[0]?.assets?[0]?.browser_download_url;
            if (!string.IsNullOrEmpty(url))
                System.Diagnostics.Process.Start(url);
        }

        // Search
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            List<Item> itms = new List<Item>();

            foreach (var item in currentDatabase)
            {
                if (item.CompareName(txtSearch.Text) && ContainsCmbBox(cmbRegion, item.Region) && ContainsCmbBox(cmbType, item.contentType)) /*(cmbRegion.Text == "ALL" || item.Region.Contains(cmbRegion.Text)))*/
                    itms.Add(item);
            }

            RefreshList(itms);
        }

        bool ContainsCmbBox(PresentationControls.CheckBoxComboBox chkbcmb, string item)
        {
            foreach (var itm in chkbcmb.CheckBoxItems)
            {
                if (itm.Checked && item.Contains(itm.Text))
                    return true;
            }
            return false;
        }

        // Browse
        private void rbnGames_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnGames.Checked)
            {
                currentDatabase = gamesDbs;
                txtSearch_TextChanged(null, null);
            }
        }

		private void rbnAvatars_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnAvatars.Checked)
			{
				currentDatabase = avatarsDbs;
				txtSearch_TextChanged(null, null);
			}
		}

		private void rbnDLC_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnDLC.Checked)
            {
                currentDatabase = dlcsDbs;
                txtSearch_TextChanged(null, null);
            }
        }

		private void rbnThemes_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnThemes.Checked)
			{
				currentDatabase = themesDbs;
				txtSearch_TextChanged(null, null);
			}
		}

		private void rbnUpdates_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnUpdates.Checked)
			{
				currentDatabase = updatesDbs;
				txtSearch_TextChanged(null, null);
			}
		}

		//private void rbnPSM_CheckedChanged(object sender, EventArgs e)
		//{
		//    if (rbnPSM.Checked)
		//    {
		//        currentDatabase = psmDbs;
		//        txtSearch_TextChanged(null, null);
		//    }
		//}

		//private void rbnPSX_CheckedChanged(object sender, EventArgs e)
		//{
		//    if (rbnPSX.Checked)
		//    {
		//        currentDatabase = psxDbs;
		//        txtSearch_TextChanged(null, null);
		//    }
		//}

		// Download
		private void btnDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Instance.downloadDir) || string.IsNullOrEmpty(Settings.Instance.pkgPath))
            {
                MessageBox.Show("You don't have a proper configuration.", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Options o = new Options();
                o.ShowDialog();
                return;
            }

            if (!File.Exists(Settings.Instance.pkgPath))
            {
                MessageBox.Show("You are missing your pkg dec.", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Options o = new Options();
                o.ShowDialog();
                return;
            }

            if (lstTitles.SelectedItems.Count == 0) return;

            foreach (ListViewItem itm in lstTitles.SelectedItems)
            {
                var a = (itm.Tag as Item);

                bool contains = false;
                foreach (var d in downloads)
                    if (d.currentDownload == a)
                    {
                        contains = true;
                        break; //already downloading
                    }

                if (!contains)
                {
                    DownloadWorker dw = new DownloadWorker(a, this);
                    lstDownloadStatus.Items.Add(dw.lvi);
                    lstDownloadStatus.AddEmbeddedControl(dw.progress, 3, lstDownloadStatus.Items.Count - 1);
                    downloads.Add(dw);
                }
            }
        }

        private void lnkOpenRenaScene_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //var u = new Uri("https://www.youtube.com/results?search_query=dead or alive");
            System.Diagnostics.Process.Start(lnkOpenRenaScene.Tag.ToString());
        }

        // lstTitles
        private void lstTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTitles.SelectedItems.Count > 0)
            {
                var itm = (lstTitles.SelectedItems[0].Tag as Item);
                if (itm.ItsPS3 || itm.ItsPS4)
                {
                    if (string.IsNullOrEmpty(itm.zRif))
                    {
                        lb_ps3licenseType.BackColor = Color.LawnGreen;
                        lb_ps3licenseType.Text = "RAP NOT REQUIRED, use ReActPSN/PSNPatch";
                    }
                    else if (itm.zRif.ToLower().Contains("UNLOCK/LICENSE BY DLC".ToLower())) lb_ps3licenseType.Text = "UNLOCK BY DLC";
                    else lb_ps3licenseType.Text = "";
                }
                else
                {
                    lb_ps3licenseType.Text = "";
                }
            }
        }

        private void lstTitles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (currentOrderColumn == e.Column)
                currentOrderInverted = !currentOrderInverted;
            else
            {
                currentOrderColumn = e.Column; currentOrderInverted = false;
            }

            this.lstTitles.ListViewItemSorter = new ListViewItemComparer(currentOrderColumn, currentOrderInverted);
            // Call the sort method to manually sort.
            lstTitles.Sort();
        }

        private void lstTitles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                //listView1.MultiSelect = true;
                foreach (ListViewItem item in lstTitles.Items)
                {
                    item.Selected = true;
                }
            }
        }

        // lstTitles Menu Strip
        private void showTitleDlcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstTitles.SelectedItems.Count == 0) return;

            Item t = (lstTitles.SelectedItems[0].Tag as Item);
            if (t.DLCs > 0)
            {
                rbnDLC.Checked = true;
                txtSearch.Text = t.TitleId;
            }
        }

        private void downloadAllDlcsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in lstTitles.SelectedItems)
            {

                var parrent = (itm.Tag as Item);

                foreach (var a in parrent.DlcItm)
                {
                    bool contains = false;
                    foreach (var d in downloads)
                        if (d.currentDownload == a)
                        {
                            contains = true;
                            break; //already downloading
                        }

                    if (!contains)
                    {
                        DownloadWorker dw = new DownloadWorker(a, this);
                        lstDownloadStatus.Items.Add(dw.lvi);
                        lstDownloadStatus.AddEmbeddedControl(dw.progress, 3, lstDownloadStatus.Items.Count - 1);
                        downloads.Add(dw);
                    }
                }
            }
        }

        // lstDownloadStatus
        private void lstDownloadStatus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                //listView1.MultiSelect = true;
                foreach (ListViewItem item in lstDownloadStatus.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstDownloadStatus.SelectedItems.Count == 0) return;
            foreach (ListViewItem a in lstDownloadStatus.SelectedItems)
            {
                DownloadWorker itm = (a.Tag as DownloadWorker);
                itm.Pause();
            }
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstDownloadStatus.SelectedItems.Count == 0) return;
            foreach (ListViewItem a in lstDownloadStatus.SelectedItems)
            {
                DownloadWorker itm = (a.Tag as DownloadWorker);
                itm.Resume();
            }
        }

        // lstDownloadStatus Menu Strip
        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstDownloadStatus.SelectedItems.Count == 0) return;
            foreach (ListViewItem a in lstDownloadStatus.SelectedItems)
            {
                DownloadWorker itm = (a.Tag as DownloadWorker);
                itm.Cancel();
                //itm.DeletePkg();
            }
        }

        private void retryUnpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstDownloadStatus.SelectedItems.Count == 0) return;
            foreach (ListViewItem a in lstDownloadStatus.SelectedItems)
            {
                DownloadWorker itm = (a.Tag as DownloadWorker);
                itm.Unpack();
            }
        }

        private void clearCompletedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DownloadWorker> toDel = new List<DownloadWorker>();
            List<ListViewItem> toDelLVI = new List<ListViewItem>();

            foreach (var i in downloads)
            {
                if (i.status == WorkerStatus.Canceled || i.status == WorkerStatus.Completed)
                    toDel.Add(i);
            }

            foreach (ListViewItem i in lstDownloadStatus.Items)
            {
                if (toDel.Contains(i.Tag as DownloadWorker))
                    toDelLVI.Add(i);
            }

            foreach (var i in toDel)
                downloads.Remove(i);
            toDel.Clear();

            foreach (var i in toDelLVI)
                lstDownloadStatus.Items.Remove(i);
            toDelLVI.Clear();
        }

        // Timers
        private void timer1_Tick(object sender, EventArgs e)
        {
            int workingThreads = 0;
            foreach (var dw in downloads)
            {
                if (dw.status == WorkerStatus.Running)
                    workingThreads++;
            }

            if (workingThreads < Settings.Instance.simultaneousDl)
            {
                foreach (var dw in downloads)
                {
                    if (dw.status == WorkerStatus.Queued)
                    {
                        dw.Start();
                        break;
                    }
                }
            }
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Item previousSelectedItem = null;

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Update view

            if (lstTitles.SelectedItems.Count == 0) return;
            Item itm = (lstTitles.SelectedItems[0].Tag as Item);

            if (itm != previousSelectedItem)
            {
                previousSelectedItem = itm;

                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();

                Task.Run(() =>
                {
                    Helpers.Renascene r = new Helpers.Renascene(itm);

                    if (r.imgUrl != null)
                    {
                        Invoke(new Action(() =>
                        {
                            ptbCover.LoadAsync(r.imgUrl);
                            label5.Text = r.ToString();
                            lnkOpenRenaScene.Tag = r.url;
                            lnkOpenRenaScene.Visible = true;
                        }));

                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            ptbCover.Image = null;
                            label5.Text = "";
                            lnkOpenRenaScene.Visible = false;
                        }));

                    }
                }, tokenSource.Token);
            }
        }

        private void PauseAllBtnClick(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in lstDownloadStatus.Items)
            {
                (itm.Tag as DownloadWorker).Pause();
            }
        }

        private void ResumeAllBtnClick(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in lstDownloadStatus.Items)
            {
                (itm.Tag as DownloadWorker).Resume();
            }
        }


        private void lstTitles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var a = (sender as ListView);
                if (a.SelectedItems.Count > 0)
                {
                    var itm = (a.SelectedItems[0].Tag as Item);
                    if (itm.DLCs == 0)
                    {
                        showTitleDlcToolStripMenuItem.Enabled = false;
                        downloadAllDlcsToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        showTitleDlcToolStripMenuItem.Enabled = true;
                        downloadAllDlcsToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        private void ShowDescriptionPanel(object sender, EventArgs e)
        {
            Desc d = new Desc(lstTitles);
            d.Show();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
	}

	class Release
    {
        public string tag_name = "";
        public Asset[] assets = null;
    }

    class Asset
    {
        public string browser_download_url = "";
    }

    enum DatabaseType
	{
		// PSV
		Vita,
		VitaDLC,
		VitaTheme,
		VitaUpdate,

		// PSP
		PSP,
		PSPDLC,
		PSPTheme,

		// PS3
		PS3,
		PS3Avatar,
		PS3DLC,
		PS3Theme,

		// PS4
		PS4,
		PS4DLC,
		PS4Theme,
		PS4Update,

		// Others
		ItsPsm,
		ItsPSX,
	}
}
