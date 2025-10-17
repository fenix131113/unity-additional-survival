using System.Collections;
using Core;
using Core.GameStatesSystem;
using Mirror;
using UnityEngine;
using VContainer;
using WeaponSystem;

namespace Player
{
    public class PlayerShoot : NetworkBehaviour
    {
        [Inject] private InputSystem_Actions _input;
        [Inject] private BulletsPool _bulletsPool;
        [Inject] private GameStates _gameStates;
        
        private PlayerWeapons _weapons;
        private PlayerAim _aim;
        
        [SyncVar]
        private bool _canShoot = true;

        #region Client

        private void Awake()
        {
            _weapons = GetComponent<PlayerWeapons>();
            _aim = GetComponent<PlayerAim>();
        }

        private void Start() => ObjectInjector.InjectObject(this);

        private void Update()
        {
            if(!isLocalPlayer)
                return;
            
            if(_input.Player.Shoot.IsPressed())
                OnShoot();
        }

        private void OnShoot()
        {
            if(!_canShoot || !_gameStates.PlayerAttack)
                return;
            
            Cmd_Shoot();
        }

        [ClientRpc]
        private void ActivateBullet(GameObject bullet)
        {
            bullet.SetActive(true);
            bullet.transform.position = _weapons.CurrentWeapon.ShootPoint.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, _aim.RotAngle);
        }

        #endregion

        #region Server

        [Command]
        private void Cmd_Shoot()
        {
            if(!_canShoot)
                return;
            
            var bullet = _bulletsPool.TakeFromPool(_weapons.CurrentWeapon);
            
            if (!bullet)
                bullet = SpawnBullet();
            else
            {
                bullet.transform.rotation = Quaternion.Euler(0, 0, _aim.RotAngle);
                bullet.transform.position = _weapons.CurrentWeapon.ShootPoint.position;
            }

            bullet.gameObject.SetActive(true);
            ActivateBullet(bullet.gameObject);
            StartCoroutine(ShootCooldown());
        }

        [Server]
        private Bullet SpawnBullet()
        {
            var spawned = Instantiate(_weapons.CurrentWeapon.BulletPrefab, _weapons.CurrentWeapon.ShootPoint.position, Quaternion.Euler(0, 0, _aim.RotAngle));
            spawned.InitPool(_weapons.CurrentWeapon, _bulletsPool);
            NetworkServer.Spawn(spawned.gameObject);
            return spawned;
        }

        private IEnumerator ShootCooldown()
        {
            _canShoot = false;
            
            yield return new WaitForSeconds(_weapons.CurrentWeapon.ShootCooldown);

            _canShoot = true;
        }
        
        #endregion
    }
}