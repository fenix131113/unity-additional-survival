using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using InventorySystem;
using Mirror;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.View
{
    public class ResourcesHint : MonoBehaviour
    {
        [SerializeField] private CanvasGroup hintRoot;
        [SerializeField] private RectTransform hintRootRect;
        [SerializeField] private Image hintItemPrefab;
        [SerializeField] private float yOffset;
        [SerializeField] private float animTime = 0.15f;

        private Tween _currentTween;
        private PlayerInventory _inventory;
        private readonly Dictionary<int, GameObject> _resourceItems = new();

        private void Start() => StartCoroutine(WaitForPlayerCoroutine());

        public void ShowHint(List<BuildingRequirements> requirements, Vector2 position)
        {
            foreach (var value in _resourceItems.Values)
                value.SetActive(false);
            
            position.y += yOffset;

            foreach (var req in requirements)
            {
                _resourceItems[req.ItemData.ID].transform.GetChild(0).GetComponent<TMP_Text>().text =
                    req.Count.ToString();
                _resourceItems[req.ItemData.ID].gameObject.SetActive(true);
            }

            hintRootRect.position = position;
            hintRoot.alpha = 0;
            hintRoot.gameObject.SetActive(true);

            _currentTween?.Kill();
            _currentTween = hintRoot.DOFade(1, animTime);
        }

        public void HideHint()
        {
            _currentTween?.Kill();
            _currentTween = hintRoot.DOFade(0, animTime);
            _currentTween.onComplete += () => hintRoot.gameObject.SetActive(false);
        }

        private void SpawnResourceItem(Item item)
        {
            var spawned = Instantiate(hintItemPrefab, hintRoot.transform);
            _resourceItems.Add(item.ID, spawned.gameObject);
            spawned.sprite = ItemsDatabase.GetItemData(item).Icon;
            spawned.gameObject.SetActive(false);
        }

        private IEnumerator WaitForPlayerCoroutine()
        {
            while (!NetworkClient.localPlayer)
                yield return null;

            yield return null;

            _inventory = NetworkClient.localPlayer.GetComponent<PlayerInventory>();

            foreach (var item in _inventory.GetItems())
                SpawnResourceItem(item);
        }
    }
}