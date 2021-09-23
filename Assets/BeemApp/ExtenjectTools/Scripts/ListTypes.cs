using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Beem.Extenject {
    /// <summary>
    /// Type sheet of inherited classes
    /// </summary>
    public static class ListTypes {
        public static IEnumerable<Type> GetTypes<T>() {
            Type ourtype = typeof(T);
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));  // using System.Linq
            return list;
        }
    }
}
