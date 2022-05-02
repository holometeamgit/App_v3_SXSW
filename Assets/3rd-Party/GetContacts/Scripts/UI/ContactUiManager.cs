using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ContactListMobile
{
    public class ContactUiManager : MonoBehaviour
    {
        [SerializeField] private ContactManager _contactManager;
        [SerializeField] private GameObject _contactElementUi;

        [SerializeField] private Transform _contactsContent;
        [SerializeField] private Button _fetchButton;

        [SerializeField] private Slider _loadingBar;
        [SerializeField] private Text _loadingBarText;
        public void OnEnable()
        {
            _fetchButton.onClick.AddListener(FetchAndCreate);
        }
        
        public void OnDisable()
        {
            _fetchButton.onClick.RemoveAllListeners();
        }
        
        private void FetchAndCreate()
        {
            _contactManager.FetchContacts();
            
            // StopCoroutine(CreateAsync());
            StartCoroutine(CreateAsync());
        }

        private IEnumerator CreateAsync()
        {
            var contactsCount = _contactManager.GetContacts().ContactList.Count;
            Debug.Log(contactsCount);
            for (var i = 0; i < contactsCount; i++)
            {
                var contact = _contactManager.GetContacts().ContactList[i];
                Debug.Log(i);
                
                _loadingBar.value = (float)i / contactsCount;
                _loadingBarText.text = "Loading (" + i + "/" + contactsCount + ")";
                
                var go = Instantiate(_contactElementUi, _contactsContent);
                go.GetComponent<ContactUiElement>().Set(contact);
                
                yield return null;
            }
            
            _loadingBarText.text = "Done!";
        }
    }
}
