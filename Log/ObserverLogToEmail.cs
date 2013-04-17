using System.Net.Mail;

namespace Log
{
    /// <summary>
    /// Sends log events via email.
    /// </summary>
    /// <remarks>
    /// GoF Design Pattern: Observer.
    /// The Observer Design Pattern allows this class to attach itself to an
    /// the logger and 'listen' to certain events and be notified of the event. 
    /// </remarks>
    public class ObserverLogToEmail : ILog
    {
        private string _from;
        private string _to;
        private string _subject;
        private string _body;
        private SmtpClient _smtpClient;

        /// <summary>
        /// Constructor for the ObserverlogToEmail class
        /// </summary>
        /// <param name="from">From email address.</param>
        /// <param name="to">To email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="smtpClient">Smtp email client.</param>
        public ObserverLogToEmail(string from, string to, string subject, string body, SmtpClient smtpClient)
		{
            _from = from;
            _to = to;
            _subject = subject;
            _body = body;
            _smtpClient = smtpClient;
		}

        #region ILog Members

        /// <summary>
        /// Sends a log request via email.
        /// </summary>
        /// <remarks>
        /// Actual email 'Send' calls are commented out.
        /// Uncomment if you have the proper email privileges.
        /// </remarks>
        /// <param name="sender">Sender of the log request.</param>
        /// <param name="e">Parameters of the log request.</param>
        public void Log(object sender, LogEventArgs e)
        {
            string message = "[" + e.Date.ToString() + "] " +
               e.SeverityString + ": " + e.Message;

            // Commented out for now. You need privileges to send email.
            // _smtpClient.Send(_from, _to, _subject, body);
        }

        #endregion
    }
}
