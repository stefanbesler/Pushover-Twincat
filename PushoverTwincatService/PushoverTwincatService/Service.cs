using System.ServiceProcess;

namespace Pushover.Twincat
{
    class Service : ServiceBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void OnStart(string[] args)
        {
            var server = new Server(25733, "Twinpack Pushover Server", null);
            server.ConnectServerAndWaitAsync(System.Threading.CancellationToken.None);
        }

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
    }
}
