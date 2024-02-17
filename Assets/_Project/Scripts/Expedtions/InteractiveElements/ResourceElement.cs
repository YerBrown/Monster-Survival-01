using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResourceElement : InteractiveElement
{
    public int LP = 10; //Loot points, por cada punto que resta es un loot 
    public List<LootRate> Loot = new List<LootRate>(); //Posible loot chances
    public ResourceSO ResourceInfo;
    public ResourceElementEventChannelSO OnResourceElementInteracted;
    public bool LootFinished = false;
    public Sprite LootFinishedSprite;
    public TMP_Text LootText;
    public float AnimY = 25;
    public float AnimDuration = 1f;

    public GameObject DropedItemContainerPrefab;
    private Sequence secuencia; // Referencia a la secuencia de Dotween
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        if (LP > 0)
        {
            if (OnResourceElementInteracted != null)
                OnResourceElementInteracted.RaiseEvent(this);
        }
    }
    public (ItemSlot, ItemSlot, int) HitResource(int hitPoints, Inventory targetInventory, Transform targetTransform)
    {
        if (LootFinished) return (null, null, 0);
        //Play hit animation
        if (LP - hitPoints < 0)
        {
            hitPoints = LP;
        }
        LP -= hitPoints;
        ItemSlot newLoot = GetLoot(hitPoints);
        //Loot that is remaining because of space lack in inventory
        ItemSlot remainingLoot = new();
        if (newLoot != null)
            remainingLoot = AddLootToInventory(targetInventory, newLoot);

        //Remaining loot add to droped container
        if (remainingLoot != null && remainingLoot.Amount > 0)
        {
            SpawnDropItemsContainer(targetTransform.position, remainingLoot);
        }

        if (LP <= 0)
        {
            //Finish resource loot
            LootFinished = true;
            BlockMovement = false;
            GetComponent<SpriteRenderer>().sprite = LootFinishedSprite;
            if (OnResourceElementInteracted != null)
                OnResourceElementInteracted.RaiseEvent(this);
        }
        ShowLootText(newLoot);
        return (newLoot, remainingLoot, hitPoints);
    }
    private ItemSlot GetLoot(int lp)
    {
        int maxNumber = 0;
        foreach (LootRate rate in Loot)
        {
            maxNumber += rate.ChanceRate;
        }
        int lootNumber = Random.Range(0, maxNumber);
        for (int i = 0; i < Loot.Count; i++)
        {
            if (lootNumber < Loot[i].ChanceRate)
            {
                //lp = loot point done
                return new ItemSlot(Loot[i].ItemType, Loot[i].Amount * lp);
            }
            else
            {
                lootNumber -= Loot[i].ChanceRate;
                maxNumber -= Loot[i].ChanceRate;
            }
        }
        return null;
    }
    private ItemSlot AddLootToInventory(Inventory inventoryTarget, ItemSlot item)
    {
        if (inventoryTarget == null) return null;
        int remaining = inventoryTarget.AddNewItem(item);
        if (remaining != item.Amount)
        {
            Debug.Log($"{inventoryTarget.Inv_Name} has loted {item.Amount - remaining} {item.ItemInfo.i_Name}");
        }
        if (remaining > 0)
        {
            if (remaining == item.Amount)
            {
                Debug.Log($"No slot free in {inventoryTarget.Inv_Name} inventory");
                return item;
            }
            else
            {
                Debug.Log($"Not enough space in {inventoryTarget.Inv_Name} inventory, only {item.Amount - remaining} added");
                return new ItemSlot(item.ItemInfo, remaining);
            }
        }
        return null;
    }

    public void ShowLootText(ItemSlot loot)
    {
        if (LootText == null) return;
        // Detener y eliminar la secuencia anterior si existe
        if (secuencia != null)
        {
            secuencia.Kill();
        }
        //Set start values to text
        LootText.transform.localPosition = Vector3.zero;
        LootText.text = $"+ {loot.Amount} {loot.ItemInfo.i_Name}";
        LootText.color = Color.white;
        LootText.gameObject.SetActive(true);
        // Crear una secuencia de tweens encadenados
        secuencia = DOTween.Sequence();
        // Agregar tweens para mover el texto y cambiar su color
        secuencia.Append(LootText.rectTransform.DOLocalMoveY(AnimY, AnimDuration).SetEase(Ease.InOutQuad)); // Mover durante 2 segundos

        secuencia.Join(LootText.DOColor(Color.clear, AnimDuration).SetEase(Ease.InOutQuint)); // Cambiar el color durante 2 segundos

        // Agregar una función OnComplete para desactivar el GameObject cuando la secuencia termine
        secuencia.OnComplete(() =>
        {
            // Desactivar el GameObject del texto
            LootText.gameObject.SetActive(false);
        });

        // Reproducir la secuencia
        secuencia.Play();
    }
    private RaycastHit2D? DetectOverlayTile(Vector3 checkPosition)
    {

        RaycastHit2D[] hits = Physics2D.RaycastAll(checkPosition, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }
    public void SpawnDropItemsContainer(Vector3 targetPos, ItemSlot dropedSlot)
    {
        var overlayDetected = DetectOverlayTile(targetPos);
        if (overlayDetected.HasValue)
        {
            OverlayTile targetPositionOverlay = overlayDetected.Value.collider.gameObject.GetComponent<OverlayTile>();
            DropedItemsContainerElement dropedContainer = null;
            if (targetPositionOverlay.I_Element != null && targetPositionOverlay.I_Element is DropedItemsContainerElement)
            {
                dropedContainer = targetPositionOverlay.I_Element.GetComponent<DropedItemsContainerElement>();
            }
            else if (targetPositionOverlay.I_Element == null)
            {
                dropedContainer = Instantiate(DropedItemContainerPrefab, targetPos, Quaternion.identity).GetComponent<DropedItemsContainerElement>();
            }

            if (dropedContainer == null) return;
            dropedContainer.AddNewItem(dropedSlot);
        }
    }

    [System.Serializable]
    public class LootRate
    {
        public ItemsSO ItemType;
        public int Amount;
        public int ChanceRate = 10;
    }
}
