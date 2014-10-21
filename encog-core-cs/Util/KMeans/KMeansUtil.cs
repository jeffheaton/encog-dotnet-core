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
using System.Collections;
using System.Collections.Generic;

namespace Encog.Util.KMeans
{
    /// <summary>
    /// Generic KMeans clustering object.
    /// </summary>
    /// <typeparam name="TK">The type to cluster</typeparam>
    public class KMeansUtil<TK> where TK : class
    {
        /// <summary>
        /// The clusters.
        /// </summary>
        private readonly IList<Cluster<TK>> _clusters;

        /// <summary>
        /// The number of clusters.
        /// </summary>
        private readonly int _k;

        /// <summary>
        /// Construct the clusters.  Call process to perform the cluster.
        /// </summary>
        /// <param name="theK">The number of clusters.</param>
        /// <param name="theElements">The elements to cluster.</param>
        public KMeansUtil(int theK, IList theElements)
        {
            _k = theK;
            _clusters = new List<Cluster<TK>>(theK);
            InitRandomClusters(theElements);
        }

        /// <summary>
        /// The number of clusters.
        /// </summary>
        public int Count
        {
            get { return _clusters.Count; }
        }

        /// <summary>
        /// Create random clusters. 
        /// </summary>
        /// <param name="elements">The elements to cluster.</param>
        private void InitRandomClusters(IList elements)
        {
            int clusterIndex = 0;
            int elementIndex = 0;

            // first simply fill out the clusters, until we run out of clusters
            while ((elementIndex < elements.Count) && (clusterIndex < _k)
                   && (elements.Count - elementIndex > _k - clusterIndex))
            {
                var element = elements[elementIndex];

                bool added = false;

                // if this element is identical to another, add it to this cluster
                for (int i = 0; i < clusterIndex; i++)
                {
                    Cluster<TK> cluster = _clusters[i];

                    if (cluster.Centroid().Distance((TK) element) == 0)
                    {
                        cluster.Add(element as TK);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    _clusters.Add(new Cluster<TK>(elements[elementIndex] as TK));
                    clusterIndex++;
                }
                elementIndex++;
            }

            // create
            while (clusterIndex < _k && elementIndex < elements.Count)
            {
                _clusters.Add(new Cluster<TK>(elements[elementIndex] as TK));
                elementIndex++;
                clusterIndex++;
            }

            // handle case where there were not enough clusters created, 
            // create empty ones.
            while (clusterIndex < _k)
            {
                _clusters.Add(new Cluster<TK>());
                clusterIndex++;
            }

            // otherwise, handle case where there were still unassigned elements
            // add them to the nearest clusters.
            while (elementIndex < elements.Count)
            {
                var element = elements[elementIndex];
                NearestCluster(element as TK).Add(element as TK);
                elementIndex++;
            }
        }

        /// <summary>
        /// Perform the cluster.
        /// </summary>
        public void Process()
        {
            bool done;
            do
            {
                done = true;

                for (int i = 0; i < _k; i++)
                {
                    Cluster<TK> thisCluster = _clusters[i];
                    var thisElements = thisCluster.Contents as List<TK>;

                    for (int j = 0; j < thisElements.Count; j++)
                    {
                        TK thisElement = thisElements[j];

                        // don't make a cluster empty
                        if (thisCluster.Centroid().Distance(thisElement) > 0)
                        {
                            Cluster<TK> nearestCluster = NearestCluster(thisElement);

                            // move to nearer cluster
                            if (thisCluster != nearestCluster)
                            {
                                nearestCluster.Add(thisElement);
                                thisCluster.Remove(j);
                                done = false;
                            }
                        }
                    }
                }
            } while (!done);
        }

        /// <summary>
        /// Find the nearest cluster to the element. 
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The nearest cluster.</returns>
        private Cluster<TK> NearestCluster(TK element)
        {
            double distance = Double.PositiveInfinity;
            Cluster<TK> result = null;

            foreach (Cluster<TK> t in _clusters)
            {
                double thisDistance = t.Centroid().Distance(element);

                if (distance > thisDistance)
                {
                    distance = thisDistance;
                    result = t;
                }
            }

            return result;
        }

        /// <summary>
        /// Get a cluster by index.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The cluster.</returns>
        public ICollection<TK> Get(int index)
        {
            return _clusters[index].Contents;
        }

        /// <summary>
        /// Get a cluster by index.
        /// </summary>
        /// <param name="i">The index to get.</param>
        /// <returns>The cluster.</returns>
        public Cluster<TK> GetCluster(int i)
        {
            return _clusters[i];
        }
    }
}
