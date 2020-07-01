# Swagger

There are three main components to Swashbuckle:

- Swashbuckle.AspNetCore.Swagger: a Swagger object model and middleware to expose SwaggerDocument objects as JSON endpoints.

- Swashbuckle.AspNetCore.SwaggerGen: a Swagger generator that builds SwaggerDocument objects directly from your routes, controllers, and models. It's typically combined with the Swagger endpoint middleware to automatically expose Swagger JSON.

- Swashbuckle.AspNetCore.SwaggerUI: an embedded version of the Swagger UI tool. It interprets Swagger JSON to build a rich, customizable experience for describing the web API functionality. It includes built-in test harnesses for the public methods.

# Package installation

Swashbuckle can be added with the following approaches:

- From the Package Manager Console window:

  - Go to View > Other Windows > Package Manager Console
  - Navigate to the directory in which the TodoApi.csproj file exists
  - Execute the following command:

```PowerShell
Install-Package Swashbuckle.AspNetCore -Version 5.5.0
```

- From the Manage NuGet Packages dialog:

  - Right-click the project in Solution Explorer > Manage NuGet Packages
  - Set the Package source to "nuget.org"
  - Ensure the "Include prerelease" option is enabled
  - Enter "Swashbuckle.AspNetCore" in the search box
  - Select the latest "Swashbuckle.AspNetCore" package from the Browse tab and click Install

## Add and configure Swagger middleware

Add the Swagger generator to the services collection in the Startup.ConfigureServices method:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<TodoContext>(opt =>
        opt.UseInMemoryDatabase("TodoList"));
    services.AddControllers();

    // Register the Swagger generator, defining 1 or more Swagger documents
    services.AddSwaggerGen();
}
```

In the Startup.Configure method, enable the middleware for serving the generated JSON document and the Swagger UI:

```C#
public void Configure(IApplicationBuilder app)
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

> Note
> Swashbuckle relies on MVC's Microsoft.AspNetCore.Mvc.ApiExplorer to discover the routes and endpoints. If the project calls AddMvc, routes and endpoints are discovered automatically. When calling AddMvcCore, the AddApiExplorer method must be explicitly called. For more information, see Swashbuckle, ApiExplorer, and Routing.

The preceding UseSwaggerUI method call enables the Static File Middleware. If targeting .NET Framework or .NET Core 1.x, add the Microsoft.AspNetCore.StaticFiles NuGet package to the project.

Launch the app, and navigate to http://localhost:<port>/swagger/v1/swagger.json. The generated document describing the endpoints appears as shown in Swagger specification (swagger.json).

The Swagger UI can be found at http://localhost:<port>/swagger. Explore the API via Swagger UI and incorporate it in other programs.

> Tip
> To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:

```C#
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});
```

If using directories with IIS or a reverse proxy, set the Swagger endpoint to a relative path using the `./` prefix. For example, `./swagger/v1/swagger.json`. Using `/swagger/v1/swagger.json` instructs the app to look for the JSON file at the true root of the URL (plus the route prefix, if used). For example, use `http://localhost:<port>/<route_prefix>/swagger/v1/swagger.json` instead of `http://localhost:<port>/<virtual_directory>/<route_prefix>/swagger/v1/swagger.json.`

> Note
> By default, Swashbuckle generates and exposes Swagger JSON in version 3.0 of the specification—officially called the OpenAPI Specification. To support backwards compatibility, you can opt into exposing JSON in the 2.0 format instead. This 2.0 format is important for integrations such as Microsoft Power Apps and Microsoft Flow that currently support OpenAPI version 2.0. To opt into the 2.0 format, set the SerializeAsV2 property in Startup.Configure:

```C#
public void Configure(IApplicationBuilder app)
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

## Customize and extend

Swagger provides options for documenting the object model and customizing the UI to match your theme.

In the Startup class, add the following namespaces:

```C#
using System;
using System.Reflection;
using System.IO;
```

### API info and description

The configuration action passed to the AddSwaggerGen method adds information such as the author, license, and description:

In the Startup class, import the following namespace to use the OpenApiInfo class:

```C#
using Microsoft.OpenApi.Models;
```

Using the OpenApiInfo class, modify the information displayed in the UI:

```C#
// Register the Swagger generator, defining 1 or more Swagger documents
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "A simple example ASP.NET Core Web API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Shayne Boyer",
            Email = string.Empty,
            Url = new Uri("https://twitter.com/spboyer"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
});
```

The Swagger UI displays the version's information:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Swagger_Api_Info.png?raw=true 'Request Pipeline')

## XML comments

XML comments can be enabled with the following approaches:

- Right-click the project in Solution Explorer and select Edit <project_name>.csproj.
- Manually add the highlighted lines to the .csproj file:

```XML
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

Enabling XML comments provides debug information for undocumented public types and members. Undocumented types and members are indicated by the warning message. For example, the following message indicates a violation of warning code 1591:

```text
warning CS1591: Missing XML comment for publicly visible type or member 'TodoController.GetAll()'
```

To suppress warnings project-wide, define a semicolon-delimited list of warning codes to ignore in the project file. Appending the warning codes to \$(NoWarn); applies the C# default values too.

```XML
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

To suppress warnings only for specific members, enclose the code in #pragma warning preprocessor directives. This approach is useful for code that shouldn't be exposed via the API docs. In the following example, warning code CS1591 is ignored for the entire Program class. Enforcement of the warning code is restored at the close of the class definition. Specify multiple warning codes with a comma-delimited list.

```C#
namespace TodoApi
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args) =>
            BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
#pragma warning restore CS1591
}
```

Configure Swagger to use the XML file that's generated with the preceding instructions. For Linux or non-Windows operating systems, file names and paths can be case-sensitive. For example, a TodoApi.XML file is valid on Windows but not CentOS.

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<TodoContext>(opt =>
        opt.UseInMemoryDatabase("TodoList"));
    services.AddControllers();

    // Register the Swagger generator, defining 1 or more Swagger documents
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ToDo API",
            Description = "A simple example ASP.NET Core Web API",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Shayne Boyer",
                Email = string.Empty,
                Url = new Uri("https://twitter.com/spboyer"),
            },
            License = new OpenApiLicense
            {
                Name = "Use under LICX",
                Url = new Uri("https://example.com/license"),
            }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
}
```

In the preceding code, **Reflection** is used to build an XML file name matching that of the web API project. The **AppContext.BaseDirectory** property is used to construct a path to the XML file. Some Swagger features (for example, schemata of input parameters or HTTP methods and response codes from the respective attributes) work without the use of an XML documentation file. For most features, namely method summaries and the descriptions of parameters and response codes, the use of an XML file is mandatory.

Adding triple-slash comments to an action enhances the Swagger UI by adding the description to the section header. Add a `<summary>` element above the Delete action:

```C#
/// <summary>
/// Deletes a specific TodoItem.
/// </summary>
/// <param name="id"></param>
[HttpDelete("{id}")]
public IActionResult Delete(long id)
{
    var todo = _context.TodoItems.Find(id);

    if (todo == null)
    {
        return NotFound();
    }

    _context.TodoItems.Remove(todo);
    _context.SaveChanges();

    return NoContent();
}
```

The Swagger UI displays the inner text of the preceding code's `<summary>` element:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Swagger_Delete_UI.png?raw=true 'Request Pipeline')

The UI is driven by the generated JSON schema:

```Json
"delete": {
    "tags": [
        "Todo"
    ],
    "summary": "Deletes a specific TodoItem.",
    "operationId": "ApiTodoByIdDelete",
    "consumes": [],
    "produces": [],
    "parameters": [
        {
            "name": "id",
            "in": "path",
            "description": "",
            "required": true,
            "type": "integer",
            "format": "int64"
        }
    ],
    "responses": {
        "200": {
            "description": "Success"
        }
    }
}
```

Add a `<remarks>` element to the Create action method documentation. It supplements information specified in the `<summary>` element and provides a more robust Swagger UI. The `<remarks>` element content can consist of text, JSON, or XML.

```C#
/// <summary>
/// Creates a TodoItem.
/// </summary>
/// <remarks>
/// Sample request:
///
///     POST /Todo
///     {
///        "name": "Item1",
///        "isComplete": true,
///        "id": 0,  Set to zero EF assigns Id you can refactor this
///     }
///
/// </remarks>
/// <param name="item"></param>
/// <returns>A newly created TodoItem</returns>
/// <response code="201">Returns the newly created item</response>
/// <response code="400">If the item is null</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<TodoItem> Create(TodoItem item)
{

     await _repo.InsertAsync(todoItem);

     return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);

}
```

Notice the UI enhancements with these additional comments:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Swagger_Create_example.png?raw=true 'Request Pipeline')

## Data annotations

Mark the model with attributes, found in the System.ComponentModel.DataAnnotations namespace, to help drive the Swagger UI components.

Add the [Required] attribute to the Name property of the TodoItem class:

```C#
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem : BaseEntity
    {


        [Required]
        public string Name { get; set; }

        [DefaultValue(false)]
        public bool IsComplete { get; set; }
    }
}

```

The presence of this attribute changes the UI behavior and alters the underlying JSON schema:

```Json
"definitions": {
    "TodoItem": {
        "required": [
            "name"
        ],
        "type": "object",
        "properties": {
            "id": {
                "format": "int64",
                "type": "integer"
            },
            "name": {
                "type": "string"
            },
            "isComplete": {
                "default": false,
                "type": "boolean"
            }
        }
    }
},
```

Add the `[Produces("application/json")]` attribute to the API controller. Its purpose is to declare that the controller's actions support a response content type of application/json:

```C#
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;
```
The **Response Content Type** drop-down selects this content type as the default for the controller's GET actions:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Swagger_Application_Json.png?raw=true 'Request Pipeline')

As the usage of data annotations in the web API increases, the UI and API help pages become more descriptive and useful.

### Describe response types

Developers consuming a web API are most concerned with what's returned—specifically response types and error codes (if not standard). The response types and error codes are denoted in the XML comments and data annotations.

The Create action returns an HTTP 201 status code on success. An HTTP 400 status code is returned when the posted request body is null. Without proper documentation in the Swagger UI, the consumer lacks knowledge of these expected outcomes. Fix that problem by adding the highlighted lines in the following example:

```C#
/// <summary>
/// Creates a TodoItem.
/// </summary>
/// <remarks>
/// Sample request:
///
///     POST /Todo
///     {
///        "id": 1,
///        "name": "Item1",
///        "isComplete": true
///     }
///
/// </remarks>
/// <param name="item"></param>
/// <returns>A newly created TodoItem</returns>
/// <response code="201">Returns the newly created item</response>
/// <response code="400">If the item is null</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<TodoItem> Create(TodoItem item)
{
    _context.TodoItems.Add(item);
    _context.SaveChanges();

    return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
}
```
The Swagger UI now clearly documents the expected HTTP response codes:

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/Swagger_Post_Responses.png?raw=true 'Request Pipeline')

In ASP.NET Core 2.2 or later, conventions can be used as an alternative to explicitly decorating individual actions with [ProducesResponseType]. For more information, see Use web API conventions.

To support the [ProducesResponseType] decoration, the Swashbuckle.AspNetCore.Annotations package offers extensions to enable and enrich the response, schema, and parameter metadata.

## Customize the UI (Optional on your own exersise)

The default UI is both functional and presentable. However, API documentation pages should represent your brand or theme. Branding the Swashbuckle components requires adding the resources to [serve static files](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1) and building the folder structure to host those files.

If targeting .NET Framework or .NET Core 1.x, add the Microsoft.AspNetCore.StaticFiles NuGet package to the project:

```XML
<PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.0.0" />
```

The preceding NuGet package is already installed if targeting .NET Core 2.x and using the metapackage.



Enable Static File Middleware:

```C#
public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```


[Static Files Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1)

To inject additional CSS stylesheets, add them to the project's wwwroot folder and specify the relative path in the middleware options:

```C#
app.UseSwaggerUI(c =>
{
     c.InjectStylesheet("/swagger-ui/custom.css");
}
```

[Swagger Docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio)

# Logging

First, install the Serilog.AspNetCore NuGet package into your app.

```Console
dotnet add package Serilog.AspNetCore
```

or install by right click Solution file TodoApi.sln and select manage Nutget packages for solution.

In the search window paste `Serilog.AspNetCore`.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/InstallSerilog_Nuget.png?raw=true 'Request Pipeline')

Next, in your application's Program.cs file, configure Serilog first. A try/catch block will ensure any configuration issues are appropriately logged:

```C#
using Serilog;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
```

Then, add UseSerilog() to the Generic Host in CreateHostBuilder().

```C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <-- Add this line
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
}
```

[Serilog Docs](https://github.com/serilog/serilog-aspnetcore)


