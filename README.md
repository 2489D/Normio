# Normio

Online Exam/Quiz Management API provider

# Architecture

```
User Client
(ClientReq/ClientRes)
Persistence Layer(PL)
((Aggregate, Command), Event list))
Function layer
```

## User

1. Host: Online exam managers
    - Do we need hierachy for hosts? (e.g. master host and other hosts)
2. Students: students enrolled for the online exam


## Persistence Layer

1. Provides ReadModels for each user client
    - A read model defines what kinds of data should be visible to the user
2. Persistence Aggregate


## Function Layer (oldState -> newState)

1. Progress



