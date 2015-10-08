using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PPlayer
{
    public partial class Form_About : XtraForm
    {
        public Form_About(string AboutText, string Version)
        {
            InitializeComponent();
            lbAbout.Text = AboutText;
            if (Version != "") lbVersion.Text = "Ver " + Version;
            else lbVersion.Text = "";
        }

        public Form_About(string AboutText)
        {
            InitializeComponent();
            lbAbout.Text = AboutText;
            lbVersion.Text = "";
        }

        private void AboutForm_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkMailTo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:chernikov.a.s@yandex.ru");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
                return;
            }
        }

        private void linkMailTo_MouseMove(object sender, MouseEventArgs e)
        {
            linkMailTo.LinkColor = Color.SteelBlue;            
        }

        private void linkMailTo_MouseLeave(object sender, EventArgs e)
        {
            linkMailTo.LinkColor = Color.SkyBlue;
            toolTip.Hide(linkMailTo);
        }

        private void linkMailTo_MouseHover(object sender, EventArgs e)
        {
            toolTip.Show(linkMailTo.Tag.ToString(), linkMailTo);
        }        
    }    
}