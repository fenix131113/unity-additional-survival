using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.GameStatesSystem;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using WeaponSystem;

namespace Player
{
    public class PlayerWeapons : NetworkBehaviour
    {
        public Weapon CurrentWeapon => _allowWeapons[_currentWeaponIndex];

        [SyncVar(hook = nameof(OnCurrentWeaponIndexChanged))]
        private int _currentWeaponIndex;

        [SerializeField] private List<Weapon> startWeapons;
        [SerializeField] private Transform weaponsContainer;
        [Inject] private InputSystem_Actions _input;
        [Inject] private GameStates _gameStates;
        private bool _canChangeWeapon = true;

        private readonly List<Weapon> _allowWeapons = new();

        public event Action<Weapon, Weapon> OnCurrentWeaponChanged;
        public event Action<Weapon> OnWeaponsListChanged;

        #region Client

        private void Awake() => _allowWeapons.AddRange(startWeapons);

        private void Start()
        {
            if(_currentWeaponIndex >= 0)
                SetWeaponVisible(-1, _currentWeaponIndex);
            
            if (!isLocalPlayer)
                return;
            
            ObjectInjector.InjectObject(this);

            if (_allowWeapons.Count > 0)
                Cmd_ChangeCurrentWeapon(0);
        }

        private void Update()
        {
            if (!isLocalPlayer || !_input.Player.enabled || !_canChangeWeapon || !_gameStates.PlayerWeaponSwitch)
                return;

            int newIndex;

            if (_allowWeapons.Count == 0)
                return;

            var currentIndex = _allowWeapons.IndexOf(CurrentWeapon);
            if (currentIndex < 0)
                currentIndex = 0;

            if (Mouse.current.scroll.y.ReadValue() > 0)
            {
                newIndex = (currentIndex + 1) % _allowWeapons.Count;
                Cmd_ChangeCurrentWeapon(newIndex);
                StartCoroutine(WeaponChangeDelay());
            }
            else if (Mouse.current.scroll.y.ReadValue() < 0)
            {
                newIndex = (currentIndex - 1 + _allowWeapons.Count) % _allowWeapons.Count;
                Cmd_ChangeCurrentWeapon(newIndex);
                StartCoroutine(WeaponChangeDelay());
            }
        }

        private void OnCurrentWeaponIndexChanged(int oldIndex, int newIndex)
        {
            SetWeaponVisible(oldIndex, newIndex);
            OnCurrentWeaponChanged?.Invoke(_allowWeapons[oldIndex], _allowWeapons[newIndex]);
        }

        [ClientRpc]
        private void Rpc_AddAllowWeapon(int childIndex) => _allowWeapons.Add(weaponsContainer.GetChild(childIndex).GetComponent<Weapon>());

        private IEnumerator WeaponChangeDelay()
        {
            _canChangeWeapon = false;

            yield return new WaitForSeconds(0.1f);

            _canChangeWeapon = true;
        }

        #endregion

        #region Server

        [Command]
        private void Cmd_ChangeCurrentWeapon(int index)
        {
            var old = _currentWeaponIndex;

            SetWeaponVisible(old, index);

            _currentWeaponIndex = index;
            OnCurrentWeaponChanged?.Invoke(_allowWeapons[old], _allowWeapons[index]);
        }

        [Server]
        public void AddAllowWeapon(Weapon weapon)
        {
            if (_allowWeapons.Contains(weapon))
                return;

            _allowWeapons.Add(weapon);

            var childIndex = -1;
            for (var i = 0; i < weaponsContainer.childCount; i++)
            {
                if (weaponsContainer.GetChild(i).gameObject != weapon.gameObject)
                    continue;
                
                childIndex = i;
                break;
            }

            Rpc_AddAllowWeapon(childIndex);

            OnWeaponsListChanged?.Invoke(weapon);
        }

        #endregion

        private void SetWeaponVisible(int old, int newIndex)
        {
            if (old >= 0)
                _allowWeapons[old].gameObject.SetActive(false);

            _allowWeapons[newIndex].gameObject.SetActive(true);
        }
    }
}