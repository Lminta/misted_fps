using System.Collections.Generic;
using Carica.Core.UI;

namespace Carica.Core.Configs
{
    public interface IUIConfig
    {
        public Dictionary<ScreenID, string>  ScreenReference { get; }
        public bool LazyLoad { get; }
        public bool ReUse { get; }
    }
}