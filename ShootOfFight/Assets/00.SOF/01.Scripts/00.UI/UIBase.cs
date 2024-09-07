//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//namespace SOF.Scripts.UI
//{
//    public class UIBase : MonoBehaviour
//    {
//        private Dictionary<Type, UnityEngine.Object[]> _objects = new();
//        private bool _isInitialized = false;

//        private void Start()
//        {
//            Initialize();
//        }

//        public virtual bool Initialize()
//        {
//            if (_isInitialized)
//                return false;
//            _isInitialized = true;
//            return true;
//        }

//        void Bind<T>(Type type) where T : UnityEngine.Object
//        {
//            string[] names = Enum.GetNames(type);
//            var objs = new UnityEngine.Object[names.Length];
//            _objects.Add(typeof(T), objs);

//            for (int i = 0; i < names.Length; i++)
//            {
//                if (typeof(T) == typeof(GameObject))
//                    objs[i] = Utilities.FindChild(gameObject, names[i], true);
//                else
//                    objs[i] = Utilities.FindChild<T>(gameObject, names[i], true);
//                if (objs[i] == null)
//                    Debug.LogError($"[UI Base : {name}] 바인드 실패 : {names[i]}");
//            }
//        }

//        protected void BindObject(Type type) => Bind<GameObject>(type);
//        protected void BindImage(Type type) => Bind<Image>(type);
//        protected void BindText(Type type) => Bind<Text>(type);
//        protected void BindButton(Type type) => Bind<Button>(type);

//        private T Get<T>(int index) where T : UnityEngine.Object
//        {
//            if (!_objects.TryGetValue(typeof(T), out var objs))
//                return null;
//            return objs[index] as T;
//        }

//        protected GameObject GetObject(int index) => Get<GameObject>(index);
//        protected Image GetImage(int index) => Get<Image>(index);
//        protected TextMeshProUGUI GetText(int index) => Get<TextMeshProUGUI>(index);
//        protected Button GetButton(int index) => Get<Button>(index);

//        public static void BindEvent(GameObject obj, Action action = null, Action<BaseEventData> dragAction = null, UIEvnet type = UIEvent.Click)
//        {
//            UIEventHandler uIEventHandler = utilities.GetOrAddComponent<UIEventHandler>(obj);

//            switch (type)
//            {
//                case UIEvent.CLick:
//                    uIEventHandler.onClickHandler -= action;
//                    uIEventHandler.onClickHandler += action;
//                    break;
//                case UIEvent.Pressed:
//                    uIEventHandler.onPointerPressedHandler -= action;
//                    uIEventHandler.onPointerPressedHandler += action;
//                    break;
//                case UIEvnet.PointerUp:
//                    uIEventHandler.onClickHandler -= action;
//                    uIEventHandler.onClickHandler += action;
//                    break;
//                case UIEvnet.PointerDown:
//                    uIEventHandler.onClickHandler -= action;
//                    uIEventHandler.onClickHandler += action;
//                    break;
//                case UiEvent.Drag:
//                    uIEventHandler.onDragHandler -= dragAction;
//                    uIEventHandler.onDragHandler += dragAction;
//                    break;
//                case UIEvent.BeginDrag:
//                    uIEventHandler.onBeginDragHandler -= dragAction;
//                    uIEventHandler.onBeginDragHandler += dragAction;
//                case UIEvent.EndDrag:
//                    uIEventHandler.onEndDragHandler -= dragAction;
//                    uIEventHandler.onEndDragHandler += dragAction;
//                    break;

//            }
//        }
//    }

//    public class UIEventHandler : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
//    {
//        public Action onClickHandler = null;
//        public Action onPointerPressedHandler = null;
//        public Action onPointerDownHandler = null;
//        public Action onPointerUpHandler = null;
//        public Action<BaseEventData> onDragHandler = null;
//        public Action<BaseEventData> onBeginDragHandler = null;
//        public Action<BaseEventData> onEndDragHandler = null;

//        private bool pressed = false;

//        private void Update()
//        {
//            if (pressed)
//                onPointerPressedHandler?.Invoke();
//        }

//        public void OnPointerClick(PointerEventData eventData)
//        {
//            onClickHandler?.Invoke();
//        }

//        public void OnPointerUp(PointerEventData eventData)
//        {
//            pressed = true;
//            onPointerUpHandler?.Invoke();
//        }

//        public void OnPointerDown(PointerEventData eventData)
//        {
//            pressed = false;
//            onPointerDownHandler?.Invoke();
//        }

//        public void OnDrag(PointerEventData eventData)
//        {
//            onDragHandler?.Invoke(eventData);
//        }

//        public void OnBeginDrag(PointerEventData eventData)
//        {
//            onBeginDragHandler?.Invoke(eventData);
//        }

//        public void OnEndDrag(PointerEventData eventData)
//        {
//            onEndDragHandler?.Invoke(eventData);
//        }
//    }
//}
