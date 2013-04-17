
namespace Log
{
    /// <summary>
    /// Writes log events to a database.
    /// </summary>
    /// <remarks>
    /// GoF Design Pattern: Observer.
    /// The Observer Design Pattern allows this class to attach itself to an
    /// the logger and 'listen' to certain events and be notified of the event. 
    /// </remarks>
    public class ObserverLogToDatabase : ILog
    {
        /// <summary>
        /// Writes a log request to the database.
        /// </summary>
        /// <remarks>
        /// Actual database insert statements are commented out.
        /// You can activate this if you have the proper database 
        /// configuration and access privileges.
        /// </remarks>
        /// <param name="sender">Sender of the log request.</param>
        /// <param name="e">Parameters of the log request.</param>
        public void Log(object sender, LogEventArgs e)
        {
            // Example code of entering a log event to database
            string message = "[" + e.Date.ToString() + "] " +
               e.SeverityString + ": " + e.Message;

            // Something like
            string sql = "INSERT INTO LOG (message) VALUES('" + message + "')";

            // Commented out for now. You need database to store log values. 
            //Db.Update(sql);
        }
    }
}
