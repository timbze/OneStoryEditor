﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Chorus.UI.Clone;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class AiRepoSelectionForm : TopForm
    {
        public new NewProjectWizard Parent;

        public AiRepoSelectionForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            foreach (string strServerLabel in Program.MapServerToUrlHost.Keys)
                comboBoxServer.Items.Add(strServerLabel);
        }

        public string InternetAddress
        {
            get
            {
                return (checkBoxInternet.Checked  && (comboBoxServer.SelectedItem != null))
                    ? comboBoxServer.SelectedItem.ToString() 
                    : null;
            }
            set
            {
                comboBoxServer.SelectedItem = value;
                checkBoxInternet.Checked = (comboBoxServer.SelectedItem != null);
            }
        }

        public string ProjectName
        {
            get { return textBoxProjectName.Text; }
            set { textBoxProjectName.Text = value; }
        }

        public string ProjectFolder { get; set; }
        public string SourceLanguageName { get; set; }
        public string TargetLanguageName { get; set; }
        private bool? _doingPush;
        public bool? DoingPush
        {
            get { return _doingPush; }
            set
            {
                _doingPush = value;
                UpdateLabels();
            }
        }

        public string NetworkAddress
        {
            get
            {
                return checkBoxNetwork.Checked ? textBoxNetwork.Text : null;
            }
            set
            {
                textBoxNetwork.Text = value;
                checkBoxNetwork.Checked = !String.IsNullOrEmpty(value);
            }
        }

        public const string SharedFolderSuffix = "AdaptItSharedProjects";

        public static string GetFullNetworkAddress(string strNetworkAddress, string strProjectName)
        {
            if (String.IsNullOrEmpty(strNetworkAddress) || String.IsNullOrEmpty(strProjectName))
                return null;

            return Path.Combine(strNetworkAddress,
                                Path.Combine(SharedFolderSuffix,
                                             strProjectName));
        }

        public static string GetFullInternetAddress(string strServer, string strProjectName)
        {
            string strInternetAddress;
            if (String.IsNullOrEmpty(strProjectName) 
                || String.IsNullOrEmpty(strServer)
                || String.IsNullOrEmpty(strInternetAddress = Program.LookupRepoUrlHost(strServer)))
                return null;

            return String.Format("{0}/{1}", strInternetAddress, strProjectName);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxNetwork_TextChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void buttonBrowseNetwork_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog
                          {
                              Description = Localizer.Str("Browse for the Network folder where the shared Adapt It repository is located")
                          };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                NetworkAddress = dlg.SelectedPath;
                checkBoxNetwork.Checked = true;
            }
        }

        private void buttonPushToInternet_Click(object sender, EventArgs e)
        {
            if (DoingPush == true)
                DoPush();
            else if (DoingPush == false)
                DoPull();
        }

        private void DoPush()
        {
            string strAiWorkFolder, strProjectFolderName, strHgUsername, strHgPassword;
            if (!GetAiRepoSettings(out strAiWorkFolder,
                out strProjectFolderName, out strHgUsername, out strHgPassword))
                return;

            ProjectFolder = Path.Combine(strAiWorkFolder, strProjectFolderName);
            var dlg = new HgRepoForm
            {
                ProjectName = ProjectName,
                UrlBase = Program.LookupRepoUrlHost(Properties.Resources.IDS_DefaultRepoServer),
                Username = strHgUsername,
                Password = strHgPassword
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Program.SetAdaptItHgParameters(ProjectFolder, ProjectName, InternetAddress, strHgUsername, strHgPassword);
                Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
            }
        }

        private void buttonPullFromInternet_Click(object sender, EventArgs e)
        {
            DoPull();
        }

        private void DoPull()
        {
            string strAiWorkFolder;
            string strProjectFolderName;
            string strHgUsername;
            string strHgPassword;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName, 
                out strHgUsername, out strHgPassword))
                return;

            if (!Directory.Exists(strAiWorkFolder))
                Directory.CreateDirectory(strAiWorkFolder);

            var model = new GetCloneFromInternetModel(strAiWorkFolder)
            {
                AccountName = strHgUsername,
                Password = strHgPassword,
                ProjectId = ProjectName,
                SelectedServerLabel = comboBoxServer.SelectedItem.ToString(),
                LocalFolderName = strProjectFolderName
            };

            using (var dlg = new GetCloneFromInternetDialog(model))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    ProjectFolder = dlg.PathToNewlyClonedFolder;
                    ProjectName = model.ProjectId;

                    // here (with pull) is one of the few places we actually query the user
                    //  for a username/password. Currently, the code assumes that they will
                    //  be the same as the project account, so make sure that's the case
                    if ((Parent != null) && (Parent.LoggedInMember != null))
                    {
                        /* I think this is not a problem
                        if ((!String.IsNullOrEmpty(Parent.LoggedInMember.HgUsername)
                            && (Parent.LoggedInMember.HgUsername != model.AccountName))
                            || (!String.IsNullOrEmpty(Parent.LoggedInMember.HgPassword)
                            && (Parent.LoggedInMember.HgPassword != model.Password)))
                        {
                            // means the user entered a different account/password than what's
                            //  being used by the project file
                            throw new ApplicationException(
                                "It isn't currently supported for your username and password to be different from the username/password for the project account. Contact bob_eaton@sall.com to correct this");
                        }
                        */
                        // in the case that the project isn't being used on the internet, but
                        //  the AdaptIt project is, then set the username/password for it.
                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgUsername))
                            Parent.LoggedInMember.HgUsername = model.AccountName;

                        if (String.IsNullOrEmpty(Parent.LoggedInMember.HgPassword))
                            Parent.LoggedInMember.HgPassword = model.Password;
                    }

                    Program.SetAdaptItHgParameters(ProjectFolder, ProjectName,
                        dlg.ThreadSafeUrl);
                    // don't need to do this if we just did a pull, no?
                    //  Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
                }
            }
        }

        private bool GetAiRepoSettings(out string strAiWorkFolder, 
            out string strProjectFolderName, out string strHgUsername, out string strHgPassword)
        {
            strHgPassword = null;
            strHgUsername = null;
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return false;

            if (Parent != null)
            {
                // but override by the current configuration
                if (!String.IsNullOrEmpty(Parent.HgUsername))
                    strHgUsername = Parent.HgUsername;

                if (!String.IsNullOrEmpty(Parent.HgPassword))
                    strHgPassword = Parent.HgPassword;
            }

            return true;
        }

        private bool GetAiRepoSettings(out string strAiWorkFolder, out string strProjectFolderName)
        {
            // e.g. <My Documents>\Adapt It Unicode Work
            strAiWorkFolder = AdaptItKBReader.AdaptItWorkFolder;

            // e.g. "Kangri to Hindi adaptations"
            strProjectFolderName = String.Format(Properties.Resources.IDS_AdaptItProjectFolderFormat,
                                                 SourceLanguageName, TargetLanguageName);

            return true;
        }

        /*
        private static bool ExtractActualLanguageNames(string strProjectName, 
            out string strSourceLanguage, out string strTargetLanguage)
        {
            strSourceLanguage = null;
            strTargetLanguage = null;

            // strProjectName is e.g. aikb-{0}-{1}. We need to return {0} and {1}
            const string strTo = "_To_";
            int nIndex = strProjectName.IndexOf(strTo);
            if (nIndex == -1)
                return false;

            int nPrefixLen = "AdaptItKb_".Length;
            strSourceLanguage = strProjectName.Substring(nPrefixLen, nIndex - nPrefixLen);
            strTargetLanguage = strProjectName.Substring(nIndex + strTo.Length);
            return true;
        }
        */

        private void checkBoxInternet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void checkBoxNetwork_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void buttonPushToNetwork_Click(object sender, EventArgs e)
        {
            string strAiWorkFolder;         // e.g. C:\Users\Bob\Documents\Adapt It Unicode Work
            string strProjectFolderName;    // e.g. Kangri to Hindi adaptations
            if (!GetAiRepoSettings(out strAiWorkFolder, out strProjectFolderName))
                return;

            ProjectFolder = Path.Combine(strAiWorkFolder, strProjectFolderName);
            Program.SetAdaptItHgParametersNetworkDrive(ProjectFolder, ProjectName, NetworkAddress);
            Program.SyncWithAiRepository(ProjectFolder, ProjectName, true);
        }

        private Regex FilterProjectName = new Regex("[A-Z]");

        private void textBoxProjectName_TextChanged(object sender, EventArgs e)
        {
            Match match = FilterProjectName.Match(textBoxProjectName.Text);
            if (match.Success)
            {
                textBoxProjectName.SelectionStart = match.Index;
                textBoxProjectName.SelectionLength = match.Length;

                toolTip.Show(
                    Localizer.Str("Enter the project name of the repository on the internet (e.g. aikb-hindi-english). Length between 2 and 20 characters. Only lower case letters (a-z), numbers and dashes are allowed AND the repository must have already been created by the repository administrator (bob_eaton@sall.com)"),
                    textBoxProjectName);
            } 
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            buttonPushToNetwork.Enabled = checkBoxNetwork.Checked;
            buttonPushToInternet.Enabled = checkBoxInternet.Checked;

            // if the '...' button was clicked, then show both arrow buttons
            if (DoingPush == null)
            {
                buttonPullFromInternet.Visible = true;
            }
            else
            {
                buttonPullFromInternet.Visible = false;
                buttonPushToInternet.Image = Properties.Resources.SyncArrowVertical_16x;
                buttonPullFromInternet.Refresh();
                toolTip.SetToolTip(buttonPullFromInternet,
                                   ((bool)DoingPush) 
                                        ? Localizer.Str("Click to upload the shared project to the internet")
                                        : Localizer.Str("Click to download the shared project from the internet"));
            }

            labelNetworkPath.Text = GetFullNetworkAddress(NetworkAddress, ProjectName);
            labelFullInternetUrl.Text = GetFullInternetAddress(InternetAddress, ProjectName);
        }

        private void comboBoxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }
    }
}
