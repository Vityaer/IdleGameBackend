namespace Common
{
    public class BaseObject
    {
        public string Name;

        public int Amount;
        public override string ToString()
        => Amount.ToString();

        public BaseObject()
        {
            Amount = 0;
        }

        public virtual bool EqualsZero
            => Amount == 0;

        public virtual void Add(int count)
        {
            Amount += count;
        }

        public virtual void Remove(int count)
        {
            Amount -= count;
        }

        public virtual bool CheckCount(int count)
            => Amount >= count;
    }
}