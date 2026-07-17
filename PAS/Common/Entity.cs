namespace PAS.Common {
    public class Entity {
        public Guid Id { get; private set; }

        protected void SetId(Guid value) => Id = value;
    }
}
