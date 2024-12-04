using Common;
using Models.Data.Inventories;

namespace UIController.Rewards.PosibleRewards
{
    public class PosibleObjectData
    {
        public int Posibility;

        public virtual BaseObject CreateGameObject()
        {
            return null;
        }
    }

    public class PosibleObjectData<T> : PosibleObjectData
        where T : InventoryBaseItem
    {
        public T Value;

        public override BaseObject CreateGameObject()
        {
            var result = Value.CreateGameObject();
            result.Amount = 0;
            return result;
        }
    }
}