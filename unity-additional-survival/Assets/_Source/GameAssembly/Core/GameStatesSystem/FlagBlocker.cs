using System;

namespace Core.GameStatesSystem
{
    public class FlagBlocker : IDisposable
    {
        public event Action<FlagBlocker> OnUnblocked;
        public BlockerType BlockerType { get; private set; }

        public FlagBlocker(BlockerType blockerType) => BlockerType = blockerType;

        public void Dispose()
        {
            OnUnblocked?.Invoke(this);
            OnUnblocked = null;
        }
    }
}