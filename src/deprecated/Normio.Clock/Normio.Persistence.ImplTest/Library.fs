namespace Normio.Persistence.ImplTest

open NEventStore

module Say =
    let hello name =
        Wireup.Init()
            .UsingSqlPersistence("l")
        printfn "Hello %s" name
