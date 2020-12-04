# Web Hook HUB
The proposal for this project is that want to have a Web Hook Hub to manage all the webhook notification of the same company

## Build Status

&nbsp; | `master` | `dev`
--- | --- | --- 
<!---**Windows** | [![Windows Build Status](https://ci.appveyor.com/api/projects/status/70m632jkycqpnsp9/branch/master?svg=true)](https://ci.appveyor.com/project/odinserj/hangfire-525)  | [![Windows Build Status](https://ci.appveyor.com/api/projects/status/70m632jkycqpnsp9/branch/dev?svg=true)](https://ci.appveyor.com/project/odinserj/hangfire-525) -->
**Linux / OS X** | [![Travis CI Build Status](https://travis-ci.com/gmoreno90/webhookhub.svg?branch=main)](https://travis-ci.com/github/gmoreno90/webhookhub) | [![Linux and OS X Build Status](https://travis-ci.com/gmoreno90/webhookhub.svg?branch=develop)](https://travis-ci.com/github/gmoreno90/webhookhub)


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

### Service Allways

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


License
----

MIT

**Free Software, Hell Yeah!**
