using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// Tests for string extension class.
    /// </summary>
    [TestClass]
    public class StringExtensionsTests
    {
        /// <summary>
        /// DecorateWithSingleQuotes test.
        /// </summary>
        /// <param name="input">The value passed.</param>
        /// <param name="expected">The result expected.</param>
        [TestMethod, TestCategory("Unit")]
        [DataRow("Single", "'Single'")]
        [DataRow("One,Two", "'One','Two'")]
        [DataRow("One,Two,Three", "'One','Two','Three'")]
        [DataRow("", "")]
        public void DecorateWithSingleQuotes(string input, string expected)
        {
            // Act
            var result = input.DecorateWithSingleQuotes();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// FindDifferences test.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void FindDifferences_DifferentData()
        {
            // Arrange
            var originalString = "This is the         original         string for comparison.";
            var comparedToString = "This is the         compared to         string with different data.";

            var expected = Tuple.Create("original|for|comparison.", "compared|to|with|different|data.");

            // Act
            var result = originalString.FindDifferences(comparedToString);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// FindDifferences test.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void FindDifferences_SameData()
        {
            // Arrange
            var originalString = "This is the original     string for comparison.";
            var comparedToString = "This is the original     string for comparison.";

            var expected = Tuple.Create(string.Empty, string.Empty);

            // Act
            var result = originalString.FindDifferences(comparedToString);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
