# ListPool<T>

Allocation-free implementation of `IList<T>` using ArrayPool with two variants, `ListPool<T>` and `ValueListPool<T>`

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/faustodavid/ListPool/Build)](https://github.com/faustodavid/ListPool/actions)
[![Coveralls github](https://img.shields.io/coveralls/github/faustodavid/ListPool)](https://coveralls.io/github/faustodavid/ListPool)
[![Nuget](https://img.shields.io/nuget/v/ListPool)](https://www.nuget.org/packages/ListPool/)
[![Nuget](https://img.shields.io/nuget/dt/listpool)](https://www.nuget.org/packages/ListPool/)
[![GitHub](https://img.shields.io/github/license/faustodavid/ListPool)](https://github.com/faustodavid/ListPool/blob/master/LICENSE)


## Installation

Available on [nuget](https://www.nuget.org/packages/ListPool/)

	PM> Install-Package ListPool

Requirements:
* System.Memory (>= 4.5.3)


## Introduction

When performance matter, **ListPool** provides all the goodness of ArrayPool with the usability of `IList<T>`, support for `Span<T>` and **serialization**.
    
It has two high-performance variants `ListPool<T>` and `ValueListPool<T>`.
    
We recommend to use `ListPool<T>` over `ValueListPool<T>` for most of use-cases. You should use `ValueListPool<T>` when working with small collections of primitive types with stackalloc, or when reusing arrays.    

Differences:

* `ListPool<T>`:
  * ReferenceType
  * Serializable
  * Because it is a class it has a constant heap allocation of ~56 bytes regardless the size

* `ValueListPool<T>`:
  * stack only
  * Allocation-free
  * Can be created using stackalloc or an array as initial buffer
  * **Cannot be serialized/deserialized**
  * **Cannot be created with parameterless constructors**, otherwise it is created in an invalid state
  * Because it is ValueType when it is passed to other methods, it is passed by copy, not by reference. In case it is required to be updated, it is required to use the "ref" modifier in the parameter.
    
    
 ## Benchmarks
To see all the benchmarks and details, please click the following link <a>https://github.com/faustodavid/ListPool/tree/master/perf/docs/results<a/>
    
### Inserting an item in the middle of the list

You can observe that `ListPool<T>` Mean is faster and it does not allocate in the heap when resizing. Zero heap allocation is vital to improve throughput by reducing "GC Wait" time.

<img src="https://github.com/faustodavid/ListPool/raw/master/perf/docs/results/graph/ListPoolInsertBenchmarks.JPG" />

### Create list indicating the capacity, adding N items and performing a foreach

By indicating the capacity, we avoid regrowing, which is one of the slowest operations for `List<T>`, so we can pay more attention to already well-optimized scenarios by improving the Add and Enumeration time. As you can observe, `ListPool<T>` Mean is faster and has 40 bytes of heap allocations, which are used to create the class.

<img src="https://raw.githubusercontent.com/faustodavid/ListPool/master/perf/docs/results/graph/CreateAndAddAndEnumerateAReferenceBenchmarks.JPG" />

### Doing a foreach in a list of N size.
ListPool enumeration is way faster than List for small and large sizes.

<img src="https://github.com/faustodavid/ListPool/raw/master/perf/docs/results/graph/ListPoolEnumerateBenchmarks.JPG" />


 ## How to use

 `ListPool<T>` and `ValueListPool<T>` implement IDisposable. After finishing their use, you must **dispose** the list.

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

 *Note: `ListPool<T>` is not been dispose at `MapToResult`. It is dispose at the caller.*

  ```csharp
static void Main()
{
    using ListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ListPool<ExampleResult> exampleResults = MapToResult(examples); 
    ...
}

public static ListPool<ExampleResult> MapToResult(IReadOnlyCollection<Example> examples)
{
    ListPool<ExampleResult> examplesResult = new ListPool<ExampleResult>(examples.Count);
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
    using ListPool<Example> examples = new GetAllExamplesUseCase().Query();
    using ListPool<ExampleResult> examplesResult = examples.Select(example => new ExampleResult(example)).ToListPool();
    ...
}
  ```

Updating ValueListPool<T> in other methods:

*Note: The use of `ref` is required for `ValueListPool<T>` when it is updated in other methods because it is a ValueType. `ListPool<T>` does not require it.*

  ```csharp
static void Main()
{
    Span<int> initialBuffer = stackalloc int[500];
    ValueListPool<int> numbers = new ValueListPool<int>(initialBuffer, ValueListPool<int>.SourceType.UseAsInitialBuffer)
    for(int i; i < 500; i++)
    {
        numbers.Add(i);
    }

    AddNumbers(ref numbers);
    ...
    numbers.Dispose();
}

static void AddNumbers(ref ValueListPool<int> numbers)
{
    numbers.Add(1);
    numbers.Add(2);
}
  ```



## Contributors

A big thanks to the project contributors!

* [Sergey Mokin](https://github.com/SergeyMokin)
