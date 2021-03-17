#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'DomainProviderText'-class.</summary>
   [CustomEditor(typeof(Provider.DomainProviderText))]
   public class DomainProviderTextEditor : BaseProviderEditor
   {
      //empty
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)