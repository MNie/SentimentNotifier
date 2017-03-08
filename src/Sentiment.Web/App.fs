namespace Sentiment.Web
    module App =
    open Nancy
    open Sentiment.Data
    open Sentiment.Azure
    open Sentiment.Mail

    type App() as this =
        inherit NancyModule()
        do
            let getSentimentScore =
                DataProvider().GetFromLastWeek()
                |> AzureProvider().GetSentiment
            this.Get.["/"] <- fun _ -> "Hello World!" :> obj
            this.Get.["/Estimate"] <- fun _ ->
                getSentimentScore
                |> this.Response.AsJson
                :> obj
            this.Get.["/RawData"] <- fun _ ->
                DataProvider().GetFromLastWeek()
                |> this.Response.AsJson
                :> obj
            this.Get.["/SendMail"] <- fun _ ->
                getSentimentScore
                |> MailSender().Send
                "ok"
                |> this.Response.AsText
                :> obj