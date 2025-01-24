using UnityEngine;

namespace Carica.Core.UI
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        
        public Transform MainCanvasRoot => _mainCanvas.transform;
    }
}