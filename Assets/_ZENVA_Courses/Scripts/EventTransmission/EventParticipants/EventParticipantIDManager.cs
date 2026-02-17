using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace EventTransmission
{
    /// <summary>
    /// Manages unique 64-bit IDs for all event system participants: events, publishers, subscribers, and others.
    /// </summary>
    public static class EventParticipantIDManager
    {
        /// <summary>
        /// Participant type encoding for bit-level ID generation.
        /// </summary>
        public enum ParticipantType : byte
        {
            Invalid = 0x00,
            Event = 0x01,
            Publisher = 0x02,
            Subscriber = 0x03,
            Other = 0x04
        }


        private const ulong DefaultEventInstanceID = 0UL;
        private const ulong DefaultAsyncEventInstanceID = 1UL;
        private const ulong DefaultPublisherInstanceID = 2UL;
        private const ulong DefaultSubscriberInstanceID = 3UL;

        public const ulong MaxInstanceID = 0x00FFFFFFFFFFFFFFUL; // Maximum instance ID (lower 56 bits)

        private static readonly ConcurrentDictionary<ulong, IEventParticipant> _participantIDs = new();
        private static readonly HashSet<IDefaultEventParticipant> _defaultEventParticipants = new();

        // Instance counter (atomic)
        private static long _counter = 3L;


        // Static constructor to register default participants
        static EventParticipantIDManager()
        {
            var defaultEvent = DefaultEvent.Instance;
            var defaultAsnycEvent = DefaultAsyncEvent.Instance;
            var defaultPublisher = DefaultPublisher.Instance;
            var defaultSubscriber = DefaultSubscriber.Instance;

            _defaultEventParticipants.Add(defaultEvent);
            _defaultEventParticipants.Add(defaultAsnycEvent);
            _defaultEventParticipants.Add(defaultPublisher);
            _defaultEventParticipants.Add(defaultSubscriber);

            foreach (var defaultParticipant in _defaultEventParticipants)
            {
                _participantIDs.TryAdd(defaultParticipant.EventParticipantID, defaultParticipant);
            }

            _counter = _defaultEventParticipants.Count - 1;
        }


        #region PublicMethods
        /// <summary>
        /// Generates a unique 64-bit ID for the given event participant (publisher, subscriber, event, or other).
        /// </summary>
        /// <param name="participant">The event participant for which to generate an ID.</param>
        /// <returns>the unique 64-bit ID as decimal value</returns>
        public static ulong GenerateID(IEventParticipant participant)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant), "Participant cannot be null when generating an ID.");

            ParticipantType participantType = GetParticipantType(participant);

            if (participantType == ParticipantType.Invalid)
                throw new ArgumentException("Participant type could not be determined.", nameof(participant));

            long instance = Interlocked.Increment(ref _counter);
            ulong id = EncodeID(participantType, (ulong)instance);

            while (!_participantIDs.TryAdd(id, participant))
            {
                instance = Interlocked.Increment(ref _counter);
                id = EncodeID(participantType, (ulong)instance);
            }

            return id;
        }

        /// <summary>
        /// Returns all currently registered participants.
        /// </summary>
        /// <returns>enumerable of all registered participants</returns>
        public static IEnumerable<IEventParticipant> GetAllParticipants() => _participantIDs.Values;

        /// <summary>
        /// Decodes the instance number from an encoded ID.
        /// </summary>
        /// <param name="id">The unique 64-bit ID of the participant.</param>
        /// <returns>the instance id (lower 56 bits)</returns>
        public static ulong GetInstanceFromID(ulong id)
        {
            return id & 0x00FFFFFFFFFFFFFFUL;
        }

        /// <summary>
        /// Gets the event participant by its unique ID.
        /// </summary>
        /// <param name="id">The unique 64-bit ID of the participant.</param>
        /// <returns>the participant or null if not found</returns>
        public static IEventParticipant GetParticipantByID(ulong id)
        {
            if (!_participantIDs.TryGetValue(id, out var participant))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarningFormat("No event participant found with ID: {0}", id);
#endif
                return null;
            }
            return participant;
        }

        /// <summary>
        /// Decodes the participant type from an encoded ID.
        /// </summary>
        /// <param name="id">The unique 64-bit ID of the participant.</param>
        /// <returns>the participant type (upper 8 bits) as enum</returns>
        public static ParticipantType GetTypeFromID(ulong id)
        {
            return (ParticipantType)((id >> 56) & 0xFF);
        }

        /// <summary>
        /// Releases the participant's ID from the registry, if it doesn't have a default value.
        /// </summary>
        /// <param name="participant">The participant whose ID should be released.</param>
        public static void ReleaseID(IEventParticipant participant)
        {
            if (participant == null)
                return;

            foreach (var defaultParticipant in _defaultEventParticipants)
            {
                if (participant.EventParticipantID == defaultParticipant.EventParticipantID)
                    return;
            }

            _participantIDs.TryRemove(participant.EventParticipantID, out _);
        }
        #endregion


        #region InternalMethods
        /// <summary>
        /// Generates the predefined ID for a default event participant (DefaultEvent, DefaultAsyncEvent, DefaultPublisher, DefaultSubscriber).
        /// </summary>
        /// <param name="defaultEventParticipant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static ulong GenerateDefaultParticipantID(IDefaultEventParticipant defaultEventParticipant)
        {
            if (defaultEventParticipant == null)
                throw new ArgumentNullException(nameof(defaultEventParticipant));

            var participantType = GetParticipantType(defaultEventParticipant);

            ulong instance;
            switch (participantType)
            {
                case ParticipantType.Event:
                    if (defaultEventParticipant is DefaultEvent)
                        instance = DefaultEventInstanceID;
                    else if (defaultEventParticipant is DefaultAsyncEvent)
                        instance = DefaultAsyncEventInstanceID;
                    else
                        throw new ArgumentException("Unrecognized default event type.", nameof(defaultEventParticipant));
                    break;
                case ParticipantType.Publisher:
                    instance = DefaultPublisherInstanceID;
                    break;
                case ParticipantType.Subscriber:
                    instance = DefaultSubscriberInstanceID;
                    break;
                default:
                    throw new ArgumentException("The provided default event participant is not recognized.", nameof(defaultEventParticipant));
            }

            return EncodeID(participantType, instance);
        }
        #endregion


        #region PrivateMethods
        /// <summary>
        /// Encodes the type and instance of an event participant into a 64-bit ID (type: upper 8 bits, instance: lower 56 bits).
        /// </summary>
        /// <param name="instance">The instance number (should be unique)</param>
        /// <param name="participantType">The participant type (event, publisher, subscriber, or other)</param>
        /// <returns>the encoded ID</returns>
        private static ulong EncodeID(ParticipantType participantType, ulong instance)
        {
            return ((ulong)participantType << 56) | (instance & 0x00FFFFFFFFFFFFFFUL);
            /*
             * 0x00FFFFFFFFFFFFFFUL: 64-bit hexadecimal bitmask to isolate the lower 56 bits of a 64-bit unsigned integer.
             * In binary: 00000000 11111111 11111111 11111111 11111111 11111111 11111111 11111111
            */
        }

        /// <summary>
        /// Determines the type for an event participant (event, publisher, subscriber, or other).
        /// </summary>
        /// <param name="participant">The event participant whose type is to be determined.</param>
        /// <returns>the participant type as enum</returns>
        private static ParticipantType GetParticipantType(IEventParticipant participant)
        {
            if (participant == null)
                return ParticipantType.Invalid;

            var typeAttribute = (EventParticipantTypeAttribute)Attribute.
                GetCustomAttribute(participant.GetType(), typeof(EventParticipantTypeAttribute));

            if (typeAttribute != null)
                return typeAttribute.Type;

            if (participant is IEvent) return ParticipantType.Event;
            if (participant is IPublisher) return ParticipantType.Publisher;
            if (participant is ISubscriber) return ParticipantType.Subscriber;

            return ParticipantType.Other;
        }
        #endregion
    }
}