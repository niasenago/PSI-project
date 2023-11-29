#!/bin/bash

# Set the relative path to the test folder
test_folder="CollabApp.UnitTests"

# Change directory to the test folder
cd "$test_folder" || exit 1

# Run the dotnet test command
dotnet test /p:CollectCoverage=true /p:ExcludeByFile="**/*Migrations/*.cs" /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/ /p:Logger="html;logfilename=testResults.html"

# Change directory to the TestResults folder
cd TestResults || exit 1

# Run reportgenerator command
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report

# Open the coverage report in the default browser
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    open coverage-report/index.html
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    # Linux
    xdg-open coverage-report/index.html || gnome-open coverage-report/index.html || kde-open coverage-report/index.html
elif [[ "$OSTYPE" == "msys"* || "$OSTYPE" == "win32" || "$OSTYPE" == "cygwin" ]]; then
    # Windows
    start coverage-report/index.html
else
    echo "Unsupported operating system"
fi
