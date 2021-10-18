using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Build for Dynamic Links
    /// </summary>

    public interface IParameters {
        string Get { get; }
    }

    public class LinkBuilder : IParameters {

        public class AndroidParameters : IParameters {

            private string _apn;
            private string _afl;

            public string Get {
                get {
                    string parameter = string.Empty;
                    parameter += $"&apn={_apn}";
                    parameter += $"&afl={_afl}";
                    return parameter;
                }
            }

            public AndroidParameters(string apn, string afl) {
                _apn = apn;
                _afl = afl;
            }

        }

        public class iOSParameters : IParameters {

            private string _ibi;
            private string _ifl;
            private string _isi;


            public string Get {
                get {
                    string parameter = string.Empty;
                    parameter += $"&ibi={_ibi}";
                    parameter += $"&ifl={_ifl}";
                    parameter += $"&isi={_isi}";
                    return parameter;
                }
            }

            public iOSParameters(string ibi, string ifl, string isi) {
                _ibi = ibi;
                _ifl = ifl;
                _isi = isi;
            }

        }

        public class DesktopParameters : IParameters {

            private string _ofl;

            public string Get {
                get {
                    string parameter = string.Empty;
                    parameter += $"&ofl={_ofl}";
                    return parameter;
                }
            }

            public DesktopParameters(string ofl) {
                _ofl = ofl;
            }

        }

        public class SocialMetaTagParameters : IParameters {

            private string _st;
            private string _sd;
            private string _si;

            public string Get {
                get {
                    string parameter = string.Empty;
                    parameter += $"&st={_st}";
                    parameter += $"&sd={_sd}";
                    parameter += $"&si={_si}";
                    return parameter;
                }
            }

            public SocialMetaTagParameters(string st, string sd, string si) {
                _st = st;
                _sd = sd;
                _si = si;
            }
        }

        private AndroidParameters _androidLinkBuilder;
        private iOSParameters _iOSLinkBuilder;
        private DesktopParameters _desktopLinkBuilder;
        private SocialMetaTagParameters _socialMetaTagBuilder;

        public string Get {
            get {
                return _androidLinkBuilder.Get + _iOSLinkBuilder.Get + _desktopLinkBuilder.Get + _socialMetaTagBuilder.Get;
            }
        }


        public LinkBuilder(AndroidParameters androidLinkBuilder, iOSParameters iOSLinkBuilder, DesktopParameters desktopLinkBuilder, SocialMetaTagParameters socialMetaTagBuilder) {
            _androidLinkBuilder = androidLinkBuilder;
            _iOSLinkBuilder = iOSLinkBuilder;
            _desktopLinkBuilder = desktopLinkBuilder;
            _socialMetaTagBuilder = socialMetaTagBuilder;
        }


    }
}
