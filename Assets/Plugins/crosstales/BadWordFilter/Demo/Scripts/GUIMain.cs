using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crosstales.BWF.Demo
{
   /// <summary>Main GUI controller.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_demo_1_1_g_u_i_main.html")]
   public class GUIMain : MonoBehaviour
   {
      #region Variables

      public bool AutoTest = true;
      public bool AutoReplace = false;
      public bool ReplaceLeet = true;
      public bool SimpleCheck = true;

      public float IntervalCheck = 0.5f;
      public float IntervalReplace = 0.5f;

      public InputField Text;
      public Text OutputText;

      public Text BadWordList;
      public Text BadWordCounter;

      public Text Name;
      public Text Version;
      public Text Scene;

      public Toggle TestEnabled;
      public Toggle ReplaceEnabled;

      public Toggle Badword;
      public Toggle Domain;
      public Toggle Capitalization;
      public Toggle Punctuation;

      public InputField BadwordReplaceChars;
      public InputField DomainReplaceChars;
      public InputField CapsTrigger;
      public InputField PuncTrigger;

      public Toggle LeetReplace;
      public Toggle SimpleCheckToggle;

      public Image BadWordListImage;

      public Color32 GoodColor = new Color32(0, 255, 0, 192);
      public Color32 BadColor = new Color32(255, 0, 0, 192);


      public Model.ManagerMask BadwordManager = Model.ManagerMask.BadWord;
      public Model.ManagerMask DomManager = Model.ManagerMask.Domain;
      public Model.ManagerMask CapsManager = Model.ManagerMask.Capitalization;
      public Model.ManagerMask PuncManager = Model.ManagerMask.Punctuation;

      public System.Collections.Generic.List<string> Sources = new System.Collections.Generic.List<string>(30);

      private System.Collections.Generic.List<string> badWords = new System.Collections.Generic.List<string>();

      private float elapsedTimeCheck = 0f;
      private float elapsedTimeReplace = 0f;

      private bool tested = false;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         if (Name != null)
            Name.text = Util.Constants.ASSET_NAME;

         if (Version != null)
            Version.text = Util.Constants.ASSET_VERSION;

         if (Scene != null)
         {
            Scene.text = SceneManager.GetActiveScene().name;
         }

         if (!AutoTest && TestEnabled != null)
            TestEnabled.isOn = false;

         if (!AutoReplace && ReplaceEnabled != null)
            ReplaceEnabled.isOn = false;

         if (BadwordManager != Model.ManagerMask.BadWord && Badword != null)
            Badword.isOn = false;

         if (DomManager != Model.ManagerMask.Domain && Domain != null)
            Domain.isOn = false;

         if (CapsManager != Model.ManagerMask.Capitalization && Capitalization != null)
            Capitalization.isOn = false;

         if (PuncManager != Model.ManagerMask.Punctuation && Punctuation != null)
            Punctuation.isOn = false;

         Manager.BadWordManager.isReplaceLeetSpeak = ReplaceLeet;
         if (!ReplaceLeet && LeetReplace != null)
            LeetReplace.isOn = false;

         Manager.BadWordManager.isSimpleCheck = SimpleCheck;
         if (!SimpleCheck && SimpleCheckToggle != null)
            SimpleCheckToggle.isOn = false;

         if (BadwordReplaceChars != null)
            BadwordReplaceChars.text = Manager.BadWordManager.ReplaceCharacters;

         if (DomainReplaceChars != null)
            DomainReplaceChars.text = Manager.DomainManager.ReplaceCharacters;

         if (CapsTrigger != null)
            CapsTrigger.text = Manager.CapitalizationManager.CharacterNumber.ToString();

         if (PuncTrigger != null)
            PuncTrigger.text = Manager.PunctuationManager.CharacterNumber.ToString();

         if (BadWordList != null)
            BadWordList.text = badWords.Count > 0 ? string.Empty : "Not tested";

         if (Text != null)
            Text.text = string.Empty;
      }

      public void Update()
      {
         elapsedTimeCheck += Time.deltaTime;
         elapsedTimeReplace += Time.deltaTime;

         if (AutoTest && !AutoReplace && elapsedTimeCheck > IntervalCheck)
         {
            Test();

            elapsedTimeCheck = 0f;
         }

         if (AutoReplace && elapsedTimeReplace > IntervalReplace)
         {
            Replace();

            elapsedTimeReplace = 0f;
         }

         if (BadwordReplaceChars != null)
            Manager.BadWordManager.ReplaceCharacters = BadwordReplaceChars.text;

         if (DomainReplaceChars != null)
            Manager.DomainManager.ReplaceCharacters = DomainReplaceChars.text;

         int number;
         bool res;

         if (CapsTrigger != null)
         {
            res = int.TryParse(CapsTrigger.text, out number);
            Manager.CapitalizationManager.CharacterNumber = res ? number > 2 ? number : 2 : 2;
            CapsTrigger.text = Manager.CapitalizationManager.CharacterNumber.ToString();
         }

         if (PuncTrigger != null)
         {
            res = int.TryParse(PuncTrigger.text, out number);
            Manager.PunctuationManager.CharacterNumber = res ? number > 2 ? number : 2 : 2;
            PuncTrigger.text = Manager.PunctuationManager.CharacterNumber.ToString();
         }

         if (tested)
         {
            if (badWords.Count > 0)
            {
               BadWordList.text = string.Join(System.Environment.NewLine, badWords.ToArray());
               BadWordListImage.color = BadColor;
            }
            else
            {
               BadWordList.text = "No bad words found";
               BadWordListImage.color = GoodColor;
            }
         }

         if (BadWordCounter != null)
            BadWordCounter.text = badWords.Count.ToString();

         if (OutputText != null)
            OutputText.text = BWFManager.Mark(Text.text, badWords);
      }

      #endregion


      #region Public methods

      public void TestChanged(bool val)
      {
         AutoTest = val;
      }

      public void ReplaceChanged(bool val)
      {
         AutoReplace = val;
      }

      public void BadwordChanged(bool val)
      {
         BadwordManager = val ? Model.ManagerMask.BadWord : Model.ManagerMask.None;
      }

      public void DomainChanged(bool val)
      {
         DomManager = val ? Model.ManagerMask.Domain : Model.ManagerMask.None;
      }

      public void CapitalizationChanged(bool val)
      {
         CapsManager = val ? Model.ManagerMask.Capitalization : Model.ManagerMask.None;
      }

      public void PunctuationChanged(bool val)
      {
         PuncManager = val ? Model.ManagerMask.Punctuation : Model.ManagerMask.None;
      }

      public void LeetChanged(bool val)
      {
         Manager.BadWordManager.isReplaceLeetSpeak = val;
      }

      public void SimpleChanged(bool val)
      {
         Manager.BadWordManager.isSimpleCheck = val;
      }

      public void FullscreenChanged(bool val)
      {
         Screen.fullScreen = val;
      }

      public void Test()
      {
         tested = true;

         badWords = BWFManager.GetAll(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());
      }

      public void Replace()
      {
         tested = true;

         Text.text = BWFManager.ReplaceAll(Text.text, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());

         badWords.Clear();
      }

      public void OpenAssetURL()
      {
         Application.OpenURL(Util.Constants.ASSET_CT_URL);
      }

      public void OpenCTURL()
      {
         Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
      }

      public void Quit()
      {
         if (Application.isEditor)
         {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
         }
         else
         {
            Application.Quit();
         }
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)