namespace Sentiment.Web
    module Main=
    open Nancy
    open System
    open System
    open Sentiment.Job


    [<Literal>]
    let uriString = "http://localhost:9000"

    [<EntryPoint>]
    let main argv =
        let nancy = new Nancy.Hosting.Self.NancyHost(Uri(uriString))
        nancy.Start()
        Console.WriteLine("Nancy is running at {0}", uriString)
        JobFactory.scheduleJob()

        while true do Console.ReadLine() |> ignore
        0
