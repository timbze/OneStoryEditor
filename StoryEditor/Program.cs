﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;
using LibChorus.Tests;

namespace OneStoryProjectEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.RecentProjects == null)
                Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.RecentProjectPaths == null)
                Properties.Settings.Default.RecentProjectPaths = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.ProjectNameToHgUrl == null)
                Properties.Settings.Default.ProjectNameToHgUrl = new System.Collections.Specialized.StringCollection();
            _mapProjectNameToHgUrl = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUrl);

            if (Properties.Settings.Default.ProjectNameToHgUsername == null)
                Properties.Settings.Default.ProjectNameToHgUsername = new System.Collections.Specialized.StringCollection();
            _mapProjectNameToHgUsername = ArrayToDictionary(Properties.Settings.Default.ProjectNameToHgUsername);

            bool bNeedToSave = false;
            System.Diagnostics.Debug.Assert(Properties.Settings.Default.RecentProjects.Count == Properties.Settings.Default.RecentProjectPaths.Count);
            if (Properties.Settings.Default.RecentProjects.Count != Properties.Settings.Default.RecentProjectPaths.Count)
            {
                Properties.Settings.Default.RecentProjects.Clear();
                Properties.Settings.Default.RecentProjectPaths.Clear();
                bNeedToSave = true;
            }

            if (bNeedToSave)
                Properties.Settings.Default.Save();

            try
            {
                // one of the first things this might do is try to get a project from the internet, in which case
                //  the OneStory folder should exist
                if (!Directory.Exists(ProjectSettings.OneStoryProjectFolderRoot))
                    Directory.CreateDirectory(ProjectSettings.OneStoryProjectFolderRoot);

                Application.Run(new StoryEditor(Properties.Resources.IDS_MainStoriesSet));
#if DEBUG
                if (_astrProjectForSync.Count > 0)
                    if (MessageBox.Show("Do repository Sync?", Properties.Resources.IDS_Caption, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
#endif
                foreach (string strProjectFolder in _astrProjectForSync)
                    SyncWithRepository(strProjectFolder, false);
            }
            catch (Exception ex)
            {
                string strMessage = String.Format("Error occurred:{0}{0}{1}", Environment.NewLine, ex.Message);
                if (ex.InnerException != null)
                    strMessage += String.Format("{0}{1}", Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(strMessage, Properties.Resources.IDS_Caption);
            }
        }

        static List<string> _astrProjectForSync = new List<string>();
        static Dictionary<string, string> _mapProjectNameToHgUrl;
        static Dictionary<string, string> _mapProjectNameToHgUsername;

        public static void SetHgParameters(string strProjectName, string strUrl, string strUsername)
        {
            System.Diagnostics.Debug.Assert((_mapProjectNameToHgUrl != null) && (_mapProjectNameToHgUsername != null));
            _mapProjectNameToHgUrl[strProjectName] = strUrl;
            _mapProjectNameToHgUsername[strProjectName] = strUsername;
            Properties.Settings.Default.ProjectNameToHgUrl = DictionaryToArray(_mapProjectNameToHgUrl);
            Properties.Settings.Default.ProjectNameToHgUsername = DictionaryToArray(_mapProjectNameToHgUsername);
            Properties.Settings.Default.Save();
        }

        public static void SetProjectForSyncage(string strProjectFolder)
        {
            if (!_astrProjectForSync.Contains(strProjectFolder))
                _astrProjectForSync.Add(strProjectFolder);
        }

        // e.g. http://bobeaton:helpmepld@hg-private.languagedepot.org/snwmtn-test

        public static void SyncWithRepository(string strProjectFolder, bool bIsOpening)
        {
            string strProjectName = Path.GetFileNameWithoutExtension(strProjectFolder);
            string strHgUsername, strRepoUrl;
            if (!QueryHgRepoParameters(strProjectName, out strHgUsername, out strRepoUrl))
                return;

            using (var setup = new RepositorySetup(strHgUsername))
            {
                setup.ProjectFolderConfig.FolderPath = strProjectFolder;
                setup.ProjectFolderConfig.IncludePatterns.Add("*.onestory");
                setup.ProjectFolderConfig.IncludePatterns.Add("*.xml"); // the P7 key terms list
                Application.EnableVisualStyles();
                
                setup.Repository.SetKnownRepositoryAddresses(new []
                {
                    RepositoryAddress.Create("language depot", strRepoUrl),
                });

                setup.Repository.SetDefaultSyncRepositoryAliases(new[] { "language depot" });

                // for when we launch the program, just do a quick & dirty send/receive, but for
                //  closing, we can be more informative
                SyncUIDialogBehaviors suidb;
                SyncUIFeatures suif;
                if (bIsOpening)
                {
                    suidb = SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished;
                    suif = SyncUIFeatures.Minimal;
                }
                else
                {
                    suidb = SyncUIDialogBehaviors.Lazy;
                    suif = SyncUIFeatures.NormalRecommended;
                }

                using (var dlg = new SyncDialog(setup.ProjectFolderConfig, suidb, suif))
                {
                    dlg.ShowDialog();
                }
            }
        }

        private static bool QueryHgRepoParameters(string strProjectName, out string strUsername, out string strRepoUrl)
        {
            string strHgUrl = (_mapProjectNameToHgUrl.ContainsKey(strProjectName))
                ? _mapProjectNameToHgUrl[strProjectName] : null;
            string strHgUsername = (_mapProjectNameToHgUsername.ContainsKey(strProjectName))
                ? _mapProjectNameToHgUsername[strProjectName] : null;
            if (String.IsNullOrEmpty(strHgUrl))
            {
                HgRepoForm dlg = new HgRepoForm
                                        {
                                            ProjectName = strProjectName,
                                            UrlBase = "http://hg-private.languagedepot.org",
                                            Username = strHgUsername
                                        };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    strRepoUrl = dlg.Url;
                    strUsername = dlg.Username;
                    SetHgParameters(strProjectName, strRepoUrl, strUsername);
                    return true;
                }
            }
            else
            {
                strRepoUrl = strHgUrl;
                strUsername = strHgUsername;
                return true;
            }

            strUsername = strRepoUrl = null;
            return false;
        }

        public static Dictionary<string, string> ArrayToDictionary(System.Collections.Specialized.StringCollection data)
        { 
            var map = new Dictionary<string, string>(); 
            for (var i = 0; i < data.Count; i += 2)
            {
                map.Add(data[i], data[i + 1]);
            }
            
            return map; 
        }

        public static System.Collections.Specialized.StringCollection DictionaryToArray(Dictionary<string, string> map)
        {
            var lst = new System.Collections.Specialized.StringCollection();
            foreach (KeyValuePair<string,string> kvp in map)
            {
                lst.Add(kvp.Key);
                lst.Add(kvp.Value);
            }

            return lst;
        }
    }
}
