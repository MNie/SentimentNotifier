namespace Sentiment.Job
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

    module JobFactory =
        let private triggerJob =
            TriggerBuilder.Create()
                .WithSimpleSchedule(fun x ->
                    x.WithIntervalInHours(24 * 7).RepeatForever() |> ignore)
                .Build()

        let scheduleJob () =
            let schedulerFactory = StdSchedulerFactory()
            let scheduler = schedulerFactory.GetScheduler()
            scheduler.Start()
            let job = JobBuilder.Create<Job>().Build()
            scheduler.ScheduleJob(job, triggerJob) |> ignore