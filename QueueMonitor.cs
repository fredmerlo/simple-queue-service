using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Simple.Queue;

namespace Simple.Queue
{
    public class QueueMonitor
    {
        private static readonly int SLEEP_TIMEOUT = 30000;
        private RunState runstate = RunState.Stoped;

        public MessageProcessorDelegate MessageProcessor = null;
        private ManualResetEvent PauseTrigger = new ManualResetEvent(false);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendMessage(Message message)
        {
            Messages.Enqueue(message);			
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if(runstate != RunState.Started)
            {
                if(runstate == RunState.Paused)
                {
                    runstate = RunState.Started;
                    PauseTrigger.Set();
                }
                else if(runstate == RunState.Stoped)
                {
                    runstate = RunState.Started;

                    var ts = new ThreadStart(MainLoop);
                    var t = new Thread(ts);
                    t.Start();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if(runstate != RunState.Stoped)
            {
                Messages.Enqueue(new Message{State = RunState.Stoped});
                runstate = RunState.Stoped;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Pause()
        {
            if(runstate != RunState.Paused)
            {
                runstate = RunState.Paused;
            }
        }

        private void MainLoop()
        {
            try
            {
                for (; ; )
                {
                    if (runstate == RunState.Paused)
                    {
                        PauseTrigger.WaitOne(-1, false);
                        PauseTrigger.Reset();
                    }

                    if (Messages.HasMessages)
                    {
                        var msg = Messages.Dequeue();

                        if (msg != null)
                        {
                            if (msg.Payload != null)
                            {
                                MessageProcessor(msg.Payload);

                                if (msg.State == RunState.Stoped)
                                {
                                    return;
                                }
                            }
                            else if (msg.State == RunState.Stoped)
                            {
                                return;
                            }
                        }
                    }
                    PauseTrigger.WaitOne(SLEEP_TIMEOUT, false);
                }
            }
            catch (Exception e)
            {
                var source = "Simple Queue Service";

                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, "Application");

                EventLog.WriteEntry(source, e.Message + "\r\n" + e.StackTrace, EventLogEntryType.Error, 100);
            }
        }
    }
}