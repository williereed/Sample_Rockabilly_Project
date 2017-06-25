# Sample_Rockabilly_Project
Adds Selenium via Nuget, adds Environment and Browser to Test Case name

Open the Solution and:
1. Right Click SampleApp set it to be the Startup project
2. Right Click the Solution and select Properties

2a. Select the Debug tab

2b. In the Command Line Arguments enter: Run All Chrome;Edge;Firefox;InternetExplorer

Browser Support:

Internet Explorer

It is required to read the following:
https://github.com/SeleniumHQ/selenium/wiki/InternetExplorerDriver#required-configuration

Edge

Must be running a Windows Insider Preview to enable the Edge driver to run successfully.
I am running Windows 10 Enterprise Insider Preview Build 16226.rs_prerelease 170616-2021
This project includes the Edge driver version 10.0.16215.1000
The Edge driver (16215) and Windows (16226) should be the same number.
