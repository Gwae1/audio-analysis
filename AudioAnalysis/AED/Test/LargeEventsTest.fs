﻿open Common
open FsCheck
open FsCheckXunit
open GetAcousticEventsTest
open QutSensors.AudioAnalysis.AED.GetAcousticEvents
open QutSensors.AudioAnalysis.AED.LargeEvents
open Xunit
    
[<Fact>]
let testLastMin () =
    Assert.Equal(1, lastMin [0..2] [2;1;2])
    Assert.Equal(2, lastMin [0..3] [1;2;1;1])
    Assert.Equal(0, lastMin [0..2] [1;1;2])
    Assert.Equal(3, lastMin [0..4] [2;1;1;0;1])
    Assert.Equal(1, lastMin [0..3] [2;1;2;1])
    
[<Fact>]
let testThreshold () =
    let MATLAB_LENGTH = 1229 // TODO also defined in GetAcousticEventsTest
    let aem = loadEventsFile "AE.txt" MATLAB_LENGTH    
    Assert.Equal(9000, threshold aem)

[<Fact>]
let testAeToMatrixBounds () =   
    overwriteGenerators<ArbitraryModifiers>() // TODO duplicated
    let propBounds ae = 
        let m = aeToMatrix ae
        m.NumRows = ae.Bounds.Height && m.NumCols = ae.Bounds.Width
    check config propBounds
    
[<Fact>]
let testAeToMatrixElements () =
    overwriteGenerators<ArbitraryModifiers>() // TODO duplicated
    let propElements ae =
        let f i j x = 
            let inSet = Set.contains (ae.Bounds.Top+i, ae.Bounds.Left+j) ae.Elements
            if x = 1.0 then inSet else not inSet
        aeToMatrix ae |> Math.Matrix.foralli f
    check config propElements
    
[<Fact>]
let separateLargeEventsTest () =
    let ae2 = loadTestFile "I6b.txt" |> getAcousticEvents |> separateLargeEvents
    let ae2m = loadEventsFile "AE2.txt" 1249 // TODO hardcoded in filterSmallEvents test as well
    Assert.Equal(Seq.length ae2m, Seq.length ae2)
    Assert.Equal(Seq.sort ae2m, Seq.sort ae2)