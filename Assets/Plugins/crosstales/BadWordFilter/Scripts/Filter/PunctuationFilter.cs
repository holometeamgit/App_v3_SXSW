﻿using UnityEngine;
using System.Linq;

namespace Crosstales.BWF.Filter
{
   /// <summary>Filter for excessive punctuation. The class can also replace all punctuations inside a string.</summary>
   public class PunctuationFilter : BaseFilter
   {
      #region Variables

      /// <summary>RegEx to find excessive punctuation.</summary>
      public System.Text.RegularExpressions.Regex RegularExpression { get; private set; }

      private int characterNumber;

      #endregion


      #region Properties

      /// <summary>Defines the number of allowed punctuations in a row.</summary>
      public int CharacterNumber
      {
         get { return characterNumber; }
         set
         {
            characterNumber = value < 2 ? 2 : value;

            RegularExpression = new System.Text.RegularExpressions.Regex(@"[?!,.;:-]{" + (characterNumber + 1) + @",}", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
         }
      }

      /// <summary>Checks the readiness status of the filter.</summary>
      /// <returns>True if the filter is ready.</returns>
      public override bool isReady
      {
         get
         {
            return true; //is always ready
         }
      }

      #endregion


      #region Constructor

      /// <summary>Instantiate the class.</summary>
      /// <param name="punctuationCharacterNumber">Defines the number of allowed punctuations in a row.</param>
      public PunctuationFilter(int punctuationCharacterNumber /*, string markPrefix, string markPostfix */)
      {
         CharacterNumber = punctuationCharacterNumber;
      }

      #endregion


      #region Implemented methods

      public override bool Contains(string text, params string[] sourceNames) //sources are ignored
      {
         bool result = false;

         if (string.IsNullOrEmpty(text))
         {
            logContains();
         }
         else
         {
            result = RegularExpression.Match(text).Success;
         }

         return result;
      }

      public override System.Collections.Generic.List<string> GetAll(string text, params string[] sourceNames) //sources are ignored
      {
         System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();

         if (string.IsNullOrEmpty(text))
         {
            logGetAll();
         }
         else
         {
            System.Text.RegularExpressions.MatchCollection matches = RegularExpression.Matches(text);

            foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
            {
               if (Util.Constants.DEV_DEBUG)
                  Debug.Log("Test string contains an excessive punctuation: '" + capture.Value + "'");

               if (!result.Contains(capture.Value))
               {
                  result.Add(capture.Value);
               }
            }
         }

         return result.Distinct().OrderBy(x => x).ToList();
      }


      public override string ReplaceAll(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames) //sources are ignored
      {
         string result = text;

         if (string.IsNullOrEmpty(text))
         {
            logReplaceAll();

            result = string.Empty;
         }
         else
         {
            System.Text.RegularExpressions.MatchCollection matches = RegularExpression.Matches(text);

            foreach (var capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
            {
               if (Util.Constants.DEV_DEBUG)
                  Debug.Log("Test string contains an excessive punctuation: '" + capture.Value + "'");

               result = result.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + capture.Value.Substring(0, characterNumber) + postfix);
            }
         }

         return result;
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)