using UniverseRift.Models.Common;

namespace UniverseRift.GameModelDatas.Players
{
    public class PlayerData
    {
        public int Id { get; set; }
        public string Name = string.Empty;
        public int Level = 1;
        public int VipLevel;
        public int GuildId;
        public string AvatarPath;

        public PlayerData() { }

        public PlayerData(Player player)
        {
            Id = player.Id;
            Name = player.Name;
            Level = player.Level;
            VipLevel = player.VipLevel;
            AvatarPath = player.AvatarPath;
            GuildId = player.GuildId;
        }
    }
}