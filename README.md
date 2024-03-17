# ServantSoftware.SettingsOnEF

An EF Core extension for centralizing application settings in the database. Each setting class corresponds to a table, with each table having a single-row configuration.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen) ![Version](https://img.shields.io/badge/version-1.0.0-blue) ![License](https://img.shields.io/badge/license-MIT-green)
![Nuget](https://img.shields.io/nuget/v/ServantSoftware.EFCore.Json)

A specialized [JSON settings manager](README.Json.md) uses the [JSON EF Core Provider](https://github.com/Servant-Software-LLC/FileBased.DataProviders/blob/main/README.EFCore.Json.md)

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Easy Setup**: Integrate with your existing EF Core projects seamlessly.
- **Centralized Settings**: Store and manage your app settings in a unified manner.
- **Single-row Configurations**: Each setting class corresponds to a table with a unique configuration row.

## Getting Started

### Prerequisites

- .NET 7 or higher
- An existing EF Core provider to use for storage

### Installation

1. Install the `ServantSoftware.SettingsOnEF` package via NuGet:

```
dotnet add package ServantSoftware.SettingsOnEF
```

| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ServantSoftware.SettingsOnEF.Common`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.SettingsOnEF.Common.svg)](https://www.nuget.org/packages/ServantSoftware.SettingsOnEF.Common/)
| `ServantSoftware.SettingsOnEF`       | [![NuGet](https://img.shields.io/nuget/v/ServantSoftware.SettingsOnEF.svg)](https://www.nuget.org/packages/ServantSoftware.SettingsOnEF/)


### Usage
1. Define your settings class and mark it with the SettingsEntity attribute. Here is an example:
```csharp
namespace SettingsOnEF.Tests.Test_Classes;

[SettingsEntity]
public class SomeSetting
{
    public string SomeProperty { get; set; }
}
```
2. Use the Get and Update methods from SettingsManager to retrieve and update settings:
```csharp
var settingsManager = new SettingsManager(contextBuilder => contextBuilder.UseSqlite($"Data Source=InMemorySample;Mode=Memory;Cache=Shared"));

var setting = settingsManager.Get<SomeSetting>();
setting.SomeProperty = "NewValue";
settingsManager.Update(setting);
```

For more detailed documentation, check our [Wiki](#).

## Contributing

We welcome contributions to SettingsOnEF! Here's how you can help:

1. **Fork** the repository on GitHub.
2. **Clone** your fork locally.
3. **Commit** your changes on a dedicated branch.
4. **Push** your branch to your fork.
5. Submit a **pull request** from your fork to the main repository.
6. Engage in the review process and address feedback.

Please read our [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process and coding standards.

## Thread Safety Policy

SettingsOnEF leverages Entity Framework Core (EF Core) for managing application settings storage. It's important to note that `DbContext`, a fundamental part of EF Core, is not thread-safe. This means that instances of `DbContext`, and by extension, instances of `SettingsManager` and `JsonSettingsManager` provided by SettingsOnEF, should not be accessed concurrently from multiple threads. 

### Responsibilities of the Consumer

To ensure the integrity and reliability of your application, it is the consumer's responsibility to manage the thread-safe usage of SettingsOnEF. Here are some guidelines to help you avoid common pitfalls related to threading:

- **Scoped Instances**: In web applications, particularly those built with ASP.NET Core, utilize dependency injection to create scoped instances of `SettingsManager` or `JsonSettingsManager`. This ensures that each HTTP request receives its instance, effectively isolating individual requests from one another.

- **Synchronization**: For applications where SettingsOnEF might be accessed concurrently in a multi-threaded scenario, implement appropriate synchronization mechanisms. For example, you can use locks, `SemaphoreSlim`, or other synchronization primitives to control access to SettingsOnEF instances. This is crucial for batch operations, background services, or any application part that runs in parallel threads.

- **Immutable Settings Patterns**: Consider adopting a pattern where settings are loaded once at the beginning of an operation and treated as read-only for the duration of that operation. This approach can reduce the need for synchronization but requires a design that accommodates potentially stale settings if they are updated while the application is running.

### Not Recommended Practices

- **Singleton Pattern**: Avoid using a single, application-wide instance of `SettingsManager` or `JsonSettingsManager` without proper synchronization, as this can lead to data corruption and concurrency exceptions.

- **Static Access**: Similarly, static methods or properties that internally use a shared `DbContext` instance should be avoided unless they are designed to handle concurrent access securely.

### Conclusion

By adhering to these guidelines and designing your application with thread safety in mind, you can effectively mitigate concurrency-related issues while using SettingsOnEF. Remember, managing thread safety is a shared responsibility between the library and its consumers. If your application scenario requires concurrent access to SettingsOnEF, planning and implementing a thread-safe strategy is essential for maintaining data integrity and application stability.

### Issues

Feel free to submit issues and enhancement requests.

## License

SettingsOnEF is licensed under the MIT License. See [LICENSE](LICENSE) for more information.

