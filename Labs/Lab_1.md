# Getting Started

[Asp.Net Core Docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)

1. From the File menu, select New > Project.
2. Select the ASP.NET Core Web Application template and click Next.
3. Name the project TodoApi and click Create.
4. In the Create a new ASP.NET Core Web Application dialog, confirm that .NET Core and ASP.NET Core 3.1 are selected. Select the API template and click Create.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/CreateANewASPDotNetCoreWebApp.png?raw=true 'Request Pipeline')

## Test the Api

he project template creates a WeatherForecast API. Call the Get method from a browser to test the app.

Press Ctrl+F5 to run the app. Visual Studio launches a browser and navigates to https://localhost:<port>/WeatherForecast, where <port> is a randomly chosen port number.

If you get a dialog box that asks if you should trust the IIS Express certificate, select Yes. In the Security Warning dialog that appears next, select Yes.

```json
[
  {
    "date": "2019-07-16T19:04:05.7257911-06:00",
    "temperatureC": 52,
    "temperatureF": 125,
    "summary": "Mild"
  },
  {
    "date": "2019-07-17T19:04:05.7258461-06:00",
    "temperatureC": 36,
    "temperatureF": 96,
    "summary": "Warm"
  },
  {
    "date": "2019-07-18T19:04:05.7258467-06:00",
    "temperatureC": 39,
    "temperatureF": 102,
    "summary": "Cool"
  },
  {
    "date": "2019-07-19T19:04:05.7258471-06:00",
    "temperatureC": 10,
    "temperatureF": 49,
    "summary": "Bracing"
  },
  {
    "date": "2019-07-20T19:04:05.7258474-06:00",
    "temperatureC": -1,
    "temperatureF": 31,
    "summary": "Chilly"
  }
]
```

## Add a model class

1. In Solution Explorer, right-click the project. Select Add > New Folder. Name the folder Models.

2. Right-click the Models folder and select Add > Class. Name the class TodoItem and select Add.

3. Replace the template code with the following code:

```cs
    public class TodoItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}
```

The Id property functions as the unique key in a relational database.

Model classes can go anywhere in the project, but the Models folder is used by convention.

## Add a database context

The database context is the main class that coordinates Entity Framework functionality for a data model. This class is created by deriving from the Microsoft.EntityFrameworkCore.DbContext class.

### Add Microsoft.EntityFrameworkCore.SqlServer

1. From the Tools menu, select NuGet Package Manager > Manage NuGet Packages for Solution.
2. Select the Browse tab, and then enter Microsoft.EntityFrameworkCore.SqlServer in the search box.
3. Select Microsoft.EntityFrameworkCore.SqlServer in the left pane.
4. Select the Project check box in the right pane and then select Install.
5. Use the preceding instructions to add the Microsoft.EntityFrameworkCore.InMemory NuGet package.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/InstallEntityFrameWork.png?raw=true 'Request Pipeline')

## Add the TodoContext database context

1. Right-click the Models folder and select Add > Class. Name the class TodoContext and click Add.

2) Enter the following code:

```c
 using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
```

## Register the database context

In ASP.NET Core, services such as the DB context must be registered with the dependency injection (DI) container. The container provides the service to controllers.

Update Startup.cs with the following highlighted code:

```c
 // Unused usings removed
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt =>
               opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

The preceding code:

- Removes unused using declarations.
- Adds the database context to the DI container.
- Specifies that the database context will use an in-memory database.

## Scaffold a controller

1. Right-click the Controllers folder.

2. Select Add > New Scaffolded Item.

3. Select API Controller with actions, using Entity Framework, and then select Add.

4. In the Add API Controller with actions, using Entity Framework dialog:
   - Select TodoItem (TodoApi.Models) in the Model class.
   - Select TodoContext (TodoApi.Models) in the Data context class.
   - Select Add.

The generated code:

- Marks the class with the `[ApiController]` attribute. This attribute indicates that the controller responds to web API requests. For information about specific behaviors that the attribute enables, see Create web APIs with ASP.NET Core.
- Uses DI to inject the database context (TodoContext) into the controller. The database context is used in each of the CRUD methods in the controller.

The ASP.NET Core templates for:

- Controllers with views include `[action]` in the route template.
- API controllers don't include `[action]` in the route template.

When the `[action]` token isn't in the route template, the action name is excluded from the route. That is, the action's associated method name isn't used in the matching route.

## Examine the PostTodoItem create method

Replace the return statement in the PostTodoItem to use the nameof operator:

```c#
   // POST: api/TodoItems
[HttpPost]
public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
{
    _context.TodoItems.Add(todoItem);
    await _context.SaveChangesAsync();

    //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
    return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
}
```

The preceding code is an HTTP POST method, as indicated by the [HttpPost] attribute. The method gets the value of the to-do item from the body of the HTTP request.

The CreatedAtAction method:

- Returns an HTTP 201 status code if successful. HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
- Adds a Location header to the response. The Location header specifies the URI of the newly created to-do item. For more information, see 10.2.2 201 Created.
- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.

## Postman

### Test PostTodoItem with Postman

1. Create a new request.

2. Set the HTTP method to POST.

3. Select the Body tab.

4. Select the raw radio button.

5. Set the type to JSON (application/json).

6. In the request body enter JSON for a to-do item:

```Json
      {
  "name":"walk dog",
  "isComplete":true
}
```

7. Select Send

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postman_Post_Exp_1.png?raw=true 'Request Pipeline')

### Test the location header URI

1. Select the Headers tab in the Response pane.

2. Copy the Location header value:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postman_Location_header.png?raw=true 'Request Pipeline')

3. Set the method to GET.

4. Paste the URI (for example, https://localhost:5001/api/TodoItems/1).

5. Select Send.

## Examine the Get methods

These methods implement two Get endpoints:

- GET /api/TodoItems
- GET /api/TodoItems/{id}

Test the app by calling the two endpoints from a browser or Postman. For example:

- https://localhost:5001/api/TodoItems
- https://localhost:5001/api/TodoItems/1

A response similar to the following is produced by the call to GetTodoItems:

```Json
        [
  {
    "id": 1,
    "name": "Item1",
    "isComplete": false
  }
]
```

### Test Get with Postman

1. Create a new request.
2. Set the HTTP method to GET.
3. Set the request URL to https://localhost:<port>/api/TodoItems. For example, https://localhost:5001/api/TodoItems.
4. Set Two pane view in Postman.
5. Select Send.

This app uses an in-memory database. If the app is stopped and started, the preceding GET request will not return any data. If no data is returned, POST data to the app.

## Routing and URL paths

The [HttpGet] attribute denotes a method that responds to an HTTP GET request. The URL path for each method is constructed as follows:

- Start with the template string in the controller's Route attribute:

```C#
      [Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
        _context = context;
    }
```

- Replace [controller] with the name of the controller, which by convention is the controller class name minus the "Controller" suffix. For this sample, the controller class name is TodoItemsController, so the controller name is "TodoItems". ASP.NET Core routing is case insensitive.

- If the [HttpGet] attribute has a route template (for example, [HttpGet("products")]), append that to the path. This sample doesn't use a template. For more information, see Attribute routing with Http[Verb] attributes.

In the following GetTodoItem method, "{id}" is a placeholder variable for the unique identifier of the to-do item. When GetTodoItem is invoked, the value of "{id}" in the URL is provided to the method in its id parameter.

```c#
        // GET: api/TodoItems/5
[HttpGet("{id}")]
public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
{
    var todoItem = await _context.TodoItems.FindAsync(id);

    if (todoItem == null)
    {
        return NotFound();
    }

    return todoItem;
}
```

## Return values

The return type of the GetTodoItems and GetTodoItem methods is ActionResult<T> type. ASP.NET Core automatically serializes the object to JSON and writes the JSON into the body of the response message. The response code for this return type is 200, assuming there are no unhandled exceptions. Unhandled exceptions are translated into 5xx errors.

ActionResult return types can represent a wide range of HTTP status codes. For example, GetTodoItem can return two different status values:

- If no item matches the requested ID, the method returns a 404 NotFound error code.
- Otherwise, the method returns 200 with a JSON response body. Returning item results in an HTTP 200 response.

## The PutTodoItem method

Examine the PutTodoItem method:

```c#
        // PUT: api/TodoItems/5
[HttpPut("{id}")]
public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
{
    if (id != todoItem.Id)
    {
        return BadRequest();
    }

    _context.Entry(todoItem).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!TodoItemExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}
```

PutTodoItem is similar to PostTodoItem, except it uses HTTP PUT. The response is 204 (No Content). According to the HTTP specification, a PUT request requires the client to send the entire updated entity, not just the changes. To support partial updates, use HTTP PATCH.

If you get an error calling PutTodoItem, call GET to ensure there's an item in the database.

## Test the PutTodoItem method

This sample uses an in-memory database that must be initialized each time the app is started. There must be an item in the database before you make a PUT call. Call GET to insure there's an item in the database before making a PUT call.

Update the to-do item that has ID = 1 and set its name to "feed fish":

```Json
       {
    "ID":1,
    "name":"feed fish",
    "isComplete":true
  }
```

The following image shows the Postman update:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postmand_Put.png?raw=true 'Request Pipeline')

## The DeleteTodoItem method

Examine the DeleteTodoItem method:

```c#
        // DELETE: api/TodoItems/5
[HttpDelete("{id}")]
public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
{
    var todoItem = await _context.TodoItems.FindAsync(id);
    if (todoItem == null)
    {
        return NotFound();
    }

    _context.TodoItems.Remove(todoItem);
    await _context.SaveChangesAsync();

    return todoItem;
}
```

### Test the DeleteTodoItem method

Use Postman to delete a to-do item:

- Set the method to DELETE.
- Set the URI of the object to delete (for example https://localhost:5001/api/TodoItems/1).
- Select **Send**.

## Prevent over-posting

Currently the sample app exposes the entire TodoItem object. Production apps typically limit the data that's input and returned using a subset of the model. There are multiple reasons behind this and security is a major one. The subset of a model is usually referred to as a Data Transfer Object (DTO), input model, or view model. **DTO** is used in this article.

A DTO may be used to:

- Prevent over-posting.
- Hide properties that clients are not supposed to view.
- Omit some properties in order to reduce payload size.
- Flatten object graphs that contain nested objects. Flattened object graphs can be more convenient for clients.

To demonstrate the DTO approach, update the TodoItem class to include a secret field:

```C#
public class TodoItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
    public string Secret { get; set; }
}

```

The secret field needs to be hidden from this app, but an administrative app could choose to expose it.

Verify you can post and get the secret field.

Create a DTO model:

```C#
        public class TodoItemDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}

```

Update the TodoItemsController to use TodoItemDTO:

```C#
        [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        return await _context.TodoItems
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return ItemToDTO(todoItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
    {
        if (id != todoItemDTO.Id)
        {
            return BadRequest();
        }

        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Name = todoItemDTO.Name;
        todoItem.IsComplete = todoItemDTO.IsComplete;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
    {
        var todoItem = new TodoItem
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id) =>
         _context.TodoItems.Any(e => e.Id == id);

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
}

```

Verify you can't post or get the secret field.

## Dependency Injection

### **Refactor for Repository Pattern**

1. Add Connection String to the `appsetting.json` file.

The connection string specifies SQL Server LocalDB.

```Json
        {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "TodosContext": "Server=.;Database=Todos;IntegratedSecurity=True"
  }
}
```

2. Now lets make some changes to the start file to use SQL instead of an Inmemory database.

## Startup.cs

ASP.NET Core is built with `dependency injection`. Services (such as the EF Core database context) are registered with dependency injection during application startup. Components that require these services (such as Razor Pages) are provided these services via constructor parameters. The constructor code that gets a database context instance is shown later in the tutorial.

The scaffolding tool automatically registered the context class with the dependency injection container.

- In ConfigureServices, the highlighted lines were added by the scaffolder:

```C#
        public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();

    services.AddDbContext<SchoolContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("TodosContext")));
}

```

The name of the connection string is passed in to the context by calling a method on a DbContextOptions object. For local development, the ASP.NET Core configuration system reads the connection string from the appsettings.json file.

## Create the database

3. Update Program.cs to create the database if it doesn't exist:
   - We need to add the below code in the Program class

```C#
      using ContosoUniversity.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<TodoContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

```

## Test in Postman

1. In Postman create a new request and select Post past in this URI `http://localhost:4300/api/TodoItems`.

2. Paste in the below code into the Body tab make sure that raw with a type of JSON is selected (see image below for reference).

```Json
   {

    "name":"feed fish",
    "isComplete":true
}
```

- You should see the below results with a Status code of 201 Created

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postman_Post_Exp_2.png?raw=true 'Request Pipeline')

- Now lets see if we can retrive the post from the Sql database.

  1. Create a new Request in Postman select Get and paste in this URI `http://localhost:4300/api/TodoItems`
  2. Select Send.

  ![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postman_Get_Exp_2.png?raw=true 'Request Pipeline')

You should see the above 200 Ok status code with our newly created todo in Sql.

## Time to clean up Our Controllers

We want to now move or persistence implementation out of our controllers and into a repository pattern.

Create a new folder called Repository

Right click on Repository folder you just created and add a new class called BaseEntity and paste in below code.

Abstracting away Id we can have different Id implementantions like in Mongodb this gives greater flexability for future changes and decouples us from a particular Database.

```C#

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public class BaseEntity
    {

            public long Id { get; set; }


    }
}

```

We will generics to also supprt greatly flexablity and assist in Unit testing.

Right click on the IRepository folder and add a new interface called IRepository.cs

Paste in the code below:

```C#
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {

      Task DeleteAsync(T entity);

      Task<T> GetAsync(long id);

      Task<List<T>> GetAsync();

      Task<T> InsertAsync(T entity);

      Task UpdateAsync(T entity);

    }
}
```

Now we will create a TodoRespository Abstracting away our persistance implementation from our controller

Right click on the Reposistory folders and create a new class called TodoRepository.cs

Paste in the below Code

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Repository
{
    public class TodoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly TodoContext _context;
        protected DbSet<T> _entities;

        public TodoRepository(TodoContext context)
        {
            this._context = context;
            this._entities = context.Set<T>();
        }


        public async Task DeleteAsync(T entity)
        {
             _entities.Remove(entity);
           await _context.SaveChangesAsync();
        }

        public async Task<T> GetAsync(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<List<T>> GetAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> InsertAsync(T entity)
        {
          var newEntity = await  _entities.AddAsync(entity);
          await _context.SaveChangesAsync();

            return newEntity.Entity;
        }

        public async Task UpdateAsync(T entity)
        {

            if (!TodoItemExists(entity.Id))
                throw new ArgumentException($"Couldn't find matching {nameof(T)} with Id={entity.Id}");

            _context.Entry(entity).State = EntityState.Modified;


            await _context.SaveChangesAsync();
        }


        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }

}
```

Now we must use .Net Core Dependency Injection.

We will neee to update our Startup.cs with the below code

Add the using statements:

```C#
using TodoApi.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
```

now in the ConfigureServices method we will need to add:

```C#
 services.TryAddScoped<IRepository<TodoItem>, TodoRepository<TodoItem>>();
```

Finally lets clean up all the database code and replace with our Respository by Updating our TodoItemsController.

Notice we replaced `_context` with `_repo` context is now implemented in our Respository.

```C#
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IRepository<TodoItem> _repo;

        public TodoItemsController(IRepository<TodoItem> repo)
        {
            _repo = repo;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {

            return await _repo.GetAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {

            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }



            try
            {
                await _repo.UpdateAsync(todoItem);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {

           await _repo.InsertAsync(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            await _repo.DeleteAsync(todoItem);

            return todoItem;
        }

        private async Task<bool> TodoItemExists(long id)
        {
            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
    }
}

```

Now Lets test out our work using the put post and delete both Get by Id and Get all Items in PostMan.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Postman_Get_Exp_2.png?raw=true 'Request Pipeline')

[EF Core Docs](https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio)
