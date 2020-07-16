namespace Xpandables.Samples.Desktop.Views
{
    partial class LoginForm
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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.LoginFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.LoginFormErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.Token = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.LoginFormBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoginFormErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(219, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "label3";
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(242, 84);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(211, 23);
            this.Email.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "label4";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(242, 136);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(211, 23);
            this.Password.TabIndex = 4;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(50, 183);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(173, 37);
            this.BtnCancel.TabIndex = 5;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(260, 183);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(193, 38);
            this.BtnConnect.TabIndex = 6;
            this.BtnConnect.Text = "Connect";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // LoginFormErrorProvider
            // 
            this.LoginFormErrorProvider.ContainerControl = this;
            // 
            // Token
            // 
            this.Token.Location = new System.Drawing.Point(12, 227);
            this.Token.Name = "Token";
            this.Token.Size = new System.Drawing.Size(493, 45);
            this.Token.TabIndex = 7;
            this.Token.Text = "label1";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.BtnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(517, 281);
            this.Controls.Add(this.Token);
            this.Controls.Add(this.BtnConnect);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Email);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xpandables Login";
            ((System.ComponentModel.ISupportInitialize)(this.LoginFormBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoginFormErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.BindingSource LoginFormBindingSource;
        private System.Windows.Forms.ErrorProvider LoginFormErrorProvider;
        private System.Windows.Forms.Label Token;
    }
}