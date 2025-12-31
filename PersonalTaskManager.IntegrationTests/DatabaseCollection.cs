using Xunit;

[CollectionDefinition("SqliteDatabase")]
public class SqliteDatabaseCollection 
    : ICollectionFixture<SqliteDatabaseFixture>
{
}
