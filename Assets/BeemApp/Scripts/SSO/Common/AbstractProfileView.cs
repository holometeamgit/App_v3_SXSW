using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
namespace Beem.SSO {
    /// <summary>
    /// Abstract Profile View
    /// </summary>
    public abstract class AbstractProfileView : MonoBehaviour {

        /// <summary>
        /// Refresh Data
        /// </summary>
        /// <param name="firebaseUser"></param>
        public abstract void Refresh(FirebaseUser firebaseUser);
    }
}
