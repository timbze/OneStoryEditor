using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sword;

namespace OneStoryProjectEditor
{
    public partial class MinimalHtmlForm : Form
    {
        public MinimalHtmlForm()
        {
            InitializeComponent();
        }

        private void MinimalHtmlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void MinimalHtmlForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }

    public class MoveConNoteTooltip : MinimalHtmlForm
    {
        public void SetDocumentText(ConsultNoteDataConverter aConNote, 
            StoryEditor theSE, Point ptLocation)
        {
            Location = ptLocation;
            if (!theSE.splitContainerLeftRight.Panel2Collapsed)
                Width = theSE.splitContainerLeftRight.Panel2.Size.Width;

            webBrowser.DocumentText = aConNote.Html(null, 
                theSE.StoryProject.TeamMembers, theSE.LoggedOnMember, 
                theSE.TheCurrentStory, 0, 0);
        }
    }

	public class NetBibleFootnoteTooltip : MinimalHtmlForm
	{
        Manager _manager;
        Dictionary<string, Module> _lstModules = new Dictionary<string, Module>();
        string action, type, value;

        public NetBibleFootnoteTooltip(Manager manager)
		{
            _manager = manager;
		}

        public void ShowFootnote(string key, Point location)
        {
            //Record the key so we don't popup this hover over again
            Tag = key;

            //Set the location
            Location = location;

            //Parse the link
            string strModule = null;
            key = key.Substring(key.IndexOf('?') + 1); //key.Replace("passagestudy.jsp?", "");
            string[] splitKey = key.Split('&');
            action = splitKey[0].Replace("action=", "");
            type = splitKey[1].Replace("type=", "");
            value = splitKey[2].Replace("value=", "");
            if (splitKey.GetUpperBound(0) >= 3)
                strModule = splitKey[3].Replace("module=", "");

            string strReference = null;
            if (splitKey.GetUpperBound(0) >= 4)
            {
                strReference = splitKey[4].Replace("passage=", "");
                strReference = strReference.Replace("%3A", ":");
                strReference = strReference.Replace("+", " ");
            }

            /*
            if (action.Equals("showStrongs") && type.Equals("Greek"))
                ShowStrongsGreek(value);
            else if (action.Equals("showStrongs") && type.Equals("Hebrew"))
                ShowStrongsHebrew(value);
            else if (action.Equals("showMorph") && type.Contains("strongMorph"))
                ShowMorphRobinson(value);
            else if (action.Equals("showMorph") && type.Contains("robinson"))
                ShowMorphRobinson(value);
            else 
            */

            Module moduleForNote = null;
            if (action.Equals("showNote") && type.Contains("x") && !String.IsNullOrEmpty(strModule))
            {
                // I'm imagining that the module of the note could be different from the module of the text
                if (!_lstModules.TryGetValue(strModule, out moduleForNote))
                {
                    moduleForNote = _manager.GetModuleByName(strModule);
                    _lstModules.Add(strModule, moduleForNote);
                }

                moduleForNote.KeyText = strReference;
                ShowNote(moduleForNote, value);
            }
            else if (action.Equals("showNote") && type.Contains("n") && !String.IsNullOrEmpty(strModule))
            {
                // I'm imagining that the module of the note could be different from the module of the text
                if (!_lstModules.TryGetValue(strModule, out moduleForNote))
                {
                    moduleForNote = _manager.GetModuleByName(strModule);
                    _lstModules.Add(strModule, moduleForNote);
                }
                moduleForNote.KeyText = strReference;
                ShowNote(moduleForNote, value);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
                SetDisplayText(moduleForNote, "");
            }

            Show();
            webBrowser.Focus();
        }

        private void ShowNote(Module module, string NoteType)
        {
            var footnoteLines = module.GetEntryAttribute("Footnote", NoteType, "body", '1');
            var strFootnote = string.Join(Environment.NewLine, footnoteLines);
            SetDisplayText(module, strFootnote);
        }

        internal void SetDisplayText(Module module, string text)
		{
			//Used until I 
			if (string.IsNullOrEmpty(text))
				text =
				  "UNKNOWN KEY\r\n\r\n"
				+ "Key: " + Tag + "\r\n"
				+ "Action: " + action + "\r\n"
				+ "Type: " + type + "\r\n"
				+ "Value: " + value + "\r\n";

            // c_str is incorrectly returning a utf-8 encode string as widened utf-16, so work-around:
            byte[] aby = Encoding.Default.GetBytes(text);
            text = Encoding.UTF8.GetString(aby);

            if (module != null)
            {
                string strFontName, strModuleVersion = module.Name;
                if (Program.MapSwordModuleToFont.TryGetValue(strModuleVersion, out strFontName))
                    text = String.Format(NetBibleViewer.CstrAddFontFormat, text, strFontName);
            }

            //Display text in a web browser so we get Greek/Hebrew, etc.
            webBrowser.DocumentText = text; // .Replace("<br>", "\r\n").Replace("<br />", "\r\n");
		}

        /*
		private void ShowStrongsGreek(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);
			
			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("StrongsGreek");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}

		private void ShowStrongsHebrew(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);

			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("StrongsHebrew");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}

		private void ShowMorphRobinson(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);

			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("Robinson");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}
        */
	}
}