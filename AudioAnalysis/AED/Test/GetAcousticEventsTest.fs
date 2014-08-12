﻿module GetAcousticEventsTest

open Common
open QutSensors.AudioAnalysis.AED.GetAcousticEvents
open QutSensors.AudioAnalysis.AED.Util
open Xunit

[<Fact>]
let spiderTest () = 
    let m = Math.Matrix.zero 3 4
    Assert.Equal(Set.empty, spider m [(0,0)] Set.empty)
    m.[0,2] <- 1.0
    Assert.Equal(Set.ofList [(0,2)], spider m [(0,2)] Set.empty)
    m.[0,2] <- 1.0
    m.[1,2] <- 1.0 // resulting matrix: 0 0 1 0
    m.[2,1] <- 1.0 //                   1 0 1 0
    m.[1,0] <- 1.0 //                   0 1 0 0 
    Assert.Equal(Set.ofList [(0,2);(1,0);(1,2);(2,1)], spider m [(0,2)] Set.empty)
 
[<Fact>]   
let getAcousticEventsTestQuick () =
    let m = Math.Matrix.zero 4 3
    Assert.Equal([], getAcousticEvents m)
    m.[0,1] <- 1.0
    Assert.Equal([{Bounds=lengthsToRect 1 0 1 1; Elements=Set.ofList [(0,1)]}], (getAcousticEvents m))
    m.[1,1] <- 1.0
    m.[1,2] <- 1.0
    Assert.Equal([{Bounds=lengthsToRect 1 0 2 2; Elements=Set.ofList [(0,1);(1,1);(1,2)]}], (getAcousticEvents m))
    m.[3,0] <- 1.0
    Assert.Equal(2, List.length (getAcousticEvents m))

[<Fact>]
let getAcousticEventsTest () =
    let f md =
        let ae = loadTestFile "I6.csv" md |> getAcousticEvents |> bounds
        let aem = loadIntEventsFile "AE1.csv" md  
        Assert.Equal(Seq.length aem, Seq.length ae)
        Assert.Equal(Seq.sort aem, Seq.sort ae)
    testAll f

[<Fact>]
let ``spidering test - testing blocking`` () = 

    let testMatrix = parseStringAsMatrix @"
000000000000
000000000000
001111111000
001111111000
001111111000
000000000000
"

    let results = spider testMatrix [(2, 2)] Set.empty

    let expected = 
        Set.ofList [
                    (2,2); (2,3); (2,4); (2,5); (2,6); (2, 7); (2, 8);
                    (3,2); (3,3); (3,4); (3,5); (3,6); (3, 7); (3, 8);
                    (4,2); (4,3); (4,4); (4,5); (4,6); (4, 7); (4, 8);
                   ]

    Assert.Equal(expected, results)

[<Fact>]
let ``spidering test - testing blocking with hole`` () = 

    let testMatrix = parseStringAsMatrix @"
000000000000
000000000000
001111111000
001000001000
001111111000
000000000000
"

    let results = spider testMatrix [(2, 2)] Set.empty

    let expected = 
        Set.ofList [
                    (2,2); (2,3); (2,4); (2,5); (2,6); (2, 7); (2, 8);
                    (3,2);                                     (3, 8);
                    (4,2); (4,3); (4,4); (4,5); (4,6); (4, 7); (4, 8);
                   ]

    Assert.Equal(expected, results)

[<Fact>]
let ``spidering test - blank edge`` () = 

    let testMatrix = parseStringAsMatrix @"
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111110111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
0000000000000000000000000000000000000000111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000001111111111111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000001111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000001111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000011110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000011111111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000111111111111111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110
1111111111111111111111111111111111111111111111111111100000111111111111111111111111111111111111111111111111111100000111111111111111111111111111111111111111110
1111111111111111111111111111111111111110000000001111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110
"
    let expectedElements = hitsToCoordinates testMatrix

    let results = spider testMatrix [(0,0)] Set.empty

    Assert.Equal(expectedElements, results)
