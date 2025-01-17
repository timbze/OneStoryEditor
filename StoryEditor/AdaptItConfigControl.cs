﻿using System;
using System.IO;
using System.Windows.Forms;
using ECInterfaces;
using NetLoc;
using SilEncConverters40;

namespace OneStoryProjectEditor
{
    public partial class AdaptItConfigControl : UserControl
    {
        public new NewProjectWizard Parent;
        public ProjectSettings.AdaptItConfiguration.AdaptItBtDirection BtDirection;

        private string _strSourceLanguageName;
        public string SourceLanguageName
        {
            get
            {
                return _strSourceLanguageName;
            }
            set
            {
                _strSourceLanguageName = value;
                _strAdaptItProjectName = null;
            }
        }

        private string _strTargetLanguageName;
        public string TargetLanguageName 
        {
            get { return _strTargetLanguageName; }
            set
            {
                _strTargetLanguageName = value;
                _strAdaptItProjectName = null;
            }
        }

        public AdaptItConfigControl()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        private ProjectSettings.AdaptItConfiguration _adaptItConfiguration;
        public ProjectSettings.AdaptItConfiguration AdaptItConfiguration
        { 
            get
            {
                var eType = AdaptItProjectType;
                if (eType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                    return null;

                if (_adaptItConfiguration == null)
                    _adaptItConfiguration = new ProjectSettings.AdaptItConfiguration();

                _adaptItConfiguration.BtDirection = BtDirection;    // from parent
                _adaptItConfiguration.ProjectType = AdaptItProjectType; // from user
                _adaptItConfiguration.ConverterName = AdaptItConverterName;
                _adaptItConfiguration.ProjectFolderName = _strAdaptItProjectFolderName;
                _adaptItConfiguration.RepoProjectName = GetProjectNameOrDefault(eType);
                _adaptItConfiguration.RepositoryServer = _strAdaptItRepositoryServer;
                _adaptItConfiguration.NetworkRepositoryPath = _strAdaptItNetworkRepositoryPath;
                return _adaptItConfiguration;
            }
            set
            {
                _adaptItConfiguration = value;
                if (_adaptItConfiguration != null)
                {
                    AdaptItConverterName = _adaptItConfiguration.ConverterName;
                    _strAdaptItProjectFolderName = _adaptItConfiguration.ProjectFolderName;
                    _strAdaptItProjectName = _adaptItConfiguration.RepoProjectName;
                    _strAdaptItRepositoryServer = _adaptItConfiguration.RepositoryServer;
                    _strAdaptItNetworkRepositoryPath = _adaptItConfiguration.NetworkRepositoryPath;
                    AdaptItProjectType = _adaptItConfiguration.ProjectType;
                    System.Diagnostics.Debug.Assert(_adaptItConfiguration.BtDirection == BtDirection);
                }
                else
                {
                    textBoxProjectPath.Clear();
                    AdaptItProjectType = ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None;
                    AdaptItConverterName = _strAdaptItNetworkRepositoryPath = null;
                    InitSharedOnlyFieldDefaults();
                }
            }
        }

        private void InitSharedOnlyFieldDefaults()
        {
            _strAdaptItProjectName = GetProjectNameOrDefault(AdaptItProjectType);
            _strAdaptItRepositoryServer = Properties.Resources.IDS_DefaultRepoServer;
        }

        private void ResetSharedOnlyFields()
        {
            _strAdaptItProjectName = 
                _strAdaptItRepositoryServer = 
                _strAdaptItNetworkRepositoryPath = null;
        }

        private string AdaptItConverterName
        {
            get { return textBoxProjectPath.Text; }
            set { textBoxProjectPath.Text = value; }
        }

        private string _strAdaptItProjectFolderName;
        private string _strAdaptItProjectName;
        private string _strAdaptItRepositoryServer;
        private string _strAdaptItNetworkRepositoryPath;

        private string GetProjectNameOrDefault(ProjectSettings.AdaptItConfiguration.AdaptItProjectType eType)
        {
        /*
            if (String.IsNullOrEmpty(_strAdaptItProjectName)
                && !String.IsNullOrEmpty(SourceLanguageName)
                && !String.IsNullOrEmpty(TargetLanguageName)
                && (eType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject))
            {
                _strAdaptItProjectName = String.Format(Properties.Resources.AdaptItProjectRepositoryFormat,
                                                       SourceLanguageName.ToLower(), 
                                                       TargetLanguageName.ToLower());
            }
        */
            return _strAdaptItProjectName;
        }

        private ProjectSettings.AdaptItConfiguration.AdaptItProjectType AdaptItProjectType
        {
            get
            {
                if (radioButtonLocal.Checked)
                    return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly;
                if (radioButtonShared.Checked)
                    return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject;
                return ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None;
            }
            set
            {
                if (ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly == value)
                    radioButtonLocal.Checked = true;
                else if (ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject == value)
                    radioButtonShared.Checked = true;
                else 
                    radioButtonNone.Checked = true;
            }
        }

        private void radioButtonLocal_Click(object sender, EventArgs e)
        {
            ResetSharedOnlyFields();
            // first let's see if an AI Lookup transducer already exists with the
            //  proper name
            string strAiProjectFolder = AdaptItKBReader.AdaptItProjectFolder(null, SourceLanguageName, TargetLanguageName);
            if (Directory.Exists(strAiProjectFolder))
            {
                string strConverterSpec = AdaptItKBReader.AdaptItLookupFileSpec(null, SourceLanguageName, TargetLanguageName);
                if (File.Exists(strConverterSpec))
                {
                    string strConverterName = AdaptItKBReader.AdaptItLookupConverterName(SourceLanguageName, TargetLanguageName);
                    if (theECs.ContainsKey(strConverterName))
                    {
                        IEncConverter theEc = theECs[strConverterName];
                        if (theEc is AdaptItEncConverter)
                        {
                            AdaptItConverterName = theEc.Name;
                            _strAdaptItProjectFolderName =
                                Path.GetFileNameWithoutExtension(
                                    AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
                            Parent.Modified = true;
                            return;
                        }
                    }
                }
            }

            // otherwise, let's see if the user wants us to create it or browse for it
            DialogResult res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Click 'Yes' to create an Adapt It project to use for back-translation from '{0}' to '{1}'. Click 'No' to browse for an existing Adapt It project instead"),
                                                             SourceLanguageName, TargetLanguageName),
                                               StoryEditor.OseCaption,
                                               MessageBoxButtons.YesNoCancel);
            
            // 'Yes' means the user is asking us to create the AI project
            if (res == DialogResult.Yes)
            {
                CreateAiProject();
                // this return was uncommented here, but I think we want to make it 'modified' (i.e. fall thru below) if we do this...
                // return;
            }

            // 'No' means browse for it
            if (res == DialogResult.No)
                buttonBrowse_Click(sender, e);

            Parent.Modified = true;
        }

        private void CreateAiProject()
        {
            try
            {
                ProjectSettings.LanguageInfo liSource, liTarget;
                var theEc = AdaptItGlossing.InitLookupAdapter(Parent.ProjSettings, BtDirection,
                                                              Parent.LoggedInMember,
                                                              out liSource, out liTarget);
                AdaptItConverterName = theEc.Name;
                _strAdaptItProjectFolderName =
                    Path.GetFileNameWithoutExtension(
                        AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
            }
            catch (Exception ex)
            {
                Program.ShowException(ex);
            }
        }

        private void radioButtonShared_Click(object sender, EventArgs e)
        {
            DoSharedAiProjectClick(null);
        }

        private EncConverters _theECs;
        private EncConverters theECs
        {
            get
            {
                if (_theECs == null)
                    _theECs = new EncConverters();
                return _theECs;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.None)
                return;

            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.LocalAiProjectOnly)
            {
                string strConverterName = null;
                IEncConverter theEC = new AdaptItEncConverter();
                if (theECs.AutoConfigureEx(theEC,
                                           ConvType.Unicode_to_from_Unicode,
                                           ref strConverterName,
                                           "UNICODE", "UNICODE"))
                {
                    AdaptItConverterName = theEC.Name;
                }
            }

            if (AdaptItProjectType == ProjectSettings.AdaptItConfiguration.AdaptItProjectType.SharedAiProject)
            {
                DoSharedAiProjectClick(null);
            }

            Parent.Modified = true;
        }

        private void DoSharedAiProjectClick(bool? doingPush)
        {
            IEncConverter theEc = null;
            string strProjectFolder, strConverterName;

            if (String.IsNullOrEmpty(_strAdaptItProjectName) || String.IsNullOrEmpty(_strAdaptItRepositoryServer))
            {
                // means it isn't initialized
                DialogResult res = LocalizableMessageBox.Show(Localizer.Str("Is the shared Adapt It project on your computer now? (click 'Yes' to browse for it; click 'No' to enter the repository information for it)"),
                                                   StoryEditor.OseCaption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                    return;

                if (res == DialogResult.No)
                {
                    // if it doesn't already exist, then we should create it... but not if they are intending to pull it
                    res = LocalizableMessageBox.Show(Localizer.Str("Is the shared Adapt It project already on the internet? (i.e. will you downloading it? click 'No' if you want to create a new empty project for it)"),
                                                     StoryEditor.OseCaption,
                                                     MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                        return;

                    doingPush = (res == DialogResult.No);
                    if (doingPush == true)
                        CreateAiProject();

                    // then fall thru
                }
                else // the project *is* on this machine...
                {
                    // first let's see if an AI Lookup transducer already exists with the
                    //  proper name
                    strConverterName = AdaptItKBReader.AdaptItLookupConverterName(SourceLanguageName,
                                                                                  TargetLanguageName);
                    if (theECs.ContainsKey(strConverterName))
                    {
                        theEc = theECs[strConverterName];
                        if (theEc is AdaptItEncConverter)
                        {
                            AdaptItConverterName = theEc.Name;
                            strProjectFolder =
                                AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier);
                            res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Is the shared Adapt It project in the '{0}' folder?"),
                                                                strProjectFolder),
                                                  StoryEditor.OseCaption,
                                                  MessageBoxButtons.YesNoCancel);
                            if (res == DialogResult.Cancel)
                                return;

                            if (res == DialogResult.No)
                                theEc = null;

                            // the 'yes' case falls through and skips the next if statement
                            _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                        }
                    }

                    if (theEc == null)
                    {
                        // this means we don't know which one it was, so query for which project 
                        //  the user wants to share
                        theEc = new AdaptItEncConverter();
                        if (theECs.AutoConfigureEx(theEc,
                                                   ConvType.Unicode_to_from_Unicode,
                                                   ref strConverterName,
                                                   "UNICODE", "UNICODE"))
                        {
                            AdaptItConverterName = theEc.Name;
                            _strAdaptItProjectFolderName =
                                Path.GetFileNameWithoutExtension(
                                    AdaptItGlossing.GetAiProjectFolderFromConverterIdentifier(theEc.ConverterIdentifier));
                        }
                        else
                            return;
                    }

                    // in case the user has just created it via Local and switches to Shared, we
                    //  need to re setup the project name, etc...
                    if (String.IsNullOrEmpty(_strAdaptItProjectName) ||
                        String.IsNullOrEmpty(_strAdaptItRepositoryServer))
                        InitSharedOnlyFieldDefaults();

                    // now we know which local AI project it is and it's EncConverter, but now
                    //  we need to possibly push the project.
                    DoPushPull(true, out strProjectFolder);
                    if (!String.IsNullOrEmpty(strProjectFolder))
                        _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
                    Parent.Modified = true;
                    return;
                }
            }

            DoPushPull(doingPush, out strProjectFolder);
            if (String.IsNullOrEmpty(strProjectFolder))
                return;

            _strAdaptItProjectFolderName = Path.GetFileNameWithoutExtension(strProjectFolder);
            Parent.Modified = true;

            strConverterName = "Lookup in " + _strAdaptItProjectFolderName;
            string strConverterSpec = Path.Combine(strProjectFolder,
                                                   _strAdaptItProjectFolderName + ".xml");
            theECs.AddConversionMap(strConverterName, strConverterSpec,
                ConvType.Unicode_to_from_Unicode, EncConverters.strTypeSILadaptit,
                "UNICODE", "UNICODE", ProcessTypeFlags.DontKnow);

            theEc = theECs[strConverterName];
            if (theEc == null)
            {
                LocalizableMessageBox.Show(Localizer.Str(@"Unable to create an AdaptIt glossing converter for the project! Are you missing the file <InstallDir>\EC\Plugins\AI 4.0.0.0 Plugin Details.xml"),
                                           StoryEditor.OseCaption,
                                           MessageBoxButtons.OKCancel);
                return;
            }
            AdaptItConverterName = theEc.Name;
        }

        private void DoPushPull(bool? doingPush, out string strProjectFolder)
        {
            strProjectFolder = null;
            var dlg = new AiRepoSelectionForm
                          {
                              SourceLanguageName = SourceLanguageName,
                              TargetLanguageName = TargetLanguageName,
                              InternetAddress = _strAdaptItRepositoryServer,
                              NetworkAddress = _strAdaptItNetworkRepositoryPath,
                              ProjectName = _strAdaptItProjectName,
                              Parent = Parent,
                              DoingPush = doingPush
                          };

            try
            {
                // this dialog takes care of push and pull
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    strProjectFolder = dlg.ProjectFolder;
                    _strAdaptItProjectName = dlg.ProjectName;
                    _strAdaptItRepositoryServer = dlg.InternetAddress;
                    _strAdaptItNetworkRepositoryPath = dlg.NetworkAddress;
                }
            }
            catch (Exception ex)
            {
                string strErrorMsg = String.Format(Localizer.Str("Unable to Send/Receive the AdaptIt project '{1}' from the requested server{0}{2}{0}{3}"),
                    Environment.NewLine, _strAdaptItProjectName,
                    ((ex.InnerException != null) ? ex.InnerException.Message : ""), ex.Message);
                LocalizableMessageBox.Show(strErrorMsg, StoryEditor.OseCaption);
            }
        }

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            ResetSharedOnlyFields();
            AdaptItConverterName = null;
            Parent.Modified = true;
        }
    }
}
