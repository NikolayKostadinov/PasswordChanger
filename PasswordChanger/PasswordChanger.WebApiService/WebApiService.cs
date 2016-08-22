using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordChanger.WebApiService
{
    public partial class WebApiService : ServiceBase
    {
        private readonly ILog logger;

        public WebApiService(ILog loggerParam)
        {
            InitializeComponent();
            this.logger = loggerParam;
        }

        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(new ThreadStart(WebApiHostMain.App_Start));
            t.Start();
            logger.Info("WebApi Service started");
            WebApiHostMain.StopWebApiServer = false;
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            // JUST test
            WebApiHostMain.StopWebApiServer = true;
            logger.Info("WebApi Service stopped");
        }
    }
}
