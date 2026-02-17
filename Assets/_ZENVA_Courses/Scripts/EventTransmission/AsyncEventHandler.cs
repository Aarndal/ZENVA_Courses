using System.Threading;
using Cysharp.Threading.Tasks;

namespace EventTransmission
{
    public delegate UniTask AsyncEventHandler<in TEventArgs>(object sender, TEventArgs eventArgs, CancellationToken cancellationToken = default) where TEventArgs : IEventArgs;
}
