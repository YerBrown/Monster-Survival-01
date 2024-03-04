
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;
using static UnityEditor.Rendering.FilterWindow;
[Serializable]
public class ExpeditionData
{
    public List<FieldData> Fields = new List<FieldData>(); //All fields in expedition
    //Return the field data by the coordinates in the map
    public FieldData GetField(Vector2Int coordinates)
    {
        foreach (var field in Fields)
        {
            if (field.Coordinates == coordinates)
            {
                return field;
            }
        }
        //If the coordinates aren't in the current data, we have to add a new field with the coordinates
        FieldData newFieldData = new FieldData(coordinates);
        Fields.Add(newFieldData);
        return newFieldData;
    }
    
    [Serializable]
    public class FieldData
    {
        public Vector2Int Coordinates;                                      //Cordinates of the field in the expedition
        public List<BlockerData> Blockers = new List<BlockerData>();        //All blockers element in the field
        public List<ContainerData> Containers = new List<ContainerData>();  //All containers element in the field
        //public List<ResourceData> Creatures = new List<ResourceData>();   //All creatures element in the field
        public List<ItemData> Items = new List<ItemData>();                 //All items element in the field
        public List<ResourceData> Resources = new List<ResourceData>();     //All resources element in the field
        public List<SwitcherData> Switchers = new List<SwitcherData>();     //All switchers element in the field
        public FieldData(Vector2Int coordinates)
        {
            Coordinates = coordinates;
        }
        public void ClearData()
        {
            Blockers.Clear(); 
            Containers.Clear(); 
            Items.Clear(); 
            Resources.Clear(); 
            Switchers.Clear();
        }
        //Updates an interactive elements 
        public void UpdateData(InteractiveElement updatedElement)
        {
            switch (updatedElement)
            {
                case BlockerElement blocker_e:
                    //Create new data
                    BlockerData newBlocker = new BlockerData(blocker_e.ID, blocker_e.Interactive_Element_ID, (Vector3)blocker_e.transform.position, blocker_e.IsBlockingMovement);
                    //Check if already have a data for that element
                    for (int i = 0; i < Blockers.Count; i++)
                    {
                        if (blocker_e.ID == Blockers[i].ID)
                        {
                            //Replace with new data
                            Blockers[i] = newBlocker;
                            return;
                        }
                    }
                    //If is a new element add a new element
                    Blockers.Add(newBlocker);
                    break;
                case ContainerElement container_e:
                    ContainerData newContainer = new ContainerData(container_e.ID, container_e.Interactive_Element_ID, container_e.transform.position, container_e.ContainerInventory, container_e.IsOpened);
                    for (int i = 0; i < Containers.Count; i++)
                    {
                        if (container_e.ID == Containers[i].ID)
                        {
                            Containers[i] = newContainer;
                            return;
                        }
                    }
                    Containers.Add(newContainer);
                    break;
                case CreatureElement creature_e:
                    break;
                case ItemElement item_e:
                    ItemData newItem = new ItemData(item_e.ID, item_e.Interactive_Element_ID, item_e.transform.position, item_e.Item.ItemInfo.i_Name, item_e.Item.Amount);
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (item_e.ID == Items[i].ID)
                        {
                            Items[i] = newItem;
                            return;
                        }
                    }
                    Items.Add(newItem);
                    break;
                case ResourceElement resource_e:
                    ResourceData newResource = new ResourceData(resource_e.ID, resource_e.Interactive_Element_ID, resource_e.transform.position, resource_e.LootPoints);
                    for (int i = 0; i < Resources.Count; i++)
                    {
                        if (resource_e.ID == Resources[i].ID)
                        {
                            Resources[i] = newResource;
                            return;
                        }
                    }
                    Resources.Add(newResource);
                    break;
                case SwitchElement switch_e:
                    SwitcherData newSwitch = new SwitcherData(switch_e.ID, switch_e.Interactive_Element_ID, switch_e.transform.position, switch_e.SwitchOn);
                    for (int i = 0; i < Switchers.Count; i++)
                    {
                        if (switch_e.ID == Switchers[i].ID)
                        {
                            Switchers[i] = newSwitch;
                            return;
                        }
                    }
                    Switchers.Add(newSwitch);
                    break;
                default:
                    break;
            }
        }
    }
    //Main data for all elemnts in field
    [Serializable]
    public class ParentData
    {
        public string ID;
        public string Element_ID;
        public Vector3 Pos;
    }
    [Serializable]
    public class BlockerData : ParentData
    {
        public bool Blocks;
        public BlockerData(string id, string element_id, Vector3 pos, bool blocks)
        {
            ID = id;
            Pos = pos;
            Element_ID = element_id;
            Blocks = blocks;
        }
    }
    [Serializable]
    public class ContainerData : ParentData
    {
        public string Inv_Name;
        public int MaxSlots;
        public bool FlexibleSlots = false;
        public bool OnlyRemoveItems = false;

        public List<ItemContData> AllItems = new List<ItemContData>();
        public bool Opened;
        [Serializable]
        public class ItemContData
        {
            public string ItemID;
            public int Amount;
            public ItemContData(string itemID, int amount)
            {
                ItemID = itemID;
                Amount = amount;
            }
        }

        public ContainerData(string id, string element_id, Vector3 pos, Inventory inventory, bool opened)
        {
            ID = id;
            Element_ID = element_id;
            Pos = pos;
            Inv_Name = inventory.Inv_Name;
            MaxSlots = inventory.MaxSlots;
            FlexibleSlots = inventory.FlexibleSlots;
            OnlyRemoveItems = inventory.OnlyRemoveItems;
            foreach (var slot in inventory.Slots)
            {
                Add(slot);
            }
            Opened = opened;
        }

        public void Add(ItemSlot slot)
        {
            AllItems.Add(new ItemContData(slot.ItemInfo.i_Name, slot.Amount));
        }
    }
    [Serializable]
    public class CreatureData : ParentData
    {
        public int LP;
        public CreatureData(string id, string element_id, Vector3 pos, int lp)
        {
            ID = id;
            Element_ID = element_id;
            Pos = pos;
            LP = lp;
        }
    }
    [Serializable]
    public class ItemData : ParentData
    {
        public string ItemID;
        public int Amount;
        public ItemData(string id, string element_id, Vector3 pos, string itemID, int amount)
        {
            ID = id;
            Element_ID = element_id;
            Pos = pos;
            ItemID = itemID;
            Amount = amount;
        }
    }
    [Serializable]
    public class ResourceData : ParentData
    {
        public int LP;
        public ResourceData(string id, string element_id, Vector3 pos, int lp)
        {
            ID = id;
            Element_ID = element_id;
            Pos = pos;
            LP = lp;
        }
    }
    [Serializable]
    public class SwitcherData : ParentData
    {
        public bool TurnedOn;
        public SwitcherData(string id, string element_id, Vector3 pos, bool turnedOn)
        {
            ID = id;
            Element_ID = element_id;
            Pos = pos;
            TurnedOn = turnedOn;
        }
    }
}
