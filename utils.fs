module Utils

open System.Net;;
open System.Text;;
open System.IO;;

let curry f a b = f (a,b)
let uncurry f (a,b) = f a b

let both f (a, b) = (f a, f b) 


let bytes_to_string = Encoding.UTF8.GetString;;

let http_req (uri : string) = WebRequest.Create(uri) :?> HttpWebRequest;;
let http_resp (req : WebRequest) = req.GetResponse() :?> HttpWebResponse;;

let stream_reader (base_stream : Stream) = new StreamReader(base_stream);;
let stream_writer (base_stream : Stream) = new StreamWriter(base_stream);;
