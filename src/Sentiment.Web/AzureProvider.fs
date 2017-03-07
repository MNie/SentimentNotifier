namespace Sentiment.Azure
    open FSharp.Data
    open Newtonsoft.Json
    open FSharp.Data.HttpRequestHeaders
    open Sentiment.Data

    type responseJson =
        {
            documents: seq<responseDoc>
            errors: string list
        }
    and responseDoc =
        {
            score: double
            id: string
        }

    type requestJson =
        {
            documents: seq<document>
        }
    and document =
        {
            language: string
            id: string
            text: string
        }

    type results =
        {
            sentiments: estimatedDoc list
        }
    and estimatedDoc =
        {
            Estimate: SentimentData
            Score: double
        }

    type AzureProvider() =
        let createJson (data:seq<SentimentData>) =
            {
                documents =
                    data
                    |> Seq.map ( fun x -> {language = "en"; id = x.Id.ToString(); text = x.Evaluation})
            }

        let getSentimentScore json =
            let serializeJson = JsonConvert.SerializeObject json
            let response = Http.RequestString(Settings.AzureApiUrl,
                body = TextRequest serializeJson,
                headers = [ContentType HttpContentTypes.Json; "Ocp-Apim-Subscription-Key", Settings.ApiKey])
            response

        member this.GetSentiment (data) =
            let estimate = getSentimentScore (createJson (data))
            let parsedSentiment = JsonConvert.DeserializeObject<responseJson>(estimate)
            parsedSentiment.documents
            |> Seq.map (fun x ->
                {
                    Estimate = data
                        |> Seq.where (fun y -> y.Id.ToString().Equals x.id)
                        |> Seq.head
                    Score = x.score
                }
            )
            |> Seq.sortBy (fun x -> x.Score)