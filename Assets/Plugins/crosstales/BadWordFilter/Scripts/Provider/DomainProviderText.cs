﻿using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

namespace Crosstales.BWF.Provider
{
   /// <summary>Text-file based domain provider.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_provider_1_1_domain_provider_text.html")]
   public class DomainProviderText : DomainProvider
   {
      #region Implemented methods

      public override void Load()
      {
         base.Load();
         
         if (Sources != null)
         {
            loading = true;

            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               foreach (var source in Sources.Where(source => source != null))
               {
                  if (source.Resource != null)
                  {
                     loadResourceInEditor(source);
                  }

                  if (!string.IsNullOrEmpty(source.URL))
                  {
                     loadWebInEditor(source);
                  }
               }

               init();
#endif
            }
            else
            {
               foreach (var source in Sources.Where(source => source != null))
               {
                  if (source.Resource != null)
                  {
                     StartCoroutine(loadResource(source));
                  }

                  if (!string.IsNullOrEmpty(source.URL))
                  {
                     StartCoroutine(loadWeb(source));
                  }
               }
            }
         }
      }

      public override void Save()
      {
         Debug.LogWarning("Save not implemented!");
      }

      #endregion


      #region Private methods

      private IEnumerator loadWeb(Data.Source src)
      {
         string uid = System.Guid.NewGuid().ToString();
         coRoutines.Add(uid);

         if (!string.IsNullOrEmpty(src.URL))
         {
            using (UnityWebRequest www = UnityWebRequest.Get(src.URL.Trim()))
            {
               www.timeout = Util.Constants.WWW_TIMEOUT;
               www.downloadHandler = new DownloadHandlerBuffer();
               yield return www.SendWebRequest();

               if (!www.isHttpError && !www.isNetworkError)
               {
                  System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(www.downloadHandler.text);

                  yield return null;

                  if (Crosstales.BWF.Util.Config.DEBUG)
                     Debug.Log(src + ": " + list.Count);

                  src.RegexCount = list.Count;

                  if (list.Count > 0)
                  {
                     domains.Add(new Model.Domains(src, list));
                  }
                  else
                  {
                     Debug.LogWarning("Source: '" + src.URL + "' does not contain any active bad words!");
                  }
               }
               else
               {
                  Debug.LogWarning("Could not load source: '" + src.URL + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'URL'?");
               }
            }
         }
         else
         {
            Debug.LogWarning("'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
         }

         coRoutines.Remove(uid);

         if (loading && coRoutines.Count == 0)
         {
            loading = false;
            init();
         }
      }

      private IEnumerator loadResource(Data.Source src)
      {
         string uid = System.Guid.NewGuid().ToString();
         coRoutines.Add(uid);

         if (src.Resource != null)
         {
            System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(src.Resource.text);

            yield return null;

            if (Crosstales.BWF.Util.Config.DEBUG)
               Debug.Log(src + ": " + list.Count);

            src.RegexCount = list.Count;

            if (list.Count > 0)
            {
               domains.Add(new Model.Domains(src, list));
            }
            else
            {
               Debug.LogWarning("Resource: '" + src.Resource + "' does not contain any active bad words!");
            }
         }
         else
         {
            Debug.LogWarning("Resource field 'Source' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
         }

         coRoutines.Remove(uid);

         if (loading && coRoutines.Count == 0)
         {
            loading = false;
            init();
         }
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      private void loadWebInEditor(Data.Source src)
      {
         if (!string.IsNullOrEmpty(src.URL))
         {
            try
            {
               System.Net.ServicePointManager.ServerCertificateValidationCallback = Util.Helper.RemoteCertificateValidationCallback;

               using (System.Net.WebClient client = new Common.Util.CTWebClient())
               {
                  string content = client.DownloadString(src.URL.Trim());

                  System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(content);

                  if (Crosstales.BWF.Util.Config.DEBUG)
                     Debug.Log(src + ": " + list.Count);

                  src.RegexCount = list.Count;

                  if (list.Count > 0)
                  {
                     domains.Add(new Model.Domains(src, list));
                  }
                  else
                  {
                     Debug.LogWarning("Source: '" + src.URL + "' does not contain any active domains!");
                  }
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogWarning("Could not load source: '" + src.URL + "'" + System.Environment.NewLine + ex + System.Environment.NewLine + "Did you set the correct 'URL'?");
            }
         }
         else
         {
            Debug.LogWarning("'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
         }

         //Debug.Log("Source: '" + src.URL + "' loaded");
      }

      private void loadResourceInEditor(Data.Source src)
      {
         if (src.Resource != null)
         {
            System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(src.Resource.text);

            if (Crosstales.BWF.Util.Config.DEBUG)
               Debug.Log(src + ": " + list.Count);

            src.RegexCount = list.Count;

            if (list.Count > 0)
            {
               domains.Add(new Model.Domains(src, list));
            }
            else
            {
               Debug.LogWarning("Resource: '" + src.Resource + "' does not contain any active bad words!");
            }
         }
         else
         {
            Debug.LogWarning("Resource field 'Source' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
         }
      }

#endif

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)