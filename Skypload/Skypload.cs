using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Skypload
{
    class Skypload : Form
    {
        private const string TITLE = "Skypload";
        private NotifyIcon m_NotifyIcon;
        private ContextMenuStrip m_Menu;

        public Skypload()
        {
            this.m_NotifyIcon = new NotifyIcon();
            this.m_NotifyIcon.Text = TITLE;
            this.m_NotifyIcon.Icon = Icon.FromHandle(Properties.Resources.csgo.GetHicon());

            this.LoadContextMenu();

            this.m_NotifyIcon.ContextMenuStrip = m_Menu;

            this.m_NotifyIcon.Visible = true;
            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        private void LoadContextMenu()
        {
            this.m_Menu = new ContextMenuStrip();

            this.m_Menu.Items.Add("Uploader un fichier", Properties.Resources.upload, UploadClick);
            this.m_Menu.Items.Add(new ToolStripSeparator());
            this.m_Menu.Items.Add("Historique", Properties.Resources.history, HistoryClick);
            this.m_Menu.Items.Add(new ToolStripSeparator());
            this.m_Menu.Items.Add("Quitter", Properties.Resources.shutdown, QuitClick);
        }

        private void HistoryClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void QuitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UploadClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result = ofd.ShowDialog();
            if(result.Equals(DialogResult.OK))
            {
                string path = ofd.FileName;

                string url = new Uploader().Upload(path);

                this.m_NotifyIcon.BalloonTipTitle = "Envoyé !";
                this.m_NotifyIcon.BalloonTipText = url;
                this.m_NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                this.m_NotifyIcon.BalloonTipClicked += new EventHandler(UploadBalloonClick);
                this.m_NotifyIcon.ShowBalloonTip(3000);
            }
        }

        private void UploadBalloonClick(object sender, EventArgs e)
        {
            Process.Start(this.m_NotifyIcon.BalloonTipText);
        }
    }
}
