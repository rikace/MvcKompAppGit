pen System.IO
open System
open System.Collections

type 'a List = E | L of 'a * 'a List
                    
let inist = L(10, L(12, L(15, E)))

let rec listSum = function
    | E -> 0
    | L(x, xs) -> x + (listSum xs)
    
/////////////////////////////////////////

type expr =
    | Cst of int
    | Prim of string * expr * expr

let rec eval (e:expr) : int =
    match e with   
    | Cst i -> i
    | Prim("+", e1, e2) -> eval e1 + eval e2
    | Prim("*", e1, e2) -> eval e1 * eval e2
    | Prim("-", e1, e2) -> eval e1 - eval e2
    | Prim("/", e1, e2) -> eval e1 / eval e2
    | _ -> failwith "ops"

let v = Prim("+", Cst 2, Prim("*", Cst 3, Cst 4))

eval(Prim("+", Cst 2, Prim("*", Cst 3, Cst 4)))


type exprf =
    | CstI of int
    | Primf of (int -> int -> int)  * exprf * exprf

let rec eval2 (e:exprf) : int =
    match e with   
    | CstI i -> i
    | Primf((f), e1, e2) -> f (eval2 e2) (eval2 e1)
//    | Primf((f), e1, e2) -> let r1 = eval2 e2
//                            let r2 = eval2 e1
//                            f r1 r2
//    | Primf((*), e1, e2) -> eval2 e1 * eval2 e2
//    | Primf((-), e1, e2) -> eval2 e1 - eval2 e2
//    | Primf((/), e1, e2) -> eval2 e1 / eval2 e2
    | _ -> failwith "ops"

eval2(Primf((+), CstI 2, Primf((*), CstI 3, CstI 4)))

let rec lookup env x = 
    match env with
    | [] -> failwith "x not found"
    | (y, v) ::r -> if x = y then v else lookup r x

let rec allFiles(dir) = seq {
    for file in System.IO.Directory.EnumerateFiles(dir) do
        yield file
    for sub in System.IO.Directory.EnumerateDirectories(dir) do
        yield! allFiles(sub)
}

//let show(x) =
//    textbox.Text <- sprintf "%A" x


//List.reduce
//List.fold
//List.scan

let arrData = ["test"; "riccardo"; "bryony"; "bugghina"; "test"; "riczardo"; "riccardl"]
let textData = "Well, Prince, so Genoa and Lucca are now just family estates of the Buonapartes. But I warn you, if you don't tell me that this means war, if you still try to defend the infamies and horrors perpetrated by that Antichrist--I really believe he is Antichrist I will have nothing more to do with you and you are no longer my friend, no longer my faithful slave, as you call yourself! But how do you do? I see I have frightened you--sit down and tell me all the news It was in July, 1805, and the speaker was the well-known Anna Pavlovna Scherer, maid of honor and favorite of the Empress Marya Fedorovna. With these words she greeted Prince Vasili Kuragin, a man of high rank and importance, who was the first to arrive at her reception. Anna Pavlovna had had a cough for some days. She was, as she said, suffering from la grippe; grippe being then a new word in St. Petersburg, used only by the elite All her invitations without exception, written in French, and delivered by a scarlet-liveried footman that morning, ran as follows If you have nothing better to do, Count (or Prince), and if the prospect of spending an evening with a poor invalid is not too terrible I shall be very charmed to see you tonight between 7 and 10--Annette Scherer havz havf"

let dataWords = (arrData @ (textData.Split(' ') |> Array.map (fun s -> s.ToLower()) |> Array.toList)) |> List.toSeq

let offSetSize = 3

let groupWordsBySubstring offset len (words:string seq) = words |> Seq.groupBy (fun word -> word.Substring(offset, len))                                                                               |> Seq.map(fun f -> offset, len, (snd f))

let check (offset:int, words:string seq) = 
    let wordLen = (words |> Seq.toList).Head.Length
    let currentCountToApply = 
            if wordLen >= offset then
                if (wordLen - offset) > offSetSize then
                    Some(offset, offSetSize)
                else Some(offset, (wordLen - offset))
            else None
    match currentCountToApply with
    | Some(o, l) -> groupWordsBySubstring o l words
    | None -> groupWordsBySubstring wordLen 0 words

let groupDataByLen (data:string seq) = seq { yield! (data |> Seq.groupBy (fun word -> word.Length) |> Seq.sortBy fst) }

let dataFirstProcess data = groupDataByLen data
                            |> Seq.map (fun f -> check(0, (snd f)))
                            |> Seq.concat 
                            |> Seq.toList

let partitionedList data = (snd (dataFirstProcess data
                            |> List.partition (fun (_, _, seqObj) -> Seq.length seqObj > 1 && (Seq.head seqObj).Length <= offSetSize)))

let partitionedList2 data = (fst (dataFirstProcess data
                            |> List.partition (fun (_, _, seqObj) -> Seq.length seqObj > 1 && (Seq.head seqObj).Length <= offSetSize)))


partitionedList2 dataWords |> List.iter (fun f -> printfn "%A" f)
partitionedList dataWords |> List.iter (fun f -> printfn "%A" f)

let dataSecondProcess = partitionedList dataWords
                        |> List.ofSeq
                        |> Seq.map (fun (offset, len, seqObj) -> check(offSetSize, seqObj)) 
                        |> Seq.concat 
                        |> Seq.toList

let partitionedListSec = (snd (dataSecondProcess 
                               |> List.partition (fun (offset, len, seqObj) -> Seq.length seqObj = 1 || (Seq.head seqObj).Length <= (offSetSize * 2))))                       

let partitionedListSec2 = (fst (dataSecondProcess 
                               |> List.partition (fun (offset, len, seqObj) -> Seq.length seqObj = 1 || (Seq.head seqObj).Length <= (offSetSize * 2))))                       


partitionedListSec |> Seq.iter (fun item -> printfn "%A" item)
partitionedListSec2 |> Seq.iter (fun item -> printfn "%A" item)

let dataThirdProcess = partitionedListSec
                        |> List.ofSeq
                        |> Seq.map (fun (offset, len, seqObj) -> check((offSetSize * 2), seqObj)) 
                        |> Seq.concat 
                        |> Seq.toList

dataThirdProcess |> List.iter (fun f -> printfn "%A" f)

let partitionedListThird= (snd (dataThirdProcess 
                                |> List.partition (fun (offset, len, seqObj) -> Seq.length seqObj = 1 || (Seq.head seqObj).Length <= (offSetSize * 3))))

let partitionedListThird2= (fst (dataThirdProcess 
                                |> List.partition (fun (offset, len, seqObj) -> Seq.length seqObj = 1 || (Seq.head seqObj).Length <= (offSetSize * 3))))

partitionedListThird |> List.iter (fun f -> printfn "%A" f)
partitionedListThird2 |> List.iter (fun f -> printfn "%A" f)


let process countPerStep data =

    ()

//|> List.partition (fun (_, _, lst) -> lst.Length > 1)

//|> Seq.filter (fun (k,v) -> Seq.length v > 1)
//|> Seq.map (fun (k,v) -> v)
//|> Seq.map (fun s -> (Seq.groupBy (fun v -> if v % 3 = 0 then 3 else 2) s))
