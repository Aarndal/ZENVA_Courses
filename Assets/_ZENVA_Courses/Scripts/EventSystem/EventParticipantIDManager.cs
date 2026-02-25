using System;
using System.Security.Cryptography;
using System.Text;

namespace EventSystem
{
    public static class EventParticipantIDManager
    {
        // Generates an integer ID: upper 8 bits = type, lower 24 bits = participant hash.
        public static int GetParticipantId(Type participantType, string uniqueKey)
        {
            int typeId = GetTypeId(participantType);
            int hashPart = GetHash24(uniqueKey);
            return (typeId << 24) | hashPart;
        }

        // Generates a stable GUID from type and unique key.
        public static Guid GetParticipantGuid(Type participantType, string uniqueKey)
        {
            // Combine type and unique key for higher uniqueness
            string combined = participantType.FullName + "|" + uniqueKey;
            using var provider = MD5.Create();
            byte[] hash = provider.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return new Guid(hash);
        }

        // Helper: assigns a stable type ID from the type's full name (first 8 bits of MD5 hash).
        private static int GetTypeId(Type type)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(type.FullName));
            return hash[0]; // 0..255, probably fine unless you have hundreds of types
        }

        // Helper: get 24-bit hash from unique key
        private static int GetHash24(string key)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            return ((hash[1] << 16) | (hash[2] << 8) | hash[3]) & 0xFFFFFF;
        }
    }
}
