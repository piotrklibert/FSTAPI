module TClient

open System.Net;;
open System.Text;;
open System.IO;;

open Utils;;
open Params;;


let request meth url (data : params) =
    let data_enc : byte array = 
        match data with 
            | Pairs p -> (Params.buildBytes p);
            | Bytes b -> b;
            | None -> [||]
    in
    match meth with
        | "GET" -> 
            let full_url = url + "?" + (bytes_to_string data_enc) in
            http_req full_url;

        | "POST" -> 
            let req = http_req url in
            let len = Array.length data_enc in
                req.Method          <- "POST";
                req.ContentType     <- "application/x-www-form-urlencoded";
                req.ContentLength   <- int64(len);

                use out = stream_writer(req.GetRequestStream()) in
                    out.Write (bytes_to_string data_enc);
                    req;
        | _ -> 
            failwith "The only supported request methods are GET and POST"
    ;;


type TClient(creds : Params.pairs) as self =
    let mutable authenticated = false;
    let mutable cookies = new CookieContainer();

    let base_url = "http://stable.tagasauris.com/";
    let login_url = "http://stable.tagasauris.com/api/2/login/";
    let base_uri = System.Uri(base_url);

    let credentials = Bytes (Params.buildBytes creds);

    let save_auth_cookies (resp : HttpWebResponse) =
        let cookie_str = resp.Headers.Item("Set-Cookie") in
            cookies.SetCookies(base_uri, cookie_str);
            authenticated <- true;

    member this.Request meth url data =
        match authenticated with
            | false -> self.Login();
            | true -> ();
        let req = request meth url data in
        req.CookieContainer <- cookies;
        req;
                
    member this.Login () =
        let req = request "POST" login_url credentials in
        let resp = http_resp req in
            save_auth_cookies resp;

    member this.Get path data =
        let req = this.Request "GET" (base_url + path) data in
        let resp = http_resp req in
            resp;

    member this.Post path data =
        // TODO: implement this
        (path, data);
    ;;
