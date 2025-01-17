﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetLoc;
using System.Linq;

namespace OneStoryProjectEditor
{
    public partial class TeamMemberForm : TopForm
    {
        private string CstrDefaultOKLabel
        {
            get { return Localizer.Str("&Login"); }
        }

        private string CstrReturnLabel
        {
            get { return Localizer.Str("&Return"); }
        }

        private readonly TeamMembersData _dataTeamMembers;
        private readonly ProjectSettings _theProjSettings;
        protected StoryProjectData _theStoryProjectData;
        private string m_strSelectedMemberName;

        Dictionary<string, TeamMemberData> m_mapNewMembersThisSession = new Dictionary<string, TeamMemberData>();

        public bool Modified;

        private TeamMemberForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public TeamMemberForm(TeamMembersData dataTeamMembers, bool bUseLoginLabel,
            ProjectSettings theProjSettings, StoryProjectData theStoryProjectData)
        {
            _dataTeamMembers = dataTeamMembers;
            _theProjSettings = theProjSettings;
            _theStoryProjectData = theStoryProjectData;

            InitializeComponent();
            Localizer.Ctrl(this);

            Localizer.Default.LocLanguage.SetFont(listBoxTeamMembersEditors);
            Localizer.Default.LocLanguage.SetFont(listBoxTeamMembersCollaborators);
            InitializeListBoxes();

            if (listBoxTeamMembersEditors.Items.Count > 0)
            {
                string strLastMemberLogin;
                if ((theProjSettings != null) &&
                    Program.MapProjectNameToLastMemberLogin.TryGetValue(theProjSettings.ProjectName, out strLastMemberLogin))
                    listBoxTeamMembersEditors.SelectedItem = strLastMemberLogin;
                else if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
                    listBoxTeamMembersEditors.SelectedItem = Properties.Settings.Default.LastMemberLogin;
            }

            if (listBoxTeamMembersCollaborators.Items.Count > 0)
            {
                string strLastMemberLogin;
                if ((theProjSettings != null) &&
                    Program.MapProjectNameToLastMemberLogin.TryGetValue(theProjSettings.ProjectName, out strLastMemberLogin))
                    listBoxTeamMembersCollaborators.SelectedItem = strLastMemberLogin;
                else if (!String.IsNullOrEmpty(Properties.Settings.Default.LastMemberLogin))
                    listBoxTeamMembersCollaborators.SelectedItem = Properties.Settings.Default.LastMemberLogin;
            }

            if (bUseLoginLabel)
                return;

            buttonOK.Text = CstrReturnLabel;
            toolTip.SetToolTip(buttonOK, Localizer.Str("Click to return to the previous window"));
        }

        private void InitializeListBoxes()
        {
            listBoxTeamMembersEditors.Items.Clear();
            foreach (var aMember in _dataTeamMembers.Values.Where(m => IsEditor(m)))
            {
                listBoxTeamMembersEditors.Items.Add(GetListBoxItem(aMember));
            }

            listBoxTeamMembersCollaborators.Items.Clear();
            foreach (var aMember in _dataTeamMembers.Values.Where(m => IsCollaborator(m)))
            {
                listBoxTeamMembersCollaborators.Items.Add(GetListBoxItem(aMember));
            }
        }

        private static bool IsCollaborator(TeamMemberData aMember)
        {
            return (TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.UNS) ||
                                TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.Crafter)) &&
                                !TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.ProjectFacilitator);
        }

        private static bool IsEditor(TeamMemberData aMember)
        {
            return (!TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.UNS) &&
                                !TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.Crafter)) ||
                                TeamMemberData.IsUser(aMember.MemberType, TeamMemberData.UserTypes.ProjectFacilitator);
        }

        private static string GetListBoxItem(TeamMemberData theTeamMember)
        {
            return GetListBoxItem(theTeamMember.Name, theTeamMember.MemberType);
        }

        public static string GetListBoxItem(string strName, TeamMemberData.UserTypes eMemberRole)
        {
            return String.Format("{0} ({1})",
                                 strName,
                                 TeamMemberData.GetMemberTypeAsDisplayString(eMemberRole));
        }

        private static void ParseListBoxItem(string strItem, 
            out string strName, out TeamMemberData.UserTypes eMemberRole)
        {
            int nIndex = strItem.LastIndexOf(" (");
            strName = strItem.Substring(0, nIndex);
            string strRole = strItem.Substring(nIndex + 2, strItem.Length - nIndex - 3);
            eMemberRole = TeamMemberData.GetMemberTypeFromDisplayString(strRole);
        }

        public string SelectedMemberName
        {
            get { return m_strSelectedMemberName; }
        }

        public string SelectedMember
        {
            set
            {
                if (tabControl1.SelectedTab == tabEditors)
                    listBoxTeamMembersEditors.SelectedItem = value;
                else
                    listBoxTeamMembersCollaborators.SelectedItem = value;
            }
        }
        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void listBoxTeamMembersEditors_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessSelectedIndexChanged(listBoxTeamMembersEditors);
        }

        private void listBoxTeamMembersCollaborators_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessSelectedIndexChanged(listBoxTeamMembersCollaborators);
        }

        private void ProcessSelectedIndexChanged(ListBox listBoxTeamMembers)
        {
            bool bOneSelected = (listBoxTeamMembers.SelectedIndex != -1);
            buttonEditMember.Enabled = buttonOK.Enabled = bOneSelected;

            if (bOneSelected)
            {
                TeamMemberData.UserTypes eUserType;
                ParseListBoxItem((string)listBoxTeamMembers.SelectedItem,
                                 out m_strSelectedMemberName, out eUserType);

                if (_dataTeamMembers.ContainsKey(m_strSelectedMemberName))
                {
                    var theMember = _dataTeamMembers[m_strSelectedMemberName];
                    buttonMergeUns.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                    TeamMemberData.UserTypes.UNS));
                    buttonMergeCrafter.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                        TeamMemberData.UserTypes.Crafter));
                    buttonMergeProjectFacilitators.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                                                                                    TeamMemberData.UserTypes.
                                                                                        ProjectFacilitator));
                    buttonMergeConsultant.Visible = (TeamMemberData.IsUser(theMember.MemberType,
                        TeamMemberData.UserTypes.ConsultantInTraining | TeamMemberData.UserTypes.IndependentConsultant));

                    buttonMergeCoach.Visible = (TeamMemberData.IsUser(theMember.MemberType, TeamMemberData.UserTypes.Coach));

                    buttonDeleteMember.Visible = !_theStoryProjectData.DoesReferenceExist(theMember) &&                                             // no references
                        ((m_strSelectedMemberName != TeamMembersData.CstrBrowserMemberName) && (eUserType != TeamMemberData.UserTypes.JustLooking));// but ignore the Browser (Just Looking) one (no need to delete that)
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabCollaborators)
            {
                tabControl1.SelectedTab = tabEditors;
                return;
            }

            // this button should only be enabled if a team member is selected
            if (listBoxTeamMembersEditors.SelectedIndex == -1)
            return;

            // if the selected user is a UNS, this is probably a mistake.
            TeamMemberData theMember = _dataTeamMembers[m_strSelectedMemberName];
            var eAllowedLoginRoleFilter = (theMember.MemberType &
                                           (TeamMemberData.UserTypes.ProjectFacilitator |
                                            TeamMemberData.UserTypes.ConsultantInTraining |
                                            TeamMemberData.UserTypes.IndependentConsultant |
                                            TeamMemberData.UserTypes.Coach |
                                            TeamMemberData.UserTypes.EnglishBackTranslator |
                                            TeamMemberData.UserTypes.FirstPassMentor |
                                            TeamMemberData.UserTypes.JustLooking));

            if ((buttonOK.Text == CstrDefaultOKLabel)
                && (eAllowedLoginRoleFilter == TeamMemberData.UserTypes.Undefined))
            {
                LocalizableMessageBox.Show(Localizer.Str("You have added a UNS in order to identify, for example, which UNS did the back translation or a particular test. However, you as the Project Facilitator should still be logged in to enter the UNS's comments. So select your *Project Facilitator* member name and click 'Login' again"),
                                StoryEditor.OseCaption);
                return;
            }

            // when the button label is "OK", it means we're adding a UNS
            if (buttonOK.Text == CstrDefaultOKLabel)
            {
                Program.MapProjectNameToLastMemberLogin[_theProjSettings.ProjectName] = SelectedMemberName;
                Properties.Settings.Default.ProjectNameToLastMemberLogin = Program.DictionaryToArray(Program.MapProjectNameToLastMemberLogin);
                Program.MapProjectNameToLastUserType[_theProjSettings.ProjectName] = eAllowedLoginRoleFilter.ToString();
                Properties.Settings.Default.ProjectNameToLastUserType = Program.DictionaryToArray(Program.MapProjectNameToLastUserType);
                Properties.Settings.Default.LastMemberLogin = SelectedMemberName;
                Properties.Settings.Default.LastUserType = eAllowedLoginRoleFilter.ToString();
                Properties.Settings.Default.Save();
            }

            // _projSettings.IsConfigured = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonAddNewMember_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabEditors)
            {
                processAddNewMember(listBoxTeamMembersEditors);
            }
            else
            {
                processAddNewMember(listBoxTeamMembersCollaborators);
            }
        }

        private void processAddNewMember(ListBox listBoxTeamMembers)
        {
            // unselect any member and set the target tab (see 
            //  tabControlProjectMetaData_Selected for what happens)
            listBoxTeamMembers.SelectedIndex = -1;

            var dlg = new EditMemberForm(null, _theProjSettings, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var strItem = GetListBoxItem(dlg.MemberName, dlg.MemberType);
                if (listBoxTeamMembers.Items.Contains(strItem) ||
                    _dataTeamMembers.ContainsKey(dlg.MemberName))
                {
                    LocalizableMessageBox.Show(
                        String.Format(
                            Localizer.Str(
                                "Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then select the name in the listbox and click the 'Edit Member' button"),
                            dlg.MemberName), StoryEditor.OseCaption);
                    return;
                }

                Modified = true;

                // add this new user to the proj file
                var theNewMemberData = new TeamMemberData(dlg.MemberName,
                        dlg.MemberType, String.Format("mem-{0}", Guid.NewGuid()),
                        dlg.Email, dlg.SkypeID, dlg.TeamViewerID, dlg.Phone, dlg.AltPhone,
                        dlg.BioData)
                {
                    DefaultAllowed = dlg.DefaultAllowed,
                    DefaultRequired = dlg.DefaultRequired
                };

                _dataTeamMembers.Add(dlg.MemberName, theNewMemberData);
                m_mapNewMembersThisSession.Add(dlg.MemberName, theNewMemberData);
                // listBoxTeamMembers.Items.Add(strItem);
                InitializeListBoxes();  // this function decides which list it goes into

                // let's also make sure the correct tab is selected
                InsureCorrectTableIsSelected(theNewMemberData).SelectedItem = strItem;
            }
        }

        /// <summary>
        /// this method will determine if the correct tab is shown for the given member and
        /// as a short cut (e.g. to selecting a record in the list box), it will return the 
        /// relevant listbox for visible tab
        /// </summary>
        /// <param name="theMember"></param>
        /// <returns></returns>
        private ListBox InsureCorrectTableIsSelected(TeamMemberData theMember)
        {
            if (IsEditor(theMember))
            {
                if (tabControl1.SelectedTab == tabCollaborators)
                    tabControl1.SelectedTab = tabEditors;
                return listBoxTeamMembersEditors;
            }
            else
            {
                System.Diagnostics.Debug.Assert(IsCollaborator(theMember));
                if (tabControl1.SelectedTab == tabEditors)
                    tabControl1.SelectedTab = tabCollaborators;
                return listBoxTeamMembersCollaborators;
            }
        }

        private void buttonEditMember_Click(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab == tabEditors)
            {
                processEditMember(listBoxTeamMembersEditors);

            }
            else
            {
                processEditMember(listBoxTeamMembersCollaborators);
            }
        }

        private void processEditMember(ListBox listBoxTeamMembers)
        {
            // this button should only be enabled if a team member is selected
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
            int nIndex = listBoxTeamMembers.SelectedIndex;

            TeamMemberData.UserTypes eMemberRole;
            ParseListBoxItem((string)listBoxTeamMembers.SelectedItem,
                             out m_strSelectedMemberName, out eMemberRole);

            System.Diagnostics.Debug.Assert(_dataTeamMembers.ContainsKey(m_strSelectedMemberName));
            TeamMemberData theMemberData = _dataTeamMembers[m_strSelectedMemberName];
            var dlg = new EditMemberForm(theMemberData, _theProjSettings, true);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            Modified = true;

            // if the name of the edited item has been changed and the new name is already 
            //  in use, then don't change the name
            if ((dlg.MemberName != m_strSelectedMemberName)
                && _dataTeamMembers.ContainsKey(dlg.MemberName))
            {
                LocalizableMessageBox.Show(
                    String.Format(
                        Localizer.Str(
                            "Oops... you already have a member with the name, '{0}'. If you meant to edit that member, then select the name in the listbox and click the 'Edit Member' button."),
                        dlg.MemberName), StoryEditor.OseCaption);
            }
            else
                theMemberData.Name = dlg.MemberName;

            theMemberData.MemberType = dlg.MemberType;
            theMemberData.Email = dlg.Email;
            theMemberData.AltPhone = dlg.AltPhone;
            theMemberData.Phone = dlg.Phone;
            theMemberData.BioData = dlg.BioData;
            theMemberData.SkypeID = dlg.SkypeID;
            theMemberData.TeamViewerID = dlg.TeamViewerID;
            theMemberData.DefaultAllowed = dlg.DefaultAllowed;
            theMemberData.DefaultRequired = dlg.DefaultRequired;

            // update the role listbox
            // listBoxMemberRoles.Items[nIndex] = TeamMemberData.GetMemberTypeAsDisplayString(theMemberData.MemberType);
            if (theMemberData.Name != m_strSelectedMemberName)
            {
                _dataTeamMembers.Remove(m_strSelectedMemberName);
                m_strSelectedMemberName = theMemberData.Name;
                _dataTeamMembers.Add(m_strSelectedMemberName, theMemberData);
            }

            InitializeListBoxes();

            var strItem = GetListBoxItem(theMemberData.Name, theMemberData.MemberType);
            InsureCorrectTableIsSelected(theMemberData).SelectedItem = strItem;

            // keep a hang on it so we don't try to, for example, give it a new guid
            if (!m_mapNewMembersThisSession.ContainsKey(dlg.MemberName))
                m_mapNewMembersThisSession.Add(dlg.MemberName, theMemberData);
        }

        private void buttonDeleteMember_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabEditors)
            {
                processDeleteMember(listBoxTeamMembersEditors);
            }
            else
            {
                processDeleteMember(listBoxTeamMembersEditors);
            }
        }

        private void processDeleteMember(ListBox listBoxTeamMembers)
        {
            // this is only enabled if we added the member this session
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedItem != null);
            TeamMemberData.UserTypes eUserType;
            ParseListBoxItem((string)listBoxTeamMembers.SelectedItem,
                                out m_strSelectedMemberName, out eUserType);

            RemoveTracesOfMember(m_strSelectedMemberName, eUserType);

            m_mapNewMembersThisSession.Remove(m_strSelectedMemberName);
            buttonDeleteMember.Visible = false; // make it false
        }

        private void listBoxTeamMembersEditors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(sender, e);
        }

        private void buttonMergeUns_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.UNS, _theStoryProjectData.ReplaceUns);
        }

        private void buttonMergeProjectFacilitators_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.ProjectFacilitator, _theStoryProjectData.ReplaceProjectFacilitator);
        }

        private void buttonMergeCrafter_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.Crafter, _theStoryProjectData.ReplaceCrafter);
        }

        private void buttonMergeConsultant_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.IndependentConsultant | TeamMemberData.UserTypes.ConsultantInTraining, _theStoryProjectData.ReplaceConsultant);
        }

        private void buttonMergeCoach_Click(object sender, EventArgs e)
        {
            // this button should only be enabled if a team member is selected
            ReplaceMember(TeamMemberData.UserTypes.Coach, _theStoryProjectData.ReplaceCoach);
        }

        private delegate void ReplaceMemberDelegate(string strOldUnsGuid, string strNewUnsGuid);

        private void ReplaceMember(TeamMemberData.UserTypes eRole, ReplaceMemberDelegate replaceMemberDelegate)
        {
            if(tabControl1.SelectedTab == tabEditors)
            {
                ProcessReplaceMember(listBoxTeamMembersEditors, eRole, replaceMemberDelegate);
            }
            else
            {
                ProcessReplaceMember(listBoxTeamMembersCollaborators, eRole, replaceMemberDelegate);
            }
        }

        private void ProcessReplaceMember(ListBox listBoxTeamMembers, TeamMemberData.UserTypes eRole, ReplaceMemberDelegate replaceMemberDelegate)
        {
            System.Diagnostics.Debug.Assert(listBoxTeamMembers.SelectedIndex != -1);
            int nIndex = listBoxTeamMembers.SelectedIndex;

            TeamMemberData.UserTypes eMemberRole;
            ParseListBoxItem((string)listBoxTeamMembers.SelectedItem,
                             out m_strSelectedMemberName, out eMemberRole);

            System.Diagnostics.Debug.Assert(_dataTeamMembers.ContainsKey(m_strSelectedMemberName));
            TeamMemberData theMemberData = _dataTeamMembers[m_strSelectedMemberName];

            // query the UNS to merge into this UNS record
            var dlg = new MemberPicker(_theStoryProjectData, eRole)
            {
                Text = String.Format(Localizer.Str("Choose the {0} to merge into the record for '{1}'"),
                                     TeamMemberData.GetMemberTypeAsDisplayString(eRole),
                                     theMemberData.Name),
                ItemToBlock = theMemberData.Name
            };

            DialogResult res = dlg.ShowDialog();
            if (res != DialogResult.OK)
                return;

            string strOldMemberGuid = dlg.SelectedMember.MemberGuid;
            TeamMemberData.UserTypes eOrigRoles = dlg.SelectedMember.MemberType;
            try
            {
                replaceMemberDelegate(strOldMemberGuid, theMemberData.MemberGuid);
                theMemberData.MergeWith(dlg.SelectedMember);
                Modified = true;

                dlg.SelectedMember.MemberType &= ~eRole;
                if (dlg.SelectedMember.MemberType != TeamMemberData.UserTypes.Undefined)
                {
                    res = LocalizableMessageBox.Show(String.Format(Localizer.Str("'{0}' has these additional roles: '{1}'. Would you like to add those roles to '{2}' also?"),
                                                        _dataTeamMembers.GetNameFromMemberId(strOldMemberGuid),
                                                        TeamMemberData.GetMemberTypeAsDisplayString(
                                                            dlg.SelectedMember.MemberType),
                                                        _dataTeamMembers.GetNameFromMemberId(theMemberData.MemberGuid)),
                                          StoryEditor.OseCaption,
                                          MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                        return;

                    MergeOtherRoles(dlg.SelectedMember.MemberType,
                                    strOldMemberGuid,
                                    theMemberData.MemberGuid);

                    // get the index for the member we're about to add new roles to 
                    //  (since we have to update his role list)
                    nIndex = listBoxTeamMembers.FindString(GetListBoxItem(theMemberData));

                    // now add those roles just in case they aren't already
                    theMemberData.MemberType |= dlg.SelectedMember.MemberType;

                    if (nIndex != -1)
                        listBoxTeamMembers.Items[nIndex] = GetListBoxItem(theMemberData);
                    else
                        System.Diagnostics.Debug.Assert(false);
                }

                res = LocalizableMessageBox.Show(String.Format(Localizer.Str("All of the information associated with member '{0}' is now associated with member '{1}'. Click 'Yes' to delete the record for '{0}'"),
                                                    dlg.SelectedMember.Name,
                                                    theMemberData.Name),
                                      StoryEditor.OseCaption,
                                      MessageBoxButtons.YesNoCancel);

                if (res != DialogResult.Yes)
                    return;

                string strNameToDelete = _dataTeamMembers.GetNameFromMemberId(strOldMemberGuid);
                RemoveTracesOfMember(strNameToDelete, eOrigRoles);
            }
            catch (StoryProjectData.ReplaceMemberException ex)
            {
                var strErrorMsg = String.Format(Localizer.Str("Unable to merge member '{0}' into member '{1}' because in story '{2}', {3}"),
                                                _dataTeamMembers.GetNameFromMemberId(
                                                    strOldMemberGuid),
                                                _dataTeamMembers.GetNameFromMemberId(
                                                    theMemberData.MemberGuid),
                                                ex.StoryName,
                                                String.Format(ex.Format,
                                                              _dataTeamMembers.GetNameFromMemberId(
                                                                  ex.MemberGuid)));
                LocalizableMessageBox.Show(strErrorMsg, StoryEditor.OseCaption);
            }
        }

        private void RemoveTracesOfMember(string strNameToDelete, TeamMemberData.UserTypes eRoles)
        {
            _dataTeamMembers.Remove(strNameToDelete);

            if (tabControl1.SelectedTab == tabEditors)
            {
                int nIndex = listBoxTeamMembersEditors.FindString(GetListBoxItem(strNameToDelete, eRoles));
                if (nIndex != -1)
                    listBoxTeamMembersEditors.Items.RemoveAt(nIndex);
                else
                    System.Diagnostics.Debug.Assert(false);
            }
            else
            {
                int nIndex = listBoxTeamMembersCollaborators.FindString(GetListBoxItem(strNameToDelete, eRoles));
                if (nIndex != -1)
                    listBoxTeamMembersCollaborators.Items.RemoveAt(nIndex);
                else
                    System.Diagnostics.Debug.Assert(false);
            }
            Modified = true;    // just in case (so we ask to save if something is deleted because all references are gone)
        }

        private void MergeOtherRoles(TeamMemberData.UserTypes eRoles, string strOldMemberGuid, string strNewMemberGuid)
        {
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.ProjectFacilitator))
                _theStoryProjectData.ReplaceProjectFacilitator(strOldMemberGuid, strNewMemberGuid);
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.Crafter))
                _theStoryProjectData.ReplaceCrafter(strOldMemberGuid, strNewMemberGuid);
            if (TeamMemberData.IsUser(eRoles, TeamMemberData.UserTypes.UNS))
                _theStoryProjectData.ReplaceUns(strOldMemberGuid, strNewMemberGuid);
        }
    }
}
