using Bogus;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Seeding;

public static class DbSeeder
{
    private static readonly Guid AdminRoleId = Guid.Parse("7cfd6b8e-4289-4f97-911d-b24bb5608f68");
    private static readonly Guid RegularRoleId = Guid.Parse("06af8b49-9a2a-4017-af1d-c2d0ef25ec45");
    private static readonly Guid RecruiterRoleId = Guid.Parse("d032d2d0-0681-442f-b784-b2a15c1a4dde");

    private static readonly Guid[] CategoryIds =
    [
        Guid.Parse("c83d7576-9fc9-444c-81ed-f8417106f306"),
        Guid.Parse("79421e86-2141-4505-8272-53219ba4035a"),
        Guid.Parse("14bf7b07-87d4-406a-9ae8-0e1003b5d416"),
        Guid.Parse("00676f84-8715-4515-b05f-e5ee5985b56b")
    ];

    public static async Task SeedAsync(CvManagementDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        Randomizer.Seed = new Random(9001);

        var admin = SeedAdminUser(context);
        SeedAttributeCategories(context);
        var technologies = SeedTechnologies(context);
        var positions = SeedPositions(context, technologies);
        var attributeDefinitions = SeedAttributeDefinitions(context);
        var regularUsers = SeedRegularUsers(context);
        var recruiterUsers = SeedRecruiterUsers(context);
        var allUsers = new[] { admin }.Concat(regularUsers).Concat(recruiterUsers).ToList();
        var projects = SeedProjects(context, allUsers, technologies);
        var attributeValues = SeedAttributeValues(context, attributeDefinitions);
        SeedUserAttributeValues(context, allUsers, attributeValues);
        SeedCvs(context, regularUsers, positions, projects);
        SeedDiscussions(context, positions, allUsers);
        SeedAccessRules(context, positions, attributeValues);

        await context.SaveChangesAsync();
    }

    private static User SeedAdminUser(CvManagementDbContext context)
    {
        var admin = new User
        {
            Id = MakeId(1),
            Email = "admin@admin.com",
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("admin"),
            RoleId = AdminRoleId,
            IsBlocked = false,
            IsConfirmed = true,
            Version = 0,
            ProfileData = new ProfileData
            {
                FirstName = "Admin",
                LastName = "Admin",
                Location = "Minsk",
                PhoneNumber = (new Faker()).Phone.PhoneNumber(),
            }
        };
        context.Users.Add(admin);
        
        return admin;
    }

    private static List<AttributeCategory> SeedAttributeCategories(CvManagementDbContext context)
    {
        var categories = new List<AttributeCategory>
        {
            new() { Id = CategoryIds[0], Name = "Contact Info" },
            new() { Id = CategoryIds[1], Name = "Professional Info" },
            new() { Id = CategoryIds[2], Name = "Personal Info" },
            new() { Id = CategoryIds[3], Name = "Education & Work" }
        };

        context.AttributeCategories.AddRange(categories);
        
        return categories;
    }

    private static List<Technology> SeedTechnologies(CvManagementDbContext context)
    {
        var techNames = new[]
        {
            "C#", ".NET", "ASP.NET Core", "Entity Framework Core", "Blazor",
            "React", "Angular", "Vue.js", "TypeScript", "JavaScript",
            "Python", "Django", "PostgreSQL", "MongoDB", "Docker",
            "Kubernetes", "Azure", "AWS", "Redis", "RabbitMQ"
        };

        var technologies = techNames.Select(name => new Technology { Name = name }).ToList();
        context.Technologies.AddRange(technologies);
        
        return technologies;
    }

    private static List<Position> SeedPositions(CvManagementDbContext context, List<Technology> technologies)
    {
        var faker = new Faker();
        var positions = new List<Position>();
        var positionTechnologies = new List<PositionTechnology>();

        var positionDefs = new[]
        {
            (Title: "Junior .NET Developer", Desc: "Entry-level .NET development position", Level: ExpertiseLevel.Junior, MaxProj: 2u, Restricted: false, Techs: new[] { "C#", ".NET", "ASP.NET Core", "PostgreSQL" }),
            (Title: "Middle .NET Developer", Desc: "Mid-level .NET development position", Level: ExpertiseLevel.Middle, MaxProj: 3u, Restricted: false, Techs: new[] { "C#", ".NET", "ASP.NET Core", "Entity Framework Core", "PostgreSQL", "Docker" }),
            (Title: "Senior .NET Developer", Desc: "Senior .NET development position", Level: ExpertiseLevel.Senior, MaxProj: 4u, Restricted: false, Techs: new[] { "C#", ".NET", "ASP.NET Core", "Entity Framework Core", "Docker", "Azure", "Redis" }),
            (Title: "Frontend Developer", Desc: "Frontend development position", Level: ExpertiseLevel.Middle, MaxProj: 3u, Restricted: true, Techs: new[] { "React", "TypeScript", "JavaScript", "Vue.js" }),
            (Title: "Fullstack Developer", Desc: "Fullstack development position", Level: ExpertiseLevel.Senior, MaxProj: 4u, Restricted: true, Techs: new[] { "C#", "ASP.NET Core", "React", "TypeScript", "PostgreSQL", "Docker" }),
            (Title: "DevOps Engineer", Desc: "DevOps engineering position", Level: ExpertiseLevel.Senior, MaxProj: 3u, Restricted: false, Techs: new[] { "Docker", "Kubernetes", "Azure", "AWS", "Python" })
        };

        for (var i = 0; i < positionDefs.Length; i++)
        {
            var def = positionDefs[i];
            var position = new Position
            {
                Id = MakeId(100 + i),
                Title = def.Title,
                Description = def.Desc,
                ExpertiseLevel = def.Level,
                MaxProjects = def.MaxProj,
                CreatedAt = faker.Date.Past(1).ToUniversalTime(),
                Restricted = def.Restricted,
                Version = 0
            };
            positions.Add(position);

            foreach (var techName in def.Techs)
            {
                positionTechnologies.Add(new PositionTechnology
                {
                    PositionId = position.Id,
                    TechnologyId = techName
                });
            }
        }

        context.Positions.AddRange(positions);
        context.PositionTechnologies.AddRange(positionTechnologies);
        
        return positions;
    }

    private static List<AttributeDefinition> SeedAttributeDefinitions(CvManagementDbContext context)
    {
        var phoneDef = new AttributeDefinition
        {
            Id = MakeId(200),
            Name = "Phone",
            AttributeCategoryId = CategoryIds[0],
            DataType = AttributeDataType.String
        };

        var linkedinDef = new AttributeDefinition
        {
            Id = MakeId(201),
            Name = "LinkedIn URL",
            AttributeCategoryId = CategoryIds[0],
            DataType = AttributeDataType.String
        };

        var summaryDef = new AttributeDefinition
        {
            Id = MakeId(202),
            Name = "Summary",
            AttributeCategoryId = CategoryIds[1],
            DataType = AttributeDataType.Text
        };

        var expDef = new AttributeDefinition
        {
            Id = MakeId(203),
            Name = "Years of Experience",
            AttributeCategoryId = CategoryIds[1],
            DataType = AttributeDataType.Numeric
        };

        var birthDef = new AttributeDefinition
        {
            Id = MakeId(204),
            Name = "Date of Birth",
            AttributeCategoryId = CategoryIds[2],
            DataType = AttributeDataType.Date
        };

        var relocationDef = new AttributeDefinition
        {
            Id = MakeId(205),
            Name = "Open to Relocation",
            AttributeCategoryId = CategoryIds[2],
            DataType = AttributeDataType.Boolean
        };

        var educationDef = new AttributeDefinitionOfOneOfMany
        {
            Id = MakeId(206),
            Name = "Education Level",
            AttributeCategoryId = CategoryIds[3],
            DataType = AttributeDataType.OneOfMany,
            OneOfManyOptions =
            [
                new() { Id = MakeId(300), Value = "High School", OneOfManyId = MakeId(206) },
                new() { Id = MakeId(301), Value = "Bachelor", OneOfManyId = MakeId(206) },
                new() { Id = MakeId(302), Value = "Master", OneOfManyId = MakeId(206) },
                new() { Id = MakeId(303), Value = "PhD", OneOfManyId = MakeId(206) }
            ]
        };

        var workFormatDef = new AttributeDefinitionOfOneOfMany
        {
            Id = MakeId(207),
            Name = "Work Format",
            AttributeCategoryId = CategoryIds[3],
            DataType = AttributeDataType.OneOfMany,
            OneOfManyOptions =
            [
                new() { Id = MakeId(304), Value = "Remote", OneOfManyId = MakeId(207) },
                new() { Id = MakeId(305), Value = "Office", OneOfManyId = MakeId(207) },
                new() { Id = MakeId(306), Value = "Hybrid", OneOfManyId = MakeId(207) }
            ]
        };

        var definitions = new List<AttributeDefinition>
        {
            phoneDef, linkedinDef, summaryDef, expDef, birthDef, relocationDef,
            educationDef, workFormatDef
        };

        context.AttributeDefinitions.AddRange(definitions);
        
        return definitions;
    }

    private static List<User> SeedRegularUsers(CvManagementDbContext context)
    {
        var faker = new Faker();

        var users = new List<User>();
        for (var i = 0; i < 15; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var user = new User
            {
                Id = MakeId(1000 + i),
                Email = faker.Internet.Email(firstName, lastName),
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("password"),
                RoleId = RegularRoleId,
                IsBlocked = false,
                IsConfirmed = true,
                Version = 0,
                ProfileData = new ProfileData
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Location = faker.Address.City(),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                }
            };
            users.Add(user);
        }

        context.Users.AddRange(users);
        
        return users;
    }

    private static List<User> SeedRecruiterUsers(CvManagementDbContext context)
    {
        var faker = new Faker();

        var users = new List<User>();
        for (var i = 0; i < 5; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var user = new User
            {
                Id = MakeId(2000 + i),
                Email = faker.Internet.Email(firstName, lastName),
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("password"),
                RoleId = RecruiterRoleId,
                IsBlocked = false,
                IsConfirmed = true,
                Version = 0,
                ProfileData = new ProfileData
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Location = faker.Address.City(),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                }
            };
            users.Add(user);
        }

        context.Users.AddRange(users);
        
        return users;
    }

    private static List<Project> SeedProjects(CvManagementDbContext context, List<User> users, List<Technology> technologies)
    {
        var faker = new Faker();
        var projects = new List<Project>();
        var projectTechnologies = new List<ProjectTechnology>();

        var projectId = 0;
        foreach (var user in users)
        {
            var projectCount = faker.Random.Int(1, 3);
            for (var j = 0; j < projectCount; j++)
            {
                var project = new Project
                {
                    Id = MakeId(3000 + projectId),
                    Name = faker.Commerce.ProductName(),
                    Description = faker.Lorem.Sentence(),
                    UserId = user.Id,
                    Version = 0
                };
                projects.Add(project);

                var techs = faker.PickRandom(technologies, faker.Random.Int(1, 3));
                foreach (var tech in techs)
                {
                    projectTechnologies.Add(new ProjectTechnology
                    {
                        ProjectId = project.Id,
                        TechnologyId = tech.Name
                    });
                }

                projectId++;
            }
        }

        context.Projects.AddRange(projects);
        context.ProjectTechnologies.AddRange(projectTechnologies);
        
        return projects;
    }

    private static List<AttributeValue> SeedAttributeValues(CvManagementDbContext context, List<AttributeDefinition> definitions)
    {
        var faker = new Faker();
        var attributeValues = new List<AttributeValue>();

        var valueId = 0;

        foreach (var def in definitions)
        {
            switch (def.DataType)
            {
                case AttributeDataType.String:
                    for (var i = 0; i < 3; i++)
                    {
                        attributeValues.Add(new StringAttributeValue
                        {
                            Id = MakeId(4000 + valueId++),
                            AttributeDefinitionId = def.Id,
                            Value = faker.Phone.PhoneNumber()
                        });
                    }
                    break;

                case AttributeDataType.Text:
                    for (var i = 0; i < 2; i++)
                    {
                        attributeValues.Add(new MarkdownAttributeValue
                        {
                            Id = MakeId(4000 + valueId++),
                            AttributeDefinitionId = def.Id,
                            Value = faker.Lorem.Paragraph()
                        });
                    }
                    break;

                case AttributeDataType.Numeric:
                    for (var i = 0; i < 3; i++)
                    {
                        attributeValues.Add(new NumericAttributeValue
                        {
                            Id = MakeId(4000 + valueId++),
                            AttributeDefinitionId = def.Id,
                            Value = faker.Random.Float(1, 15)
                        });
                    }
                    break;

                case AttributeDataType.Date:
                    for (var i = 0; i < 3; i++)
                    {
                        attributeValues.Add(new DateAttributeValue
                        {
                            Id = MakeId(4000 + valueId++),
                            AttributeDefinitionId = def.Id,
                            Value = DateOnly.FromDateTime(faker.Date.Past(40, DateTime.UtcNow.AddYears(-18)).ToUniversalTime())
                        });
                    }
                    break;

                case AttributeDataType.Boolean:
                    attributeValues.Add(new BooleanAttributeValue
                    {
                        Id = MakeId(4000 + valueId++),
                        AttributeDefinitionId = def.Id,
                        Value = true
                    });
                    attributeValues.Add(new BooleanAttributeValue
                    {
                        Id = MakeId(4000 + valueId++),
                        AttributeDefinitionId = def.Id,
                        Value = false
                    });
                    break;

                case AttributeDataType.OneOfMany:
                    var oneOfManyDef = (AttributeDefinitionOfOneOfMany)def;
                    foreach (var option in oneOfManyDef.OneOfManyOptions)
                    {
                        attributeValues.Add(new OneOfManyAttributeValue
                        {
                            Id = MakeId(4000 + valueId++),
                            AttributeDefinitionId = def.Id,
                            OptionId = option.Id
                        });
                    }
                    break;
            }
        }

        context.AttributeValues.AddRange(attributeValues);
        
        return attributeValues;
    }

    private static void SeedUserAttributeValues(CvManagementDbContext context, List<User> users, List<AttributeValue> attributeValues)
    {
        var faker = new Faker();
        var userAttributeValues = new List<UserAttributeValue>();

        foreach (var user in users)
        {
            var selectedValues = faker.PickRandom(attributeValues, faker.Random.Int(2, 5));
            foreach (var attrValue in selectedValues)
            {
                userAttributeValues.Add(new UserAttributeValue
                {
                    UserId = user.Id,
                    AttributeValueId = attrValue.Id
                });
            }
        }

        context.UserAttributeValues.AddRange(userAttributeValues);
    }

    private static void SeedCvs(CvManagementDbContext context, List<User> users, List<Position> positions, List<Project> projects)
    {
        var faker = new Faker();
        var cvs = new List<Cv>();
        var cvProjects = new List<CvProject>();

        var selectedUsers = faker.PickRandom(users, 4).ToList();
        var shuffledPositions = faker.Random.Shuffle(positions).ToList();

        for (var i = 0; i < selectedUsers.Count; i++)
        {
            var user = selectedUsers[i];
            var position = shuffledPositions[i];
            var userProjects = projects.Where(p => p.UserId == user.Id).ToList();

            var cv = new Cv
            {
                Id = MakeId(5000 + i),
                UserId = user.Id,
                PositionId = position.Id,
                Likes = (uint)faker.Random.Int(0, 100),
                Published = true,
                Version = 0
            };
            cvs.Add(cv);

            foreach (var project in userProjects)
            {
                cvProjects.Add(new CvProject
                {
                    CvId = cv.Id,
                    ProjectId = project.Id
                });
            }
        }

        context.Cvs.AddRange(cvs);
        context.CvProjects.AddRange(cvProjects);
    }

    private static void SeedDiscussions(CvManagementDbContext context, List<Position> positions, List<User> users)
    {
        var faker = new Faker();
        var discussions = new List<Discussion>();
        var messages = new List<DiscussionMessage>();

        foreach (var position in positions)
        {
            var discussion = new Discussion
            {
                Id = MakeId(6000 + discussions.Count),
                PositionId = position.Id
            };
            discussions.Add(discussion);

            if (!faker.Random.Bool(0.7f))
            {
                continue;
            }

            var messageCount = faker.Random.Int(2, 3);
            var sentAt = DateTime.UtcNow.AddDays(-30);
            for (var j = 0; j < messageCount; j++)
            {
                sentAt = sentAt.AddDays(faker.Random.Int(1, 5));
                messages.Add(new DiscussionMessage
                {
                    Id = MakeId(7000 + messages.Count),
                    Text = faker.Lorem.Sentence(),
                    UserId = faker.PickRandom(users).Id,
                    DiscussionId = discussion.Id,
                    SentAt = sentAt
                });
            }
        }

        context.Discussions.AddRange(discussions);
        context.DiscussionMessages.AddRange(messages);
    }

    private static void SeedAccessRules(CvManagementDbContext context, List<Position> positions, List<AttributeValue> attributeValues)
    {
        var faker = new Faker();
        var accessRules = new List<AccessRule>();

        var restrictedPositions = positions.Where(p => p.Restricted).ToList();

        var ruleId = 0;
        foreach (var position in restrictedPositions)
        {
            var ruleCount = faker.Random.Int(2, 3);
            var selectedValues = faker.PickRandom(attributeValues, ruleCount);

            foreach (var attrValue in selectedValues)
            {
                accessRules.Add(new AccessRule
                {
                    Id = MakeId(8000 + ruleId++),
                    PositionId = position.Id,
                    AttributeValueId = attrValue.Id,
                    FilterOperator = faker.PickRandom<FilterOperator>()
                });
            }
        }

        context.AccessRules.AddRange(accessRules);
    }

    private static Guid MakeId(int unique) => Guid.Parse($"00000000-0000-0000-0000-{unique:D12}");
}
