using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ContactListMobile
{
   public class ContactUiElement : MonoBehaviour
   {
      [SerializeField] private Text _letter;
      [SerializeField] private Text _givenNameAndFamilyName;
      [SerializeField] private Text _phoneNumber;

      public void Set(Contact contact)
      {
         _letter.text = contact.GivenName.Length > 0 ? contact.GivenName[0].ToString() : "-";

         _givenNameAndFamilyName.text = contact.GivenName + " " + contact.FamilyName;

         if (contact.PhoneNumbers.Count > 0)
         {
            _phoneNumber.text = contact.PhoneNumbers[0].Value;
         }
      }
   }
}