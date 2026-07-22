using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Get a layout from the non-relational db.
    /// </summary>
    public class NonRelationalDbAuditLogService : IAuditLogService
    {
        private readonly INonRelationalDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRelationalDbAuditLogService"/> class.
        /// </summary>
        /// <param name="settingService">The setting service to use.</param>
        /// <param name="db">The db to use.</param>
        public NonRelationalDbAuditLogService(INonRelationalDb db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task Log(string message)
        {
            var document = new Dictionary<string, string>
            {
                { "id", Guid.NewGuid().ToString() },
                { "message", message }
            };

            await _db.SaveDocument(document);
        }

        /// <inheritdoc/>
        public async Task<bool> CheckNoRunningInstanceOfFeedReader()
        {
            var lastSuccessfulRunTime = await GetLastSuccessfulRunTime();

            if (lastSuccessfulRunTime != null)
            {
                var result = await _db.GetDocumentsForSqlQuery<string>(
                    "SELECT value count(1) FROM c " +
                    "WHERE c.status =  'Started' " +
                    "AND c.action ='Import' " +
                    $"AND c.startDateTime > '{lastSuccessfulRunTime}'");

                return result.Any() ? int.Parse(result.First()) == 0 : false;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<string> GetLastSuccessfulRunTime()
        {
            var result = await _db.GetDocumentsForSqlQuery<string>("SELECT value c.endDateTime FROM c where c.status =  'Successful' and c.action ='Import' order by c.endDateTime desc OFFSET 0 LIMIT 1");

            return result.Any() ? result.First() : null;
        }
    }
}