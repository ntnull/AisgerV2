namespace Aisger.Models.Repository
{
    public abstract class SqlRepository
    {
        protected SqlRepository()
        {
            AppContext = CreateDatabaseContext();
        }


        protected aisgerEntities AppContext { get; set; }
        // создание контекста базы данных. необходимо использовать using
        public virtual aisgerEntities CreateDatabaseContext()
        {
            return new aisgerEntities();
        }

        public virtual aisgerEntities CreateDatabaseContext(bool isProxy)
        {
            return new aisgerEntities(isProxy);
        }
    }
}