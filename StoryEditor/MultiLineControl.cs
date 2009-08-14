﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class MultiLineControl : OneStoryProjectEditor.ResizableControl
    {
        public MultiLineControl(StoryStageLogic storyStageLogic, MultipleLineDataConverter aMLDC)
            : base(storyStageLogic)
        {
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            System.Diagnostics.Debug.Assert(tableLayoutPanel.RowCount == 1, "otherwise, fix this assumption: RetellingsControl.cs.20");
            tableLayoutPanel.RemoveRow(1);  // remove the one default one we start with

            // finally populate the buttons on that tool strip
            System.Diagnostics.Debug.Assert(aMLDC.Count > 0);
            int nNumRows = 0;
            foreach (StringTransfer strRowData in aMLDC)
            {
                InitRow(aMLDC.LabelTextFormat, strRowData, ref nNumRows);
            }

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        protected void InitRow(string strLabelTextFormat, StringTransfer strRowData, ref int nNumRows)
        {
            int nLayoutRow = nNumRows++;

            Label label = new Label();
            label.Anchor = AnchorStyles.Left;
            label.AutoSize = true;
            label.Name = strLabelTextFormat + nNumRows.ToString();
            label.Text = String.Format(strLabelTextFormat, nNumRows);

            CtrlTextBox tb = new CtrlTextBox(
                strLabelTextFormat + CstrSuffixTextBox + nNumRows.ToString(),
                this, strRowData, TeamMemberData.UserTypes.eUndefined);

            // add the label and tool strip as a new row to the table layout panel
            InsertRow(nLayoutRow);
            tableLayoutPanel.Controls.Add(label, 0, nLayoutRow);
            tableLayoutPanel.Controls.Add(tb, 1, nLayoutRow);
        }
    }
}