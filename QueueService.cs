using System.Net;
using System.ServiceProcess;

namespace Simple.Queue
{
    partial class QueueService : ServiceBase
    {
        private QueueMonitor queueMonitor= new QueueMonitor();
        private MessageProcessor messageProcessor = new MessageProcessor();

        public QueueService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            queueMonitor.MessageProcessor = messageProcessor.Run;
            queueMonitor.Start();
        }

        protected override void OnStop()
        {
            queueMonitor.Stop();
            queueMonitor.MessageProcessor = null;
        }

        protected override void OnPause()
        {
            queueMonitor.Pause();
        }
    }
}
