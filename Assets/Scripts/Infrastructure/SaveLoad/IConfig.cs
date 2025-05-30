namespace Wattle.Wild.Infrastructure
{
    public interface IConfig
    {
        public string FileName { get; }
        public void Deserialize(string data);
        public string Serialize();
    }
}
