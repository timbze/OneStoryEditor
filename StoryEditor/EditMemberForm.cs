﻿using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class EditMemberForm : TopForm
    {
        private readonly TeamMemberData _theMemberData;
        private readonly ProjectSettings _theProjSettings;

        private EditMemberForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public EditMemberForm(TeamMemberData theMemberData, 
            ProjectSettings theProjSettings, bool bAllowNameRoleEdits)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            _theMemberData = theMemberData;
            _theProjSettings = theProjSettings;

            if (theMemberData == null) 
                return;

            textBoxName.Text = theMemberData.Name;
            MemberType = theMemberData.MemberType;
            textBoxEmail.Text = theMemberData.Email;
            textBoxPhoneNumber.Text = theMemberData.Phone;
            textBoxAltPhone.Text = theMemberData.AltPhone;
            textBoxSkypeID.Text = theMemberData.SkypeID;
            textBoxTeamViewer.Text = theMemberData.TeamViewerID;
            textBoxBioData.Text = theMemberData.BioData;
            DefaultAllowed = theMemberData.DefaultAllowed;
            DefaultRequired = theMemberData.DefaultRequired;

            if (!bAllowNameRoleEdits)
            {
                textBoxName.Enabled = groupBoxRole.Enabled = false;
                Text = Localizer.Str("View Member Information (to edit, use 'Project', 'Login', 'Edit Member')");
            }
        }

        public DialogResult UpdateMember()
        {
            DialogResult res = ShowDialog();
            if (res == DialogResult.OK)
            {
                if (MemberName != _theMemberData.Name)
                {
                    // make sure this isn't a mistake.
                    res = LocalizableMessageBox.Show(String.Format(Localizer.Str("Are you sure you want to change this member's name? [If you are trying to add a new member, then do it through the 'Project', 'Login' window instead. Clicking 'Yes' here will permanently remove the record for '{0}' and replace it with this new information for '{1}']"),
                                                        _theMemberData.Name, MemberName),
                                          StoryEditor.OseCaption, MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                        return res;
                }
                _theMemberData.Name = MemberName;
                _theMemberData.MemberType = MemberType;
                _theMemberData.Email = Email;
                _theMemberData.AltPhone = AltPhone;
                _theMemberData.Phone = Phone;
                _theMemberData.BioData = BioData;
                _theMemberData.SkypeID = SkypeID;
                _theMemberData.TeamViewerID = TeamViewerID;
                _theMemberData.DefaultAllowed = DefaultAllowed;
                _theMemberData.DefaultRequired = DefaultRequired;
            }
            return res;
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(MemberName) 
                || (MemberType == TeamMemberData.UserTypes.Undefined))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "You have to enter at least a name and indicate a role (even if you're 'Just Looking')!"),
                    StoryEditor.OseCaption);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        public string MemberName
        {
            get { return textBoxName.Text.Trim(); }
            set { textBoxName.Text = value; }
        }

        public TeamMemberData.UserTypes MemberType
        {
            get
            {
                TeamMemberData.UserTypes type = TeamMemberData.UserTypes.Undefined;
                if (checkBoxProjectFacilitator.Checked)
                    type |= TeamMemberData.UserTypes.ProjectFacilitator;
                if (checkBoxCrafter.Checked)
                    type |= TeamMemberData.UserTypes.Crafter;
                if (checkBoxUns.Checked)
                    type |= TeamMemberData.UserTypes.UNS;
                if (checkBoxLangSpecialtyReviewer.Checked)
                    type |= TeamMemberData.UserTypes.FirstPassMentor;
                if (radioButtonEnglishBackTranslator.Checked)
                    return TeamMemberData.UserTypes.EnglishBackTranslator;
                if (radioButtonConsultantInTraining.Checked)
                    return TeamMemberData.UserTypes.ConsultantInTraining;
                if (radioButtonIndependentConsultant.Checked)
                    return TeamMemberData.UserTypes.IndependentConsultant;
                if (radioButtonCoach.Checked)
                    return TeamMemberData.UserTypes.Coach;
                if (radioButtonJustViewing.Checked)
                    return TeamMemberData.UserTypes.JustLooking;
                return type;
            }
            set
            {
                if (TeamMemberData.IsUser(value,
                    TeamMemberData.UserTypes.ProjectFacilitator | 
                    TeamMemberData.UserTypes.Crafter | 
                    TeamMemberData.UserTypes.UNS | 
                    TeamMemberData.UserTypes.FirstPassMentor))
                {
                    checkBoxProjectFacilitator.Checked =
                        TeamMemberData.IsUser(value,
                                              TeamMemberData.UserTypes.ProjectFacilitator);
                    checkBoxCrafter.Checked =
                        TeamMemberData.IsUser(value,
                                              TeamMemberData.UserTypes.Crafter);
                    checkBoxUns.Checked =
                        TeamMemberData.IsUser(value,
                                              TeamMemberData.UserTypes.UNS);
                    checkBoxLangSpecialtyReviewer.Checked =
                        TeamMemberData.IsUser(value,
                                              TeamMemberData.UserTypes.FirstPassMentor);
                }
                else
                {
                    switch (value)
                    {
                        case TeamMemberData.UserTypes.EnglishBackTranslator:
                            radioButtonEnglishBackTranslator.Checked = true;
                            break;
                        case TeamMemberData.UserTypes.ConsultantInTraining:
                            radioButtonConsultantInTraining.Checked = true;
                            break;
                        case TeamMemberData.UserTypes.IndependentConsultant:
                            radioButtonIndependentConsultant.Checked = true;
                            break;
                        case TeamMemberData.UserTypes.Coach:
                            radioButtonCoach.Checked = true;
                            break;
                        case TeamMemberData.UserTypes.JustLooking:
                            radioButtonJustViewing.Checked = true;
                            break;
                        default:
                            System.Diagnostics.Debug.Assert(false); // should get here.
                            break;
                    }
                }
            }
        }

        private string TrimForPossibleNull(TextBox tb)
        {
            string strTbText = null;
            if (tb.Text != null)
                strTbText = tb.Text.Trim();
            return strTbText;
        }

        public string Email
        {
            get { return TrimForPossibleNull(textBoxEmail); }
            set { textBoxEmail.Text = value; }
        }

        public string Phone
        {
            get { return TrimForPossibleNull(textBoxPhoneNumber); }
            set { textBoxPhoneNumber.Text = value; }
        }

        public string AltPhone
        {
            get { return TrimForPossibleNull(textBoxAltPhone); }
            set { textBoxAltPhone.Text = value; }
        }

        public string SkypeID
        {
            get { return TrimForPossibleNull(textBoxSkypeID); }
            set { textBoxSkypeID.Text = value; }
        }

        public string TeamViewerID
        {
            get { return TrimForPossibleNull(textBoxTeamViewer); }
            set { textBoxTeamViewer.Text = value; }
        }

        public string BioData
        {
            get { return TrimForPossibleNull(textBoxBioData); }
            set { textBoxBioData.Text = value; }
        }

        private void radioButtonEnglishBackTranslator_Click(object sender, EventArgs e)
        {
            LocalizableMessageBox.Show(
                Localizer.Str(
                    "Talk with your project consultant before creating an 'Outside English back-translator' member on your team. The role of 'Outside English back-translator' is only used when the team doesn't have anyone to do an English back-translation and they arrange with some other team to do their English BT for them. If your team is doing its own English back-translations, then don't create an 'Outside English back-translator' member (you would do it as the Project Facilitator)"),
                StoryEditor.OseCaption);
            radioButton_CheckedChanged(sender, e);
        }

        private void radioButtonConsultantInTraining_CheckedChanged(object sender, EventArgs e)
        {
            SetDefaultTasksButtonState();
            radioButton_CheckedChanged(sender, e);
        }

        private void SetDefaultTasksButtonState()
        {
            buttonSetDefaultTasks.Visible = (checkBoxProjectFacilitator.Checked ||
                                             radioButtonConsultantInTraining.Checked);
        }

        private void buttonSetDefaultTasks_Click(object sender, EventArgs e)
        {
            if (_theProjSettings == null)
            {
                LocalizableMessageBox.Show(Localizer.Str("Can't configure these settings until after the project file is opened. Try again later"),
                                StoryEditor.OseCaption);
                return;
            }

            // find out from the consultant what tasks they want to set in the story
            try
            {
                if (checkBoxProjectFacilitator.Checked)
                    GetPfDefaultTasks();
                else if (radioButtonConsultantInTraining.Checked)
                    GetCitDefaultTasks();
                return;
            }
            catch (Exception ex)
            {
                LocalizableMessageBox.Show(ex.Message, StoryEditor.OseCaption);
            }
        }

        private long _DefaultAllowed;
        public long DefaultAllowed
        {
            get
            {
                if (_DefaultAllowed == 0)
                {
                    if (checkBoxProjectFacilitator.Checked)
                        return (long) TasksPf.DefaultAllowed;
                    if (radioButtonConsultantInTraining.Checked)
                        return (long) TasksCit.DefaultAllowed;
                }
                return _DefaultAllowed;
            }
            set { _DefaultAllowed = value; }
        }

        private long _DefaultRequired;
        public long DefaultRequired
        {
            get
            {
                if (_DefaultRequired == 0)
                {
                    if (checkBoxProjectFacilitator.Checked)
                        return (long) TasksPf.DefaultRequired;
                    if (radioButtonConsultantInTraining.Checked)
                        return (long) TasksCit.DefaultRequired;
                }
                return _DefaultRequired;
            }
            set { _DefaultRequired = value; }
        }

        private void GetPfDefaultTasks()
        {
            var dlg = new SetPfTasksForm(_theProjSettings,
                                         (TasksPf.TaskSettings)DefaultAllowed,
                                         (TasksPf.TaskSettings)DefaultRequired, true)
                                         {Text = GetDefaultTaskTitleText};

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            DefaultAllowed = (long)dlg.TasksAllowed;
            DefaultRequired = (long)dlg.TasksRequired;
        }

        private void GetCitDefaultTasks()
        {
            var dlg = new SetCitTasksForm((TasksCit.TaskSettings) DefaultAllowed,
                                          (TasksCit.TaskSettings) DefaultRequired) 
                                          {Text = GetDefaultTaskTitleText};

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            DefaultAllowed = (long)dlg.TasksAllowed;
            DefaultRequired = (long)dlg.TasksRequired;
        }

        private string GetDefaultTaskTitleText
        {
            get { return String.Format(Localizer.Str("Set Default Tasks for {0}"), textBoxName.Text); }
        }

        private void checkBoxRole_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            if ((cb != null) && cb.Checked)
            {
                radioButtonJustViewing.Checked =
                    radioButtonIndependentConsultant.Checked =
                    radioButtonEnglishBackTranslator.Checked =
                    radioButtonConsultantInTraining.Checked =
                    radioButtonCoach.Checked = false;
            }
        
            SetDefaultTasksButtonState();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if ((rb != null) && rb.Checked)
            {
                checkBoxProjectFacilitator.Checked =
                    checkBoxCrafter.Checked =
                    checkBoxUns.Checked = false;
            }
        }
    }
}
