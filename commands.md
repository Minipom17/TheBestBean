dotnet restore: Restores the dependencies and tools for a project. It is run implicitly by other commands like build and run, so you only need to run it manually in special cases.

dotnet build: Compiles the project and its dependencies into a set of binaries. The output is placed in the bin/ directory.

dotnet run: Builds and runs the application from the source code. This is the most common command for local development.

dotnet watch: Automatically restarts or hot-reloads the application whenever a file change is detected. This is a very useful command for iterative development.

Example: dotnet watch run 

Deployment commands
dotnet publish: Compiles the application and gathers all necessary files into a directory for deployment to a hosting environment. This is the official and recommended way to prepare an app for production.
Example: dotnet publish -c Release -o ./output creates a production-ready package in the ./output directory. 

Restarting the application
How you restart an ASP.NET Core application depends on whether it's in development or production. 

Local development
With dotnet watch: The watcher will automatically detect code changes and restart the application for you.

Manual restart: If you are using dotnet run, you can stop the process by pressing Ctrl + C, and then run dotnet run again. 