open System.Net;;
open Utils;;
open Params;;


let print_reply (req : HttpWebResponse) =
    use stream = stream_reader (req.GetResponseStream()) in
        printfn "%A" (stream.ReadToEnd());;


[<EntryPoint>]
let main args = 
    let cred = [
        ("login", "YOUR_LOGIN_HERE");
        ("password", "YOUR_PASSWORD_HERE")] 
    in
    let c = new TClient.TClient(cred) in 
        let resp = c.Get "api/2/job/" None in
            print_reply resp;
        let resp = c.Get "api/2/mediaobject/" None in
            print_reply resp;
            0;;
