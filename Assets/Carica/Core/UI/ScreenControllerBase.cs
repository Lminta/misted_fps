using UnityEngine;

namespace Carica.Core.UI
{
    public abstract class ScreenControllerBase : MonoBehaviour
    {
        [SerializeField] protected ScreenViewBase _screenView;
        
        public virtual void Setup<TPayload>(TPayload payload) where TPayload : IScreenPayload
        {
            
        }

        public virtual void Open()
        {
            _screenView.Open();
        }

        public virtual void Close()
        {
            _screenView.Close();
        }
    }
}