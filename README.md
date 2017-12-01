# Welcome to Project Overwatch!
Overwatch is an application made to demonstrate the possibilities surrounding the use of Azure's Cognitive Services to gain insight concerning activity on an oil and gas well site.
This application focuses on elevating alerts pertaining to the physical security and safety of a well site.  Overwatch makes use of the following Azure technologies:

- Azure App Service (for hosting the Angular front-end)
- Azure Functions (serving as an API layer for the front-end as well as handling image processing)
- Cognitive Services (Custom Vision API to gain insight in to images seen)
- Azure SQL

# Installation process
This repository provides you with everything you need to get up and running with Overwatch.

Pull down the repository.  In it, you will notice two main areas.  The Overwatch folder contains the Angular front-end.  The VS-Overwatch folder contains a 
Visual Studio 2017 solution.  In this solution, you will find a few Azure Functions as well as a shared library.

The Overwatch front-end can be built using webpack as in normal Angular distributions by issuing the following Angular CLI command in the root folder:
        
        ng build

This command will generate a "dist" folder.  The content of this folder can be deployed to an Azure App Service Web App.  NOTE: Please ensure that the 
web.config file in the root of the Angular app is deployed to the Azure Web App.  Without it, Angular routes will not work properly.

Deploy the Azure Functions to an Azure Functions App.

Provision the included SQL schema in an Azure SQL database.

In the VS-Overwatch solution, you will find an ARM template that will provision all of the PaaS infrastructure needed for this application.

NOTE: Please ensure that the necessary connection strings are set in the Azure Functions app for the ability to connect to the
Azure SQL databse and Blob storage.

One additional component that needs to be provisioned is a Cognitive Services Custom Vision project.  Sample images are located in the root of the repository for training models and testing predictions.

Also, within the solution, you will notice that public APIs are being referenced for crime and weather data.

# Build and Test

Due to time constraints involved in creating this app during the (awesome) 2017 Devon Hackathon, some URLs will need to be swapped out
in a few places within the application.  In the Angular project and the Visual Studio solution, perform a search for both 'http://hpk' and 'https://hpk' to find
all places that require URLs to be swapped to the locations you have provisioned.

After all of the PaaS components are deployed, you can begin uploading images into blob storage.  The location of the images uploaded should adhere to the following path structure:

        /trucks/{propertyNumber}/

Where {propertyNumber} is a property number that can be found in the Property table within the Azure SQL database.

Once an image is uploaded, the ImageProcessor Azure Function will trigger on the upload, send the image in to Custom Vision and will then persist
any necessary alerts to the PropertyAlert table in the Azure SQL database.  The Angular front-end will then appropriately display the alerts.
