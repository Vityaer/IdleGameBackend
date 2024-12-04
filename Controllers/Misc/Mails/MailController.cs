using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Misc.Json;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Common;
using UniverseRift.GameModelDatas.Players;
using UniverseRift.Models.Misc;
using UniverseRift.Models.Misc.Communications;
using UniverseRift.Models.Results;
using UniverseRift.Models.Rewards;
using UniverseRift.Services.Rewarders;

namespace UniverseRift.Controllers.Misc.Mails
{
    public class MailController : ControllerBase, IMailController
    {
        private const int LETTER_CHARS_LIMIT = 140;
        private const int LETTER_DAY_LIMIT = 10;

        private readonly AplicationContext _context;
        private readonly ICommonDictionaries _commonDictionaries;
        private readonly IRewardService _clientRewardService;
        private readonly IJsonConverter _jsonConverter;

        public MailController(
            AplicationContext context,
            ICommonDictionaries commonDictionaries,
            IRewardService clientRewardService,
            IJsonConverter jsonConverter
            )
        {
            _jsonConverter = jsonConverter;
            _clientRewardService = clientRewardService;
            _context = context;
            _commonDictionaries = commonDictionaries;
        }

        public async Task GetPlayerSave(int playerId, CommunicationData communicationData)
        {
            var allLetters = await _context.LetterDatas.ToListAsync();
            var playerLetters = allLetters
                .FindAll(letter => letter.SenderPlayerId == playerId
                || letter.ReceiverPlayerId == playerId
                );

            communicationData.LetterDatas = playerLetters;

            var allChatMessages = await _context.ChatMessageDatas.ToListAsync();
            communicationData.ChatMessages = allChatMessages;

            var allPlayers = await _context.Players.ToListAsync();
            foreach ( var letter in playerLetters)
            {
                var player = allPlayers.Find(player => player.Id == letter.SenderPlayerId);
                if (player == null)
                    continue;

                communicationData.AddPlayerData(new PlayerData(player));
            }

            foreach (var chatMessage in allChatMessages)
            {
                var player = allPlayers.Find(player => player.Id == chatMessage.PlayerWritterId);
                if (player == null)
                    continue;

                communicationData.AddPlayerData(new PlayerData(player));
            }
        }

        [HttpPost]
        [Route("Mail/GetRewardFromLetter")]
        public async Task<AnswerModel> GetRewardFromLetter(int playerId, int letterId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var letter = await _context.LetterDatas.FindAsync(letterId);
            if (letter == null || letter.ReceiverPlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            if (letter.RewardId < 0)
            {
                answer.Error = "Reward empty";
                return answer;
            }

            if (letter.IsRewardReceived)
            {
                answer.Error = "Reward recived early.";
                return answer;
            }

            var rewardServerData = await _context.RewardServerDatas.FindAsync(letter.RewardId);
            if (rewardServerData == null)
            {
                answer.Error = "Reward not found!";
                return answer;
            }

            letter.IsRewardReceived = true;
            await _context.SaveChangesAsync();
            answer.Result = rewardServerData.RewardJSON;
            return answer;
        }

        [HttpPost]
        [Route("Mail/OpenLetter")]
        public async Task<AnswerModel> OpenLetter(int playerId, int letterId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var letter = await _context.LetterDatas.FindAsync(letterId);
            if (letter == null || letter.ReceiverPlayerId != playerId)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            letter.IsOpened = true;
            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Mail/CreateLetter")]
        public async Task<AnswerModel> CreateLetter(int playerId, int otherPlayerId, string message)
        {
            var answer = new AnswerModel();

            if (message.Length > LETTER_CHARS_LIMIT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var player = await _context.Players.FindAsync(playerId);

            if (player == null || player.LetterSendedCount >= LETTER_DAY_LIMIT)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var otherPlayer = await _context.Players.FindAsync(otherPlayerId);

            if (otherPlayer == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var bans = await _context.PlayerBanRecords.ToListAsync();
            var similarBan = bans.Find(ban => ban.PlayerId == otherPlayerId && ban.TargetBanPlayerId == playerId);
            if (similarBan != null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var newLetter = new LetterData
            {
                SenderPlayerId = playerId,
                ReceiverPlayerId = otherPlayerId,
                Message = message,
                Topic = "PlayerMessageLabel",
                CreateDateTime = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat)
            };

            await _context.LetterDatas.AddAsync(newLetter);
            player.LetterSendedCount += 1;

            await _context.SaveChangesAsync();
            answer.Result = "Success";
            return answer;
        }

        
        [HttpPost]
        [Route("Chat/CreateChatMessage")]
        public async Task<AnswerModel> CreateChatMessage(int playerId, string message)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var chatMessage = new ChatMessageData()
            {
                PlayerWritterId = playerId,
                Message = message,
                CreateDateTime = DateTime.UtcNow.ToString(Constants.Common.DateTimeFormat)
            };

            await _context.ChatMessageDatas.AddAsync(chatMessage);
            await _context.SaveChangesAsync();

            answer.Result = "Success";
            return answer;
        }

        [HttpPost]
        [Route("Chat/LoadChat")]
        public async Task<AnswerModel> LoadChatMessage(int playerId)
        {
            var answer = new AnswerModel();
            var player = await _context.Players.FindAsync(playerId);

            if (player == null)
            {
                answer.Error = "Wrong data";
                return answer;
            }

            var chatMessages = await _context.ChatMessageDatas.ToListAsync();
            answer.Result = _jsonConverter.Serialize(chatMessages);
            return answer;
        }

        public async Task OnRegisterPlayer(int playerId)
        {
            var battlepas = new BattlepasData(playerId);
            await _context.BattlepasDatas.AddAsync(battlepas);
            await _context.SaveChangesAsync();
        }
    }
}
