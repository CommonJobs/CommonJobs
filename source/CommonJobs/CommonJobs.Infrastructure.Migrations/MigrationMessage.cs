using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Migrations
{
    public class MigrationMessage
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public MigrationActionType Action { get; set; }
        public Exception Exception { get; set; }

        public MigrationMessage()
        {
            Date = DateTime.Now;
        }

        public MigrationMessage(MigrationActionType action, string message)
            : this()
        {
            Action = action;
            Message = message;
        }

        public MigrationMessage(MigrationActionType action, Exception exception)
            : this(action, CreateMessage(exception))
        {
            Exception = exception;
        }

        private static string CreateMessage(Exception exception)
        {
            return string.Format("Exception {0}\nType: {1}\nStackTrace: {2}", exception.GetType(), exception.Message, exception.StackTrace);
        }
    }
}
