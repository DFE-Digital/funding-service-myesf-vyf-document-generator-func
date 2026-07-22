using Newtonsoft.Json;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// The CalculationAndFundingLinesCollectionHelper class.
    /// </summary>
    public static class CalculationAndFundingLinesCollectionHelper
    {
        /// <summary>
        /// Gets the funding value data validated schema version.
        /// </summary>
        /// <param name="fundingValue">The funding value.</param>
        /// <param name="inputSchema">The input schema.</param>
        /// <returns>The validated schemaVersion.</returns>
        public static CalculationAndFundingLinesCollection GetSchemaBasedCalculationAndFundingLinesCollection(this string fundingValue, string inputSchema)
        {
            var returnItem = new CalculationAndFundingLinesCollection
            {
                Calculations = new Dictionary<int, CalculationNoNesting>(),
                FundingLines = new Dictionary<int, FundingLineNoNesting>()
            };

            double.TryParse(inputSchema, out var schemaVersion);
            switch (schemaVersion)
            {
                case 1.1:
                    try
                    {
                        var resultV11 = JsonConvert.DeserializeObject<FundingValueNested11>(fundingValue);

                        return GetVersion11CalculationAndFundingLinesCollection(resultV11, returnItem);
                    }
                    catch
                    {
                        var resultV12 = JsonConvert.DeserializeObject<FundingValueNested12>(fundingValue);

                        return GetVersion12CalculationAndFundingLinesCollection(resultV12, returnItem);
                    }

                case 1.2:
                    try
                    {
                        var resultV12 = JsonConvert.DeserializeObject<FundingValueNested12>(fundingValue);

                        return GetVersion12CalculationAndFundingLinesCollection(resultV12, returnItem);
                    }
                    catch
                    {
                        var resultV11 = JsonConvert.DeserializeObject<FundingValueNested11>(fundingValue);

                        return GetVersion11CalculationAndFundingLinesCollection(resultV11, returnItem);
                    }

                case 1.0:
                    try
                    {
                        var resultV10 = JsonConvert.DeserializeObject<FundingValueNested10>(fundingValue);

                        return GetV10CalculationAndFundingLinesCollection(resultV10, returnItem);
                    }
                    catch
                    {
                        var resultV11 = JsonConvert.DeserializeObject<FundingValueNested11>(fundingValue);

                        return GetVersion11CalculationAndFundingLinesCollection(resultV11, returnItem);
                    }

                default:
                    return returnItem;
            }
        }

        private static CalculationAndFundingLinesCollection GetV10CalculationAndFundingLinesCollection(
            FundingValueNested10 resultV10, CalculationAndFundingLinesCollection returnItem)
        {
            if (resultV10.FundingLines == null)
            {
                return returnItem;
            }

            resultV10.FundingLines.ToList().ForEach(fundingLine =>
                UpsertFundingLineIfNotAdded(fundingLine, returnItem.FundingLines, returnItem.Calculations));

            resultV10.Calculations?.ToList().ForEach(calculation =>
                UpsertCalculationIfNotAdded(calculation, returnItem.Calculations));

            return returnItem;
        }

        private static CalculationAndFundingLinesCollection GetVersion12CalculationAndFundingLinesCollection(
            FundingValueNested12 resultV12, CalculationAndFundingLinesCollection returnItem)
        {
            if (resultV12.Calculations != null)
            {
                foreach (var calculation in resultV12.Calculations)
                {
                    if (!returnItem.Calculations.ContainsKey(calculation.TemplateCalculationId))
                    {
                        returnItem.Calculations.Add(calculation.TemplateCalculationId, calculation);
                    }
                }
            }

            if (resultV12.FundingLines != null)
            {
                foreach (var fundingLine in resultV12.FundingLines)
                {
                    if (!returnItem.FundingLines.ContainsKey(fundingLine.TemplateLineId))
                    {
                        returnItem.FundingLines.Add(fundingLine.TemplateLineId, fundingLine);
                    }
                }
            }

            return returnItem;
        }

        private static CalculationAndFundingLinesCollection GetVersion11CalculationAndFundingLinesCollection(
            FundingValueNested11 result, CalculationAndFundingLinesCollection returnItem)
        {
            if (result.Calculations != null)
            {
                foreach (var (_, value) in result.Calculations)
                {
                    returnItem.Calculations.Add(value.TemplateCalculationId, value);
                }
            }

            if (result.FundingLines != null)
            {
                foreach (var (_, value) in result.FundingLines)
                {
                    returnItem.FundingLines.Add(value.TemplateLineId, value);
                }
            }

            return returnItem;
        }

        private static void UpsertFundingLineIfNotAdded(
            FundingLine line,
            IDictionary<int, FundingLineNoNesting> lineDictionary,
            IDictionary<int, CalculationNoNesting> calculationDictionary)
        {
            if (line.Calculations != null)
            {
                foreach (var subCalc in line.Calculations)
                {
                    UpsertCalculationIfNotAdded(subCalc, calculationDictionary);
                }
            }

            if (line.FundingLines != null)
            {
                foreach (var subLine in line.FundingLines)
                {
                    UpsertFundingLineIfNotAdded(subLine, lineDictionary, calculationDictionary);
                }
            }

            if (!lineDictionary.ContainsKey(line.TemplateLineId))
            {
                lineDictionary.Add(line.TemplateLineId, line);
                return;
            }

            if (line.Value == null)
            {
                return;
            }

            var existingLine = lineDictionary[line.TemplateLineId];

            if (existingLine.Value != null)
            {
                return;
            }

            existingLine.Value = line.Value;
        }

        private static void UpsertCalculationIfNotAdded(Calculation calculation, IDictionary<int, CalculationNoNesting> dictionary)
        {
            if (calculation.Calculations != null)
            {
                foreach (var subCalc in calculation.Calculations)
                {
                    UpsertCalculationIfNotAdded(subCalc, dictionary);
                }
            }

            if (!dictionary.ContainsKey(calculation.TemplateCalculationId))
            {
                dictionary.Add(calculation.TemplateCalculationId, calculation);
                return;
            }

            if (calculation.Value == null)
            {
                return;
            }

            var existingCalculation = dictionary[calculation.TemplateCalculationId];

            if (existingCalculation.Value != null)
            {
                return;
            }

            existingCalculation.Value = calculation.Value;
        }
    }
}