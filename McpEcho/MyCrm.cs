using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpEcho;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<string> Skills { get; set; } = new();

    // Primary constructor
    public Person(Guid id, string name, string surname, int age, List<string> skills)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Age = age;
        Skills = skills;
    }
}

[McpServerToolType]
public sealed class MyCrm
{
    private static readonly List<Person> People =
    [
        new Person(Guid.NewGuid(), "John", "Doe", 30, ["C#", "SQL", "Azure"]),
        new Person(Guid.NewGuid(), "Jane", "Smith", 25, ["JavaScript", "React", "Node.js"]),
        new Person(Guid.NewGuid(), "Alice", "Johnson", 28, ["Python", "Django", "Machine Learning"])
    ];


    [McpServerTool, Description("Get personal information by ID.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-lookup")]
    [McpMeta("aliases", "find-by-id, lookup-by-id")]
    public Person? GetPersonById([Description("Person ID as Guid")] Guid id)
    {
        var person = People.FirstOrDefault(p => p.Id == id);

        return person;
    }

    [McpServerTool, Description("Find a person by their first name. Returns detailed CRM record.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "employee-search")]
    [McpMeta("aliases", "find-person, lookup-person, search-contact")]
    public Person? GetPersonByName([Description("Person name")] string name)
    {
        var person = People.FirstOrDefault(p => p.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

        return person;
    }

    [McpServerTool, Description("Find a person by their surname. Returns detailed CRM record.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "employee-search")]
    [McpMeta("aliases", "find-person, lookup-person, search-contact")]
    public Person? GetPersonBySurname([Description("Person surname")] string surname)
    {
        var person = People.FirstOrDefault(p => p.Surname.StartsWith(surname, StringComparison.OrdinalIgnoreCase));

        return person;
    }

    [McpServerTool, Description("Get personal information for all persons with given skill.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "skill-search")]
    [McpMeta("aliases", "find-by-skill, search-by-skill")]
    public List<Person> GetPersonsBySkill([Description("Person skill")] string skill)
    {
        var persons = People.Where(p => p.Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (persons.Count == 0)
        {
            return [];
        }

        return persons;
    }

    [McpServerTool, Description("Get all persons from the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "list-all")]
    [McpMeta("aliases", "list-persons, get-all-contacts")]
    public List<Person> GetAllPersons()
    {
        return People.ToList();
    }

    [McpServerTool, Description("Get skill statistics showing how many people have each skill.")]
    [McpMeta("category", "analytics")]
    [McpMeta("operation", "aggregate")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Dictionary")]
    [McpMeta("context", "skill-analytics")]
    [McpMeta("aliases", "skill-stats, skill-distribution")]
    public Dictionary<string, int> GetSkillStatistics()
    {
        var skillStats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var person in People)
        {
            foreach (var skill in person.Skills)
            {
                if (!skillStats.TryAdd(skill, 1))
                {
                    skillStats[skill]++;
                }
            }
        }

        return skillStats;
    }

    [McpServerTool, Description("Get the average age of all persons in the CRM system.")]
    [McpMeta("category", "analytics")]
    [McpMeta("operation", "aggregate")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "double")]
    [McpMeta("context", "age-analytics")]
    [McpMeta("aliases", "avg-age, mean-age")]
    public double GetAverageAge()
    {
        if (People.Count == 0)
        {
            return 0;
        }

        return People.Average(p => p.Age);
    }

    [McpServerTool, Description("Get the oldest person in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "age-ranking")]
    [McpMeta("aliases", "find-oldest, get-senior")]
    public Person? GetOldestPerson()
    {
        return People.OrderByDescending(p => p.Age).FirstOrDefault();
    }

    [McpServerTool, Description("Get the person with the most skills in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "read")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "skill-ranking")]
    [McpMeta("aliases", "find-most-skilled, get-expert")]
    public Person? GetMostSkilledPerson()
    {
        return People.OrderByDescending(p => p.Skills.Count).FirstOrDefault();
    }

    [McpServerTool, Description("Search for persons by full-text query in name, surname, and skills.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "search")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "List<Person>")]
    [McpMeta("context", "full-text-search")]
    [McpMeta("aliases", "find-persons, search-contacts, query-persons")]
    public List<Person> SearchPersons(
        [Description("Search query to match against name, surname, or skills")] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var results = People.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Surname.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Skills.Any(s => s.Contains(query, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        return results;
    }

    [McpServerTool, Description("Add a new person to the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "create")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "create-person, insert-person, new-contact")]
    public Person AddPerson(
        [Description("Person's first name")] string name,
        [Description("Person's surname")] string surname,
        [Description("Person's age")] int age,
        [Description("List of person's skills")] List<string> skills)
    {
        var newPerson = new Person(Guid.NewGuid(), name, surname, age, skills);
        People.Add(newPerson);
        return newPerson;
    }

    [McpServerTool, Description("Update an existing person's information in the CRM system.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "update")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "Person")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "modify-person, edit-person, update-contact")]
    public Person? UpdatePerson(
        [Description("Person ID as Guid")] Guid id,
        [Description("Person's first name")] string name,
        [Description("Person's surname")] string surname,
        [Description("Person's age")] int age,
        [Description("List of person's skills")] List<string> skills)
    {
        var person = People.FirstOrDefault(p => p.Id == id);
        
        if (person == null)
        {
            return null;
        }

        person.Name = name;
        person.Surname = surname;
        person.Age = age;
        person.Skills = skills;

        return person;
    }

    [McpServerTool, Description("Delete a person from the CRM system by ID.")]
    [McpMeta("category", "crm")]
    [McpMeta("operation", "delete")]
    [McpMeta("dataSource", "crm-memory")]
    [McpMeta("resultType", "bool")]
    [McpMeta("context", "person-management")]
    [McpMeta("aliases", "remove-person, delete-contact, remove-contact")]
    public bool DeletePerson([Description("Person ID as Guid")] Guid id)
    {
        var person = People.FirstOrDefault(p => p.Id == id);
        
        if (person == null)
        {
            return false;
        }

        People.Remove(person);
        return true;
    }
}
