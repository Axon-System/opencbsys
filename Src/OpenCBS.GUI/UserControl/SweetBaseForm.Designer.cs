namespace OpenCBS.GUI.UserControl
{
    partial class SweetBaseForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SweetBaseForm));
            this.SuspendLayout();
            // 
            // SweetBaseForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 255);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SweetBaseForm";
            this.Text = "SweetBaseForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SweetBaseForm_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SweetBaseForm_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SweetBaseForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SweetBaseForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}