using Newtonsoft.Json.Linq;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Get a layout from non-relational db.
    /// </summary>
    public class NonRelationalDbLayoutService : ILayoutService
    {
        private readonly ISettingService _settingService;
        private readonly INonRelationalDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRelationalDbLayoutService"/> class.
        /// </summary>
        /// <param name="settingService">The setting service to use.</param>
        /// <param name="db">The db to use.</param>
        public NonRelationalDbLayoutService(ISettingService settingService, INonRelationalDb db)
        {
            _settingService = settingService;
            _db = db;
        }

        /// <inheritdoc/>
        public (string layoutKey, List<string> layoutIds) LookupLayoutId(string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string providerType, string providerSubType)
        {
            if (string.IsNullOrEmpty(fundingStreamCode) || string.IsNullOrEmpty(providerType) || string.IsNullOrEmpty(providerSubType) || string.IsNullOrEmpty(fundingPeriodCode) || string.IsNullOrEmpty(cutoffDate))
            {
                throw new Exception("A required parameter is missing. " +
                    $"fundingStreamCode: {fundingStreamCode}, providerType: {providerType}, providerSubType: {providerSubType}, fundingPeriodCode: {fundingPeriodCode}, cutoffDate: {cutoffDate}");
            }

            var providerTypeReplaced = Regex.Replace(providerType, @"\s", string.Empty);
            var providerSubTypeReplaced = Regex.Replace(providerSubType, @"\s", string.Empty);

            var idLookupKey = $"LayoutID_{fundingStreamCode}_{providerTypeReplaced}_{providerSubTypeReplaced}_{fundingPeriodCode}";
            var layoutId = _settingService.GetSetting(idLookupKey);

            if (string.IsNullOrEmpty(layoutId))
            {
                var originalIdLookupKey = idLookupKey;
                idLookupKey = $"LayoutID_{fundingStreamCode}_General_General_{fundingPeriodCode}";
                layoutId = _settingService.GetSetting(idLookupKey);

                if (string.IsNullOrEmpty(layoutId))
                {
                    throw new Exception($"Cannot find id for key in '{originalIdLookupKey}', '{idLookupKey}'");
                }
            }

            var layoutIds = layoutId.Split(',').ToList();

            return (idLookupKey, layoutIds);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, object>> GetLayout(string layoutId)
        {
            var jsonDocument = await _db.GetDocumentById(layoutId);
            return ((JObject)jsonDocument["Data"]).ToObject<Dictionary<string, object>>();
        }

        /// <inheritdoc/>
        public string LookupFileNameFormat(string layoutId, int index)
        {
            if (string.IsNullOrEmpty(layoutId))
            {
                throw new Exception("A required parameter is missing. layoutId: {layoutId}");
            }

            var idLookupKey = $"FileName_{layoutId}";
            var filenameFormat = _settingService.GetSetting(idLookupKey);

            if (string.IsNullOrEmpty(filenameFormat))
            {
                throw new Exception($"Cannot find id for key in '{filenameFormat}'");
            }

            var filenameFormats = filenameFormat.Split(',').ToList();

            return filenameFormats[index];
        }

        /// <inheritdoc/>
        public string LookupFileType(string layoutId)
        {
            if (string.IsNullOrEmpty(layoutId))
            {
                throw new Exception("A required parameter is missing. layoutId: {layoutId}");
            }

            var idLookupKey = $"FileType_{layoutId}";
            var filetype = _settingService.GetSetting(idLookupKey);

            if (string.IsNullOrEmpty(filetype))
            {
                return Constants.DocumentConstants.Pdf;
            }

            return filetype;
        }
    }
}