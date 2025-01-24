using Carica.Core.Configs;

namespace Carica.Core.AssetManagement
{
    public interface IStaticDataProvider
    {
        public IUIConfig UIConfig { get; }
    }
}