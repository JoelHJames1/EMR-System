
****LoginForm Component****

Module: LoginForm
Description
The LoginForm component is a part of the React frontend of the application. It provides an interface for a user to log in. It handles the login process by interacting with a backend API endpoint, and upon a successful login, the user is navigated to a dashboard.

States
const [email, setEmail] = useState("")
This state stores the email input from the user.

const [password, setPassword] = useState("")
This state stores the password input from the user.

const [showToast, setShowToast] = useState(false)
This state handles the visibility of a Bootstrap Toast component, which is used to show error messages when login fails.

Functions
const handleSubmit = async (event)
This function handles the submission of the login form. It interacts with the API endpoint https://localhost:7099/api/User/login via a POST request. If the request is successful, it stores the received token and user data in the local storage and navigates to the /dashboard. If the request results in an HTTP 401 status code, it shows a Bootstrap Toast error message. For other error conditions, it logs the error to the console.

**JSX**
The LoginForm component renders a form for the user to input their email and password, as well as a submit button to trigger the login process. It also renders a Bootstrap Toast component to show error messages when login fails.

**Description**
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
