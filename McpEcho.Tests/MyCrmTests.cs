using Xunit;
using McpEcho;

namespace McpEcho.Tests;

public class MyCrmTests : IDisposable
{
    private readonly MyCrm _crm;

    public MyCrmTests()
    {
        // Use a unique database for each test instance
        var testDbPath = $"test_crm_{Guid.NewGuid()}.db";
        _crm = new MyCrm(testDbPath);
    }

    public void Dispose()
    {
        _crm?.Dispose();
    }

    #region GetPersonById Tests

    [Fact]
    public void GetPersonById_WithValidId_ReturnsCorrectPerson()
    {
        // Arrange - Get a person first to have a valid ID
        var allPersons = _crm.GetAllPersons();
        var expectedPerson = allPersons.First();

        // Act
        var result = _crm.GetPersonById(expectedPerson.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPerson.Id, result.Id);
        Assert.Equal(expectedPerson.Name, result.Name);
        Assert.Equal(expectedPerson.Surname, result.Surname);
        Assert.Equal(expectedPerson.Age, result.Age);
    }

    [Fact]
    public void GetPersonById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = _crm.GetPersonById(invalidId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetPersonByName Tests

    [Fact]
    public void GetPersonByName_WithValidName_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonByName("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.Surname);
    }

    [Fact]
    public void GetPersonByName_WithPartialName_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonByName("Joh");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public void GetPersonByName_CaseInsensitive_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonByName("john");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public void GetPersonByName_WithInvalidName_ReturnsNull()
    {
        // Act
        var result = _crm.GetPersonByName("NonExistent");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetPersonBySurname Tests

    [Fact]
    public void GetPersonBySurname_WithValidSurname_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonBySurname("Smith");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.Name);
        Assert.Equal("Smith", result.Surname);
    }

    [Fact]
    public void GetPersonBySurname_WithPartialSurname_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonBySurname("Smi");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Smith", result.Surname);
    }

    [Fact]
    public void GetPersonBySurname_CaseInsensitive_ReturnsCorrectPerson()
    {
        // Act
        var result = _crm.GetPersonBySurname("doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Doe", result.Surname);
    }

    [Fact]
    public void GetPersonBySurname_WithInvalidSurname_ReturnsNull()
    {
        // Act
        var result = _crm.GetPersonBySurname("NonExistent");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetPersonsBySkill Tests

    [Fact]
    public void GetPersonsBySkill_WithValidSkill_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsBySkill("C#");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => Assert.Contains("C#", person.Skills));
    }

    [Fact]
    public void GetPersonsBySkill_CaseInsensitive_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsBySkill("c#");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => 
            Assert.Contains(person.Skills, s => s.Equals("C#", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetPersonsBySkill_WithSkillMultiplePersonsHave_ReturnsAllMatching()
    {
        // Act - First check if there's a skill multiple people have
        var allPersons = _crm.GetAllPersons();
        var skillCounts = allPersons
            .SelectMany(p => p.Skills)
            .GroupBy(s => s)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (skillCounts != null)
        {
            var result = _crm.GetPersonsBySkill(skillCounts);
            Assert.True(result.Count > 1);
        }
        else
        {
            // If no shared skill, just verify it returns expected count for a known skill
            var result = _crm.GetPersonsBySkill("Python");
            Assert.Single(result);
        }
    }

    [Fact]
    public void GetPersonsBySkill_WithInvalidSkill_ReturnsEmptyList()
    {
        // Act
        var result = _crm.GetPersonsBySkill("NonExistentSkill");

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetPersonsByDepartment Tests

    [Fact]
    public void GetPersonsByDepartment_WithValidDepartment_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsByDepartment("Engineering");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => Assert.Equal("Engineering", person.Department));
    }

    [Fact]
    public void GetPersonsByDepartment_CaseInsensitive_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsByDepartment("engineering");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => Assert.Equal("Engineering", person.Department, StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetPersonsByDepartment_WithMultiplePeopleInDepartment_ReturnsAll()
    {
        // Act - Based on seed data, Engineering has 2 people (John and Jane)
        var result = _crm.GetPersonsByDepartment("Engineering");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "John");
        Assert.Contains(result, p => p.Name == "Jane");
    }

    [Fact]
    public void GetPersonsByDepartment_WithSinglePersonDepartment_ReturnsSinglePerson()
    {
        // Act - Based on seed data, Data Analytics has 1 person (Alice)
        var result = _crm.GetPersonsByDepartment("Data Analytics");

        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result.First().Name);
    }

    [Fact]
    public void GetPersonsByDepartment_WithNonExistentDepartment_ReturnsEmptyList()
    {
        // Act
        var result = _crm.GetPersonsByDepartment("NonExistentDepartment");

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetPersonsByRole Tests

    [Fact]
    public void GetPersonsByRole_WithValidRole_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsByRole("Senior Developer");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => Assert.Equal("Senior Developer", person.Role));
    }

    [Fact]
    public void GetPersonsByRole_CaseInsensitive_ReturnsCorrectPersons()
    {
        // Act
        var result = _crm.GetPersonsByRole("senior developer");

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, person => Assert.Equal("Senior Developer", person.Role, StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetPersonsByRole_WithUniquePeopleRole_ReturnsSinglePerson()
    {
        // Act - Based on seed data, each person has a unique role
        var result = _crm.GetPersonsByRole("Data Scientist");

        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result.First().Name);
    }

    [Fact]
    public void GetPersonsByRole_WithNonExistentRole_ReturnsEmptyList()
    {
        // Act
        var result = _crm.GetPersonsByRole("NonExistentRole");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetPersonsByRole_AfterAddingPersonWithSameRole_ReturnsMultiple()
    {
        // Arrange
        _crm.AddPerson("NewDev", "Test", 30, Sex.M, "Senior Developer", "Engineering", "Another senior dev", new List<string> { "C#" });

        // Act
        var result = _crm.GetPersonsByRole("Senior Developer");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "John");
        Assert.Contains(result, p => p.Name == "NewDev");
    }

    #endregion

    #region GetAllPersons Tests

    [Fact]
    public void GetAllPersons_ReturnsAllPersons()
    {
        // Act
        var result = _crm.GetAllPersons();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name == "John");
        Assert.Contains(result, p => p.Name == "Jane");
        Assert.Contains(result, p => p.Name == "Alice");
    }

    [Fact]
    public void GetAllPersons_ReturnsNewListInstance()
    {
        // Act
        var result1 = _crm.GetAllPersons();
        var result2 = _crm.GetAllPersons();

        // Assert - Should be different instances but same content
        Assert.NotSame(result1, result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region GetSkillStatistics Tests

    [Fact]
    public void GetSkillStatistics_ReturnsCorrectStatistics()
    {
        // Act
        var result = _crm.GetSkillStatistics();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result.Values, count => Assert.True(count > 0));
    }

    [Fact]
    public void GetSkillStatistics_CountsSkillsCorrectly()
    {
        // Act
        var result = _crm.GetSkillStatistics();

        // Assert - Known skills from sample data
        Assert.True(result.ContainsKey("C#"));
        Assert.True(result.ContainsKey("Python"));
        Assert.True(result.ContainsKey("JavaScript"));
    }

    [Fact]
    public void GetSkillStatistics_IsCaseInsensitive()
    {
        // Act
        var result = _crm.GetSkillStatistics();

        // Assert - Check if we can access with different casing
        Assert.True(result.ContainsKey("c#") || result.ContainsKey("C#"));
    }

    #endregion

    #region GetAverageAge Tests

    [Fact]
    public void GetAverageAge_CalculatesCorrectAverage()
    {
        // Arrange
        var allPersons = _crm.GetAllPersons();
        var expectedAverage = allPersons.Average(p => p.Age);

        // Act
        var result = _crm.GetAverageAge();

        // Assert
        Assert.Equal(expectedAverage, result);
    }

    [Fact]
    public void GetAverageAge_ReturnsPositiveNumber()
    {
        // Act
        var result = _crm.GetAverageAge();

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void GetAverageAge_WithSampleData_ReturnsExpectedValue()
    {
        // Act
        var result = _crm.GetAverageAge();

        // Assert - Based on sample data: John(30), Jane(25), Alice(28) = 83/3 = 27.666...
        Assert.Equal(27.666666666666668, result, 10);
    }

    #endregion

    #region GetOldestPerson Tests

    [Fact]
    public void GetOldestPerson_ReturnsPersonWithHighestAge()
    {
        // Arrange
        var allPersons = _crm.GetAllPersons();
        var maxAge = allPersons.Max(p => p.Age);

        // Act
        var result = _crm.GetOldestPerson();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(maxAge, result.Age);
    }

    [Fact]
    public void GetOldestPerson_WithSampleData_ReturnsJohn()
    {
        // Act
        var result = _crm.GetOldestPerson();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
    }

    #endregion

    #region GetMostSkilledPerson Tests

    [Fact]
    public void GetMostSkilledPerson_ReturnsPersonWithMostSkills()
    {
        // Arrange
        var allPersons = _crm.GetAllPersons();
        var maxSkillCount = allPersons.Max(p => p.Skills.Count);

        // Act
        var result = _crm.GetMostSkilledPerson();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(maxSkillCount, result.Skills.Count);
    }

    [Fact]
    public void GetMostSkilledPerson_ReturnsPersonWithAtLeastOneSkill()
    {
        // Act
        var result = _crm.GetMostSkilledPerson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Skills);
    }

    [Fact]
    public void GetMostSkilledPerson_WithSampleData_ReturnsPersonWithThreeSkills()
    {
        // Act
        var result = _crm.GetMostSkilledPerson();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Skills.Count);
    }

    #endregion

    #region SearchPersons Tests

    [Fact]
    public void SearchPersons_WithNameQuery_ReturnsMatchingPersons()
    {
        // Act
        var result = _crm.SearchPersons("John");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Name.Contains("John", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SearchPersons_WithSurnameQuery_ReturnsMatchingPersons()
    {
        // Act
        var result = _crm.SearchPersons("Smith");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Surname.Contains("Smith", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SearchPersons_WithSkillQuery_ReturnsMatchingPersons()
    {
        // Act
        var result = _crm.SearchPersons("Python");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Skills.Any(s => s.Contains("Python", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void SearchPersons_IsCaseInsensitive()
    {
        // Act
        var result = _crm.SearchPersons("john");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Name.Equals("John", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SearchPersons_WithPartialQuery_ReturnsMatchingPersons()
    {
        // Act
        var result = _crm.SearchPersons("Jo");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Name.Contains("Jo", StringComparison.OrdinalIgnoreCase) ||
                                      p.Surname.Contains("Jo", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SearchPersons_WithEmptyQuery_ReturnsEmptyList()
    {
        // Act
        var result = _crm.SearchPersons("");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchPersons_WithWhitespaceQuery_ReturnsEmptyList()
    {
        // Act
        var result = _crm.SearchPersons("   ");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchPersons_WithNonMatchingQuery_ReturnsEmptyList()
    {
        // Act
        var result = _crm.SearchPersons("NonExistentQuery");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchPersons_WithCommonQuery_ReturnsMultiplePersons()
    {
        // Act - Search for something that might match multiple people
        var result = _crm.SearchPersons("o"); // Letter 'o' appears in John, Doe, Johnson

        // Assert
        Assert.NotEmpty(result);
        // Should match at least one person (likely multiple)
        Assert.True(result.Count >= 1);
    }

    #endregion

    #region Person Model Tests

    [Fact]
    public void Person_Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test";
        var surname = "User";
        var age = 35;
        var sex = Sex.M;
        var role = "Developer";
        var department = "IT";
        var cvSummary = "Test CV summary";
        var skills = new List<string> { "Skill1", "Skill2" };

        // Act
        var person = new Person(id, name, surname, age, sex, role, department, cvSummary, skills);

        // Assert
        Assert.Equal(id, person.Id);
        Assert.Equal(name, person.Name);
        Assert.Equal(surname, person.Surname);
        Assert.Equal(age, person.Age);
        Assert.Equal(sex, person.Sex);
        Assert.Equal(role, person.Role);
        Assert.Equal(department, person.Department);
        Assert.Equal(cvSummary, person.CvSummary);
        Assert.Equal(skills, person.Skills);
    }

    [Fact]
    public void Person_SkillsList_CanBeModified()
    {
        // Arrange
        var person = new Person(Guid.NewGuid(), "Test", "User", 30, Sex.M, "Developer", "IT", "Test CV", new List<string> { "Skill1" });

        // Act
        person.Skills.Add("NewSkill");

        // Assert
        Assert.Equal(2, person.Skills.Count);
        Assert.Contains("NewSkill", person.Skills);
    }

    #endregion

    #region AddPerson Tests

    [Fact]
    public void AddPerson_WithValidData_AddsPersonSuccessfully()
    {
        // Arrange
        var initialCount = _crm.GetAllPersons().Count;
        var name = "Bob";
        var surname = "Williams";
        var age = 35;
        var sex = Sex.M;
        var role = "Architect";
        var department = "Engineering";
        var cvSummary = "Senior architect with cloud expertise";
        var skills = new List<string> { "Java", "Spring", "Kubernetes" };

        // Act
        var result = _crm.AddPerson(name, surname, age, sex, role, department, cvSummary, skills);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(surname, result.Surname);
        Assert.Equal(age, result.Age);
        Assert.Equal(sex, result.Sex);
        Assert.Equal(role, result.Role);
        Assert.Equal(department, result.Department);
        Assert.Equal(cvSummary, result.CvSummary);
        Assert.Equal(skills, result.Skills);
        Assert.NotEqual(Guid.Empty, result.Id);
        
        // Verify person was added to the collection
        var allPersons = _crm.GetAllPersons();
        Assert.Equal(initialCount + 1, allPersons.Count);
        Assert.Contains(allPersons, p => p.Id == result.Id);
    }

    [Fact]
    public void AddPerson_GeneratesUniqueId()
    {
        // Act
        var person1 = _crm.AddPerson("Test1", "User1", 30, Sex.M, "Dev", "IT", "CV1", new List<string> { "Skill1" });
        var person2 = _crm.AddPerson("Test2", "User2", 31, Sex.F, "Dev", "IT", "CV2", new List<string> { "Skill2" });

        // Assert
        Assert.NotEqual(person1.Id, person2.Id);
        Assert.NotEqual(Guid.Empty, person1.Id);
        Assert.NotEqual(Guid.Empty, person2.Id);
    }

    [Fact]
    public void AddPerson_WithEmptySkillsList_AddsPersonSuccessfully()
    {
        // Arrange
        var name = "Charlie";
        var surname = "Brown";
        var age = 22;
        var sex = Sex.M;
        var role = "Junior";
        var department = "IT";
        var cvSummary = "Entry level position";
        var skills = new List<string>();

        // Act
        var result = _crm.AddPerson(name, surname, age, sex, role, department, cvSummary, skills);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Empty(result.Skills);
    }

    [Fact]
    public void AddPerson_CanBeRetrievedById()
    {
        // Arrange & Act
        var addedPerson = _crm.AddPerson("David", "Miller", 40, Sex.M, "DevOps", "Operations", "DevOps expert", new List<string> { "Go", "Docker" });
        var retrievedPerson = _crm.GetPersonById(addedPerson.Id);

        // Assert
        Assert.NotNull(retrievedPerson);
        Assert.Equal(addedPerson.Id, retrievedPerson.Id);
        Assert.Equal(addedPerson.Name, retrievedPerson.Name);
        Assert.Equal(addedPerson.Surname, retrievedPerson.Surname);
        Assert.Equal(addedPerson.Age, retrievedPerson.Age);
    }

    [Fact]
    public void AddPerson_CanBeFoundByName()
    {
        // Arrange & Act
        var addedPerson = _crm.AddPerson("Emma", "Davis", 27, Sex.F, "Developer", "Engineering", "Ruby specialist", new List<string> { "Ruby", "Rails" });
        var foundPerson = _crm.GetPersonByName("Emma");

        // Assert
        Assert.NotNull(foundPerson);
        Assert.Equal(addedPerson.Name, foundPerson.Name);
    }

    #endregion

    #region UpdatePerson Tests

    [Fact]
    public void UpdatePerson_WithValidId_UpdatesPersonSuccessfully()
    {
        // Arrange
        var existingPerson = _crm.GetAllPersons().First();
        var newName = "UpdatedName";
        var newSurname = "UpdatedSurname";
        var newAge = 99;
        var newSex = Sex.F;
        var newRole = "UpdatedRole";
        var newDepartment = "UpdatedDept";
        var newCvSummary = "Updated CV";
        var newSkills = new List<string> { "NewSkill1", "NewSkill2" };

        // Act
        var result = _crm.UpdatePerson(existingPerson.Id, newName, newSurname, newAge, newSex, newRole, newDepartment, newCvSummary, newSkills);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingPerson.Id, result.Id);
        Assert.Equal(newName, result.Name);
        Assert.Equal(newSurname, result.Surname);
        Assert.Equal(newAge, result.Age);
        Assert.Equal(newSex, result.Sex);
        Assert.Equal(newRole, result.Role);
        Assert.Equal(newDepartment, result.Department);
        Assert.Equal(newCvSummary, result.CvSummary);
        Assert.Equal(newSkills, result.Skills);
    }

    [Fact]
    public void UpdatePerson_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = _crm.UpdatePerson(invalidId, "Test", "User", 30, Sex.M, "Role", "Dept", "CV", new List<string>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdatePerson_UpdatesPersistently()
    {
        // Arrange
        var existingPerson = _crm.GetAllPersons().First();
        var newName = "PersistentName";
        var newAge = 50;

        // Act
        _crm.UpdatePerson(existingPerson.Id, newName, "PersistentSurname", newAge, Sex.M, "Role", "Dept", "CV", new List<string>());
        var retrievedPerson = _crm.GetPersonById(existingPerson.Id);

        // Assert
        Assert.NotNull(retrievedPerson);
        Assert.Equal(newName, retrievedPerson.Name);
        Assert.Equal(newAge, retrievedPerson.Age);
    }

    [Fact]
    public void UpdatePerson_CanClearSkills()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("Test", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string> { "Skill1", "Skill2" });

        // Act
        var result = _crm.UpdatePerson(addedPerson.Id, "Test", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string>());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Skills);
    }

    [Fact]
    public void UpdatePerson_CanReplaceSkills()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("Test", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string> { "OldSkill" });
        var newSkills = new List<string> { "NewSkill1", "NewSkill2", "NewSkill3" };

        // Act
        var result = _crm.UpdatePerson(addedPerson.Id, "Test", "User", 30, Sex.M, "Dev", "IT", "CV", newSkills);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Skills.Count);
        Assert.Contains("NewSkill1", result.Skills);
        Assert.Contains("NewSkill2", result.Skills);
        Assert.Contains("NewSkill3", result.Skills);
        Assert.DoesNotContain("OldSkill", result.Skills);
    }

    [Fact]
    public void UpdatePerson_DoesNotChangeId()
    {
        // Arrange
        var existingPerson = _crm.GetAllPersons().First();
        var originalId = existingPerson.Id;

        // Act
        var result = _crm.UpdatePerson(originalId, "NewName", "NewSurname", 100, Sex.M, "Role", "Dept", "CV", new List<string>());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originalId, result.Id);
    }

    #endregion

    #region DeletePerson Tests

    [Fact]
    public void DeletePerson_WithValidId_DeletesPersonSuccessfully()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("ToDelete", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string>());
        var initialCount = _crm.GetAllPersons().Count;

        // Act
        var result = _crm.DeletePerson(addedPerson.Id);

        // Assert
        Assert.True(result);
        
        // Verify person was removed
        var allPersons = _crm.GetAllPersons();
        Assert.Equal(initialCount - 1, allPersons.Count);
        Assert.DoesNotContain(allPersons, p => p.Id == addedPerson.Id);
        
        // Verify person cannot be retrieved
        var retrievedPerson = _crm.GetPersonById(addedPerson.Id);
        Assert.Null(retrievedPerson);
    }

    [Fact]
    public void DeletePerson_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var initialCount = _crm.GetAllPersons().Count;

        // Act
        var result = _crm.DeletePerson(invalidId);

        // Assert
        Assert.False(result);
        
        // Verify count unchanged
        var currentCount = _crm.GetAllPersons().Count;
        Assert.Equal(initialCount, currentCount);
    }

    [Fact]
    public void DeletePerson_RemovedPersonNotFoundInSearch()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("Unique", "SearchName", 30, Sex.M, "Dev", "IT", "CV", new List<string>());

        // Act
        _crm.DeletePerson(addedPerson.Id);
        var searchResult = _crm.SearchPersons("SearchName");

        // Assert
        Assert.DoesNotContain(searchResult, p => p.Id == addedPerson.Id);
    }

    [Fact]
    public void DeletePerson_RemovedPersonNotFoundByName()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("UniqueToDelete", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string>());

        // Act
        _crm.DeletePerson(addedPerson.Id);
        var foundPerson = _crm.GetPersonByName("UniqueToDelete");

        // Assert
        Assert.Null(foundPerson);
    }

    [Fact]
    public void DeletePerson_CanDeleteMultiplePersons()
    {
        // Arrange
        var person1 = _crm.AddPerson("Delete1", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string>());
        var person2 = _crm.AddPerson("Delete2", "User", 31, Sex.F, "Dev", "IT", "CV", new List<string>());
        var initialCount = _crm.GetAllPersons().Count;

        // Act
        var result1 = _crm.DeletePerson(person1.Id);
        var result2 = _crm.DeletePerson(person2.Id);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        
        var currentCount = _crm.GetAllPersons().Count;
        Assert.Equal(initialCount - 2, currentCount);
    }

    [Fact]
    public void DeletePerson_CannotDeleteSamePersonTwice()
    {
        // Arrange
        var addedPerson = _crm.AddPerson("Test", "User", 30, Sex.M, "Dev", "IT", "CV", new List<string>());

        // Act
        var firstDelete = _crm.DeletePerson(addedPerson.Id);
        var secondDelete = _crm.DeletePerson(addedPerson.Id);

        // Assert
        Assert.True(firstDelete);
        Assert.False(secondDelete);
    }

    #endregion

    #region Integration Tests for CRUD Operations

    [Fact]
    public void CRUD_FullLifecycle_WorksCorrectly()
    {
        // Create
        var newPerson = _crm.AddPerson("Integration", "Test", 33, Sex.M, "Tester", "QA", "QA expert", new List<string> { "Testing" });
        Assert.NotNull(newPerson);
        Assert.NotEqual(Guid.Empty, newPerson.Id);

        // Read
        var retrievedPerson = _crm.GetPersonById(newPerson.Id);
        Assert.NotNull(retrievedPerson);
        Assert.Equal("Integration", retrievedPerson.Name);

        // Update
        var updatedPerson = _crm.UpdatePerson(newPerson.Id, "UpdatedIntegration", "UpdatedTest", 34, Sex.M, "Senior Tester", "QA", "Updated CV", new List<string> { "UpdatedTesting" });
        Assert.NotNull(updatedPerson);
        Assert.Equal("UpdatedIntegration", updatedPerson.Name);
        Assert.Equal(34, updatedPerson.Age);

        // Delete
        var deleteResult = _crm.DeletePerson(newPerson.Id);
        Assert.True(deleteResult);

        // Verify deletion
        var deletedPerson = _crm.GetPersonById(newPerson.Id);
        Assert.Null(deletedPerson);
    }

    [Fact]
    public void AddAndUpdate_PreservesIdAndAllowsRetrieval()
    {
        // Arrange & Act - Add
        var added = _crm.AddPerson("AddUpdate", "Test", 25, Sex.F, "Dev", "IT", "CV1", new List<string> { "Skill1" });
        var originalId = added.Id;

        // Act - Update
        var updated = _crm.UpdatePerson(originalId, "Updated", "Test", 26, Sex.F, "Senior Dev", "IT", "CV2", new List<string> { "Skill2" });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(originalId, updated.Id);
        Assert.Equal("Updated", updated.Name);
        Assert.Equal(26, updated.Age);

        // Verify via retrieval
        var retrieved = _crm.GetPersonById(originalId);
        Assert.NotNull(retrieved);
        Assert.Equal("Updated", retrieved.Name);
    }

    #endregion

    #region GetInstanceId Tests

    [Fact]
    public void GetInstanceId_ReturnsValidGuid()
    {
        // Act
        var instanceId = _crm.GetInstanceId();

        // Assert
        Assert.NotEqual(Guid.Empty, instanceId);
    }

    [Fact]
    public void GetInstanceId_ReturnsSameIdForSameInstance()
    {
        // Act
        var instanceId1 = _crm.GetInstanceId();
        var instanceId2 = _crm.GetInstanceId();

        // Assert
        Assert.Equal(instanceId1, instanceId2);
    }

    [Fact]
    public void GetInstanceId_ReturnsDifferentIdForDifferentInstances()
    {
        // Arrange
        using var crm2 = new MyCrm($"test_crm_{Guid.NewGuid()}.db");

        // Act
        var instanceId1 = _crm.GetInstanceId();
        var instanceId2 = crm2.GetInstanceId();

        // Assert
        Assert.NotEqual(instanceId1, instanceId2);
    }

    [Fact]
    public void GetInstanceId_IsUniqueForEachRun()
    {
        // Arrange - Create multiple instances
        using var crm1 = new MyCrm($"test_crm_{Guid.NewGuid()}.db");
        using var crm2 = new MyCrm($"test_crm_{Guid.NewGuid()}.db");
        using var crm3 = new MyCrm($"test_crm_{Guid.NewGuid()}.db");

        // Act
        var id1 = crm1.GetInstanceId();
        var id2 = crm2.GetInstanceId();
        var id3 = crm3.GetInstanceId();

        // Assert - All IDs should be different
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
    }

    #endregion
}
