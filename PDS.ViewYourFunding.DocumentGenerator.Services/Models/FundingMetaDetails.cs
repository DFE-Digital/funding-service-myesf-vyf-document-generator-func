namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// The funding meta details.
    /// </summary>
    public class FundingMetaDetails
    {
        /// <summary>
        /// Gets a value indicating whether this instance is existing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is existing; otherwise, <c>false</c>.
        /// </value>
        public bool IsExisting => !IsInYearOpener;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in year opener.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in year opener; otherwise, <c>false</c>.
        /// </value>
        public bool IsInYearOpener { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is second year in year opener.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is second year in year opener; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecondYearInYearOpener { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is april to august opener.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is april to august opener; otherwise, <c>false</c>.
        /// </value>
        public bool IsAprilToAugustOpener { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is september to march opener.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is september to march opener; otherwise, <c>false</c>.
        /// </value>
        public bool IsSeptemberToMarchOpener { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is free school.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is free school; otherwise, <c>false</c>.
        /// </value>
        public bool IsFreeSchool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is academy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is academy; otherwise, <c>false</c>.
        /// </value>
        public bool IsAcademy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FundingDetails"/> is indicative.
        /// </summary>
        public bool Indicative { get; set; }
    }
}