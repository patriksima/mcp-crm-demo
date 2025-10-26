# MCP CRM Demo

A demonstration MCP (Model Context Protocol) server implementation that provides tools and resources for managing a simple Customer Relationship Management (CRM) system. This project showcases how to build an MCP server using .NET 9.0 and C# that exposes CRM functionality through MCP tools and resources.

## Overview

This MCP server provides a complete CRM solution with SQLite database backend, offering comprehensive person/contact management capabilities, analytics features, and search functionality. It demonstrates best practices for creating MCP servers with proper tool annotations, metadata, and resource management.

## Features

### Core Functionality
- **CRUD Operations**: Create, read, update, and delete person records
- **Search Capabilities**: Search by name, surname, skill, department, role, or full-text query
- **Analytics**: Skill statistics, average age calculation, oldest person, most skilled person
- **Resource Exposure**: Exposes CRM data as MCP resources for AI context
- **Instance Tracking**: Unique instance ID for each server run

### Technical Features
- **SQLite Database**: Persistent storage with automatic initialization and seeding
- **MCP Tools**: 18+ tools with detailed descriptions and metadata
- **MCP Resources**: Exposes all CRM data as a resource
- **STDIO Transport**: Uses standard input/output for MCP communication
- **Type Safety**: Strong typing with C# records and enums
- **Test Coverage**: Comprehensive unit tests included

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- SQLite (bundled via Microsoft.Data.Sqlite package)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/patriksima/mcp-crm-demo.git
cd McpEcho
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

## Usage

### Running the Server

Run the MCP server using:
```bash
dotnet run --project McpCrmDemo
```

The server will:
- Initialize the SQLite database (`crm.db`)
- Seed with sample data if the database is empty
- Start listening for MCP requests via STDIO transport

### Database

The server automatically creates and manages a SQLite database (`crm.db`) in the project directory. On first run, it seeds the database with three sample persons:
- John Doe (Senior Developer, Engineering)
- Jane Smith (Frontend Developer, Engineering)
- Alice Johnson (Data Scientist, Data Analytics)

## MCP Tools

The server exposes the following tools:

### Person Lookup
- `GetPersonById` - Get personal information by ID
- `GetPersonByName` - Find a person by their first name
- `GetPersonBySurname` - Find a person by their surname
- `GetAllPersons` - Get all persons from the CRM system

### Search Operations
- `SearchPersons` - Full-text search across name, surname, and skills
- `GetPersonsBySkill` - Get all persons with a specific skill
- `GetPersonsByDepartment` - Get all persons from a specific department
- `GetPersonsByRole` - Get all persons with a specific role/job title

### Analytics
- `GetSkillStatistics` - Get skill distribution statistics
- `GetAverageAge` - Get the average age of all persons
- `GetOldestPerson` - Get the oldest person in the system
- `GetMostSkilledPerson` - Get the person with the most skills

### CRUD Operations
- `AddPerson` - Add a new person to the CRM system
- `UpdatePerson` - Update an existing person's information
- `DeletePerson` - Delete a person from the CRM system

### System
- `GetInstanceId` - Get the unique instance identifier for this server run

## MCP Resources

The server exposes the following resources:

- `CrmData` - Returns all persons from the CRM system as a resource that can be used for AI context

## Project Structure

```
McpEcho/
├── McpCrmDemo/
│   ├── Program.cs           # Application entry point and MCP server setup
│   ├── MyCrm.cs             # MCP tools implementation
│   ├── CrmResources.cs      # MCP resources implementation
│   ├── CrmDatabase.cs       # SQLite database operations
│   └── McpCrmDemo.csproj    # Project configuration
├── McpCrmDemo.Tests/
│   ├── MyCrmTests.cs        # Unit tests for CRM functionality
│   └── McpCrmDemo.Tests.csproj
├── crm.db                   # SQLite database (auto-generated)
├── .gitignore
├── McpCrmDemo.sln           # Solution file
└── README.md
```

## Data Model

### Person Entity

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public Sex Sex { get; set; }  // M or F
    public string Role { get; set; }
    public string Department { get; set; }
    public string CvSummary { get; set; }
    public List<string> Skills { get; set; }
}
```

## Development

### Running Tests

Execute the test suite:
```bash
dotnet test
```

### Building for Release

Build in release mode:
```bash
dotnet build -c Release
```

### MCP Metadata

All tools include comprehensive metadata:
- **category**: Categorizes the operation (crm, analytics, system)
- **operation**: Type of operation (read, create, update, delete, aggregate, search)
- **dataSource**: Data source type (sqlite)
- **resultType**: Return type of the tool
- **context**: Usage context for the tool
- **aliases**: Alternative names for the tool

## Configuration

The server can be configured through:
- **Database Path**: Pass a custom database path to the `CrmDatabase` constructor
- **Transport Mechanism**: Currently uses STDIO (commented-out HTTP transport available)
- **Logging**: Logs to stderr via Microsoft.Extensions.Logging

## Integration with MCP Clients

This server can be integrated with any MCP-compatible client (e.g., Claude Desktop, Cline). Configure your MCP client to launch this server using:

```json
{
  "mcpServers": {
    "crm-demo": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/McpCrmDemo"]
    }
  }
}
```

## Dependencies

- **Microsoft.Extensions.Hosting** (9.0.10) - Application hosting infrastructure
- **Microsoft.Data.Sqlite** (9.0.10) - SQLite database provider
- **ModelContextProtocol** (0.4.0-preview.3) - MCP server implementation

## Use Cases

This demo server can be used to:
- Learn how to build MCP servers with .NET
- Understand MCP tool and resource patterns
- Prototype CRM integrations with AI assistants
- Test MCP client implementations
- Serve as a template for custom MCP servers

## License

This project is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License (CC BY-NC-ND 4.0).

**What this means:**
- ✅ You can share and redistribute the code
- ✅ You must give appropriate credit to the author
- ❌ You cannot use it for commercial purposes
- ❌ You cannot distribute modified versions

See the [LICENSE](LICENSE) file for full details or visit [https://creativecommons.org/licenses/by-nc-nd/4.0/](https://creativecommons.org/licenses/by-nc-nd/4.0/)

## Contributing

This is a demo project for learning purposes. Feel free to fork and modify for your own use cases.

## Support

For issues or questions, please open an issue on the GitHub repository.
