using System.Configuration;

namespace JWTDemo.Authentication
{
    /// <summary>
    ///     Usage:
    ///     <configSections>
    ///         <section name="tokenSettingConfig" type="EAI.Auth.JWT.TokenSettingsConfigsection"/>
    ///     </configSections>
    ///     <TokenSettingConfig>
    ///         <TokenSettings>
    ///             <add name="currentEnv" audience="website url" communicationKey="key"/>
    ///         </TokenSettings>
    ///     </TokenSettingConfig>
    /// </summary>
    public class TokenSettings : ConfigurationSection
    {
        private static readonly TokenSettingsConfigsection Settings =
            ConfigurationManager.GetSection("tokenSettingConfig") as TokenSettingsConfigsection;

        public static TokenSettingsConfigsection.TokenSettingsCollection GetTokenSettings()
        {
            return Settings.TokenSettings;
        }
    }

    public class TokenSettingsConfigsection : ConfigurationSection
    {
        //Decorate the property with the tag for your collection.
        [ConfigurationProperty("tokenSettings")]
        public TokenSettingsCollection TokenSettings
        {
            get { return this["tokenSettings"] as TokenSettingsCollection; }
        }

        [ConfigurationCollection(typeof(TokenSettingsElement))]
        public class TokenSettingsCollection : ConfigurationElementCollection
        {
            public TokenSettingsElement this[int index]
            {
                get { return (TokenSettingsElement) BaseGet(index); }
                set
                {
                    if (BaseGet(index) != null)
                        BaseRemoveAt(index);

                    BaseAdd(index, value);
                }
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new TokenSettingsElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((TokenSettingsElement) element).Name;
            }
        }

        public class TokenSettingsElement : ConfigurationElement
        {
            [ConfigurationProperty("name", IsRequired = true)]
            public string Name
            {
                get { return (string) this["name"]; }
                set { this["name"] = value; }
            }

            [ConfigurationProperty("audience", IsRequired = true)]
            public string Audience
            {
                get { return (string) this["audience"]; }
                set { this["audience"] = value; }
            }

            [ConfigurationProperty("issuer", IsRequired = true)]
            public string Issuer
            {
                get { return (string) this["issuer"]; }
                set { this["issuer"] = value; }
            }

            [ConfigurationProperty("communicationKey", IsRequired = true)]
            public string CommunicationKey
            {
                get { return (string) this["communicationKey"]; }
                set { this["communicationKey"] = value; }
            }

            [ConfigurationProperty("timeout", IsRequired = true)]
            public string Timeout
            {
                get { return (string) this["timeout"]; }
                set { this["timeout"] = value; }
            }
        }
    }
}