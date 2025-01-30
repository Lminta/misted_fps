using Carica.Core.Configs;

namespace Carica.Core.Services.AssetManagement
{
    public interface IStaticDataProvider
    {
        public IUIConfig UIConfig { get; }
    }
}