using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Carica.Core.UI;
using UnityEngine;

namespace Carica.Core.Configs
{
    [CreateAssetMenu(menuName = "Content/Configs/UI Config", fileName = "UIConfig")]
    public class UIConfig : ScriptableObject, IUIConfig
    {
        [SerializeField] private bool _lazyLoad = false;
        [SerializeField] private bool _reUse = true;
        [SerializeField] private SerializedDictionary<ScreenID, string> _screenReference = new ();
        
        public Dictionary<ScreenID, string>  ScreenReference => _screenReference;
        public bool LazyLoad => _lazyLoad;
        public bool ReUse => _reUse;
    }
}