namespace API.Services
{
    public class Service
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int SynchronizationFrequency { get; set; }
        public int HeartBeatFrequency { get; set; }
        public string MainServerHost { get; set; }
        public int MainServerPort { get; set; }
        public int ConnectionTimeout { get; set; }
    }
}