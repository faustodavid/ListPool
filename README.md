# ListPool

Allocation-free implementation of IList using ArrayPool with two variants, ListPool and ValueListPool

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/faustodavid/ListPool/Build)](https://github.com/faustodavid/ListPool/actions)
[![Coveralls github](https://img.shields.io/coveralls/github/faustodavid/ListPool)](https://coveralls.io/github/faustodavid/ListPool)
[![Nuget](https://img.shields.io/nuget/v/ListPool)](https://www.nuget.org/packages/ListPool/)
[![GitHub](https://img.shields.io/github/license/faustodavid/ListPool)](https://github.com/faustodavid/ListPool/blob/master/LICENSE)

## Introduction

When performance matter, ListPool provide all the goodness of ArrayPool with the usability of IList and support for Span.

It has two variants ListPool and ValueListPool.

Differences:

* ListPool:
  * ReferenceType
  * Serializable
  * Because it is a class it has a constant heap allocation of 64kb regardiness the size

* ValueListPool
  * ValueType
  * High-performance
  * Allocation-free
  * Cannot be deserialize
  * Cannot be created with parametless constructor, otherwise will be in invalid state. (Wich will be allow by the compiler)
  * Because it is ValueType when it is pass to other methods is pass by copy not by reference. This is good for performance but any modification wont be reflected in the original instance. In case is required to be updated need to use the "ref" keyword in the parameter.

 ## How to use

 ListPool and ValueListPool implement IDisposable, after finshing their use you must dispose the list to return the buffer to the pool.

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

  ```csharp
static void Main()
{
    using ValueListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ValueListPool<ExampleResult> examplesResult = new ValueListPool<ExampleResult>(examples.Count);
    foreach (var example in examples)
    {
        examplesResult.Add(new ExampleResult(example));
    }
    ...
}
  ```

Mapping domain object to dto using LINQ (It perfrom slower than with foreach):

  ```csharp
static void Main()
{
    using ValueListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ValueListPool<ExampleResult> examplesResult = examples.Select(example => new ExampleResult(example)).ToValueListPool();
    ...
}
  ```

## Contributors

A big thanks to the project contributors!

* [Sergey Mokin](https://github.com/SergeyMokin)
