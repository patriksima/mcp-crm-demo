using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace McpCrmDemo.Server;

public class CrmDatabase : IDisposable
{
    private readonly string _connectionString;
    private readonly string _databasePath;
    private const string DefaultDatabasePath = "crm.db";

    public CrmDatabase(string? databasePath = null)
    {
        _databasePath = databasePath ?? DefaultDatabasePath;
        _connectionString = $"Data Source={_databasePath}";
        
        InitializeDatabase();
        
        // Initialize with seed data if database is empty
        if (GetCount() == 0)
        {
            SeedData();
        }
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS People (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Surname TEXT NOT NULL,
                Age INTEGER NOT NULL,
                Sex TEXT NOT NULL,
                Role TEXT NOT NULL,
                Department TEXT NOT NULL,
                CvSummary TEXT NOT NULL,
                Skills TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_people_name ON People(Name);
            CREATE INDEX IF NOT EXISTS idx_people_surname ON People(Surname);
        ";
        command.ExecuteNonQuery();
    }

    private int GetCount()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM People";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private void SeedData()
    {
        var seedPeople = new List<Person>
        {
            new Person(Guid.NewGuid(), "John", "Doe", 30, Sex.M, "Senior Developer", "Engineering", "Experienced software engineer with 10+ years in backend development, specializing in C# and cloud technologies.", ["C#", "SQL", "Azure"]),
            new Person(Guid.NewGuid(), "Jane", "Smith", 25, Sex.F, "Frontend Developer", "Engineering", "Creative frontend developer focused on modern web technologies and user experience design.", ["JavaScript", "React", "Node.js"]),
            new Person(Guid.NewGuid(), "Alice", "Johnson", 28, Sex.F, "Data Scientist", "Data Analytics", "Data scientist with expertise in machine learning and Python-based data analysis tools.", ["Python", "Django", "Machine Learning"])
        };

        foreach (var person in seedPeople)
        {
            Add(person);
        }
    }

    public Person? GetById(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id.ToString());

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ReadPerson(reader);
        }

        return null;
    }

    public Person? GetByName(string name)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People WHERE Name LIKE @name || '%' COLLATE NOCASE LIMIT 1";
        command.Parameters.AddWithValue("@name", name);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ReadPerson(reader);
        }

        return null;
    }

    public Person? GetBySurname(string surname)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People WHERE Surname LIKE @surname || '%' COLLATE NOCASE LIMIT 1";
        command.Parameters.AddWithValue("@surname", surname);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ReadPerson(reader);
        }

        return null;
    }

    public List<Person> GetBySkill(string skill)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People";

        var people = new List<Person>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var person = ReadPerson(reader);
            if (person.Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            {
                people.Add(person);
            }
        }

        return people;
    }

    public List<Person> GetByDepartment(string department)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People WHERE Department = @department COLLATE NOCASE";
        command.Parameters.AddWithValue("@department", department);

        var people = new List<Person>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            people.Add(ReadPerson(reader));
        }

        return people;
    }

    public List<Person> GetByRole(string role)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People WHERE Role = @role COLLATE NOCASE";
        command.Parameters.AddWithValue("@role", role);

        var people = new List<Person>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            people.Add(ReadPerson(reader));
        }

        return people;
    }

    public List<Person> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People";

        var people = new List<Person>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            people.Add(ReadPerson(reader));
        }

        return people;
    }

    public List<Person> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Person>();
        }

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills 
            FROM People 
            WHERE Name LIKE '%' || @query || '%' COLLATE NOCASE
               OR Surname LIKE '%' || @query || '%' COLLATE NOCASE
               OR Role LIKE '%' || @query || '%' COLLATE NOCASE
               OR Department LIKE '%' || @query || '%' COLLATE NOCASE
               OR CvSummary LIKE '%' || @query || '%' COLLATE NOCASE
               OR Skills LIKE '%' || @query || '%' COLLATE NOCASE
        ";
        command.Parameters.AddWithValue("@query", query);

        var people = new List<Person>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            people.Add(ReadPerson(reader));
        }

        return people;
    }

    public Person Add(Person person)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO People (Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills)
            VALUES (@id, @name, @surname, @age, @sex, @role, @department, @cvSummary, @skills)
        ";
        command.Parameters.AddWithValue("@id", person.Id.ToString());
        command.Parameters.AddWithValue("@name", person.Name);
        command.Parameters.AddWithValue("@surname", person.Surname);
        command.Parameters.AddWithValue("@age", person.Age);
        command.Parameters.AddWithValue("@sex", person.Sex.ToString());
        command.Parameters.AddWithValue("@role", person.Role);
        command.Parameters.AddWithValue("@department", person.Department);
        command.Parameters.AddWithValue("@cvSummary", person.CvSummary);
        command.Parameters.AddWithValue("@skills", JsonSerializer.Serialize(person.Skills));

        command.ExecuteNonQuery();
        return person;
    }

    public Person? Update(Guid id, string name, string surname, int age, Sex sex, string role, string department, string cvSummary, List<string> skills)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE People 
            SET Name = @name, Surname = @surname, Age = @age, Sex = @sex, Role = @role, Department = @department, CvSummary = @cvSummary, Skills = @skills
            WHERE Id = @id
        ";
        command.Parameters.AddWithValue("@id", id.ToString());
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@surname", surname);
        command.Parameters.AddWithValue("@age", age);
        command.Parameters.AddWithValue("@sex", sex.ToString());
        command.Parameters.AddWithValue("@role", role);
        command.Parameters.AddWithValue("@department", department);
        command.Parameters.AddWithValue("@cvSummary", cvSummary);
        command.Parameters.AddWithValue("@skills", JsonSerializer.Serialize(skills));

        var rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected == 0)
        {
            return null;
        }

        return GetById(id);
    }

    public bool Delete(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM People WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id.ToString());

        var rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public Dictionary<string, int> GetSkillStatistics()
    {
        var allPeople = GetAll();
        var skillStats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var person in allPeople)
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

    public double GetAverageAge()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT AVG(Age) FROM People";
        
        var result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value)
        {
            return 0;
        }

        return Convert.ToDouble(result);
    }

    public Person? GetOldestPerson()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Surname, Age, Sex, Role, Department, CvSummary, Skills FROM People ORDER BY Age DESC LIMIT 1";

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ReadPerson(reader);
        }

        return null;
    }

    public Person? GetMostSkilledPerson()
    {
        var allPeople = GetAll();
        return allPeople.OrderByDescending(p => p.Skills.Count).FirstOrDefault();
    }

    private Person ReadPerson(SqliteDataReader reader)
    {
        var id = Guid.Parse(reader.GetString(0));
        var name = reader.GetString(1);
        var surname = reader.GetString(2);
        var age = reader.GetInt32(3);
        var sex = Enum.Parse<Sex>(reader.GetString(4));
        var role = reader.GetString(5);
        var department = reader.GetString(6);
        var cvSummary = reader.GetString(7);
        var skillsJson = reader.GetString(8);
        var skills = JsonSerializer.Deserialize<List<string>>(skillsJson) ?? new List<string>();

        return new Person(id, name, surname, age, sex, role, department, cvSummary, skills);
    }

    public void Dispose()
    {
        // Delete test database files if they exist
        if (_databasePath != DefaultDatabasePath && File.Exists(_databasePath))
        {
            try
            {
                // Close any open connections
                SqliteConnection.ClearAllPools();
                
                // Small delay to ensure connections are closed
                System.Threading.Thread.Sleep(10);
                
                File.Delete(_databasePath);
            }
            catch
            {
                // Ignore deletion errors for test databases
            }
        }
    }
}
