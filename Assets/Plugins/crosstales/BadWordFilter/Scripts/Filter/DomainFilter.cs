using UnityEngine;
using System.Linq;

namespace Crosstales.BWF.Filter
{
   /// <summary>Filter for domains. The class can also replace all domains inside a string.</summary>
   public class DomainFilter : BaseFilter
   {
      #region Variables

      /// <summary>Replace characters for domains.</summary>
      public string ReplaceCharacters;

      private System.Collections.Generic.List<Provider.DomainProvider> domainProvider = new System.Collections.Generic.List<Provider.DomainProvider>();

      private readonly System.Collections.Generic.List<Provider.DomainProvider> tempDomainProvider;
      private readonly System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> domainsRegex = new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>();
      private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> debugDomainsRegex = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>>();
      private bool ready = false;
      private bool readyFirstime = false;

      #endregion


      #region Properties

      /// <summary>List of all domain providers.</summary>
      /// <returns>All domain providers.</returns>
      public System.Collections.Generic.List<Provider.DomainProvider> DomainProvider
      {
         get { return domainProvider; }
         set
         {
            domainProvider = value;
            if (domainProvider != null && domainProvider.Count > 0)
            {
               foreach (Provider.DomainProvider dp in domainProvider)
               {
                  if (dp != null)
                  {
                     if (Util.Config.DEBUG_DOMAINS)
                     {
                        debugDomainsRegex.CTAddRange(dp.DebugDomainsRegex);
                     }
                     else
                     {
                        domainsRegex.CTAddRange(dp.DomainsRegex);
                     }
                  }
                  else
                  {
                     if (!Util.Helper.isEditorMode)
                        Debug.LogError("DomainProvider is null!");
                  }
               }
            }
            else
            {
               domainProvider = new System.Collections.Generic.List<Provider.DomainProvider>();

               /*
               if (!Util.Helper.isEditorMode)
                   Debug.LogWarning("No 'DomainProvider' added!" + System.Environment.NewLine + "If you want to use this functionality, please add your desired 'DomainProvider' in the editor or script.");
               */
            }
         }
      }

      /// <summary>Checks the readiness status of the filter.</summary>
      /// <returns>True if the filter is ready.</returns>
      public override bool isReady
      {
         get
         {
            bool result = true;

            if (!ready)
            {
               if (tempDomainProvider != null)
               {
                  if (tempDomainProvider.Any(dp => dp != null && !dp.isReady))
                  {
                     result = false;
                  }
               }

               if (!readyFirstime && result)
               {
                  DomainProvider = tempDomainProvider;

                  if (DomainProvider != null)
                  {
                     foreach (var src in from dp in DomainProvider where dp != null from src in dp.Sources where src != null where !sources.ContainsKey(src.Name) select src)
                     {
                        sources.Add(src.Name, src);
                     }
                  }

                  readyFirstime = true;
               }
            }

            ready = result;

            return result;
         }
      }

      #endregion


      #region Constructor

      /// <summary>Instantiate the class.</summary>
      /// <param name="domainProvider">List of all domain providers.</param>
      /// <param name="replaceCharacters">Replace characters for domains.</param>
      public DomainFilter(System.Collections.Generic.List<Provider.DomainProvider> domainProvider, string replaceCharacters /*, string markPrefix, string markPostfix*/)
      {
         tempDomainProvider = domainProvider;
         ReplaceCharacters = replaceCharacters;
      }

      #endregion


      #region Implemented methods

      public override bool Contains(string text, params string[] sourceNames)
      {
         bool result = false;

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logContains();
            }
            else
            {
               #region DEBUG

               if (Util.Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                           if (match.Success)
                           {
                              Debug.Log("Test string contains a domain: '" + match.Value + "' detected by regex '" + domainRegex + "'");
                              result = true;
                              break;
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes;
                        if (debugDomainsRegex.TryGetValue(domainResource, out domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                              if (match.Success)
                              {
                                 Debug.Log("Test string contains a domain: '" + match.Value + "' detected by regex '" + domainRegex + "'" + "' from source '" + domainResource + "'");
                                 result = true;
                                 break;
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }

                  #endregion
               }
               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     if (domainsRegex.Values.Any(domainRegex => domainRegex.Match(text).Success))
                     {
                        result = true;
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Text.RegularExpressions.Regex domainRegex;
                        if (domainsRegex.TryGetValue(domainResource, out domainRegex))
                        {
                           System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                           if (match.Success)
                           {
                              result = true;
                              break;
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return result;
      }

      public override System.Collections.Generic.List<string> GetAll(string text, params string[] sourceNames)
      {
         System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logGetAll();
            }
            else
            {
               #region DEBUG

               if (Util.Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                           {
                              Debug.Log("Test string contains a domain: '" + capture.Value + "' detected by regex '" + domainRegex + "'");

                              if (!result.Contains(capture.Value))
                              {
                                 result.Add(capture.Value);
                              }
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes;
                        if (debugDomainsRegex.TryGetValue(domainResource, out domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                              foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                              {
                                 Debug.Log("Test string contains a domain: '" + capture.Value + "' detected by regex '" + domainRegex + "'" + "' from source '" + domainResource + "'");

                                 if (!result.Contains(capture.Value))
                                 {
                                    result.Add(capture.Value);
                                 }
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }

                  #endregion
               }
               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (var capture in from domainRegex in domainsRegex.Values select domainRegex.Matches(text) into matches from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures where !result.Contains(capture.Value) select capture)
                     {
                        result.Add(capture.Value);
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Text.RegularExpressions.Regex domainRegex;
                        if (domainsRegex.TryGetValue(domainResource, out domainRegex))
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures where !result.Contains(capture.Value) select capture)
                           {
                              result.Add(capture.Value);
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return result.Distinct().OrderBy(x => x).ToList();
      }

      public override string ReplaceAll(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames)
      {
         string result = text;

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logReplaceAll();

               result = string.Empty;
            }
            else
            {
               #region DEBUG

               if (Util.Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                           {
                              Debug.Log("Test string contains a domain: '" + capture.Value + "' detected by regex '" + domainRegex + "'");

                              result = result.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Util.Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix);
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes;
                        if (debugDomainsRegex.TryGetValue(domainResource, out domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                              foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                              {
                                 Debug.Log("Test string contains a domain: '" + capture.Value + "' detected by regex '" + domainRegex + "'" + "' from source '" + domainResource + "'");

                                 result = result.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Util.Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix);
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }

                  #endregion
               }
               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     result = (from domainRegex in domainsRegex.Values from System.Text.RegularExpressions.Match match in domainRegex.Matches(text) from System.Text.RegularExpressions.Capture capture in match.Captures select capture).Aggregate(result, (current, capture) => current.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Util.Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix));
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        System.Text.RegularExpressions.Regex domainRegex;
                        if (domainsRegex.TryGetValue(domainResource, out domainRegex))
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           result = (from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture).Aggregate(result, (current, capture) => current.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Util.Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix));
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return result;
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)