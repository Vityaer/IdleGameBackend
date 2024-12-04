namespace UniverseRift.Controllers.Buildings.Guilds
{
    public class TestSendModel
    {
        public int PlayerId { get;set; }
        public string Name { get; set; } = string.Empty;

        public TestSendModel()
        {
        }

        public TestSendModel(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
        //{"PlayerId":"123","Name":"qwerty"}
}
}
