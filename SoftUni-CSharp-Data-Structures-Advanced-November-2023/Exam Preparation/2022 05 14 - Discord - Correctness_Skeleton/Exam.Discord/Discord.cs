using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Discord
{
    public class Discord : IDiscord
    {
        private readonly Dictionary<string, Message> messagesById = new Dictionary<string, Message>();
        private readonly Dictionary<string, List<Message>> messagesByChannel = new Dictionary<string, List<Message>>();

        public int Count => messagesById.Count;

        public bool Contains(Message message)
        {
            return messagesById.ContainsKey(message.Id);
        }

        public void SendMessage(Message message)
        {
            messagesById[message.Id] = message;
            if (!messagesByChannel.ContainsKey(message.Channel))
            {
                messagesByChannel[message.Channel] = new List<Message>();
            }
            messagesByChannel[message.Channel].Add(message);
        }

        public void DeleteMessage(string messageId)
        {
            if (!messagesById.ContainsKey(messageId))
            {
                throw new ArgumentException();
            }
            var message = messagesById[messageId];
            messagesById.Remove(messageId);
            messagesByChannel[message.Channel].Remove(message);
        }

        public Message GetMessage(string messageId)
        {
            if (!messagesById.TryGetValue(messageId, out var message))
            {
                throw new ArgumentException();
            }
            return message;
        }

        public void ReactToMessage(string messageId, string reaction)
        {
            if (!messagesById.ContainsKey(messageId))
            {
                throw new ArgumentException();
            }
            messagesById[messageId].Reactions.Add(reaction);
        }

        public IEnumerable<Message> GetChannelMessages(string channel)
        {
            if (!messagesByChannel.ContainsKey(channel) || messagesByChannel[channel].Count == 0)
            {
                throw new ArgumentException();
            }
            return messagesByChannel[channel];
        }

        public IEnumerable<Message> GetMessagesByReactions(List<string> reactions)
        {
            return messagesById.Values
                .Where(m => reactions.All(r => m.Reactions.Contains(r)))
                .OrderByDescending(m => m.Reactions.Count)
                .ThenBy(m => m.Timestamp);
        }

        public IEnumerable<Message> GetMessageInTimeRange(int lowerBound, int upperBound)
        {
            return messagesById.Values
                .Where(m => m.Timestamp >= lowerBound && m.Timestamp <= upperBound)
                .OrderByDescending(m => messagesByChannel[m.Channel].Count)
                .ThenBy(m => m.Timestamp);
        }

        public IEnumerable<Message> GetTop3MostReactedMessages()
        {
            return messagesById.Values
                .OrderByDescending(m => m.Reactions.Count)
                .Take(3);
        }

        public IEnumerable<Message> GetAllMessagesOrderedByCountOfReactionsThenByTimestampThenByLengthOfContent()
        {
            return messagesById.Values
                .OrderByDescending(m => m.Reactions.Count)
                .ThenBy(m => m.Timestamp)
                .ThenBy(m => m.Content.Length);
        }
    }
}
