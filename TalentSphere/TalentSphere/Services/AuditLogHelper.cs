using System.Security.Claims;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    /// <summary>
    /// Helper service to simplify audit logging.
    /// Always accepts UserID explicitly - never depends on token extraction.
    /// Usage: await _auditLogHelper.LogActionAsync(userId, "Create", "User", "User registered successfully");
    /// </summary>
    public class AuditLogHelper
    {
        private readonly IAuditLogRepository _repository;
        private readonly ILogger<AuditLogHelper> _logger;

        public AuditLogHelper(IAuditLogRepository repository, ILogger<AuditLogHelper> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Logs an action to the audit log table.
        /// UserID is always passed explicitly - no token extraction needed.
        /// </summary>
        /// <param name="userId">The UserID performing the action</param>
        /// <param name="action">Action performed (e.g., "Create", "Update", "Delete", "Login", "Register")</param>
        /// <param name="resource">Resource affected (e.g., "User", "Employee", "Compliance")</param>
        /// <param name="message">Detailed message (e.g., "User registered with email john@example.com")</param>
        public async Task LogActionAsync(int userId, string action, string resource, string message)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserID = userId,
                    Action = action,
                    Resource = resource,
                    Timestamp = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(auditLog);

                _logger.LogInformation($"Audit Log: User {userId} performed {action} on {resource}. Details: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging audit action: {ex.Message}");
                // Don't throw - audit logging failure shouldn't break the main operation
            }
        }

        /// <summary>
        /// Logs an action without message details.
        /// </summary>
        public async Task LogActionAsync(int userId, string action, string resource)
        {
            await LogActionAsync(userId, action, resource, string.Empty);
        }

        /// <summary>
        /// Logs multiple related actions in sequence (useful for batch operations)
        /// </summary>
        public async Task LogBatchAsync(int userId, string resource, params (string action, string message)[] actions)
        {
            foreach (var (action, message) in actions)
            {
                await LogActionAsync(userId, action, resource, message);
            }
        }

        /// <summary>
        /// Extract UserID from HttpContext token claims (optional helper for protected endpoints)
        /// Returns null if token is not present or invalid
        /// </summary>
        public int? ExtractUserIdFromContext(HttpContext context)
        {
            try
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error extracting UserID from context: {ex.Message}");
            }

            return null;
        }
    }
}
