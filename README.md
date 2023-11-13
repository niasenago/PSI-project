# InfoSync - Note Sharing Application
InfoSync is a collaboration application developed as part of the Software Engineering I course.

## Authors

- [@niasenago](https://www.github.com/niasenago)
- [@kara](https://github.com/ErnestasKaralius)
- [@ClawOfNyx](https://github.com/ClawOfNyx)
- [@RolandDiu](https://github.com/RolandDiu)
- [@SarunasJJ](https://github.com/SarunasJJ)


## Presentation

- You prepared an actual presentation (slides + working app demo) and are ready to present it loud before every other team.
- Every team member can present by himself about tasks he did. It is expected that after presentation (or during presentation) every team member will talk.
- You can present in your code every implemented requirement.

## Requirements
- [x] Relational database is used for storing data;
- [ ] Create generic method, event or delegate; define at least 2 generic constraints;
- [ ] Delegates usage;
- [x] Create at least 1 exception type and throw it; meaningfully deal with it; (most of the exceptions are logged to a file or a server);
- [x] Lambda expressions usage;
- [ ] Usage of threading via Thread class;
- [ ] Usage of async/await;
- [ ] Use at least 1 concurrent collection or Monitor;
- [x] Regex usage;
- [ ] No instances are created using 'new' keyword, dependency injection is used everywhere;
- [ ] Unit and integration tests coverage at least 20%;


## Getting Started 
Clone the repository to your local machine.
```bash
git clone https://github.com/niasenago/PSI-project
cd PSI-project
```
Start the application
```bash
 dotnet run
 ```

 Open your web browser and navigate to http://localhost:7200 to access InfoSync.

## Running Tests 
### Running Tests in Terminal
- Navigate to the root directory of the unit test project:
```bash
 cd CollabApp/CollabApp.UnitTests
 ```
- Execute the following command to run the tests:
```bash
 dotnet test
 ```
- After the tests have completed, you will see a summary of the results in the console. Any failed tests will be highlighted.
- You can use additional flags and options with the dotnet test command to customize test execution. Refer to the [official documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test) for more information.
### Running Tests in Visual Studio
- Launch Visual Studio and open the solution file (CollabApp.sln) that contains the unit test project.
- In Visual Studio, navigate to the "Test" menu and select "Test Explorer." This will open the Test Explorer window.
- Click the "Run All" button in the Test Explorer window. Visual Studio will discover and load all the unit tests in your solution.
- After the tests have completed, you will see a summary of the results in the Test Explorer window. Any failed tests will be highlighted. You can click on individual tests to view more detailed information.
- Visual Studio provides various options for running tests, including running specific tests, filtering tests, and more. Explore the Test Explorer for additional functionalities.


 ## License

[MIT](https://choosealicense.com/licenses/mit/)