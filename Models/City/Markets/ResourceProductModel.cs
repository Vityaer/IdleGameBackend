using Models.Data.Inventories;

namespace Models.City.Markets
{
    [Serializable]
    public class ResourceProductModel : BaseProductModel
    {
        public new ResourceData Subject;
    }
}
