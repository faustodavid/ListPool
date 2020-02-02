# `ListPool<T>`

Allocation-free implementation of IList using ArrayPool with two variants, `ListPool<T>` and `ValueListPool<T>`

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/faustodavid/ListPool/Build)](https://github.com/faustodavid/ListPool/actions)
[![Coveralls github](https://img.shields.io/coveralls/github/faustodavid/ListPool)](https://coveralls.io/github/faustodavid/ListPool)
[![Nuget](https://img.shields.io/nuget/v/ListPool)](https://www.nuget.org/packages/ListPool/)
[![GitHub](https://img.shields.io/github/license/faustodavid/ListPool)](https://github.com/faustodavid/ListPool/blob/master/LICENSE)


## Installation

Available on [nuget](https://www.nuget.org/packages/ListPool/)

	PM> Install-Package ListPool

Requirements:
* dotnet core 3.0 or above

## Introduction

When performance matter, **ListPool<T>** provides all the goodness of ArrayPool with the usability of `IList` and support for `Span<T>`.

It has two variants `ListPool<T>` and `ValueListPool<T>`.

Differences:

* ListPool<T>:
  * ReferenceType
  * Serializable
  * Because it is a class it has a constant heap allocation of ~56 bytes regardless the size

* ValueListPool<T>
  * ValueType
  * High-performance
  * Allocation-free
  * Cannot be deserialized
  * Cannot be created with parameterless constructors, otherwise it is created in an invalid state
  * Because it is ValueType when it is passed to other methods, it is passed by copy, not by reference. It is good for performance, but any modifications don't affect the original instance. In case it is required to be updated, it is required to use the "ref" keyword in the parameter.

 ## How to use

 `ListPool<T>` and `ValueListPool<T>` implement IDisposable. After finishing their use, you must dispose the list.

 Examples

 Deserialization:

 ```csharp
static async Task Main()
{
    var httpClient = HttpClientFactory.Create();
    var stream = await httpClient.GetStreamAsync("examplePath");
    using var examples = await JsonSerializer.DeserializeAsync<ListPool<string>>(stream); 
    ...
}
 ```

 Mapping domain object to dto:

 *Note: `ValueListPool<T>` is not been dispose at `MapToResult`. It is dispose at the caller.*

  ```csharp
static void Main()
{
    using ValueListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ValueListPool<ExampleResult> exampleResults = MapToResult(examples); 
    ...
}

public static ValueListPool<ExampleResult> MapToResult(IReadOnlyCollection<Example> examples)
{
    ValueListPool<ExampleResult> examplesResult = new ValueListPool<ExampleResult>(examples.Count);
    foreach (var example in examples)
    {
        examplesResult.Add(new ExampleResult(example));
    }

    return examplesResult;
}
  ```

Mapping a domain object to dto using LINQ (It perform slower than with foreach):

  ```csharp
static void Main()
{
    using ValueListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ValueListPool<ExampleResult> examplesResult = examples.Select(example => new ExampleResult(example)).ToValueListPool();
    ...
}
  ```

Updating ValueListPool<T> in other methods:

*Note: The use of `ref` is required for `ValueListPool<T>` when it is updated in other methods because it is a ValueType. `ListPool<T>` does not require it.*

  ```csharp
static void Main()
{
    ValueListPool<int> numbers = Enumerable.Range(0, 1000).ToValueListPool();
    AddNumbers(ref numbers);
    ...
    numbers.Dispose();
}

static void AddNumbers(ref ValueListPool<int> numbers)
{
    numbers.AddRange(Enumerable.Range(0, 1000));
}
  ```



## Contributors

A big thanks to the project contributors!

* [Sergey Mokin](https://github.com/SergeyMokin)
