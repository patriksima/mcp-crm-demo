using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpEcho;

public enum Sex
{
    M,
    F
}

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int Age { get; set; }
    public Sex Sex { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string CvSummary { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();

    // Primary constructor
    public Person(Guid id, string name, string surname, int age, Sex sex, string role, string department, string cvSummary, List<string> skills)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Age = age;
        Sex = sex;
        Role = role;
        Department = department;
        CvSummary = cvSummary;
        Skills = skills;
    }
}

[McpServerToolType]
public sealed class MyCrm : IDisposable
{
    private readonly CrmDatabase _database;
    private readonly Guid InstanceId;

    public MyCrm(string? databasePath = null)
    {
        InstanceId = Guid.NewGuid();
        _database = new CrmDatabase(databasePath);
    }


    [McpServerTool, Description("Get personal information by ID.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-lookup")]
    [McpMeta("aliases", "find-by-id, lookup-by-id")]
    public Person? GetPersonById([Description("Person ID as Guid")] Guid id)
    {
        return _database.GetById(id);
    }

    [McpServerTool, Description("Find a person by their first name. Returns detailed CRM record.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "employee-search")]
    [McpMeta("aliases", "find-person, lookup-person, search-contact")]
    public Person? GetPersonByName([Description("Person name")] string name)
    {
        return _database.GetByName(name);
    }

    [McpServerTool, Description("Find a person by their surname. Returns detailed CRM record.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "employee-search")]
    [McpMeta("aliases", "find-person, lookup-person, search-contact")]
    public Person? GetPersonBySurname([Description("Person surname")] string surname)
    {
        return _database.GetBySurname(surname);
    }

    [McpServerTool, Description("Get personal information for all persons with given skill.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "skill-search")]
    [McpMeta("aliases", "find-by-skill, search-by-skill")]
    public List<Person> GetPersonsBySkill([Description("Person skill")] string skill)
    {
        return _database.GetBySkill(skill);
    }

    [McpServerTool, Description("Get all persons from a specific department.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "department-search")]
    [McpMeta("aliases", "find-by-department, search-by-department")]
    public List<Person> GetPersonsByDepartment([Description("Department name")] string department)
    {
        return _database.GetByDepartment(department);
    }

    [McpServerTool, Description("Get all persons with a specific role/job title.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "role-search")]
    [McpMeta("aliases", "find-by-role, search-by-role")]
    public List<Person> GetPersonsByRole([Description("Role/job title")] string role)
    {
        return _database.GetByRole(role);
    }

    [McpServerTool, Description("Get all persons from the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "list-all")]
    [McpMeta("aliases", "list-persons, get-all-contacts")]
    public List<Person> GetAllPersons()
    {
        return _database.GetAll();
    }

    [McpServerTool, Description("Get skill statistics showing how many people have each skill.")]
    [McpMeta("category", "analytics")]
    [McpMeta("operation", "aggregate")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Dictionary")]
    [McpMeta("context", "skill-analytics")]
    [McpMeta("aliases", "skill-stats, skill-distribution")]
    public Dictionary<string, int> GetSkillStatistics()
    {
        return _database.GetSkillStatistics();
    }

    [McpServerTool, Description("Get the average age of all persons in the CRM system.")]
    [McpMeta("category", "analytics")]
    [McpMeta("operation", "aggregate")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "double")]
    [McpMeta("context", "age-analytics")]
    [McpMeta("aliases", "avg-age, mean-age")]
    public double GetAverageAge()
    {
        return _database.GetAverageAge();
    }

    [McpServerTool, Description("Get the oldest person in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "age-ranking")]
    [McpMeta("aliases", "find-oldest, get-senior")]
    public Person? GetOldestPerson()
    {
        return _database.GetOldestPerson();
    }

    [McpServerTool, Description("Get the person with the most skills in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "skill-ranking")]
    [McpMeta("aliases", "find-most-skilled, get-expert")]
    public Person? GetMostSkilledPerson()
    {
        return _database.GetMostSkilledPerson();
    }

    [McpServerTool, Description("Search for persons by full-text query in name, surname, and skills.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "search")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "full-text-search")]
    [McpMeta("aliases", "find-persons, search-contacts, query-persons")]
    public List<Person> SearchPersons(
        [Description("Search query to match against name, surname, or skills")] string query)
    {
        return _database.Search(query);
    }

    [McpServerTool, Description("Add a new person to the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "create")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "create-person, insert-person, new-contact")]
    public Person AddPerson(
        [Description("Person's first name")] string name,
        [Description("Person's surname")] string surname,
        [Description("Person's age")] int age,
        [Description("Person's sex (M/F)")] Sex sex,
        [Description("Person's role")] string role,
        [Description("Person's department")] string department,
        [Description("Person's CV summary")] string cvSummary,
        [Description("List of person's skills")] List<string> skills)
    {
        var newPerson = new Person(Guid.NewGuid(), name, surname, age, sex, role, department, cvSummary, skills);
        return _database.Add(newPerson);
    }

    [McpServerTool, Description("Update an existing person's information in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "update")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "modify-person, edit-person, update-contact")]
    public Person? UpdatePerson(
        [Description("Person ID as Guid")] Guid id,
        [Description("Person's first name")] string name,
        [Description("Person's surname")] string surname,
        [Description("Person's age")] int age,
        [Description("Person's sex (M/F)")] Sex sex,
        [Description("Person's role")] string role,
        [Description("Person's department")] string department,
        [Description("Person's CV summary")] string cvSummary,
        [Description("List of person's skills")] List<string> skills)
    {
        return _database.Update(id, name, surname, age, sex, role, department, cvSummary, skills);
    }

    [McpServerTool, Description("Delete a person from the CRM system by ID.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "delete")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "bool")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "remove-person, delete-contact, remove-contact")]
    public bool DeletePerson([Description("Person ID as Guid")] Guid id)
    {
        return _database.Delete(id);
    }

    [McpServerTool, Description("Get the unique instance identifier for this CRM instance. This ID is unique for every server run/instance.")]
    [McpMeta("category", "system")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "sqlite")]
    [McpMeta("resultType", "Guid")]
    [McpMeta("context", "instance-info")]
    [McpMeta("aliases", "get-instance-id, instance-guid, server-id")]
    public Guid GetInstanceId()
    {
        return InstanceId;
    }

    public void Dispose()
    {
        _database?.Dispose();
    }
}
