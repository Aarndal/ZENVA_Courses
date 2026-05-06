using System;
using System.Security.Cryptography;
using System.Text;

namespace EventSystem
{
    public static class EventSystemIDManager
    {
        private static readonly MD5 _md5 = MD5.Create();

        public static uint GetParticipantID(IEventParticipant participant)
        {
            if (string.IsNullOrEmpty(participant.UniqueKey))
            {
                return 0;
            }

            var participantType = participant switch
            {
                IPublisher => typeof(IPublisher),
                ISubscriber => typeof(ISubscriber),
                _ => participant.GetType(),
            };

            uint typeIDPart = GetTypeID(participantType);
            uint hashPart = GetHash24(participant.UniqueKey);

            return (typeIDPart << 24) | hashPart;
        }

        private static uint GetTypeID(Type type)
        {
            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName));
            return hash[0];
        }

        private static uint GetHash24(string key)
        {
            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            return (uint)(((hash[1] << 16) | (hash[2] << 8) | hash[3]) & 0xFFFFFF);
        }
    }
}
