(*
  Password Generator Command Line (genpass.exe)
  https://github.com/aashishkoirala/passgen-cmd
  by Aashish Koirala (aashishkoirala.github.io)

  Inspired by https://github.com/dotcypress/password
*)

module PassGenCmd

open System
open System.Text
open System.Security.Cryptography
open System.Windows.Forms

let salt (site: string) (username: string) =
  let byteify (x: string) = x |> Encoding.ASCII.GetBytes
  let hash (x: byte[]) = x |> SHA256.Create().ComputeHash
  match site.Length > 0, username.Length > 0 with
  | true, true -> sprintf "%s-%s" site username |> byteify |> hash
  | _ -> failwith "Site and username must be specified."

let key (master: string) (salt: byte[]) (size: int) =
  match master.Length > 7, salt.Length > 7 with
  | true, true ->  Rfc2898DeriveBytes(master, salt, 1000).GetBytes size
  | _ -> failwith "Master password or salt too short (must be 8 or more)."

let encode (data: byte[]) =
  let lowers = [|'a'..'z'|]
  let uppers = [|'A'..'Z'|]
  let numbers = [|'0'..'9'|]
  let symbols = [|'!'; '@'; '#'; '$'; '%'|]
  let all = Array.concat [lowers; uppers; numbers; symbols]
  let getChar (lookup: char[]) data =
    data |> Seq.map (fun x -> lookup.[int x % lookup.Length])

  match data.Length > 11 with
  | false -> failwith "Password length too small (must be 12 or more)."
  | true ->
    Seq.concat [
      (data |> Seq.take 1 |> getChar uppers);
      (data |> Seq.skip 1 |> Seq.take 1 |> getChar lowers);
      (data |> Seq.skip 2 |> Seq.take 1 |> getChar numbers);
      (data |> Seq.skip 3 |> Seq.take 1 |> getChar symbols);
      (data |> Seq.skip 4 |> getChar all)
    ] |> String.Concat

let password master site username size =
  match size > 11 with
  | false -> failwith "Password length too small (must be 12 or more)."
  | true -> (key master (salt site username) size) |> encode

[<EntryPoint; STAThread>]
let main (argv: string[]) =
  try
    (match argv with
    | [|master; site; username; sizeAsString|] ->
      password master site username (int sizeAsString)
    | [|master; site; username|] ->
      password master site username 16
    | _ ->
      failwith "Insufficient or invalid arguments.") |> Clipboard.SetText
    printfn "%s" "Password generated and copied to clipboard."
    0
  with
  | ex ->
    printfn "%s\r\n\r\n%s" ex.Message
      "USAGE: genpass master-password site-name user-name [password-length]"
    1
