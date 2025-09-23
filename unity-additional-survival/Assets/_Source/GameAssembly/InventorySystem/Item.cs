using Mirror;

namespace InventorySystem
{
    public class Item
    {
        public int ID { get; }
        public int Count { get; private set; }

        public Item(int id, int count)
        {
            ID = id;

            var source = ItemsDatabase.GetItemData(ID);

            Count = count > source.MaxCount ? source.MaxCount : count;
        }

        public bool TryRemoveCount(int value)
        {
            if (value > Count)
                return false;

            Count -= value;
            return true;
        }

        public bool TryAddCount(int value)
        {
            if (Count + value > ItemsDatabase.GetItemData(ID).MaxCount)
                return false;

            Count += value;
            return true;
        }

        public void SetCount(int value)
        {
            var data = ItemsDatabase.GetItemData(ID).MaxCount;
            if (value > data)
                Count = data;

            Count = value;
        }
    }

    public static class CustomReadWriteItem
    {
        public static void WriteMyType(this NetworkWriter writer, Item value)
        {
            writer.WriteInt(value.ID);
            writer.WriteInt(value.Count);
        }

        public static Item ReadMyType(this NetworkReader reader)
        {
            var id = reader.ReadInt();
            var count = reader.ReadInt();
            return new Item(id, count);
        }
    }
}