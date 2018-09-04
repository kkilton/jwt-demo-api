using System.Configuration;

namespace JWTDemo.Configuration
{
    /// <summary>
    ///     Usage:
    ///     <configSections>
    ///         <section name="corsSettingConfig" type="SIS.REG.API.CorsSettingsConfigsection" />
    ///     </configSections>
    ///     <corsSettingConfig>
    ///         <corsSettings>
    ///             <add name="currentEnv" origins="*" headers="*" methods="*" exposedHeaders="Token" />
    ///         </corsSettings>
    ///     </corsSettingConfig>
    /// </summary>
    public class CorsSettings : ConfigurationSection
    {
        private static readonly CorsSettingsConfigsection Settings =
            ConfigurationManager.GetSection("corsSettingConfig") as CorsSettingsConfigsection;

        public static CorsSettingsCollection GetCorsSettings()
        {
            return Settings.CorsSettings;
        }
    }

    public class CorsSettingsConfigsection : ConfigurationSection
    {
        //Decorate the property with the tag for your collection.
        [ConfigurationProperty("corsSettings")]
        public CorsSettingsCollection CorsSettings => (CorsSettingsCollection)this["corsSettings"];
    }

    [ConfigurationCollection(typeof(CorsSettingsElement))]
    public class CorsSettingsCollection : ConfigurationElementCollection
    {
        public CorsSettingsElement this[int index]
        {
            get { return (CorsSettingsElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CorsSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CorsSettingsElement)element).Name;
        }
    }

    public class CorsSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }

            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("origins", IsRequired = true)]
        public string Origins
        {
            get { return (string)this["origins"]; }

            set { this["origins"] = value; }
        }

        [ConfigurationProperty("headers", IsRequired = true)]
        public string Headers
        {
            get { return (string)this["headers"]; }

            set { this["headers"] = value; }
        }

        [ConfigurationProperty("methods", IsRequired = false)]
        public string Methods
        {
            get { return (string)this["methods"]; }

            set { this["methods"] = value; }
        }

        [ConfigurationProperty("exposedHeaders", IsRequired = false)]
        public string ExposedHeaders
        {
            get { return (string)this["exposedHeaders"]; }

            set { this["exposedHeaders"] = value; }
        }
    }
}