﻿namespace OneStoryProjectEditor
{
    partial class GlossingControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new OneStoryProjectEditor.DynamicTableLayoutPanel();
            this.textBoxSourceWord = new System.Windows.Forms.TextBox();
            this.textBoxTargetWord = new System.Windows.Forms.TextBox();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.contextMenuStripAmbiguityPicker = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.textBoxSourceWord, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxTargetWord, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonJoin, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(136, 93);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxSourceWord
            // 
            this.textBoxSourceWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSourceWord.Location = new System.Drawing.Point(3, 3);
            this.textBoxSourceWord.Name = "textBoxSourceWord";
            this.textBoxSourceWord.Size = new System.Drawing.Size(100, 20);
            this.textBoxSourceWord.TabIndex = 0;
            this.textBoxSourceWord.TabStop = false;
            this.textBoxSourceWord.TextChanged += new System.EventHandler(this.textBoxSourceWord_TextChanged);
            // 
            // textBoxTargetWord
            // 
            this.textBoxTargetWord.ContextMenuStrip = this.contextMenuStripAmbiguityPicker;
            this.textBoxTargetWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTargetWord.Location = new System.Drawing.Point(3, 29);
            this.textBoxTargetWord.Name = "textBoxTargetWord";
            this.textBoxTargetWord.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetWord.TabIndex = 0;
            this.textBoxTargetWord.TextChanged += new System.EventHandler(this.textBoxTargetWord_TextChanged);
            this.textBoxTargetWord.Enter += new System.EventHandler(this.textBoxTargetWord_Enter);
            // 
            // buttonJoin
            // 
            this.buttonJoin.Image = global::OneStoryProjectEditor.Properties.Resources.Hyperlink;
            this.buttonJoin.Location = new System.Drawing.Point(109, 3);
            this.buttonJoin.Name = "buttonJoin";
            this.tableLayoutPanel.SetRowSpan(this.buttonJoin, 2);
            this.buttonJoin.Size = new System.Drawing.Size(24, 23);
            this.buttonJoin.TabIndex = 1;
            this.buttonJoin.TabStop = false;
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.buttonJoin_Click);
            // 
            // contextMenuStripAmbiguityPicker
            // 
            this.contextMenuStripAmbiguityPicker.Name = "contextMenuStripAmbiguityPicker";
            this.contextMenuStripAmbiguityPicker.Size = new System.Drawing.Size(61, 4);
            // 
            // GlossingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "GlossingControl";
            this.Size = new System.Drawing.Size(136, 93);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DynamicTableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TextBox textBoxSourceWord;
        private System.Windows.Forms.TextBox textBoxTargetWord;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAmbiguityPicker;
    }
}
