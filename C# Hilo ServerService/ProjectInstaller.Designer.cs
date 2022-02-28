namespace WMPA06
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
            this.HiLoServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.HiLoServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // HiLoServerServiceProcessInstaller
            // 
            this.HiLoServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.HiLoServerServiceProcessInstaller.Password = null;
            this.HiLoServerServiceProcessInstaller.Username = null;
            // 
            // HiLoServerServiceInstaller
            // 
            this.HiLoServerServiceInstaller.ServiceName = "HiLoServerService";
            this.HiLoServerServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HiLoServerServiceProcessInstaller,
            this.HiLoServerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HiLoServerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller HiLoServerServiceInstaller;
    }
}