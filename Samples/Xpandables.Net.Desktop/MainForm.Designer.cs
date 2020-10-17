namespace Xpandables.Samples.Desktop
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Token = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnPictureUpdate = new System.Windows.Forms.Button();
            this.BtnPictureDelete = new System.Windows.Forms.Button();
            this.BtnPictureSave = new System.Windows.Forms.Button();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.PictureInfo = new System.Windows.Forms.Label();
            this.PictureOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.GeoLocation = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Gender = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.FirstName = new System.Windows.Forms.TextBox();
            this.LastName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnUserSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Token);
            this.groupBox1.Location = new System.Drawing.Point(12, 338);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Token";
            // 
            // Token
            // 
            this.Token.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Token.Location = new System.Drawing.Point(3, 19);
            this.Token.Name = "Token";
            this.Token.Size = new System.Drawing.Size(770, 78);
            this.Token.TabIndex = 0;
            this.Token.Text = "token";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BtnPictureUpdate);
            this.groupBox2.Controls.Add(this.BtnPictureDelete);
            this.groupBox2.Controls.Add(this.BtnPictureSave);
            this.groupBox2.Controls.Add(this.Picture);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(462, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(326, 320);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Picture";
            // 
            // BtnPictureUpdate
            // 
            this.BtnPictureUpdate.Location = new System.Drawing.Point(125, 214);
            this.BtnPictureUpdate.Name = "BtnPictureUpdate";
            this.BtnPictureUpdate.Size = new System.Drawing.Size(91, 23);
            this.BtnPictureUpdate.TabIndex = 4;
            this.BtnPictureUpdate.Text = "Update...";
            this.BtnPictureUpdate.UseVisualStyleBackColor = true;
            this.BtnPictureUpdate.Click += new System.EventHandler(this.BtnPictureUpdate_Click);
            // 
            // BtnPictureDelete
            // 
            this.BtnPictureDelete.Location = new System.Drawing.Point(12, 215);
            this.BtnPictureDelete.Name = "BtnPictureDelete";
            this.BtnPictureDelete.Size = new System.Drawing.Size(100, 23);
            this.BtnPictureDelete.TabIndex = 3;
            this.BtnPictureDelete.Text = "Delete";
            this.BtnPictureDelete.UseVisualStyleBackColor = true;
            // 
            // BtnPictureSave
            // 
            this.BtnPictureSave.Location = new System.Drawing.Point(231, 215);
            this.BtnPictureSave.Name = "BtnPictureSave";
            this.BtnPictureSave.Size = new System.Drawing.Size(89, 23);
            this.BtnPictureSave.TabIndex = 2;
            this.BtnPictureSave.Text = "Save";
            this.BtnPictureSave.UseVisualStyleBackColor = true;
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(12, 22);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(302, 186);
            this.Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Picture.TabIndex = 1;
            this.Picture.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.PictureInfo);
            this.groupBox3.Location = new System.Drawing.Point(6, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 70);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Information";
            // 
            // PictureInfo
            // 
            this.PictureInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureInfo.BackColor = System.Drawing.SystemColors.Control;
            this.PictureInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PictureInfo.Location = new System.Drawing.Point(6, 19);
            this.PictureInfo.Name = "PictureInfo";
            this.PictureInfo.Size = new System.Drawing.Size(302, 48);
            this.PictureInfo.TabIndex = 0;
            this.PictureInfo.Text = "information";
            // 
            // PictureOpenFileDialog
            // 
            this.PictureOpenFileDialog.Filter = "Images (*.png) |*.png | Images (*.jpeg)|*.jpeg|Images (*.jpg)|*.jpg";
            this.PictureOpenFileDialog.FilterIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.GeoLocation);
            this.groupBox4.Location = new System.Drawing.Point(12, 232);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(444, 100);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Location";
            // 
            // GeoLocation
            // 
            this.GeoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeoLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeoLocation.Location = new System.Drawing.Point(6, 19);
            this.GeoLocation.Name = "GeoLocation";
            this.GeoLocation.Size = new System.Drawing.Size(432, 72);
            this.GeoLocation.TabIndex = 0;
            this.GeoLocation.Text = "location";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Gender);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.Email);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.FirstName);
            this.groupBox5.Controls.Add(this.LastName);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.BtnUserSave);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(438, 208);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "User";
            // 
            // Gender
            // 
            this.Gender.AutoSize = true;
            this.Gender.Location = new System.Drawing.Point(129, 152);
            this.Gender.Name = "Gender";
            this.Gender.Size = new System.Drawing.Size(38, 15);
            this.Gender.TabIndex = 9;
            this.Gender.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Gender :";
            // 
            // Email
            // 
            this.Email.AutoSize = true;
            this.Email.Location = new System.Drawing.Point(129, 124);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(38, 15);
            this.Email.TabIndex = 7;
            this.Email.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mail :";
            // 
            // FirstName
            // 
            this.FirstName.Location = new System.Drawing.Point(129, 78);
            this.FirstName.Name = "FirstName";
            this.FirstName.Size = new System.Drawing.Size(303, 23);
            this.FirstName.TabIndex = 5;
            // 
            // LastName
            // 
            this.LastName.Location = new System.Drawing.Point(129, 39);
            this.LastName.Name = "LastName";
            this.LastName.Size = new System.Drawing.Size(303, 23);
            this.LastName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "First Name :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Last Name :";
            // 
            // BtnUserSave
            // 
            this.BtnUserSave.Location = new System.Drawing.Point(331, 179);
            this.BtnUserSave.Name = "BtnUserSave";
            this.BtnUserSave.Size = new System.Drawing.Size(101, 23);
            this.BtnUserSave.TabIndex = 0;
            this.BtnUserSave.Text = "Save";
            this.BtnUserSave.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xpandables.Desktop";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Token;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label PictureInfo;
        private System.Windows.Forms.Button BtnPictureUpdate;
        private System.Windows.Forms.Button BtnPictureDelete;
        private System.Windows.Forms.Button BtnPictureSave;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.OpenFileDialog PictureOpenFileDialog;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label GeoLocation;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button BtnUserSave;
        private System.Windows.Forms.Label Email;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox FirstName;
        private System.Windows.Forms.TextBox LastName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Gender;
        private System.Windows.Forms.Label label4;
    }
}

