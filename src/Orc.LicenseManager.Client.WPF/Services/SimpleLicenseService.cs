﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleLicenseService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.LicenseManager.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel;

    /// <summary>
    /// Simple license service.
    /// </summary>
    public class SimpleLicenseService : ISimpleLicenseService
    {
        #region Fields
        private readonly ILicenseService _licenseService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLicenseService"/> class.
        /// </summary>
        /// <param name="licenseService">The license service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="licenseService"/> is <c>null</c>.</exception>
        public SimpleLicenseService(ILicenseService licenseService)
        {
            Argument.IsNotNull(() => licenseService);

            _licenseService = licenseService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the license on the server. This method is the same as <see cref="Validate"/> but also checks the server if the license
        /// is valid.
        /// </summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <param name="applicationId">The application identifier.</param>
        /// <param name="aboutTitle">The about title.</param>
        /// <param name="aboutImage">The about image.</param>
        /// <param name="aboutText">The about text.</param>
        /// <param name="aboutSiteUrl">The about site URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="purchaseLinkUrl">The purchase link URL.</param>
        /// <returns><c>true</c> if the license is valid, <c>false</c> otherwise.</returns>
        public async Task<bool> ValidateOnServer(string serverUrl, string applicationId, string aboutTitle, string aboutImage, string aboutText, string aboutSiteUrl = null, string title = null, string purchaseLinkUrl = null)
        {
            await _licenseService.Initialize(applicationId);

            if (!_licenseService.LicenseExists())
            {
                await _licenseService.ShowSingleLicenseDialog(aboutTitle, aboutImage, aboutText, aboutSiteUrl, title, purchaseLinkUrl);
            }

            if (!_licenseService.LicenseExists())
            {
                return false;
            }

            var licenseString = _licenseService.LoadLicense();
            var licenseValidation = _licenseService.ValidateLicense(licenseString);

            if (licenseValidation.HasErrors)
            {
                return false;
            }

            return await _licenseService.ValidateLicenseOnServer(licenseString, serverUrl);
        }

        /// <summary>
        /// Validates the license in a very simple manner. This method is wrapper around the <see cref="ILicenseService" />.
        /// </summary>
        /// <param name="applicationId">The application identifier, can be any value but should be unique.</param>
        /// <param name="aboutTitle">The about title.</param>
        /// <param name="aboutImage">The about image.</param>
        /// <param name="aboutText">The about text.</param>
        /// <param name="aboutSiteUrl">The about site.</param>
        /// <param name="title">The title.</param>
        /// <param name="purchaseLinkUrl">The purchase link.</param>
        /// <returns><c>true</c> if the license is valid, <c>false</c> otherwise.</returns>
        /// <remarks>Note that this method might show a dialog so must be run on the UI thread.</remarks>
        public async Task<bool> Validate(string applicationId, string aboutTitle, string aboutImage, string aboutText, string aboutSiteUrl = null, string title = null, string purchaseLinkUrl = null)
        {
            await _licenseService.Initialize(applicationId);

            if (!_licenseService.LicenseExists())
            {
                await _licenseService.ShowSingleLicenseDialog(aboutTitle, aboutImage, aboutText, aboutSiteUrl, title, purchaseLinkUrl);
            }

            if (!_licenseService.LicenseExists())
            {
                return false;
            }

            var licenseString = _licenseService.LoadLicense();
            var licenseValidation = _licenseService.ValidateLicense(licenseString);

            return !licenseValidation.HasErrors;
        }
        #endregion
    }
}