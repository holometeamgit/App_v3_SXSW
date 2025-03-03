﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'BadWordManager'-class.</summary>
   [CustomEditor(typeof(Manager.BadWordManager))]
   public class BadWordManagerEditor : Editor
   {
      #region Variables

      private Manager.BadWordManager script;

      private string inputText = "Martians are assholes...";
      private string outputText;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Manager.BadWordManager)target;

         if (script.isActiveAndEnabled)
         {
            Manager.BadWordManager.Load();
         }
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.BadWordProviderLTR == null || script.BadWordProviderLTR.Count == 0)
            EditorGUILayout.HelpBox("No 'BadWord Provider LTR' added!" + System.Environment.NewLine + "If you want to use this functionality, please add your desired 'BadWord Provider LTR'.", MessageType.Info);

         if (script.BadWordProviderRTL == null || script.BadWordProviderRTL.Count == 0)
            EditorGUILayout.HelpBox("No 'Bad Word Provider RTL' added!" + System.Environment.NewLine + "If you want to use this functionality, please add your desired 'BadWord Provider RTL'.", MessageType.Info);

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            GUILayout.Label("Stats", EditorStyles.boldLabel);
            GUILayout.Label("Ready:\t\t" + (Manager.BadWordManager.isReady ? "Yes" : "No"));

            if (Manager.BadWordManager.isReady)
            {
               GUILayout.Label("Sources:\t\t" + Manager.BadWordManager.Sources.Count);
               GUILayout.Label("Regex Count:\t" + Manager.BadWordManager.TotalRegexCount);

               EditorHelper.SeparatorUI();

               if (script.BadWordProviderLTR != null && script.BadWordProviderLTR.Count > 0 || script.BadWordProviderRTL != null && script.BadWordProviderRTL.Count > 0)
               {
                  //EditorHelper.SeparatorUI();

                  GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

                  if (Util.Helper.isEditorMode)
                  {
                     inputText = EditorGUILayout.TextField(new GUIContent("Input Text", "Text to check."), inputText);

                     EditorHelper.ReadOnlyTextField("Output Text", outputText);

                     GUILayout.Space(8);

                     GUILayout.BeginHorizontal();
                     if (GUILayout.Button(new GUIContent(" Contains", EditorHelper.Icon_Contains, "Contains any bad words?")))
                     {
                        outputText = Manager.BadWordManager.Contains(inputText).ToString();
                     }

                     if (GUILayout.Button(new GUIContent(" Get", EditorHelper.Icon_Get, "Get all bad words.")))
                     {
                        outputText = string.Join(", ", Manager.BadWordManager.GetAll(inputText).ToArray());
                     }

                     if (GUILayout.Button(new GUIContent(" Replace", EditorHelper.Icon_Replace, "Check and replace all bad words.")))
                     {
                        outputText = Manager.BadWordManager.ReplaceAll(inputText);
                     }

                     if (GUILayout.Button(new GUIContent(" Mark", EditorHelper.Icon_Mark, "Mark all bad words.")))
                     {
                        outputText = Manager.BadWordManager.Mark(inputText);
                     }

                     GUILayout.EndHorizontal();
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }
               }
               else
               {
                  EditorGUILayout.HelpBox("Please add a 'Bad Word Provider'!", MessageType.Warning);
               }
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      #endregion
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)