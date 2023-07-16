namespace UniverseRift.GameModels
{
    public class PosibleObjectData
    {
        public float Posibility;

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
            return result;
        }
    }
}