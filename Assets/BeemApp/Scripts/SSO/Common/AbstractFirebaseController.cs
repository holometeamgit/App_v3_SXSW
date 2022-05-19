﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Abstract Controller for firebase
    /// </summary>
    public abstract class AbstractFirebaseController {

        protected FirebaseAuth _auth;

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();

        public AbstractFirebaseController(FirebaseAuth auth) {
            _auth = auth;
            Subscribe();
        }

        protected void CheckTask(Task<FirebaseUser> task, Action onSuccess = null, Action<string> onFail = null) {
            LogInTaskManager firebaseTask = new LogInTaskManager();
            firebaseTask.CheckTask(task, onSuccess, onFail);
        }

        ~AbstractFirebaseController() {
            Unsubscribe();
        }
    }
}
