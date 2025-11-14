using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpCrmDemo.Server;

[McpServerResourceType]
public class CrmResources
{
    [McpServerResource, Description("Describes the structure of a Person entity in the CRM system.")]
    [McpMeta("category", "crm-schema")]
    [McpMeta("type", "schema")]
    [McpMeta("context", "entity-description")]
    public object GetPersonSchema()
    {
        return new
        {
            Name = "Person",
            Description = "Represents a person (employee or contact) in the CRM database.",
            Fields = new Dictionary<string, string>
            {
                ["Id"] = "Unique Guid identifier for the person.",
                ["Name"] = "First name of the person.",
                ["Surname"] = "Last name of the person.",
                ["Age"] = "Age in years.",
                ["Sex"] = "Sex of the person (M/F).",
                ["Role"] = "Job title or position.",
                ["Department"] = "Department where the person works.",
                ["CvSummary"] = "Short summary of their CV or background.",
                ["Skills"] = "List of skills or competencies."
            }
        };
    }

    [McpServerResource, Description("List of departments available in the CRM system.")]
    [McpMeta("category", "crm-lookup")]
    [McpMeta("type", "enumeration")]
    [McpMeta("context", "department-list")]
    public string[] GetDepartments()
    {
        return new[]
        {
            "Engineering",
            "Sales",
            "Marketing",
            "Finance",
            "HR",
            "Management",
            "Support"
        };
    }

    [McpServerResource, Description("List of possible roles/job titles used in the CRM system.")]
    [McpMeta("category", "crm-lookup")]
    [McpMeta("type", "enumeration")]
    [McpMeta("context", "role-list")]
    public string[] GetRoles()
    {
        return new[]
        {
            "Developer",
            "Manager",
            "Analyst",
            "Sales Representative",
            "HR Specialist",
            "Designer",
            "Team Lead"
        };
    }

    [McpServerResource, Description("List of skill categories commonly used in the CRM system.")]
    [McpMeta("category", "crm-lookup")]
    [McpMeta("type", "enumeration")]
    [McpMeta("context", "skills-list")]
    public string[] GetSkills()
    {
        return new[]
        {
            "C#",
            "JavaScript",
            "SQL",
            "Project Management",
            "Communication",
            "Leadership",
            "Data Analysis",
            "Customer Service",
            "UI/UX Design"
        };
    }

    [McpServerResource, Description("Describes metadata about the CRM database itself.")]
    [McpMeta("category", "crm-metadata")]
    [McpMeta("type", "system")]
    [McpMeta("context", "system-info")]
    public static object GetDatabaseInfo()
    {
        return new
        {
            Name = "MyCRM",
            Version = "1.0.0",
            DataSource = "SQLite local DB",
            LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Description = "Demo CRM database used for testing and analytics purposes."
        };
    }
}