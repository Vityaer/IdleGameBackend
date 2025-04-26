using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.City.TaskBoards;
using UniverseRift.Models.Tasks.SimpleTask;

namespace UniverseRift.Models.Guilds
{
    [Serializable]
    public class GuildPlayerSaveContainer
    {
        public GuildData GuildData = new();
        public List<RecruitData> GuildRecruits = new();
        public List<GuildPlayerRequest> Requests = new();
		public TaskBoardData TasksData = new();

		public GuildPlayerSaveContainer()
        {
        }

        public GuildPlayerSaveContainer(GuildData guildData, List<RecruitData> guildRecruits, List<GameTask> newTasks)
        {
            GuildData = guildData;
            GuildRecruits = guildRecruits;


			TasksData ??= new TaskBoardData();

			foreach (var task in newTasks)
			{
				TasksData.ListTasks.Add(new TaskData(task));
			}

		}
    }
}
