﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NextTests.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
                    <InstrumentMatch xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                        <Type>A</Type>
                        <Identifier>100</Identifier>
                        <Currency>SEK</Currency>
                        <MainMarketId>0</MainMarketId>
                        <Longname>Ericsson A</Longname>
                        <MarketID>11</MarketID>
                        <Country>SE</Country>
                        <Shortname>ERIC A</Shortname>
                        <Marketname>OMX Stockholm</Marketname>
                        <IsinCode>SE0000108649</IsinCode>
                    </InstrumentMatch>
                ")]
        public global::Next.Dtos.InstrumentMatch EricssonInstrumentMatch {
            get {
                return ((global::Next.Dtos.InstrumentMatch)(this["EricssonInstrumentMatch"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{
    ""cmd"": ""price"",
    ""args"": {
        ""i"": ""1843"",
        ""m"": 30,
        ""t"": ""price"",
        ""trade_timestamp"": ""2010-01-22 16:17:23"",
        ""tick_timestamp"": ""2010-01-22 16:18:42"",
        ""bid"": ""0.00"",
        ""bid_volume"": 0,
        ""ask"": 101.00,
        ""ask_volume"": 200,
        ""close"": 101.00,
        ""high"": 101.00,
        ""last"": 101.00,
        ""last_volume"": 200,
        ""lot_size"": ""1"",
        ""low"": 101.00,
        ""open"": 0.00,
        ""turnover"": 6605400,
        ""turnover_volume"": 65400
    }
}")]
        public string PriceTickJson {
            get {
                return ((string)(this["PriceTickJson"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{
    ""cmd"": ""depth"",
    ""args"": {
        ""i"": ""2188"",
        ""m"": 30,
        ""t"": ""depth"",
        ""tick_timestamp"": ""2010-01-22 16:22:06"",
        ""bid1"": 1.00,
        ""bid_volume1"": 100,
        ""ask1"": 1.10,
        ""ask_volume1"": 110,
        ""bid2"": 2.00,
        ""bid_volume2"": 200,
        ""ask2"": 1.20,
        ""ask_volume2"": 120,
        ""bid3"": 3.00,
        ""bid_volume3"": 300,
        ""ask3"": 1.30,
        ""ask_volume3"": 130,
        ""bid4"": 4.00,
        ""bid_volume4"": 400,
        ""ask4"": 1.40,
        ""ask_volume4"": 140,
        ""bid5"": 5.00,
        ""bid_volume5"": 500,
        ""ask5"": 1.50,
        ""ask_volume5"": 150
    }
}")]
        public string DepthTickJson {
            get {
                return ((string)(this["DepthTickJson"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{
    ""cmd"": ""trade"",
    ""args"": {
        ""i"": ""1843"",
        ""m"": 30,
        ""t"": ""trade"",
        ""trade_timestamp"": ""2010-01-22 16:24:04"",
        ""price"": 101.00,
        ""volume"": 200,
        ""baseprice"": 101.00,
        ""broker_buying"": ""ZCFT"",
        ""broker_selling"": ""ZCFT"" 
    }
}")]
        public string TradeTickJson {
            get {
                return ((string)(this["TradeTickJson"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{\r\n    \"cmd\": \"trading_status\",\r\n    \"args\": {\r\n        \"i\": \"4208\",\r\n        \"m\"" +
            ": 11,\r\n        \"t\": \"trading_status\",\r\n        \"timestamp\": \"2010-01-25 09:00:00" +
            "\",\r\n        \"status\": \"C\",\r\n        \"source_status\": \"ContinuousTrading\" \r\n    }" +
            "\r\n}")]
        public string TradingStatusTickJson {
            get {
                return ((string)(this["TradingStatusTickJson"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{
    ""cmd"": ""index"",
    ""args"": {
        ""i"": ""SIX-IDX-DJI"",
        ""m"": ""SIX"",
        ""t"": ""index"",
        ""tick_timestamp"": ""2010-09-24 07:01:58"",
        ""last"": 10662.4199,
        ""high"": 10761.9404,
        ""low"": 10640.9199,
        ""close"": 10739.31
    }
}")]
        public string IndexTickJson {
            get {
                return ((string)(this["IndexTickJson"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"{
    ""cmd"": ""news"",
    ""args"": {
        ""itemid"": 40705006,
        ""lang"": ""da"",
        ""datetime"": ""2010-12-08 22:15:00"",
        ""sourceid"": 6,
        ""headline"": ""Amerikanske aktieindeks kl. 22:15"",
        ""instruments"": [{
            ""identifier"": ""101"",
            ""marketID"": ""11"" 
        }]
    }
}")]
        public string NewsTickJson {
            get {
                return ((string)(this["NewsTickJson"]));
            }
        }
    }
}
