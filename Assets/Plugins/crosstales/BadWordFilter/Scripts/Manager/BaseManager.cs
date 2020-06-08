using UnityEngine;

namespace Crosstales.BWF.Manager
{
   /// <summary>Base class for all managers.</summary>
   [ExecuteInEditMode]
   public abstract class BaseManager : MonoBehaviour
   {
      #region Variables

      /// <summary>Don't destroy gameobject during scene switches (default: true).</summary>
      [Header("Behaviour Settings")] [Tooltip("Don't destroy gameobject during scene switches (default: true).")]
      public bool DontDestroy = true;

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)