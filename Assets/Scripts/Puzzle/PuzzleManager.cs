using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public struct OnPuzzleInitializedEvent {}
    public struct OnPuzzleSolvedEvent {}
    public struct OnPuzzleExitEvent {}
    public partial class PuzzleManager : MonoBehaviour , ISingleton
    {
        public enum States
        {
            None,
            InActive,
            Active
        }

        public static PuzzleManager Instance => SingletonProperty<PuzzleManager>.Instance;
        public void OnSingletonInit() {}
        public static FSM<States> StateMachine => Instance.stateMachine;
        public FSM<States> stateMachine = new FSM<States>();

        private CanvasGroup canvasGroup;

        public static PuzzleBase CurrentPuzzle = null;
        public Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            TypeEventSystem.Global.Register<OnPuzzleSolvedEvent>(e => {
                if (CurrentPuzzle != null) CurrentPuzzle.OnComplete();
            });
            TypeEventSystem.Global.Register<OnPuzzleExitEvent>(e => {
                if (CurrentPuzzle != null) CurrentPuzzle.OnExit();

                StateMachine.ChangeState(States.None);
            });

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                if (CurrentPuzzle != null) StateMachine.ChangeState(States.InActive);
            });
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                if (CurrentPuzzle != null) StateMachine.ChangeState(States.Active);
            });

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.Active, new ActiveState(stateMachine, this));
            stateMachine.AddState(States.InActive, new InActiveState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        public static void Initialize(string id)
        {
            var data = GameDesignData.GetPuzzleDataById(id);
            CurrentPuzzle = Instantiate(data.PuzzlePrefab, Instance.transform).GetComponent<PuzzleBase>();

            CurrentPuzzle.OnEnter();

            StateMachine.ChangeState(States.Active);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.I)) Initialize("sample");
            if (CurrentPuzzle != null)
            {
                CurrentPuzzle.OnUpdate();
            }
        }
    }

    public partial class PuzzleManager
    {
        public class NoneState : AbstractState<States, PuzzleManager>
        {
            public NoneState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                mTarget.StartCoroutine(OnEnterCoroutine());
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));
                if (CurrentPuzzle != null) Destroy(CurrentPuzzle.gameObject);
                
                CurrentPuzzle = null;
                mTarget.CurrentCoroutine = null;
            }
        }

        public class ActiveState : AbstractState<States, PuzzleManager>
        {
            public ActiveState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.Active;

            protected override void OnEnter()
            {
                mTarget.StartCoroutine(OnEnterCoroutine());

                mTarget.canvasGroup.interactable = true;
                foreach (var col in mTarget.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = true;
                }
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

                mTarget.CurrentCoroutine = null;
            }
        }

        public class InActiveState : AbstractState<States, PuzzleManager>
        {
            public InActiveState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.InActive;

            protected override void OnEnter()
            {
                mTarget.canvasGroup.interactable = false;
                foreach (var col in mTarget.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }
            }
        }
    }
}