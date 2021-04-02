namespace DesktopClient
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.btnOpenFrmMaintenance = new System.Windows.Forms.Button();
            this.btnOpenFrmOrders = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnUser = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.btnOpenFrmMaintenance);
            this.panel1.Controls.Add(this.btnOpenFrmOrders);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(147, 577);
            this.panel1.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Top;
            this.button3.Location = new System.Drawing.Point(0, 128);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(147, 64);
            this.button3.TabIndex = 0;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // btnOpenFrmMaintenance
            // 
            this.btnOpenFrmMaintenance.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpenFrmMaintenance.Location = new System.Drawing.Point(0, 64);
            this.btnOpenFrmMaintenance.Name = "btnOpenFrmMaintenance";
            this.btnOpenFrmMaintenance.Size = new System.Drawing.Size(147, 64);
            this.btnOpenFrmMaintenance.TabIndex = 0;
            this.btnOpenFrmMaintenance.Text = "Bakım";
            this.btnOpenFrmMaintenance.UseVisualStyleBackColor = true;
            this.btnOpenFrmMaintenance.Click += new System.EventHandler(this.btnOpenFrmMaintenance_Click);
            // 
            // btnOpenFrmOrders
            // 
            this.btnOpenFrmOrders.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpenFrmOrders.Location = new System.Drawing.Point(0, 0);
            this.btnOpenFrmOrders.Name = "btnOpenFrmOrders";
            this.btnOpenFrmOrders.Size = new System.Drawing.Size(147, 64);
            this.btnOpenFrmOrders.TabIndex = 0;
            this.btnOpenFrmOrders.Text = "Siparişler";
            this.btnOpenFrmOrders.UseVisualStyleBackColor = true;
            this.btnOpenFrmOrders.Click += new System.EventHandler(this.btnOpenFrmOrders_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnUser);
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(147, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(935, 122);
            this.pnlHeader.TabIndex = 3;
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnExit.Location = new System.Drawing.Point(860, 0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 122);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinimize.Location = new System.Drawing.Point(785, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(75, 122);
            this.btnMinimize.TabIndex = 0;
            this.btnMinimize.Text = "Minimize";
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnUser
            // 
            this.btnUser.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnUser.Location = new System.Drawing.Point(710, 0);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(75, 122);
            this.btnUser.TabIndex = 0;
            this.btnUser.Text = "User: ";
            this.btnUser.UseVisualStyleBackColor = true;
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 577);
            this.ControlBox = false;
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.panel1);
            this.IsMdiContainer = true;
            this.Name = "frmMain";
            this.Text = "Robotaş Depo Yönetimi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MdiChildActivate += new System.EventHandler(this.frmMain_MdiChildActivate);
            this.panel1.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnOpenFrmMaintenance;
        private System.Windows.Forms.Button btnOpenFrmOrders;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnUser;
        private System.Windows.Forms.Button btnMinimize;
    }
}

