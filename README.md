# Introduction 
This project is to allow developers to use Couchbase as a data source to store Grain State using the [Microsoft Orleans](https://github.com/dotnet/orleans) virtual actor framework. 

# Implementation
 Currently the only way to implement the project is to add a reference to the Orleans.Persistence.Couchbase.csproj in the silo project of your solution. An example of this in the [sample](https://github.com/jaypetrin/Orleans.Persistence.Couchbase/tree/master/sample) folder.

##Coming soon Nuget package
Soon there will be a nuget package for easy implementation.

# Running the Sample
To see a working example of the project you can run the sample application. To do so, follow these steps:

1. Clone the Repo
2. Create an appsettings.json file with the following fields 
```json
{
  "CouchbaseUris": [ "https://yourcouchbaseurlhere1:8091", "https://yourcouchbaseurlhere2:8091", "https://yourcouchbaseurlhere3:8091" ],
  "CouchbaseUserName": "administrator",
  "CouchbasePassword": "password",
  "CouchbaseBucketName": "bucketName"
}
```
3. Save the appsettings.json file in the root of the SampleSilo project.
4. Start the SampleSilo project
5. Start the SampleClient project
6. In the SampleClient console type /add *name*
7. In the SampleClient console type /list
8. The name you entered in step 6 should appear
9. You can repeat steps 6-8 and the list should grow with the new names being added to the state and retrieved from the Couchbase Bucket