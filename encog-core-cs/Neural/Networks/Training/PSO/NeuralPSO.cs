//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.Neural.Networks.Structure;
using Encog.Util.Concurrency;
using Encog.Neural.Networks.Training.Propagation;
using Encog.ML.Train;
using Encog.Util;
using System.Threading.Tasks;

namespace Encog.Neural.Networks.Training.PSO
{
    /// <summary>
    /// Iteratively trains a population of neural networks by applying   
    /// particle swarm optimisation (PSO).
    /// 
    /// Contributed by:
    /// Geoffroy Noel
    /// https://github.com/goffer-looney 
    /// 
    /// References: 
    ///  James Kennedy and Russell C. Eberhart, Particle swarm optimization, 
    /// Proceedings of the IEEE International Conference on Neural Networks, 
    /// 1995, pp. 1942-1948
    /// 
    /// </summary>
    public class NeuralPSO : BasicTraining, IMultiThreadable
    {
        /// <summary>
        /// For PSO, this does not set the absolute number of threads.  
        /// Set to 1 to use single-threaded, set to anything but one to
        /// use multithreaded.
        /// </summary>
        public int ThreadCount { get; set; }

        protected VectorAlgebra m_va;
        protected ICalculateScore m_calculateScore;
        protected IRandomizer m_randomizer;

        /// <summary>
        /// Swarm state and memories.
        /// </summary>
        protected BasicNetwork[] m_networks;
        protected double[][] m_velocities;
        protected double[][] m_bestVectors;
        protected double[] m_bestErrors;
        protected int m_bestVectorIndex;

        /// <summary>
        /// Although this is redundant with m_bestVectors[m_bestVectorIndex],
        /// m_bestVectors[m_bestVectorIndex] is not thread safe.
        /// </summary>
        private double[] m_bestVector;
        BasicNetwork m_bestNetwork = null;

        /// <summary>
        /// Typical range is 20 - 40 for many problems. 
        /// More difficult problems may need much higher value. 
        /// Must be low enough to keep the training process 
        /// computationally efficient.
        /// </summary>
        protected int m_populationSize = 30;

        /// <summary>
        /// Determines the size of the search space. 
        /// The position components of particle will be bounded to 
        /// [-maxPos, maxPos]
        /// A well chosen range can improve the performance. 
        /// -1 is a special value that represents boundless search space.
        /// </summary>
        protected double m_maxPosition = -1;



        /// <summary>
        /// Maximum change one particle can take during one iteration.
        /// Imposes a limit on the maximum absolute value of the velocity 
        /// components of a particle. 
        /// Affects the granularity of the search.
        /// If too high, particle can fly past optimum solution.
        /// If too low, particle can get stuck in local minima.
        /// Usually set to a fraction of the dynamic range of the search
        /// space (10% was shown to be good for high dimensional problems).
        /// -1 is a special value that represents boundless velocities. 
        /// </summary>
        protected double m_maxVelocity = 2;

        /// <summary>
        /// c1, cognitive learning rate >= 0
        /// tendency to return to personal best position
        /// </summary>
        protected double m_c1 = 2.0;

        /// <summary>
        /// c2, social learning rate >= 0
        /// tendency to move towards the swarm best position
        /// </summary>
        protected double m_c2 = 2.0;

        /// <summary>
        /// w, inertia weight.
        /// Controls global (higher value) vs local exploration 
        /// of the search space. 
        /// Analogous to temperature in simulated annealing.
        /// Must be chosen carefully or gradually decreased over time.
        /// Value usually between 0 and 1.
        /// </summary>
        protected double m_inertiaWeight = 0.4;

        /// <summary>
        /// If true, the position of the previous global best position 
        /// can be updated *before* the other particles have been modified.
        /// </summary>
        private bool m_pseudoAsynchronousUpdate = false;

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="network">an initialised Encog network. 
        ///                          The networks in the swarm will be created with 
        ///                          the same topology as this network.</param>
        /// <param name="randomizer">any type of Encog network weight initialisation
        ///                          object.</param>
        /// <param name="calculateScore">any type of Encog network scoring/fitness object.</param>
        /// <param name="populationSize">the swarm size.</param>
        public NeuralPSO(BasicNetwork network,
                IRandomizer randomizer, ICalculateScore calculateScore,
                int populationSize)
            : base(TrainingImplementationType.Iterative)
        {



            // initialisation of the member variables
            m_populationSize = populationSize;
            m_randomizer = randomizer;
            m_calculateScore = calculateScore;
            m_bestNetwork = network;

            m_networks = new BasicNetwork[m_populationSize];
            m_velocities = null;
            m_bestVectors = new double[m_populationSize][];
            m_bestErrors = new double[m_populationSize];
            m_bestVectorIndex = -1;

            // get a vector from the network.
            m_bestVector = NetworkCODEC.NetworkToArray(m_bestNetwork);

            m_va = new VectorAlgebra();
        }

        /// <summary>
        /// Initialise the particle positions and velocities, 
        /// personal and global bests.
        /// Only does this if they have not yet been initialised.
        /// </summary>
        public void InitPopulation()
        {
            if (m_velocities == null)
            {
                int dimensionality = m_bestVector.Length;
                m_velocities = EngineArray.AllocateDouble2D(m_populationSize, dimensionality);
                // run an initialisation iteration
                IterationPSO(true);
            }
        }

        /// <summary>
        /// Runs one PSO iteration over the whole population of networks.
        /// </summary>
        public override void Iteration()
        {
            InitPopulation();

            PreIteration();
            IterationPSO(false);
            PostIteration();
        }

        /// <summary>
        /// Handle an individual particle.
        /// </summary>
        /// <param name="i">The particle index.</param>
        /// <param name="init">True if this is the init pass.</param>
        protected void HandleParticle(int i, bool init)
        {
            NeuralPSOWorker worker = new NeuralPSOWorker(this, i, init);
            worker.Run();
        }

        /// <summary>
        /// Internal method for the iteration of the swarm. 
        /// </summary>
        /// <param name="init">true if this is an initialisation iteration.</param>
        protected void IterationPSO(bool init)
        {
            if (ThreadCount == 1)
            {
                for (int i = 0; i < m_populationSize; i++)
                {

                    HandleParticle(i, init);
                }
            }
            else
            {
                Parallel.For(0, m_populationSize, i =>
                {
                    HandleParticle(i, init);
                });
            }
            
            UpdateGlobalBestPosition();
        }

        /// <summary>
        /// Update the velocity, position and personal 
        /// best position of a particle.
        /// </summary>
        /// <param name="particleIndex">index of the particle in the swarm</param>
        /// <param name="init">if true, the position and velocity
        ///                          will be initialised. </param>
        public void UpdateParticle(int particleIndex, bool init)
        {
            int i = particleIndex;
            double[] particlePosition = null;
            if (init)
            {
                // Create a new particle with random values.
                // Except the first particle which has the same values 
                // as the network passed to the algorithm.
                if (m_networks[i] == null)
                {
                    m_networks[i] = (BasicNetwork)m_bestNetwork.Clone();
                    if (i > 0) m_randomizer.Randomize(m_networks[i]);
                }
                particlePosition = GetNetworkState(i);
                m_bestVectors[i] = particlePosition;

                // randomise the velocity
                m_va.Randomise(m_velocities[i], m_maxVelocity);
            }
            else
            {
                particlePosition = GetNetworkState(i);
                UpdateVelocity(i, particlePosition);

                // velocity clamping
                m_va.ClampComponents(m_velocities[i], m_maxVelocity);

                // new position (Xt = Xt-1 + Vt)
                m_va.Add(particlePosition, m_velocities[i]);

                // pin the particle against the boundary of the search space.
                // (only for the components exceeding maxPosition)
                m_va.ClampComponents(particlePosition, m_maxPosition);

                SetNetworkState(i, particlePosition);
            }
            UpdatePersonalBestPosition(i, particlePosition);
        }

        /// <summary>
        /// Update the velocity of a particle  
        /// </summary>
        /// <param name="particleIndex">index of the particle in the swarm</param>
        /// <param name="particlePosition">the particle current position vector</param>
        protected void UpdateVelocity(int particleIndex, double[] particlePosition)
        {
            int i = particleIndex;
            double[] vtmp = new double[particlePosition.Length];

            // Standard PSO formula

            // inertia weight
            m_va.Mul(m_velocities[i], m_inertiaWeight);

            // cognitive term
            m_va.Copy(vtmp, m_bestVectors[i]);
            m_va.Sub(vtmp, particlePosition);
            m_va.MulRand(vtmp, m_c1);
            m_va.Add(m_velocities[i], vtmp);

            // social term
            if (i != m_bestVectorIndex)
            {
                m_va.Copy(vtmp, m_pseudoAsynchronousUpdate ? m_bestVectors[m_bestVectorIndex] : m_bestVector);
                m_va.Sub(vtmp, particlePosition);
                m_va.MulRand(vtmp, m_c2);
                m_va.Add(m_velocities[i], vtmp);
            }
        }

        /// <summary>
        /// Update the personal best position of a particle. 
        /// </summary>
        /// <param name="particleIndex">index of the particle in the swarm</param>
        /// <param name="particlePosition">the particle current position vector</param>
        protected void UpdatePersonalBestPosition(int particleIndex, double[] particlePosition)
        {
            // set the network weights and biases from the vector
            double score = m_calculateScore.CalculateScore(m_networks[particleIndex]);

            // update the best vectors (g and i)
            if ((m_bestErrors[particleIndex] == 0) || IsScoreBetter(score, m_bestErrors[particleIndex]))
            {
                m_bestErrors[particleIndex] = score;
                m_va.Copy(m_bestVectors[particleIndex], particlePosition);
            }
        }

        /// <summary>
        /// Update the swarm's best position
        /// </summary>
        protected void UpdateGlobalBestPosition()
        {
            bool bestUpdated = false;
            for (int i = 0; i < m_populationSize; i++)
            {
                if ((m_bestVectorIndex == -1) || IsScoreBetter(m_bestErrors[i], m_bestErrors[m_bestVectorIndex]))
                {
                    m_bestVectorIndex = i;
                    bestUpdated = true;
                }
            }
            if (bestUpdated)
            {
                m_va.Copy(m_bestVector, m_bestVectors[m_bestVectorIndex]);
                m_bestNetwork.DecodeFromArray(m_bestVector);
                Error = m_bestErrors[m_bestVectorIndex];
            }
        }

        /// <summary>
        /// Compares two scores. 
        /// </summary>
        /// <param name="score1">a score</param>
        /// <param name="score2">a score</param>
        /// <returns>true if score1 is better than score2</returns>
        bool IsScoreBetter(double score1, double score2)
        {
            return ((m_calculateScore.ShouldMinimize && (score1 < score2)) || ((!m_calculateScore.ShouldMinimize) && (score1 > score2)));
        }

        public override TrainingContinuation Pause()
        {
            return null;
        }

        public override bool CanContinue
        {
            get
            {
                return false;
            }
        }

        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Returns the state of a network in the swarm  
        /// </summary>
        /// <param name="particleIndex">index of the network in the swarm</param>
        /// <returns>an array of weights and biases for the given network</returns>
        protected double[] GetNetworkState(int particleIndex)
        {
            return NetworkCODEC.NetworkToArray(m_networks[particleIndex]);
        }

        /// <summary>
        /// Sets the state of the networks in the swarm
        /// </summary>
        /// <param name="particleIndex">index of the network in the swarm</param>
        /// <param name="state">an array of weights and biases</param>
        protected void SetNetworkState(int particleIndex, double[] state)
        {
            NetworkCODEC.ArrayToNetwork(state, m_networks[particleIndex]);
        }

        /// <summary>
        /// Set the swarm size.
        /// </summary>
        public int PopulationSize
        {
            get
            {
                return m_populationSize;
            }
            set
            {
                m_populationSize = value;
            }
        }

        /// <summary>
        /// Sets the maximum velocity.
        /// </summary>
        public double MaxVelocity
        {
            get
            {
                return m_maxVelocity;
            }
            set
            {
                m_maxVelocity = value;
            }
        }

        /// <summary>
        /// Set the boundary of the search space (Xmax)
        /// </summary>
        public double MaxPosition
        {
            get
            {
                return m_maxPosition;
            }
            set
            {
                m_maxPosition = value;
            }
        }

        /// <summary>
        /// Sets the cognition coefficient (c1).
        /// </summary>
        public double C1
        {
            get
            {
                return m_c1;
            }
            set
            {
                m_c1 = value;
            }
        }


        /// <summary>
        /// Set the social coefficient (c2).
        /// </summary>
        public double C2
        {
            get
            {
                return m_c2;
            }
            set
            {
                m_c2 = value;
            }
        }

        /// <summary>
        /// Get the inertia weight (w) 
        /// </summary>
        public double InertiaWeight
        {
            get
            {
                return m_inertiaWeight;
            }
            set
            {
                m_inertiaWeight = value;
            }
        }

        /// <summary>
        /// Get a description of all the current settings.
        /// </summary>
        public String Description
        {
            get
            {
                StringBuilder result = new StringBuilder();

                result.Append("pop = ");
                result.Append(m_populationSize);
                result.Append(", w = ");
                result.Append(Format.FormatDouble(m_inertiaWeight, 2));
                result.Append(", c1 = ");
                result.Append(Format.FormatDouble(m_c1, 2));
                result.Append(", c2 = ");
                result.Append(Format.FormatDouble(m_c2, 2));
                result.Append(", Xmax = ");
                result.Append(Format.FormatDouble(m_maxPosition, 2));
                result.Append(", Vmax = ");
                result.Append(Format.FormatDouble(m_maxVelocity, 2));
                return result.ToString();
            }

        }

        /// <summary>
        /// Keep a reference to the passed population of networks.
        /// This population is not copied, it will evolve during training.   
        /// </summary>
        /// <param name="initialPopulation"></param>
        public void SetInitialPopulation(BasicNetwork[] initialPopulation)
        {
            m_networks = initialPopulation;
        }

        public override IMLMethod Method
        {
            get { return m_bestNetwork; }
        }        
    }
}
