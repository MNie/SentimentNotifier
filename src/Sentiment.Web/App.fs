namespace Sentiment.Web
    module App =
    open Nancy
    open Sentiment.Data
    open Sentiment.Azure

    type App() as this =
        inherit NancyModule()
        do
            this.Get.["/"] <- fun _ -> "Hello World!" :> obj
            this.Get.["/Estimate"] <- fun _ ->
                DataProvider().GetFromLastWeek()
                |> AzureProvider().GetSentiment
                :> obj