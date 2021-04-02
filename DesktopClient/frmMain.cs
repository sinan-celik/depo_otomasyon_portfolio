using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.Tag = "frmMain";

        }
        public bool IsFormOpened(string formName)
        {
            FormCollection fc = Application.OpenForms;

            bool isOpen = false;

            foreach (Form frm in fc)
            {
                //iterate through
                if (frm.Tag.ToString() == formName)
                {
                    isOpen = true;
                }
            }
            return isOpen;
        }
        private void btnOpenFrmOrders_Click(object sender, EventArgs e)
        {

            if (!IsFormOpened("frmOrders"))
            {
                frmOrders frm = new frmOrders();
                frm.Tag = "frmOrders";
                frm.Name = "frmOrders";
                frm.MdiParent = this;
                frm.Show();
            }
            else
            {
                Form form = Application.OpenForms["frmOrders"];
                form.Activate();
            }
        }




        private void btnOpenFrmMaintenance_Click(object sender, EventArgs e)
        {

            if (!IsFormOpened("frmMaintenance"))
            {
                frmMaintenance frm = new frmMaintenance();
                frm.Tag = "frmMaintenance";
                frm.Name = "frmMaintenance";
                frm.MdiParent = this;
                frm.Show();
            }
            else
            {
                Form form = Application.OpenForms["frmMaintenance"];
                form.Activate();
            }
        }

        private void frmMain_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
            {

                //tabForms.Visible = false;
            }
            // If no any child form, hide tabControl 
            else
            {
                this.ActiveMdiChild.WindowState = FormWindowState.Maximized;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnUser_Click(object sender, EventArgs e)
        {

        }
    }
}
