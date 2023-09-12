using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace quanLyCongViec.Localization
{
    public static class quanLyCongViecLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(quanLyCongViecConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(quanLyCongViecLocalizationConfigurer).GetAssembly(),
                        "quanLyCongViec.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
