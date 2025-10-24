using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpEcho;

[McpServerResourceType]
public class CrmResources : IDisposable
{
    private readonly CrmDatabase _database;

    public CrmResources()
    {
        _database = new CrmDatabase();
    }

    [McpServerResource, Description("Get all persons from the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "list-all")]
    [McpMeta("aliases", "list-persons, get-all-contacts")]    
    public List<Person> CrmData()
    {
        return _database.GetAll();
    }

    public void Dispose()
    {
        _database?.Dispose();
    }
}
