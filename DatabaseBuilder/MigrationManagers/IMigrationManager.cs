namespace DatabaseBuilder.MigrationManagers
{
    public interface IMigrationManager
    {
        public void RunMigrations(object connection);

        public void RollBackMigrations(object connection);
    }
}
