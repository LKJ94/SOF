using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

namespace SOF.Scripts.UI
{
    public class UIManager : ETC.Singleton<UIManager>
    {
        private GameObject _root;

        public GameObject Root
        {
            get
            {
                _root = GameObject.Find("UIRoot");
                if (_root == null)
                    _root = new("UIRoot");
                return _root;
            }
        }

        private UIScene _sceneUI;
        public UIScene SceneUI => _sceneUI;

        private int _popUpOrder = 10;
        private Stack<UIPopUp> _popUpStack = new();
        public int PopUpCount => _popUpStack.Count;

        public void SetCanvas(GameObject obj, int? sortOrder = 0)
        {
            Canvas canvas = obj.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            CanvasScaler scaler = obj.GetOrAddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new(1920, 1080);

            obj.GetOrAddComponent<GraphicRaycaster>();

            if (!sortOrder.HasValue)
            {
                canvas.sortingOrder = _popUpOrder;
                _popUpOrder++;
            }
            else
                canvas.sortingOrder = sortOrder.Value;
        }

        public void SetEventSystem(GameObject obj)
        {
            obj.GetOrAddComponent<EventSystem>();
            obj.GetOrAddComponent<InputSystemUIInputModule>();
        }

        protected override void Initialize()
        {
            SetCanvas(Root);
            SetEventSystem(gameObject);

            // ShowSceneUI<UISceneTest>();
        }

        public T ShowScene<T>(string name = null) where T : UIScene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            // 나중에 Addressable에서 Load한 프리팹으로 바꿔야함
            GameObject obj = Resources.Load<GameObject>($"UI/{name}");
            obj = Instantiate(obj, Root.transform);

            //GameObject obj = ResourceManager.Instantiate($"{name}.prefab");
            //obj.transform.SetParent(Root.transform);

            _sceneUI = obj.GetOrAddComponent<T>();
            return _sceneUI as T;
        }

        public T ShowPopUpUI<T>(string name = null) where T : UIPopUp
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject obj = Resources.Load<GameObject>($"UI/{name}");
            obj = Instantiate(obj, Root.transform);

            //GameObject obj = ResourceManager.Instantiate($"{name}.prefab");
            //obj.transform.SetParent(Root.transform);

            T popUp = obj.GetOrAddComponent<T>();
            _popUpStack.Push(popUp);

            //RefreshTimeScale();

            return popUp;
        }

        public void ClosePopUp(UIPopUp popUp)
        {
            if (_popUpStack.Count == 0)
                return;
            if (_popUpStack.Peek() != popUp)
            {
                Debug.LogError($"[UIManager] CloesePopUp({popUp.name}) : 팝업창을 닫는데 실패");
                return;
            }
            ClosePopUp();
        }

        public void ClosePopUp()
        {
            if (_popUpStack.Count == 0)
                return;

            UIPopUp popUp = _popUpStack.Pop();
            // 나중에 ResourceManager에서 Destroy 해야함
            Destroy(popUp.gameObject);
            //ResourceManager.Destroy(popUp);
            _popUpOrder--;
            //RefreshTimeScale();
        }

        public void CloseAllPopup()
        {
            while (_popUpStack.Count > 0)
                ClosePopUp();
        }
    }
}