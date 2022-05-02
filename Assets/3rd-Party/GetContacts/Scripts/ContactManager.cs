using System.IO;
using UnityEngine;

namespace ContactListMobile {
    public class ContactManager : MonoBehaviour {
        [SerializeField] private Contacts _contacts;

        public Contacts GetContacts() { return _contacts; }

        public void FetchContacts() {
            var jsonFilePath = iOSContactsListPlugin.GetAllContacts();

            if (!string.IsNullOrEmpty(jsonFilePath)) {
                using var streamReader = File.OpenText(jsonFilePath);
                var result = streamReader.ReadToEnd();
                Debug.Log(result);
                _contacts = JsonUtility.FromJson<Contacts>(result);
            }
        }
    }
}
