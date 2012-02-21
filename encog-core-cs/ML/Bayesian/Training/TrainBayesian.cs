using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.Data;
using Encog.ML.Bayesian.Training.Estimator;
using Encog.ML.Bayesian.Training.Search;
using Encog.ML.Bayesian.Training.Search.k2;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.Bayesian.Training
{
    /// <summary>
    /// Train a Bayesian network.
    /// </summary>
    public class TrainBayesian : BasicTraining
    {
        /// <summary>
        /// What phase of training are we in?
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Init phase.
            /// </summary>
            Init,
            /// <summary>
            /// Searching for a network structure.
            /// </summary>
            Search,
            /// <summary>
            /// Search complete.
            /// </summary>
            SearchDone,
            /// <summary>
            /// Finding probabilities.
            /// </summary>
            Probability,
            /// <summary>
            /// Finished training.
            /// </summary>
            Finish,
            /// <summary>
            /// Training terminated.
            /// </summary>
            Terminated
        };

        /// <summary>
        /// The phase that training is currently in.
        /// </summary>
        private Phase p = Phase.Init;

        /// <summary>
        /// The data used for training.
        /// </summary>
        private readonly IMLDataSet data;

        /// <summary>
        /// The network to train.
        /// </summary>
        private readonly BayesianNetwork network;

        /// <summary>
        /// The maximum parents a node should have.
        /// </summary>
        private readonly int maximumParents;

        /// <summary>
        /// The method used to search for the best network structure.
        /// </summary>
        private readonly IBayesSearch search;

        /// <summary>
        /// The method used to estimate the probabilities.
        /// </summary>
        private readonly IBayesEstimator estimator;

        /// <summary>
        /// The method used to setup the initial Bayesian network.
        /// </summary>
        private BayesianInit initNetwork = BayesianInit.InitNaiveBayes;

        /// <summary>
        /// Used to hold the query.
        /// </summary>
        private String holdQuery;

        /// <summary>
        /// Construct a Bayesian trainer. Use K2 to search, and the SimpleEstimator
        /// to estimate probability.  Init as Naive Bayes
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train.</param>
        /// <param name="theMaximumParents">The max number of parents.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                int theMaximumParents)
            : this(theNetwork, theData, theMaximumParents,
                BayesianInit.InitNaiveBayes, new SearchK2(),
                new SimpleEstimator())
        {

        }

        /// <summary>
        /// Construct a Bayesian trainer. 
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train with.</param>
        /// <param name="theMaximumParents">The maximum number of parents.</param>
        /// <param name="theInit">How to init the new Bayes network.</param>
        /// <param name="theSearch">The search method.</param>
        /// <param name="theEstimator">The estimation mehod.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                int theMaximumParents, BayesianInit theInit, IBayesSearch theSearch,
                IBayesEstimator theEstimator)
            : base(TrainingImplementationType.Iterative)
        {

            this.network = theNetwork;
            this.data = theData;
            this.maximumParents = theMaximumParents;

            this.search = theSearch;
            this.search.Init(this, theNetwork, theData);

            this.estimator = theEstimator;
            this.estimator.Init(this, theNetwork, theData);

            this.initNetwork = theInit;
            Error = 1.0;
        }

        /// <summary>
        /// Init to Naive Bayes.
        /// </summary>
        private void InitNaiveBayes()
        {
            // clear out anything from before
            this.network.RemoveAllRelations();

            // locate the classification target event
            BayesianEvent classificationTarget = this.network
                    .ClassificationTargetEvent;

            // now link everything to this event
            foreach (BayesianEvent e in this.network.Events)
            {
                if (e != classificationTarget)
                {
                    network.CreateDependency(classificationTarget, e);
                }
            }

        }

        /// <summary>
        /// Handle iterations for the Init phase.
        /// </summary>
        private void IterationInit()
        {
            this.holdQuery = this.network.ClassificationStructure;

            switch (this.initNetwork)
            {
                case BayesianInit.InitEmpty:
                    this.network.RemoveAllRelations();
                    break;
                case BayesianInit.InitNoChange:
                    break;
                case BayesianInit.InitNaiveBayes:
                    InitNaiveBayes();
                    break;
            }
            this.p = Phase.Search;
        }

        /// <summary>
        /// Handle iterations for the Search phase.
        /// </summary>
        private void IterationSearch()
        {
            if (!this.search.Iteration())
            {
                this.p = Phase.SearchDone;
            }
        }

        /// <summary>
        /// Handle iterations for the Search Done phase.
        /// </summary>
        private void IterationSearchDone()
        {
            this.network.FinalizeStructure();
            this.network.Reset();
            this.p = Phase.Probability;
        }

        /// <summary>
        /// Handle iterations for the Probability phase.
        /// </summary>
        private void IterationProbability()
        {
            if (!this.estimator.Iteration())
            {
                this.p = Phase.Finish;
            }
        }

        /// <summary>
        /// Handle iterations for the Finish phase.
        /// </summary>
        private void IterationFinish()
        {
            this.network.DefineClassificationStructure(this.holdQuery);
            Error = this.network.CalculateError(this.data);
            this.p = Phase.Terminated;
        }

        /// <inheritdoc/>
        public override bool TrainingDone
        {
            get
            {
                if (base.TrainingDone)
                    return true;
                else
                    return this.p == Phase.Terminated;
            }
        }

        /// <inheritdoc/>
        public override void Iteration()
        {
            switch (p)
            {
                case Phase.Init:
                    IterationInit();
                    break;
                case Phase.Search:
                    IterationSearch();
                    break;
                case Phase.SearchDone:
                    IterationSearchDone();
                    break;
                case Phase.Probability:
                    IterationProbability();
                    break;
                case Phase.Finish:
                    IterationFinish();
                    break;
            }

        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {

        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Returns the network.
        /// </summary>
        public BayesianNetwork Network
        {
            get
            {
                return network;
            }
        }

        /// <summary>
        /// The maximum parents a node can have.
        /// </summary>
        public int MaximumParents
        {
            get
            {
                return maximumParents;
            }
        }

        /// <summary>
        /// The search method.
        /// </summary>
        public IBayesSearch Search
        {
            get
            {
                return this.search;
            }
        }

        /// <summary>
        /// The init method.
        /// </summary>
        public BayesianInit InitNetwork
        {
            get
            {
                return initNetwork;
            }
            set
            {
                this.initNetwork = value;
            }
        }


    }
}
