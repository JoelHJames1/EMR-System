**
Description**
The IUserService interface is a part of the EMRWebAPI.Services.IServices namespace. This interface outlines the required operations for managing user data and user authentication in the Electronic Medical Record (EMR) Web API. It contains methods for creating, reading, updating, and deleting users, as well as for user registration and login.

**Methods**
Task<IActionResult> GetUsers()
Retrieves all users.

Task<IActionResult> Register(UserDto model)
Registers a new user. Accepts a User Data Transfer Object (UserDto) model which contains the necessary information for registration.

Task<IActionResult> Login(LoginDto model)
Authenticates a user. Accepts a Login Data Transfer Object (LoginDto) model which contains the necessary information for user login.

Task<IActionResult> GetUser(int id)
Retrieves a user with the specified id.

Task<IActionResult> UpdateUser(int id, UserDto model)
Updates the user with the specified id. Accepts a User Data Transfer Object (UserDto) model which contains the updated user information.

Task<IActionResult> DeleteUser(int id)
Deletes the user with the specified id.


Description
The Repository<T> class is a part of the EMRDataLayer.Repository namespace. This class is a generic repository implementation designed to interact with the Electronic Medical Record (EMR) database context using Entity Framework Core. It provides basic CRUD (Create, Read, Update, Delete) operations for any entity class T.

**Properties**
protected readonly EMRDBContext _context
The database context used by the repository. This property is set via the constructor.

private readonly DbSet<T> _dbSet
A DbSet representing the collection of all entities in the context, or that can be queried from the database, of type T.

Constructor
Repository(EMRDBContext context)
The constructor accepts a EMRDBContext instance, initializing the _context property and setting _dbSet to the set of entities of type T in the context.

Methods
Task<T> GetByIdAsync(int id)
Asynchronously retrieves an entity of type T with the specified id. Returns null if no such entity is found.

Task<IEnumerable<T>> GetAllAsync()
Asynchronously retrieves all entities of type T from the context. Returns an IEnumerable<T> containing the entities.

Task AddAsync(T entity)
Asynchronously adds a new entity of type T to the context and saves the changes to the database.

void Update(T entity)
Updates an existing entity of type T in the context and saves the changes to the database.

void Delete(T entity)
Removes an entity of type T from the context and saves the changes to the database.
