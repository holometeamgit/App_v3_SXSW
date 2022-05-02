using System;
using System.Collections.Generic;
using UnityEngine;

namespace ContactListMobile
{
    [Serializable]
    public class Contact
    {
        public string GivenName;
        public string FamilyName;

        [Serializable]
        public class PhoneNumber
        {
            public string Label;
            public string Value;
        }

        [SerializeField] public List<PhoneNumber> PhoneNumbers = new List<PhoneNumber>();
    }

    [Serializable]
    public class Contacts
    {
        [SerializeField] public List<Contact> ContactList = new List<Contact>();
    }
}
