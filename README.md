
# OmniCache
OmniCache is a .NET library that makes caching everything easy. It handles caching and invalidation for you seemlessly. This is all done through queries. The query is first executed against the cache, and if it doesn't exist, it will run it against the database. There are limitations however, of which we will describe below.


## Getting Started

### 1) Install nuget packages
``` powershell
> Install-Package OmniCache, OmniCache.EntityFramework
```
### 2) Initialise OmniCache
``` csharp
CachedDatabase.LoadAllQueries();
```

### 3) Add Cacheable attribute
``` csharp
[Cacheable]
public class Student
{
```
Add this to all classes that you want cached.
### 4) Create Query
``` csharp
public static Query<Student> getStudentsQuery = new Query<Student>(
			s => s.Status == "A" && s.DOB > new QueryParam(1));
```

### 5) Execute Query
``` csharp
List<Student> students = await cachedDb.GetMultipleAsync(getStudentsQuery, new DateTime(1990,1,1));
```
This query will run against the cache and if it doesn't exist will retrieve it from the database.

## Queries
Queries in OmniCache cannot use joins. They query a single table and all joins must be done in code. This is not actually a bad thing, it brings the load from the database into the server, making scaling better.

Here is an example of joining two tables:

``` csharp
public static Query<Movie> movieQuery = new Query<Movie>(
                               m => m.Category == new QueryParam(1));

public static Query<StoreStock> stockQuery = new Query<StoreStock>(
                               s => new QueryParam(1).Contains(s.MovieId));

public async Task<List<StoreStock>> GetMovieStock(Category category)
{
    List<Movie> movies = await cachedDB.GetMultipleAsync(movieQuery, category);
    List<StoreStock> stock = await cachedDB.GetMultipleAsync(stockQuery, movies.Select(m=>m.Id).ToList());
    return stock
}
```

If you want to combine the two sets you can just use linq:
``` csharp
var joinedData = from m in movies
                 join s in stock on m.Id equals s.MovieId
                 select new
                 {
                     Movie = m,
                     Stock = s
                 };
```

## Configuration

``` csharp
CachedDatabase.SetConfig<OmniCacheConfig>(new OmniCacheConfig
            {
                CacheProvider = CacheProviderType.LocalMemory
            });
```
Here we set OmniCache configuration to use inMemory caching. To set it to use redis:
``` csharp
CachedDatabase.SetConfig<OmniCacheConfig>(new OmniCacheConfig
            {
                CacheProvider = CacheProviderType.Redis
            });
```

And then set up redis config:
``` csharp
CachedDatabase.SetConfig<ConfigurationOptions>(new ConfigurationOptions()
                {
                    AbortOnConnectFail = false,
                    ResolveDns = true,
                    EndPoints = { "localhost" }
                });
```
This is just the standard Redis config but passed into CachedDatabase.

## FAQ
### When should I use this library?
I would suggest using the library for APIs/websites/services that you want to scale. Anything where the database is the bottleneck and you want to reduce load.

### When shouldn't I use this library?
- Your application has high writes relative to reads

- Your application has strict consistency requirements and cannot tolerate any inconsistency between the cache and the database. There's always a small window between when data is updated and when the cache is invalidated and updated where stale data may be served.

### Is this production ready?
If you're willing to debug any issues then go for it! I'm hoping to iron out all the bugs while developing future projects.

### I'm not sure I understand how this works
Take a look at the demos inside. They will demonstrate how queries are made against the cache and database.

### What else is there to do?
Currently working on making less trips to the cache. In future will look at making it work with transactions.


## Contact
Any other questions or feedback feel free to contact me at chunlimdev@gmail.com.
