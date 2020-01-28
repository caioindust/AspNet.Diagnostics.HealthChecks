# AspNet.Diagnostics.HealthChecks
[![Build status](https://dev.azure.com/caioindust/AspNet.Diagnostics.HealthChecks/_apis/build/status/AspNet.Diagnostics.HealthChecks)](https://dev.azure.com/caioindust/AspNet.Diagnostics.HealthChecks/_build/latest?definitionId=1) [![Sonarcloud Status](https://sonarcloud.io/api/project_badges/measure?project=AspNet.Diagnostics.HealthChecks&metric=alert_status)](https://sonarcloud.io/dashboard?id=AspNet.Diagnostics.HealthChecks)

This project is a version for ASP Net Full Framework for creating HealthChecks, based on [**Xabaril AspNetCore.Diagnostics.HealthChecks**](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) project.

|         | AspNet.Diagnostics.HealthChecks                                                                                                                 | AspNet.HealthChecks.UI.Client                                                                                                               |
| ------- | ----------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| *NuGet* | [![NuGet](https://img.shields.io/nuget/v/AspNet.Diagnostics.HealthChecks.svg)](https://www.nuget.org/packages/AspNet.Diagnostics.HealthChecks/) | [![NuGet](https://img.shields.io/nuget/v/AspNet.HealthChecks.UI.Client.svg)](https://www.nuget.org/packages/AspNet.HealthChecks.UI.Client/) |

### Todo list
- [x] Compatibility with .NET Framework 4.6.1 or higher
- [X] Organize in libraries with well-defined requirements
- [X] Create pipeline in azure devops for building
- [x] Configure integration with sonarcloud
- [x] Configure rule for pull request in github for master branch
- [X] Create customization of class **HealthCheckResponseWriters** to return data in JSON format equal to [**AspNetCore.HealthChecks.UI**](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) project
- [ ] Create nuget.org package release pipeline in azure devops
- [ ] Create test to validate use of Xabaril health checks
  #### HealthChecks
  - [ ] Sql Server
  - [ ] MySql
  - [ ] Oracle
  - [ ] Sqlite
  - [ ] RavenDB
  - [ ] Postgres
  - [ ] EventStore
  - [ ] RabbitMQ
  - [ ] Elasticsearch
  - [ ] Redis
  - [ ] System: Disk Storage, Private Memory, Virtual Memory
  - [ ] Azure Service Bus: EventHub, Queue and Topics
  - [ ] Azure Storage: Blob, Queue and Table
  - [ ] Azure Key Vault
  - [ ] Azure DocumentDb
  - [ ] Amazon DynamoDb
  - [ ] Amazon S3
  - [ ] Network: Ftp, SFtp, Dns, Tcp port, Smtp, Imap
  - [ ] MongoDB
  - [ ] Kafka
  - [ ] Identity Server
  - [ ] Uri: single uri and uri groups
  - [ ] Consul
  - [ ] Hangfire
  - [ ] SignalR
  - [ ] Kubernetes
- [ ] Create samples
- [ ] Adjust the readme

### Links
- [Xabaril Project](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)