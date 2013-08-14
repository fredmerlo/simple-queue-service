using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simple.Queue
{
    public class MessageProcessor
    {
        public void Run(Account account)
        {
            try
            {
                if (Repository.GetAccountDetails(account))
                {
                    var log = AccountsFactory
                        .Get(account.UID)
                        .GetCallLogFor(account.Telephone, account.Username, account.Password);

                    if (IsLogInvalid(log))
                    {
                        account.Retries++;
                        if (account.Retries < 3)
                        {
                            Messages.Enqueue(new Message { Payload = account });
                        }
                    }
                    else
                    {
                        Repository.UpdateCallLog(account, log);
                    }
                }
            }
            catch (Exception e)
            {
                var source = "Simple Queue Service";

                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, "Application");

                EventLog.WriteEntry(source, e.Message + "\r\n" + e.StackTrace, EventLogEntryType.Error, 200);
            }
        }

        private bool IsLogInvalid(List<LogEntry> log)
        {
            return log.Count == 1 && log[0].Number == null && log[0].Duration == 0;
        }
    }
}
