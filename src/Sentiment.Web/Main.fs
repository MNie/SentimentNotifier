namespace Sentiment.Web
    module Main=
    open Nancy
    open System
    open System
    open Quartz
    open Quartz.Impl
    open Sentiment.Data
    open Sentiment.Azure
    open Sentiment.Mail

    type Job () =
        interface IJob with
            member x.Execute(context: IJobExecutionContext) =
                DataProvider().GetFromLastWeek()
                |> AzureProvider().GetSentiment
                |> MailSender().Send

    let triggerJob =
        TriggerBuilder.Create()
            .WithSimpleSchedule(fun x ->
                x.WithIntervalInHours(12 * 7).RepeatForever() |> ignore)
            .Build()


    [<Literal>]
    let uriString = "http://localhost:9000"

    [<EntryPoint>]
    let main argv =
        let nancy = new Nancy.Hosting.Self.NancyHost(Uri(uriString))
        nancy.Start()
        Console.WriteLine("Nancy is running at {0}", uriString)
        let schedulerFactory = StdSchedulerFactory()
        let scheduler = schedulerFactory.GetScheduler()
        scheduler.Start()
        let job = JobBuilder.Create<Job>().Build()
        scheduler.ScheduleJob(job, triggerJob) |> ignore

        while true do Console.ReadLine() |> ignore
        0
