using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Misc.Friends;
using UniverseRift.Models.Resources;
using UniverseRift.Models.Results;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Misc.Friendships
{
    public class FriendshipController : ControllerBase, IFriendshipController
    {
        private const int MAX_PLAYER_FRIEND_COUNT = 30;
        private const int AVAILABLE_FRIEND_COUNT = 30;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IJsonConverter _jsonConverter;
        private readonly IAchievmentController _achievmentController;

        public FriendshipController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IJsonConverter jsonConverter,
            IAchievmentController achievmentController
            )
        {
            _jsonConverter = jsonConverter;
            _clientRewardService = clientRewardService;
            _context = context;
            _commonDictionaries = commonDictionaries;
            _achievmentController = achievmentController;
        }

        public async Task OnPlayerRegister(int playerId)
        {
            var friend = new PlayerAsFriendData { PlayerId = playerId };
            await _context.PlayerAsFriendDatas.AddAsync(friend);
            await _context.SaveChangesAsync();
        }

        public async Task GetPlayerSave(int playerId, CommunicationData result)
        {
            var allPlayers = await _context.Players.ToListAsync();

            var allRequsts = await _context.FriendshipRequests.ToListAsync();
            result.FriendshipRequests = allRequsts.FindAll(request => request.TargetPlayerId == playerId);

            foreach (var request in result.FriendshipRequests)
            {
                var player = allPlayers.Find(player => player.Id == request.SenderPlayerId);
                if (player == null)
                    continue;

                result.AddPlayerData(new PlayerData(player));
            }

            var allFriendships = await _context.FriendshipDatas.ToListAsync();
            result.FriendshipDatas = allFriendships
                .FindAll(
                    friendship => friendship.FirstPlayerId == playerId
                    || friendship.SecondPlayerId == playerId
                    );

            foreach (var friendship in result.FriendshipDatas)
            {
                var friendId = (friendship.FirstPlayerId == playerId) ? friendship.SecondPlayerId : friendship.FirstPlayerId;
                var friend = allPlayers.Find(player => player.Id == friendId);

                if (friend == null)
                    continue;

                result.AddPlayerData(new PlayerData(friend));
            }

            var requestPlayer = await _context.Players.FindAsync(playerId);
            if (requestPlayer != null)
            {
                result.AddPlayerData(new PlayerData(requestPlayer));

                if (requestPlayer.GuildId >= 0)
                {
                    var allRecruits = await _context.RecruitDatas.ToListAsync();
                    var guildRecruits = allRecruits.FindAll(recruit => recruit.GuildId == requestPlayer.GuildId);
                    foreach (var recruit in guildRecruits)
                    {
                        var player = allPlayers.Find(player => player.Id == recruit.PlayerId);
                        if (player == null)
                            continue;

                        result.AddPlayerData(new PlayerData(player));
                    }
                }
            }
        }

        [HttpPost]
        [Route("Friendship/GetAvailableFriends")]
        public async Task<AnswerModel> GetAvailableFriends(int playerId)
        {
            var answer = new AnswerModel();

            var allPlayer = await _context.Players.ToListAsync();
            var allPlayerAsFriends = await _context.PlayerAsFriendDatas.ToListAsync();
            var allFriendships = await _context.FriendshipDatas.ToListAsync();

            var playerFriendships = allFriendships.FindAll(x => x.FirstPlayerId == playerId || x.SecondPlayerId == playerId);

            List<AvailableFriendData> availableFriends = new();
            foreach (var playerAsFriend in allPlayerAsFriends)
            {
                if (playerAsFriend.PlayerId != playerId && playerAsFriend.CurrentFriendCount < MAX_PLAYER_FRIEND_COUNT)
                {
                    var player = allPlayer.Find(player => player.Id == playerAsFriend.PlayerId);
                    if (player == null)
                        continue;

                    if (player.IsBot)
                        continue;
                    
                    var suitableFriendship = playerFriendships.Find(x => x.FirstPlayerId == playerAsFriend.PlayerId || x.SecondPlayerId == playerAsFriend.PlayerId);
                    if (suitableFriendship != null)
                        continue;

                    var data = new AvailableFriendData()
                    {
                        PlayerId = playerAsFriend.PlayerId,
                        Name = player.Name,
                        IconPath = player.AvatarPath,
                        Level = player.Level,
                        GuildId = player.GuildId,
                    };
                    availableFriends.Add(data);

                    if (availableFriends.Count == AVAILABLE_FRIEND_COUNT)
                        break;
                }
            }

            answer.Result = _jsonConverter.Serialize(availableFriends);
            return answer;
        }

        [HttpPost]
        [Route("Friendship/CreateFriendRequest")]
        public async Task<AnswerModel> CreateFriendRequest(int playerId, int otherPlayerId)
        {
            var answer = new AnswerModel();
            var player = await _context.PlayerAsFriendDatas.FindAsync(playerId);

            if (player == null || player.CurrentFriendCount >= MAX_PLAYER_FRIEND_COUNT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var otherPlayer = await _context.PlayerAsFriendDatas.FindAsync(otherPlayerId);
            if (otherPlayer == null || otherPlayer.CurrentFriendCount >= MAX_PLAYER_FRIEND_COUNT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var friendshipRequests = await _context.FriendshipRequests.ToListAsync();
            var similarRequest = friendshipRequests
                .Find(
                request =>
                request.SenderPlayerId == playerId && request.TargetPlayerId == otherPlayerId
                || request.SenderPlayerId == otherPlayerId && request.TargetPlayerId == playerId);

            if (similarRequest != null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var request = new FriendshipRequest
            {
                SenderPlayerId = playerId,
                TargetPlayerId = otherPlayerId,
                Date = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat)
            };

            await _context.FriendshipRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Friendship/GetAllFriendRequests")]
        public async Task<AnswerModel> GetAllFriendRequests(int playerId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var requests = await _context.FriendshipRequests.ToListAsync();
            var similarRequests = requests.FindAll(request => request.TargetPlayerId == playerId);
            answer.Result = _jsonConverter.Serialize(similarRequests);
            return answer;
        }

        [HttpPost]
        [Route("Friendship/ApplyFriendRequest")]
        public async Task<AnswerModel> ApplyFriendRequest(int playerId, int requestId)
        {
            var answer = new AnswerModel();
            var player = await _context.PlayerAsFriendDatas.FindAsync(playerId);

            if (player == null || player.CurrentFriendCount >= MAX_PLAYER_FRIEND_COUNT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var request = await _context.FriendshipRequests.FindAsync(requestId);
            if (request == null || request.TargetPlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var otherPlayer = await _context.PlayerAsFriendDatas.FindAsync(request.SenderPlayerId);
            if (otherPlayer == null || otherPlayer.CurrentFriendCount >= MAX_PLAYER_FRIEND_COUNT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var friendship = new FriendshipData
            {
                FirstPlayerId = request.SenderPlayerId,
                SecondPlayerId = request.TargetPlayerId
            };

            _context.FriendshipRequests.Remove(request);

            player.CurrentFriendCount += 1;
            otherPlayer.CurrentFriendCount += 1;

            await _context.FriendshipDatas.AddAsync(friendship);
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Friendship/DenyFriendRequest")]
        public async Task<AnswerModel> DenyFriendRequest(int playerId, int requestId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var request = await _context.FriendshipRequests.FindAsync(requestId);
            if (request == null || request.TargetPlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            _context.FriendshipRequests.Remove(request);
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Friendship/SendAndReceivedHeartAllFriends")]
        public async Task<AnswerModel> SendAndReceivedHeartAllFriends(int playerId)
        {
            var answer = new AnswerModel();

            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var friendships = await _context.FriendshipDatas.ToListAsync();

            var targetFriendships = friendships.FindAll(friendship => friendship.FirstPlayerId == playerId || friendship.SecondPlayerId == playerId);
            var recievedCountHeart = 0;
            var sendedCountHeart = 0;

            var container = new HeartsContainer();

            foreach (var targetFriendship in targetFriendships)
            {
                if (targetFriendship == null)
                    continue;

                if (targetFriendship.FirstPlayerId == playerId)
                {
                    if (!targetFriendship.PresentForSecondPlayer)
                    {
                        sendedCountHeart += 1;
                        targetFriendship.PresentForSecondPlayer = true;
                        container.SendedHeartIds.Add(targetFriendship.SecondPlayerId);
                    }

                    if (targetFriendship.PresentForFirstPlayer && !targetFriendship.FirstPlayerRecieved)
                    {
                        recievedCountHeart += 1;
                        targetFriendship.FirstPlayerRecieved = true;
                        container.ReceivedHeartIds.Add(targetFriendship.SecondPlayerId);
                    }

                }
                else if (targetFriendship.SecondPlayerId == playerId)
                {
                    if (!targetFriendship.PresentForFirstPlayer)
                    {
                        sendedCountHeart += 1;
                        targetFriendship.PresentForFirstPlayer = true;
                        container.SendedHeartIds.Add(targetFriendship.FirstPlayerId);
                    }

                    if (targetFriendship.PresentForSecondPlayer && !targetFriendship.SecondPlayerRecieved)
                    {
                        recievedCountHeart += 1;
                        targetFriendship.SecondPlayerRecieved = true;
                        container.ReceivedHeartIds.Add(targetFriendship.FirstPlayerId);
                    }
                }
                else
                {
                    continue;
                }

                if (targetFriendship.PresentForFirstPlayer && targetFriendship.PresentForSecondPlayer
                    && targetFriendship.FirstPlayerRecieved && targetFriendship.SecondPlayerRecieved)
                    targetFriendship.Level += 1;
            }

            var resources = await _context.Resources.ToListAsync();
            var resource = resources.Find(res => res.PlayerId == playerId && res.Type == ResourceType.FriendHeart);

            var extraResource = new Resource(playerId, ResourceType.FriendHeart, recievedCountHeart);
            if (resource != null)
            {
                resource.Add(extraResource);
            }
            else
            {
                await _context.Resources.AddAsync(extraResource);
            }


            await _achievmentController.AchievmentUpdataData(playerId, "SendFriendHeartAchievment", sendedCountHeart);

            await _context.SaveChangesAsync();

            answer.Result = _jsonConverter.Serialize(container);
            return answer;
        }

        [HttpPost]
        [Route("Friendship/BreakFriendship")]
        public async Task<AnswerModel> BreakFriendship(int playerId, int friendshipId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var targetFriendship = await _context.FriendshipDatas.FindAsync(friendshipId);

            if (targetFriendship == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            _context.FriendshipDatas.Remove(targetFriendship);
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }
    }
}
