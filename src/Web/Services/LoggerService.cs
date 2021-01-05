using Microsoft.Extensions.Logging;
using System;
using Web.Interfaces;

namespace Web.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly IUserHelper _userHelper;
        public LoggerService(ILogger<LoggerService> logger,
            IUserHelper userHelper)
        {
            _logger = logger;
            _userHelper = userHelper;
        }

        /// <summary>
        /// The level of critical errors that require immediate response
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogCritical(string path, string type, string info, string currentUserId)
        {
            _logger.LogCritical($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[critical:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");
        }

        /// <summary>
        /// To display information that can be useful during the development and debugging of the application
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogDebug(string path, string type, string info, string currentUserId)
        {
            _logger.LogDebug($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[debug:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");
        }

        /// <summary>
        /// Errors and exceptions which occurred during the current operation and which cannot be processed
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogError(string path, string type, string info, string currentUserId)
        {
            _logger.LogError($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[error:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");

        }

        /// <summary>
        /// Track application execution flow
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogInformation(string path, string type, string info, string currentUserId)
        {
            _logger.LogInformation($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[info:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");
        }

        /// <summary>
        /// To display the most detailed messages
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogTrace(string path, string type, string info, string currentUserId)
        {
            _logger.LogTrace($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[trace:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");
        }

        /// <summary>
        /// To display messages about unexpected events
        /// </summary>
        /// <param name="path">Action address</param>
        /// <param name="type">Action type</param>
        /// <param name="info">Log information</param>
        /// <param name="currentUserId">Current user id</param>
        public void LogWarning(string path, string type, string info, string currentUserId)
        {
            _logger.LogWarning($"[{DateTime.Now.ToString()}]:[{path}]:[type:{type}]:[warning:{info}]:[user:{_userHelper.GetIdUserById(currentUserId)}]");
        }
    }
}
