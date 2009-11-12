using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.Multi
{
    /// <summary>
    /// The gradient map is used to track all of the workers and collect their
    /// gradient descents into the master neural network for training. This allows
    /// the threads to work somewhat independently and then aggregate their results
    /// at the end of each iteration.
    /// 
    /// A map is built up ahead of time to allow quick access when the training is
    /// actually running.
    /// </summary>
    public class GradientMap
    {
        /// <summary>
        /// A mapping between master levels and a list of each corresponding worker
        /// level.
        /// </summary>
        private IDictionary<PropagationLevel, IList<PropagationLevel>> levelMap = new Dictionary<PropagationLevel, IList<PropagationLevel>>();

        /// <summary>
        /// A mapping between master levels and a list of each corresponding worker
        /// level.
        /// </summary>
        private IDictionary<PropagationSynapse, IList<PropagationSynapse>> synapseMap = new Dictionary<PropagationSynapse, IList<PropagationSynapse>>();

        /// <summary>
        /// A list of all of the master levels.
        /// </summary>
        private IList<PropagationLevel> levels = new List<PropagationLevel>();

        /// <summary>
        /// A list of all of the master synapses.
        /// </summary>
        private IList<PropagationSynapse> synapses = new List<PropagationSynapse>();

        /// <summary>
        /// Construct a mapping between the master network training levels and all of
        /// the workers. This builds up the internal map that will be used to quickly
        /// collect results at the end of each training iteration.
        /// </summary>
        /// <param name="master">The training util for the master network that all results are
        /// aggregated to.</param>
        /// <param name="mprop">The MPROP object that contains all of the workers.</param>
        public GradientMap(PropagationUtil master,
                 MultiPropagation mprop)
        {
            LinkLevels(master, mprop);
        }

        /// <summary>
        /// Collect the gradient descents from all levels and synapses and place them
        /// in the master training utility.
        /// </summary>
        public void Collect()
        {
            // handle levels
            foreach (PropagationLevel masterLevel in this.levels)
            {
                CollectLevel(masterLevel);
            }

            // handle synapses
            foreach (PropagationSynapse masterSynapse in this.synapses)
            {
                CollectSynapse(masterSynapse);
            }
        }

        /// <summary>
        /// Collect the gradient descents for the specific master level. This will
        /// sum the gradient descents for all worker threads and place them in the
        /// master level.
        /// </summary>
        /// <param name="masterLevel">The level to collect from.</param>
        private void CollectLevel(PropagationLevel masterLevel)
        {
            IList<PropagationLevel> workerLevels = this.levelMap[masterLevel];

            double[] masterThresholdGradients = masterLevel.ThresholdGradents;

            foreach (PropagationLevel workerLevel in workerLevels)
            {
                double[] workerThresholdGradiends = workerLevel.ThresholdGradents;

                for (int i = 0; i < workerThresholdGradiends.Length; i++)
                {
                    masterThresholdGradients[i] += workerThresholdGradiends[i];
                    workerThresholdGradiends[i] = 0;
                }
            }

        }

        /// <summary>
        /// Collect the gradient descents from all of the worker synapses and place
        /// them in the master synapses.
        /// </summary>
        /// <param name="masterSynapse">The master synapse to recieve the results from the workers.</param>
        private void CollectSynapse(PropagationSynapse masterSynapse)
        {
            IList<PropagationSynapse> workerSynapses = this.synapseMap[masterSynapse];
            double[][] masterMatrixGradients = masterSynapse
                           .AccMatrixGradients.Data;

            foreach (PropagationSynapse workerSynapse in workerSynapses)
            {
                double[][] workerMatrixGradients = workerSynapse
                       .AccMatrixGradients.Data;

                for (int r = 0; r < masterMatrixGradients.Length; r++)
                {
                    for (int c = 0; c < masterMatrixGradients[r].Length; c++)
                    {
                        masterMatrixGradients[r][c] += workerMatrixGradients[r][c];
                        workerMatrixGradients[r][c] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// A map between master levels and lists of corresponding worker
        /// levels.
        /// </summary>
        public IDictionary<PropagationLevel, IList<PropagationLevel>> LevelMap
        {
            get
            {
                return this.levelMap;
            }
        }

        /// <summary>
        /// All of the master levels.
        /// </summary>
        public IList<PropagationLevel> Levels
        {
            get
            {
                return this.levels;
            }
        }

        /// <summary>
        /// A map between worker synapses and lists of corresponding worker
        /// synapses.
        /// </summary>
        public IDictionary<PropagationSynapse, IList<PropagationSynapse>> SynapseMap
        {
            get
            {
                return this.synapseMap;
            }
        }

        /// <summary>
        /// All of the master synapses.
        /// </summary>
        public IList<PropagationSynapse> Synapses
        {
            get
            {
                return this.synapses;
            }
        }

        /// <summary>
        /// Actually begin building the linked map between master network training
        /// levels and all of the workers.
        /// </summary>
        /// <param name="master">The training util for the master network that all results are
        /// aggregated to.</param>
        /// <param name="mprop">The MPROP object that contains all of the workers.</param>
        private void LinkLevels(PropagationUtil master,
                 MultiPropagation mprop)
        {
            // build a list of iterators to access the worker levels one at a time
            IList<IEnumerator<PropagationLevel>> workerLevelIterator = new List<IEnumerator<PropagationLevel>>();

            foreach (MPROPWorker worker in mprop.Workers)
            {
                workerLevelIterator.Add(worker.Utility.Levels.GetEnumerator());
            }

            foreach (PropagationLevel masterLevel in master.Levels)
            {
                // add to master level list
                this.levels.Add(masterLevel);

                // build a list of worker levels
                IList<PropagationLevel> workerLevels = new List<PropagationLevel>();
                foreach (IEnumerator<PropagationLevel> iterator in workerLevelIterator)
                {
                    PropagationLevel workerLevel = iterator.Current;
                    workerLevels.Add(workerLevel);
                    iterator.MoveNext();
                }
                this.levelMap[masterLevel] = workerLevels;
                LinkSynapses(masterLevel, workerLevels);
            }
        }

        /// <summary>
        /// Link the specified worker synapses to the specified master synapses.
        /// </summary>
        /// <param name="masterLevel">The master level that contains the synapses to link.</param>
        /// <param name="workerLevels">The worker levels that correspond to the master level.</param>
        private void LinkSynapses(PropagationLevel masterLevel,
                 IList<PropagationLevel> workerLevels)
        {

            IList<IEnumerator<PropagationSynapse>> workerSynapseIteratorList = new List<IEnumerator<PropagationSynapse>>();

            foreach (PropagationLevel workerLevel in workerLevels)
            {
                workerSynapseIteratorList.Add(workerLevel.Outgoing.GetEnumerator());
            }

            foreach (PropagationSynapse masterSynapse in masterLevel.Outgoing)
            {
                // add to master synapse list
                this.synapses.Add(masterSynapse);

                // build a list of worker synapses
                List<PropagationSynapse> workerSynapses = new List<PropagationSynapse>();

                foreach (IEnumerator<PropagationSynapse> iterator in workerSynapseIteratorList)
                {
                    PropagationSynapse workerSynapse = iterator.Current;
                    workerSynapses.Add(workerSynapse);
                    iterator.MoveNext();
                }

                this.synapseMap[masterSynapse] = workerSynapses;
            }
        }
    }
}
