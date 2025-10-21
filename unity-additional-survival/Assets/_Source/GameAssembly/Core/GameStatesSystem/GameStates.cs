using System;
using System.Collections.Generic;

namespace Core.GameStatesSystem
{
    public class GameStates
    {
        public bool PlayerAttack { get; private set; } = true;
        public bool PlayerWeaponSwitch { get; private set; } = true;
        public bool PlayerBuildModeChange { get; private set; } = true;

        private readonly Dictionary<BlockerType, List<FlagBlocker>> _blockers = new();

        public void RegisterBlocker(FlagBlocker blocker)
        {
            if (!_blockers.ContainsKey(blocker.BlockerType))
                _blockers.Add(blocker.BlockerType, new List<FlagBlocker>());

            _blockers[blocker.BlockerType].Add(blocker);
            blocker.OnUnblocked += OnUnblocked;
            CheckBlockers();
        }

        private void OnUnblocked(FlagBlocker blocker)
        {
            _blockers[blocker.BlockerType].Remove(blocker);
            CheckBlockers();
        }

        private void CheckBlockers()
        {
            foreach (var b in _blockers.Keys)
            {
                var setFlag = _blockers[b].Count == 0;

                switch (b)
                {
                    case BlockerType.PLAYER_ATTACK:
                        PlayerAttack = setFlag;
                        break;
                    case BlockerType.PLAYER_WEAPON_SWITCH:
                        PlayerWeaponSwitch = setFlag;
                        break;
                    case BlockerType.PLAYER_BUILD_MODE_CHANGE:
                        PlayerBuildModeChange = setFlag;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(BlockerType),
                            $"Unknown blocker with type: {b.ToString()}");
                }
            }
        }
    }
}