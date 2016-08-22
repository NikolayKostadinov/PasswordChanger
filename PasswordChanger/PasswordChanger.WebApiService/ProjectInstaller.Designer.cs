namespace PasswordChanger.WebApiService
{
    partial class ProjectInstaller
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
            this.PasswordChanger = new System.ServiceProcess.ServiceProcessInstaller();
            this.PasswordChangerService = new System.ServiceProcess.ServiceInstaller();
            // 
            // PasswordChanger
            // 
            this.PasswordChanger.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.PasswordChanger.Password = null;
            this.PasswordChanger.Username = null;
            // 
            // PasswordChangerService
            // 
            this.PasswordChangerService.DelayedAutoStart = true;
            this.PasswordChangerService.Description = "Change users password WebApi Service";
            this.PasswordChangerService.DisplayName = "PasswordChanger";
            this.PasswordChangerService.ServiceName = "PasswordChanger";
            this.PasswordChangerService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PasswordChanger,
            this.PasswordChangerService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PasswordChanger;
        private System.ServiceProcess.ServiceInstaller PasswordChangerService;
    }
}