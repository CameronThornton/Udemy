open System.Net
open System.Threading.Tasks

let urlList =
    [ "ms", "http://www.microsoft.com"
      "google", "http://www.google.com"
      "apple", "http://www.apple.com" ]

let fetchAsync (name, url: string) =
    async {
        try
            let uri = System.Uri(url)
            let webClient = new WebClient()

            let! html =
                webClient.DownloadDataTaskAsync(uri)
                |> Async.AwaitTask

            return (html.Length, name)
        with
        | ex -> return (0, ex.Message)
    }

let z =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel
    |> Async.RunSynchronously
