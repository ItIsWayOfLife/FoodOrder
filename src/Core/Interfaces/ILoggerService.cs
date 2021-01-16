
namespace Core.Interfaces
{
    public interface ILoggerService
    {
        /// <summary>
        /// To display information that can be useful during the development and debugging of the application
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogDebug(string path, string type, string info, string currentUserId);

        /// <summary>
        /// To display the most detailed messages
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogTrace(string path, string type, string info, string currentUserId);

        /// <summary>
        /// Track application execution flow
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogInformation(string path, string type, string info, string currentUserId);

        /// <summary>
        /// To display messages about unexpected events
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogWarning(string path, string type, string info, string currentUserId);

        /// <summary>
        /// Errors and exceptions which occurred during the current operation and which cannot be processed
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogError(string path, string type, string info, string currentUserId);

        /// <summary>
        /// The level of critical errors that require immediate response
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        void LogCritical(string path, string type, string info, string currentUserId);
    }
}
