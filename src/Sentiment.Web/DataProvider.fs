namespace Sentiment.Data
    module Helpers =
        open System
        let (|Null|Value|) (x: _ Nullable) =
            if x.HasValue then Value x.Value else Null

    open Microsoft.FSharp.Linq
    open System
    open FSharp.Data.TypeProviders

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

    type dbSchema = SqlDataConnection<"""Data Source=localhost;Initial Catalog=Sentiment;Integrated Security=SSPI;""">

    type DataProvider() =

        let timeComparison (dbDate: Nullable<DateTime>) comparedTo =
            match dbDate with
            | Helpers.Value dbDate -> dbDate > comparedTo
            | _ -> false

        member this.GetFromLastWeek() =
            let db = dbSchema.GetDataContext()
            let weekAgo = (DateTime.UtcNow).AddDays(-7.0)
            query {
                for row in db.Evaluations do
                where (timeComparison row.Time weekAgo)
                select row
            }
            |> Seq.map (fun row -> {Id = row.Id; User = row.UserId; EstimatedUser = row.EstimatedUserId; Evaluation = row.Comment; Score = row.Score; RecommendationScore = row.RecommendationScore; RecommendationNumber = row.Number; RecommendationId = row.ItemId; EvaluationTime = row.Time})