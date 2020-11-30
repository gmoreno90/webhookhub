# Web Hook HUB
The proposal for this project is that want to have a Web Hook Hub to manage all the webhook notification of the same company

[![Build Status](https://api.travis-ci.com/gmoreno90/webhookhub.svg?branch=develop)](https://travis-ci.com/github/gmoreno90/webhookhub)

### Main Features!
- Multiple Clientes
- Multiple Events
- Multiple Configurations
- Retry Policies
- Background Jobs
- Multi-Server Manage Jobs

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

### Tech

Dillinger uses a number of open source projects to work properly:

* [NET CORE 3.0 API] - Build the logic and the APIs
* [HangFire] - For Manage Queues and Background Proccess.


License
----

MIT

**Free Software, Hell Yeah!**
