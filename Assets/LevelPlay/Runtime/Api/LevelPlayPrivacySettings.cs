using System;
using System.Collections.Generic;

namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// This class provides privacy APIs for the LevelPlay SDK.
    /// </summary>
    public static class LevelPlayPrivacySettings
    {
        private static ILevelPlaySdk Sdk => LevelPlaySdkProvider.Get();

        /// <summary>
        /// Sets the user's GDPR consent. This flag indicates whether the user has granted consent for data
        /// collection and sharing across all networks. Consent is used for GDPR compliance.
        /// </summary>
        /// <param name="consent">true if the user has granted consent, false otherwise.</param>
        public static void SetGDPRConsent(bool consent) => Sdk?.SetConsent(consent);

        /// <summary>
        /// Sets the consent per network, a dictionary of network keys to boolean values that indicates whether
        /// the user has granted consent for each network to collect and share data. Consent is used for GDPR compliance.
        /// </summary>
        /// <param name="networkConsents">A dictionary where keys are network identifiers and values indicate consent status,
        /// true if the user has granted consent, false otherwise.</param>
        [Obsolete("Use LevelPlayPrivacySettings.SetGDPRConsent() instead.", false)]
        public static void SetGDPRConsents(Dictionary<string, bool> networkConsents) => Sdk?.SetGDPRConsents(networkConsents);

        /// <summary>
        /// Sets the CCPA (California Consumer Privacy Act) flag. This flag indicates whether the user has
        /// opted out of the sale of their personal information.
        /// </summary>
        /// <param name="value">true if the user has opted out of the sale of their personal information, false otherwise.</param>
        public static void SetCCPA(bool value) => Sdk?.SetCCPA(value);

        /// <summary>
        /// Sets the COPPA (Children's Online Privacy Protection Act) flag. This flag indicates whether the
        /// user is a child under the age of 13. This will apply COPPA settings to all supported network adapters.
        /// </summary>
        /// <param name="value">true if the user is a child under the age of 13, false otherwise.</param>
        public static void SetCOPPA(bool value) => Sdk?.SetCOPPA(value);
    }
}
