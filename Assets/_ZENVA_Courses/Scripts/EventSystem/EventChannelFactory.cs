using DebugLogger;
using System;

namespace EventSystem
{
    /// <summary>
    /// An EventChannelFactory is responsible to create IEventChannel instances when requested by the EventTransmitter.
    /// </summary>
    public class EventChannelFactory
    {
        public bool TryCreate<TEventArgs>(out IEventChannel<TEventArgs> channel) 
            where TEventArgs : IEventArgs
        {
            channel = null;
            try
            {
                channel = new EventChannel<TEventArgs>();
            }
            catch (Exception ex)
            {
                DebugLogger.DebugLogger.Debug(
                    LogMessageType.Error, 
                    this, 
                    "Failed to create event channel for IEventArgs type: {0} | GUID: {1}" +
                    "\nException Message: {2}", 
                    true, 
                    typeof(TEventArgs).Name,
                    typeof(TEventArgs).GUID,
                    ex.Message);

                return false;
            }
            return channel != null;
        }
    }
}