using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameUI
{
    /// <summary>
    /// A dynamic UI element that pops up and provides selectable options
    /// </summary>
    public class Prompt : MonoBehaviour
    {
        #region STATIC_VARS

        #endregion

        #region INSTANCE_VARS

        [SerializeField]
        protected Text description;
        [SerializeField]
        protected Transform optionList;
        #endregion

        #region STATIC_METHODS

        /// <summary>
        /// Create a basic prompt that has only an "Okay" option, which closes the prompt.
        /// Nice for errors.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Prompt Create(RectTransform parent, string description)
        {
            return Create (parent, description, new Option ("Okay", null));
        }

        /// <summary>
        /// Create a prompt with two options: "Yes" and "No"
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="description"></param>
        /// <param name="accept"></param>
        /// <param name="decline"></param>
        /// <returns></returns>
        public static Prompt Create(RectTransform parent, string description, UnityAction accept, UnityAction decline)
        {
            return Create (parent, description, new Option("Yes", accept), new Option ("No", decline));
        }

        /// <summary>
        /// Create a prompt with any number of options
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="description"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Prompt Create(RectTransform parent, string description, params Option[] options)
        {
            Prompt p = Setup (parent, description);

            foreach (Option o in options)
                p.AddOption (o);

            return p;
        }

        /// <summary>
        /// Creates a prompt with no options, but closes after a duration has passed
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="description"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Prompt Create(RectTransform parent, string description, float duration)
        {
            Prompt p = Setup (parent, description);

            Destroy (p.gameObject, duration);

            return p;
        }

        protected static Prompt Setup(RectTransform parent, string description)
        {
            GameObject pref = Resources.Load<GameObject> ("Prefabs/UI/PromptBase"); //TODO create this prefab
            GameObject inst = Instantiate (pref, parent, false);
            Prompt p = inst.GetComponent<Prompt> ();

            p.description.text = description;

            return p;
        }

        #endregion

        #region INSTANCE_METHODS

        public void AddOption(Option option)
        {
            GameObject butPref = Resources.Load<GameObject> ("Prefabs/UI/GeneralButton"); //TODO create this prefab
            GameObject inst = Instantiate (butPref, optionList, false);
            inst.transform.GetChild (0).GetComponent<Text> ().text = option.Text;
            if (option.Function == null)
                option.Function = DestroySelf;
            inst.GetComponent<Button> ().onClick.AddListener (option.Function);
        }

        private void DestroySelf()
        {
            Destroy (gameObject);
        }
        #endregion

        #region INTERNAL_TYPES

        /// <summary>
        /// Pairs a string and a UnityAction
        /// </summary>
        public struct Option
        {
            public string Text { get; set; }
            public UnityAction Function { get; set; }

            public Option(string text, UnityAction function)
            {
                Text = text;
                Function = function;
            }
        }
        #endregion
    }
}
