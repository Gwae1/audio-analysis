﻿module FindEventsLikeThis =
    // then essential point of this file is that given a data-set,
    // it should be able to run analysis in a configurable,
    // consistent manner.
    // A run will typically look like this
    //
    //  input >> cleaning >> selection >> training >> classification >> results preperation

    open FELT.Cleaners
    open FELT.Classifiers
    open FELT.Selectors
    open FELT.Trainers
    open FELT.Results
    open FELT.Core

    type WorkflowItem =
        | Cleaner of BasicCleaner
        | Selection of SelectorBase
        | Trainer of TrainerBase
        | Classifier of ClassifierBase
        | Result of ResultsComputation

    // TODO: pipe/compose
    let workflow trainingData testData  operationsList = 
        let oplst' = List.append operationsList [Result(new ResultsComputation())]
        
        let f (state: Data * Data * obj) (wfItem: WorkflowItem) =
            let trData, teData, results = state
            match wfItem with
                | Cleaner c -> (c.Clean(trData), c.Clean(teData), results)
                | Selection s -> (s.Pick(trData), teData, results)
                | Trainer t -> (t.Train(trData), teData, results)
                | Classifier c -> (trData, teData, c.Classify(trData, teData))
                | Result r -> (r.Calculate(trData, teData, results))

        List.scan f (trainingData, testData, null) operationsList


    let Basic = 
        [ 
        Cleaner(new BasicCleaner()); 
        Selection(new OneForOneSelector());
        Trainer(new OneForOneTrainer()); 
        Classifier(new EuclideanClassifier())
        ]

    let BasicGrouped = 
        [ 
        Cleaner(new BasicCleaner()); 
        Selection(new OneForOneSelector());
        Trainer(new OneForOneTrainer()); 
        Classifier(new EuclideanClassifier())
        ]

    let BasicAnti = 
        [ 
        Cleaner(new BasicCleaner()); 
        Selection(new RandomiserSelector());
        Trainer(new GroupTrainer()); 
        Classifier(new EuclideanClassifier())
        ]

    let BasicGroupedAnti = 
        [ 
        Cleaner(new BasicCleaner()); 
        Selection(new RandomiserSelector());
        Trainer(new GroupTrainer()); 
        Classifier(new EuclideanClassifier())
        ]


    let RunAnalysis trainingData testData tests =
        let result = workflow trainingData testData tests
        result
        