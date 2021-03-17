#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'BadWordProviderText'-class.</summary>
   [CustomEditor(typeof(Provider.BadWordProviderText))]
   public class BadWordProviderTextEditor : BaseProviderEditor
   {
      //empty
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)