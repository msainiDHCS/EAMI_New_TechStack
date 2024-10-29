namespace EAMI.CommonEntity
{
    public class EntityGroup<TParent, TChild>
    {
        public TParent ParentItem { get; set; }
        public List<TChild> ListItems { get; set; }
    }
}
