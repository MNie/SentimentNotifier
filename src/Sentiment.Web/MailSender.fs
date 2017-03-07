namespace Sentiment.Mail
    open System
    open System.Net.Mail
    open Sentiment.Data
    open Newtonsoft.Json

    type MailSender() =
        let createMessage (data) =
            let msg =
                new MailMessage(
                    Settings.Sender,
                    Settings.Receiver,
                    "Evaluations weekly update",
                    "Weekly summary of evaluations\r\n" + JsonConvert.SerializeObject(data)
                )
            msg.IsBodyHtml <- true
            msg

        member this.Send (data) =
            let msg = createMessage(data)
            let client = new SmtpClient(Settings.MailServer, Settings.Port)
            client.EnableSsl <- true
            client.Credentials <- System.Net.NetworkCredential(Settings.Sender, Settings.Pass)
            client.SendCompleted |> Observable.add(fun e ->
                let msg = e.UserState :?> MailMessage
                if e.Cancelled then
                    ("Mail message cancelled:\r\n" + msg.Subject) |> Console.WriteLine
                if isNull(e.Error) then
                    ("Sending mail failed for message:\r\n" + msg.Subject + ", reason:\r\n" + e.Error.ToString())
                    |> Console.WriteLine
                if msg<>Unchecked.defaultof<MailMessage> then msg.Dispose()
                if client<>Unchecked.defaultof<SmtpClient> then client.Dispose()
            )
            client.SendAsync(msg, msg)