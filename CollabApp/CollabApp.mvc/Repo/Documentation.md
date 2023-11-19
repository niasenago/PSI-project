# IGenericRepository<T> Interface

The `IGenericRepository<T>` interface defines a set of generic methods for performing common CRUD (Create, Read, Update, Delete) operations on entities. 
It is designed to work with entities that implement the `IBaseEntity` interface.
IBaseEntity interface insures, that entities have Id property.

# GenericRepository<T> Class

The `GenericRepository<T>` class is an implementation of the `IGenericRepository<T>` interface. It provides a generic implementation for some CRUD operations on entities of type `T`. The class includes a constructor that initializes the database context and DbSet for the given entity type.

## Methods

- **GetAllAsync():** `override Task<List<T>>`
  - Retrieves all entities of type `T` asynchronously. This method can be overridden to provide custom implementation.

- **GetAsync(int id):** `override Task<T>`
  - Retrieves an entity of type `T` based on its identifier asynchronously. This method can be overridden to provide custom implementation.

- **AddEntity(T entity):** `override Task<bool>`
  - Adds a new entity of type `T` asynchronously. This method **MUST** be overridden to provide custom implementation.

- **UpdateEntity(T entity):** `override Task<bool>`
  - Updates an existing entity of type `T` asynchronously. This method **MUST** be overridden to provide custom implementation.

- **DeleteEntity(T entity):** `override Task<bool>`
  - Deletes an existing entity of type `T` based on its identifier asynchronously. This method can be overridden to provide custom implementation.


- **DeleteEntitiesByExpression(Expression<Func<T, bool>> predicate):** `override Task<bool>`
  - Deletes entities of type `T` based on the specified expression asynchronously. This method can be overridden to provide custom implementation.

# IPostRepository Interface

The `IPostRepository` interface extends the `IGenericRepository<Post>` interface. We need it to work with in UnitOfWork class.

# PostRepository Class

The `PostRepository` class is an implementation of the `IPostRepository` interface and extends the `GenericRepository<Post>`. It provides implementations for AddEntity and UpdateEntity methods.

# IBoardRepository Interface

The `IBoardRepository` interface extends the `IGenericRepository<Board>` interface.We need it to work with in UnitOfWork class.

# BoardRepository Class

The `BoardRepository` class is an implementation of the `IBoardRepository` interface and extends the `GenericRepository<Board>`.It provides implementations for AddEntity and UpdateEntity methods.

# ICommentRepository Interface

The `ICommentRepository` interface extends the `IGenericRepository<Comment>` interface.We need it to work with in UnitOfWork class.

# CommentRepository Class

The `CommentRepository` class is an implementation of the `ICommentRepository` interface and extends the `GenericRepository<Comment>`. It provides implementations for AddEntity and UpdateEntity methods.

# IUnitOfWork Interface

The `IUnitOfWork` interface defines a set of repositories and a method for completing the unit of work.

- **postRepository:** `IPostRepository`
  - An instance of the `IPostRepository` interface.

- **boardRepository:** `IBoardRepository`
  - An instance of the `IBoardRepository` interface.

- **commentRepository:** `ICommentRepository`
  - An instance of the `ICommentRepository` interface.

- **CompleteAsync():** `Task`
  - Saves changes made in the unit of work to the database asynchronously.

# UnitOfWork Class

The `UnitOfWork` class is an implementation of the `IUnitOfWork` interface. It provides instances of the specific repositories and a method to complete the unit of work by saving changes to the database.

# Adding a New Model to UnitOfWork

## **Step 1: Create the `NewModel` Entity**

```csharp
// NewModel.cs
public class NewModel : IBaseEntity
{
    public int Id { get; set; }
    // Other properties specific to NewModel
}
```
## Step 2: Create the INewModelRepository Interface
```csharp
//INewModelRepository.cs
public interface INewModelRepository : IGenericRepository<NewModel>
{
    // Add any specific methods for NewModel repository if needed
}
```

## Step 3: Create the NewModelRepository Implementation
```csharp
// NewModelRepository.cs
public class NewModelRepository : GenericRepository<NewModel>, INewModelRepository
{
    public NewModelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    // Implement any additional methods specific to NewModel repository
}
```

## Step 4: Update IUnitOfWork Interface

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork
{
    IPostRepository postRepository { get; }
    IBoardRepository boardRepository { get; }
    ICommentRepository commentRepository { get; }
    INewModelRepository newModelRepository { get; } // Add the new repository
    Task CompleteAsync();
}
```

## Step 5: Update UnitOfWork class

```csharp
// UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    public IPostRepository postRepository { get; private set; }
    public IBoardRepository boardRepository { get; private set; }
    public ICommentRepository commentRepository { get; private set; }
    public INewModelRepository newModelRepository { get; private set; } // Add the new repository
    private readonly ApplicationDbContext dbContext;

    public UnitOfWork(IPostRepository postRepository, IBoardRepository boardRepository,ICommentRepository commentRepository,INewModelRepository newModelRepository ApplicationDbContext dbContext)
    {
        this.postRepository = postRepository;
        this.boardRepository = boardRepository;
        this.commentRepository = commentRepository;
        this.newModelRepository = newModelRepository;
        this.dbContext = dbContext;
    }
    // ... Existing code ...
}
```

## Step 6: Update Program.cs file
```csharp
//Program.cs
    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<IBoardRepository, BoardRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<INewModelRepository, NewModelRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
 ```
