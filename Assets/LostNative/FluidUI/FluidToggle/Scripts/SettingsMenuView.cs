using JetBrains.Annotations;
using UnityEngine;

namespace LostNative.Toolkit.FluidUI.Examples
{
    public class SettingsMenuView : MonoBehaviour
    {
        [SerializeField] private FluidToggle musicToggle;
        [SerializeField] private FluidToggle sfxToggle;
        [SerializeField] private FluidToggle notificationsToggle;
        [SerializeField] private FluidToggle hintsToggle;

        void Start()
        {
            musicToggle.OnToggle += MusicToggle_OnToggle;
            sfxToggle.OnToggle += SfxToggle_OnToggle;
            notificationsToggle.OnToggle += NotificationsToggle_OnToggle;
            hintsToggle.OnToggle += HintsToggle_OnToggle;
        }

        private void MusicToggle_OnToggle(bool optionASelected)
        {
            var optionASelectedString = (optionASelected) ? "Music off" : "Music on";
            Debug.Log(optionASelectedString);
        }

        private void SfxToggle_OnToggle(bool optionASelected)
        {
            var optionASelectedString = (optionASelected) ? "SFX off" : "SFX on";
            Debug.Log(optionASelectedString);
        }

        private void NotificationsToggle_OnToggle(bool optionASelected)
        {
            var optionASelectedString = (optionASelected) ? "Notifications off" : "Notifications on";
            Debug.Log(optionASelectedString);
        }

        private void HintsToggle_OnToggle(bool optionASelected)
        {
            var optionASelectedString = (optionASelected) ? "Hints off" : "Hints on";
            Debug.Log(optionASelectedString);
        }

        private void RemoveListeners()
        {
            musicToggle.OnToggle -= MusicToggle_OnToggle;
            sfxToggle.OnToggle -= SfxToggle_OnToggle;
            notificationsToggle.OnToggle -= NotificationsToggle_OnToggle;
            hintsToggle.OnToggle -= HintsToggle_OnToggle;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        [UsedImplicitly]
        public void OnOkClick()
        {
            Debug.Log("Ok clicked!");
        }

        [UsedImplicitly]
        public void OnCloseClick()
        {
            Debug.Log("Close clicked!");
        }
    }
}