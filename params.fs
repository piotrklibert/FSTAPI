module Params 
open Utils;;

open System.Web;;
open System.Text;;


type str_pair   =  (string * string);;
type pairs      =  str_pair list;;
type params     =  Bytes of byte array | Pairs of pairs | None;;

type quoting    =  Str of string | QFuns of (string -> string) list;;
type separator  =  Chr of string | SFun of (string -> string -> string);;

type meta       =  {termQuote: quoting; termSep: separator; pairSep: separator;}


let pjoin sep_char (a : string) (b : string) = 
    match (a,b) with
        | ("", c) -> c;
        | (d, "") -> d;
        | _ -> sprintf "%s%s%s" a sep_char b;;

let quote (str : string) = HttpUtility.UrlEncode(str);;

let simpleMeta = {termQuote = Str ""; termSep = Chr "="; pairSep = Chr "&"};;
let defaultMeta = {
    termQuote = QFuns [quote]; 
    termSep = SFun (pjoin "="); 
    pairSep = SFun (pjoin "&")
}


let getTermQuoteFunction meta =
    match meta.termQuote with
        | Str s -> (fun x -> s + x + s);
        | QFuns lst -> List.fold (fun y x -> (x>>y)) (fun x -> x) lst;

let getTermJoinFunction m : ((string * string) -> string)=
    match m.termSep with
        | Chr s -> (uncurry (pjoin s));
        | SFun f -> uncurry f

let getPairJoinFunction m : (string -> string -> string)=
    match m.pairSep with
        | Chr s -> (pjoin s)
        | SFun f -> f


let buildWith meta aListOfPairs =
    aListOfPairs 
        |> Seq.map (both (getTermQuoteFunction meta))
        |> Seq.map (getTermJoinFunction meta)
        |> Seq.fold (getPairJoinFunction meta) ""

let build aListOfPairs = 
    buildWith defaultMeta aListOfPairs

let buildBytes aListOfPairs =
    Encoding.UTF8.GetBytes (build aListOfPairs)

let buildBytesWith meta aListOfPairs =
    Encoding.UTF8.GetBytes (buildWith meta aListOfPairs)




(* 
 *
 * Unit tests for param builder...
 *
 *)
let test () = 
    let str = "string=another&another=string" in
    let data = [("string", "another"); 
                ("another", "string")] in
    let meta1 = defaultMeta in
    let meta2 = simpleMeta in
    let enc1 = buildWith meta1 data in
    let enc2 = buildWith meta2 data 
    in
        printfn "%A %A" enc1 enc2;
        str = enc1 && str = enc2;;

let test2 () =
    let 
        dict = [("te33st", "te&&&&///ttt");
                ("spam","eggs");
                ("p@#$%^#&^si", "ham"); 
                ("sit_dolor","loremIpsum"); 
                ("etc..", "asda%^^<<><sd"); 
                ("moar!", "params!");
                ("etc$$$&&&&&&..", "asd")] 
    in
    let str = (build dict) in
        printfn "%A" str;
        (String.length str) > (7 * 5);

