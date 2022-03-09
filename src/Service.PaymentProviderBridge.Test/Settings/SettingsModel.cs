using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.PaymentProviderBridge.Test.Settings
{
    public class SettingsModel
    {
        [YamlProperty("PaymentProviderBridge.Test.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}
