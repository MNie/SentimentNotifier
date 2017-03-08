namespace Sentiment.Data
    open Microsoft.FSharp.Linq
    open System
    open FSharp.Data.TypeProviders
    open System.Configuration
    open FSharp.Configuration
    open Newtonsoft.Json
    type Settings = AppSettings<"app.config">

    module Helpers =
        let (|Null|Value|) (x: _ Nullable) =
            if x.HasValue then Value x.Value else Null

    type SentimentData =
        {
            Id: int
            User: string
            EstimatedUser: string
            Evaluation: string
            Score: Nullable<float>
            RecommendationScore: Nullable<float>
            RecommendationNumber: Nullable<int>
            RecommendationId: Nullable<int>
            EvaluationTime: Nullable<DateTime>
        }

    type dbSchema = SqlDataConnection<ConnectionStringName = "db", ConfigFile = "App.config">

    type DataProvider() =
        member this.GetFromLastWeek() =
            let db = dbSchema.GetDataContext()
            let weekAgo = (DateTime.UtcNow).AddDays(-7.0)
            query {
                for row in db.Evaluations do
                where (row.Time ?> weekAgo)
                select row
            }
            |> Seq.map (fun row -> {Id = row.Id; User = row.UserId; EstimatedUser = row.EstimatedUserId; Evaluation = row.Comment; Score = row.Score; RecommendationScore = row.RecommendationScore; RecommendationNumber = row.Number; RecommendationId = row.ItemId; EvaluationTime = row.Time})