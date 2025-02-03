namespace Fog.Level
{
    public class SimpleStart : SimplePlate
    {
        protected override void Start()
        {
            base.Start();
            _createNextCallback(this);
        }
    }
}