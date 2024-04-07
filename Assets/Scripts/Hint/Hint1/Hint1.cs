using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hint.Hint1
{
    public class Hint1 : HintBase , ISingleton
    {
        public static Hint1 Instance => SingletonProperty<Hint1>.Instance;
        public void OnSingletonInit() {}

        private Button backButton;

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                TypeEventSystem.Global.Send<OnHintExitEvent>();
            });
            
            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}