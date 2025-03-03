﻿using UnityEngine;

namespace Crosstales.BWF.Manager
{
   /// <summary>Manager for excessive capitalization.</summary>
   [DisallowMultipleComponent]
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_manager_1_1_capitalization_manager.html")]
   public class CapitalizationManager : BaseManager
   {
      #region Variables

      /// <summary>Defines the number of allowed capital letters in a row. (default: 1).</summary>
      [Header("Specific Settings")] [Tooltip("Defines the number of allowed capital letters in a row. (default: 3).")]
      public int CapitalizationCharsNumber = 3;

      private static Filter.CapitalizationFilter filter;
      private static CapitalizationManager instance;
      private static bool loggedFilterIsNull = false;
      private static bool loggedOnlyOneInstance = false;

      private const string clazz = "CapitalizationManager";

      #endregion


      #region Static properties

      /// <summary>Defines the number of allowed punctuation letters in a row.</summary>
      public static int CharacterNumber
      {
         get
         {
            if (filter != null)
            {
               return filter.CharacterNumber;
            }

            return instance != null ? instance.CapitalizationCharsNumber : 3;
         }

         set
         {
            int newValue = value;

            if (newValue < 2)
            {
               newValue = 2;
            }

            if (filter != null)
            {
               filter.CharacterNumber = newValue;
               instance.CapitalizationCharsNumber = newValue;
            }
            else
            {
               if (instance != null)
               {
                  instance.CapitalizationCharsNumber = newValue;
               }
            }
         }
      }

      /// <summary>Checks the readiness status of the manager.</summary>
      /// <returns>True if the manager is ready.</returns>
      public static bool isReady
      {
         get { return filter.isReady; }
      }

      #endregion


      #region MonoBehaviour methods

      public void OnEnable()
      {
         if (instance == null)
         {
            instance = this;

            Load();

            if (!Util.Helper.isEditorMode && DontDestroy)
            {
               DontDestroyOnLoad(transform.root.gameObject);
            }

            if (Util.Config.DEBUG)
               Debug.LogWarning("Using new instance!");
         }
         else
         {
            if (!Util.Helper.isEditorMode && DontDestroy && instance != this)
            {
               if (!loggedOnlyOneInstance)
               {
                  loggedOnlyOneInstance = true;

                  Debug.LogWarning("Only one active instance of '" + clazz + "' allowed in all scenes!" + System.Environment.NewLine + "This object will now be destroyed.");
               }

               Destroy(transform.root.gameObject, 0.2f);
            }

            if (Util.Config.DEBUG)
               Debug.LogWarning("Using old instance!");
         }
      }

      public void OnValidate()
      {
         if (CapitalizationCharsNumber < 2)
         {
            CapitalizationCharsNumber = 2;
         }
      }

      #endregion


      #region Static methods

      /// <summary>Resets this object.</summary>
      public static void Reset()
      {
         filter = null;
         instance = null;
         loggedFilterIsNull = false;
         loggedOnlyOneInstance = false;
      }

      /// <summary>Loads the current filter with all settings from this object.</summary>
      public static void Load()
      {
         if (instance != null)
         {
            filter = new Filter.CapitalizationFilter(instance.CapitalizationCharsNumber);
         }
      }

      /// <summary>Searches for excessive capitalizations in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <returns>True if a match was found</returns>
      public static bool Contains(string text)
      {
         bool result = false;

         if (!string.IsNullOrEmpty(text))
         {
            if (filter != null)
            {
               result = filter.Contains(text);
            }
            else
            {
               logFilterIsNull();
            }
         }

         return result;
      }

      /// <summary>Searches for excessive capitalizations in a text (call as thread).</summary>
      /// <param name="result">out-parameter: true if a match was found</param>
      /// <param name="text">Text to check</param>
      /// <returns>True if a match was found</returns>
      public static void ContainsMT(out bool result, string text)
      {
         result = Contains(text);
      }

      /// <summary>Searches for excessive capitalizations in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <returns>List with all the matches</returns>
      public static System.Collections.Generic.List<string> GetAll(string text)
      {
         System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();

         if (!string.IsNullOrEmpty(text))
         {
            if (filter != null)
            {
               result = filter.GetAll(text);
            }
            else
            {
               logFilterIsNull();
            }
         }

         return result;
      }

      /// <summary>Searches for excessive capitalizations in a text (call as thread).</summary>
      /// <param name="result">out-parameter: List with all the matches</param>
      /// <param name="text">Text to check</param>
      public static void GetAllMT(out System.Collections.Generic.List<string> result, string text)
      {
         result = GetAll(text);
      }

      /// <summary>Searches and replaces all excessive capitalizations in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="markOnly">Only mark the words (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found capitalization (optional)</param>
      /// <param name="postfix">Postfix for every found capitalization (optional)</param>
      /// <returns>Clean text</returns>
      public static string ReplaceAll(string text, bool markOnly = false, string prefix = "", string postfix = "")
      {
         string result = text;

         if (!string.IsNullOrEmpty(text))
         {
            if (filter != null)
            {
               result = filter.ReplaceAll(text, markOnly, prefix, postfix);
            }
            else
            {
               logFilterIsNull();
            }
         }

         return result;
      }

      /// <summary>Searches and replaces all excessive capitalizations in a text  (call as thread).</summary>
      /// <param name="result">out-parameter: clean text</param>
      /// <param name="text">Text to check</param>
      /// <param name="markOnly">Only mark the words (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found capitalization (optional)</param>
      /// <param name="postfix">Postfix for every found capitalization (optional)</param>
      public static void ReplaceAllMT(out string result, string text, bool markOnly = false, string prefix = "", string postfix = "")
      {
         result = ReplaceAll(text, markOnly, prefix, postfix);
      }

      /// <summary>Unmarks the text with a prefix and postfix.</summary>
      /// <param name="text">Text with marked excessive capitalizations</param>
      /// <param name="prefix">Prefix for every found capitalization (default: bold and red, optional)</param>
      /// <param name="postfix">Postfix for every found capitalization (default: bold and red, optional)</param>
      /// <returns>Text with unmarked excessive capitalizations</returns>
      public static string Unmark(string text, string prefix = "<b><color=red>", string postfix = "</color></b>")
      {
         string result = text;

         if (!string.IsNullOrEmpty(text))
         {
            if (filter != null)
            {
               result = filter.Unmark(text, prefix, postfix);
            }
            else
            {
               logFilterIsNull();
            }
         }

         return result;
      }

      /// <summary>Marks the text with a prefix and postfix.</summary>
      /// <param name="text">Text containing excessive capitalizations</param>
      /// <param name="replace">Replace the excessive capitalizations (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found capitalizations (default: bold and red, optional)</param>
      /// <param name="postfix">Postfix for every found capitalizations (default: bold and red, optional)</param>
      /// <returns>Text with marked excessive capitalizations</returns>
      public static string Mark(string text, bool replace = false, string prefix = "<b><color=red>", string postfix = "</color></b>")
      {
         string result = text;

         if (!string.IsNullOrEmpty(text))
         {
            if (filter != null)
            {
               result = filter.Mark(text, replace, prefix, postfix);
            }
            else
            {
               logFilterIsNull();
            }
         }

         return result;
      }

      private static void logFilterIsNull()
      {
         if (!loggedFilterIsNull)
         {
            Debug.LogWarning("'filter' is null!" + System.Environment.NewLine + "Did you add the '" + clazz + "' to the current scene?");
            loggedFilterIsNull = true;
         }
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)