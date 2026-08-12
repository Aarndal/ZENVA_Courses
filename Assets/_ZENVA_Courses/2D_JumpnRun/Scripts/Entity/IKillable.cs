using System;

namespace JumpnRun
{
    public interface IKillable
    {
        event Action HasBeenKilled;

        bool TryKill();
    }
}
