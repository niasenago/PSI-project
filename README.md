# InfoSync - Note Sharing Application

## Overview

InfoSync is a comprehensive collaboration platform designed to streamline the process of sharing notes and information within a team or community, specifically targeted to students. It provides a user-friendly interface for creating, organizing, and communicating content. Developed as a part of the Software Engineering I course.

## Features

### Collaboration Boards
- **Board Management:** Users can create and manage boards which serve as a centralized space for organizing notes and information.

### Chat System
- **Real-Time Communication:** The chat feature allows users to enter and exit chat rooms, facilitating real-time discussion and collaboration.
- **Group Messaging:** Users can send messages to the group, ensuring everyone is on the same page and promoting collaborative effort.

### Posts and Sharing
- **Post Creation:** Users can easily create new posts with optional text content, supporting markdown for rich text formatting.
- **File Attachment:** The application allows for the attachment of files to posts, enabling the sharing of resources and documents.

### Post Sorting and Filtering
- **Content Sorting:** Posts can be sorted based on user-defined criteria, such as popularity, to prioritize the most relevant content.
- **Advanced Filters:** Users can filter posts by title, description, author, or date range to quickly find the information they need.

## Requirements
- [ ] Web service implemented and used;
- [x] Entity Framework and code-first migrations usage and understanding of difference between
- [ ] code/model/database first approaches;
- [ ] Usage of middleware and at least one interceptor;
- [ ] Unit and integration tests coverage at least 50%;
- [ ] Hackathon like pitch for the application is made;


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
- Navigate to the root directory of the project:
```bash
  cd CollabApp
 ```
- Execute the following command to run the tests:
```bash
 dotnet test
 ```
- After the tests have completed, you will see a summary of the results in the console. Any failed tests will be highlighted.
- You can use additional flags and options with the dotnet test command to customize test execution. Refer to the [official documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test) for more information.
### Running Tests in Visual Studio
- Launch Visual Studio and open the solution file (CollabApp.sln) that contains the unit and integration test projects.
- In Visual Studio, navigate to the "Test" menu and select "Test Explorer." This will open the Test Explorer window.
- Click the "Run All" button in the Test Explorer window. Visual Studio will discover and load all the unit tests in your solution.
- After the tests have completed, you will see a summary of the results in the Test Explorer window. Any failed tests will be highlighted. You can click on individual tests to view more detailed information.
- Visual Studio provides various options for running tests, including running specific tests, filtering tests, and more. Explore the Test Explorer for additional functionalities.

## Authors

- [@niasenago](https://www.github.com/niasenago)
- [@kara](https://github.com/ErnestasKaralius)
- [@ClawOfNyx](https://github.com/ClawOfNyx)
- [@RolandDiu](https://github.com/RolandDiu)
- [@SarunasJJ](https://github.com/SarunasJJ)

 ## License

[MIT](https://choosealicense.com/licenses/mit/)
