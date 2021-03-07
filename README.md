# Web Hook HUB
The proposal for this project is that want to have a Web Hook Hub to manage all the webhook notification of the same company

## Build Status

&nbsp; | `master` | `dev`
--- | --- | --- 
**Linux / OS X** | [![Azure CI Build Status](https://dev.azure.com/PolarisInformatica/Proyecto1/_apis/build/status/master/webhookhub-master-ci)](https://dev.azure.com/PolarisInformatica/Proyecto1/_apis/build/status/master/webhookhub-master-ci) | [![Linux and OS X Build Status](https://dev.azure.com/PolarisInformatica/Proyecto1/_apis/build/status/development/webhookhub-dev-ci)](https://dev.azure.com/PolarisInformatica/Proyecto1/_apis/build/status/development/webhookhub-dev-ci)

### Main Features!
- Multiple Clients
- Multiple Events
- Multiple Configurations (Enpoints)
- Retry Policies
- Background Jobs
- Multi-Server Manage Jobs

### Flow Diagram

[Show Here](https://sequencediagram.org/index.html#initialData=C4S2BsFMAIHVIEbQBIHtUGsUFUBCAofXAQwGMMBRAOwBMBaAPniTUx1wC4AFAeQGUAKtAAixYMWgAKANQBhcCEhVgs1DRjToFAG5KVayAEp8zFOizI8jU6wt4OWqgEcArpDdxEtgM7QASpCukN7A3iaIZmyWuHSMJOTUNBwAMiAh0KgAZtAAim5uvgCSwmH4ALxl8RgA5gBOqC600Fz1pMHeFeEs5uyMkgDMholcqCDKAIzcqOmi4l2RdjEMA0O0I2PAAExTM2LE87a9y4PDo8oAdJc7wCJ7QA)


### Configuration File
Inject appsettings.json file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "LogsDaysPurge": 2,
  "HangFireConfig": {
    "HangFireRetryIntervalInSeconds": [ 10, 30, 60, 120 ],
    "DashboardUserName": "XXXXXX",
    "DashboardPassword": "YYYYYY",
    "DashboardPath": "/HangFireDashboard"
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=XXXXXXXX;database=YYYYYYYYYY;uid=WWWWWWWWW;password=ZZZZZZZZ;"
  }
}
```

####Using Environment Variables 

```
ConnectionStrings__DefaultConnection 	server=XXXXXXXX;database=YYYYYYYYYY;uid=WWWWWWWWW;password=ZZZZZZZZ;
HangFireConfig__DashboardUserName		XXXXXXX2
HangFireConfig__DashboardPassword		YYYYYYY2
HangFireConfig__DashboardPath			/HangFireDashboard2
	
```



### Service Allways UP

In order to configure this website allways up, please follow the next documentation

https://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html


### Docker

DockerFile with the project
MSSQLServer Express For the Database

```cmd
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build
```


### Tech


* [.NET 5.0] - Build the business and the APIs
* [HangFire] - For Manage Queues and Background Process.


