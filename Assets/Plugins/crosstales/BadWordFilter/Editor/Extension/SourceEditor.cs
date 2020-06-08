#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'Source'-class.</summary>
   [CustomEditor(typeof(Data.Source))]
   public class SourceEditor : Editor
   {
      #region Variables

      private Data.Source script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Data.Source)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         GUILayout.Label("Stats", EditorStyles.boldLabel);
         GUILayout.Label("Regex Count:\t" + script.RegexCount);
      }

      #endregion
   }
}
#endif
// © 2020 crosstales LLC (https://www.crosstales.com)