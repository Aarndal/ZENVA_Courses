using System;
using System.Security.Cryptography;
using System.Text;

namespace EventSystem
{
    public static class EventSystemIDManager
    {
        // Shared, reused MD5 instance — MD5 is not thread-safe but Unity runs on a single main thread.
        private static readonly MD5 _md5 = MD5.Create();

        // Generates an integer ID: upper 8 bits = type, lower 24 bits = participant hash.
        public static uint GetParticipantID(IEventParticipant participant)
        {
            if (string.IsNullOrEmpty(participant.UniqueKey))
            {
                return 0; //! Invalid participant, no unique key
            }

            var participantType = participant switch
            {
                IPublisher => typeof(IPublisher),
                ISubscriber => typeof(ISubscriber),
                _ => participant.GetType(), //! Fallback to actual type, ensuring we can still generate an ID for custom participant types.
            };

            uint typeIDPart = GetTypeID(participantType);
            uint hashPart = GetHash24(participant.UniqueKey);

            return (typeIDPart << 24) | hashPart;
        }

        // Generates a stable GUID from type and unique key.
        public static Guid GetParticipantGuid(IEventParticipant participant)
        {
            if (string.IsNullOrEmpty(participant.UniqueKey))
            {
                return Guid.Empty; //! Invalid participant, no unique key
            }

            var participantType = participant switch
            {
                IPublisher => typeof(IPublisher),
                ISubscriber => typeof(ISubscriber),
                _ => participant.GetType(), //! Fallback to actual type, ensuring we can still generate an ID for custom participant types.
            };

            // Combine type and unique key for higher uniqueness
            string combinedKey = participantType.FullName + "|" + participant.UniqueKey;
            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(combinedKey));

            return new Guid(hash);
        }

        // Helper: Assigns a stable type ID from the type's full name (first 8 bits of MD5 hash).
        private static uint GetTypeID(Type type)
        {
            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName));

            return hash[0]; //! Provides numbers from 0 to 255, which is sufficient for most use cases and ensures a stable mapping of types to IDs.
        }

        // Helper: Get 24-bit hash from unique key
        private static uint GetHash24(string key)
        {
            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(key));

            //! Combines bytes 1, 2, and 3 from the MD5 hash into a 24-bit unsigned integer (shift and mask), ensuring the resulting value fits in the lower 24 bits.
            //! 0xFFFFFF is a mask to ensure we only get the lower 24 bits
            return (uint)(((hash[1] << 16) | (hash[2] << 8) | hash[3]) & 0xFFFFFF);
        }
    }
}
