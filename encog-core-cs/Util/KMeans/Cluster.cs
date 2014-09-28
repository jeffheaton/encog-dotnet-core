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
using System.Collections.Generic;

namespace Encog.Util.KMeans
{
    /// <summary>
    /// A cluster.
    /// </summary>
    /// <typeparam name="T">The type of data to cluster.</typeparam>
    public class Cluster<T>
    {
        /// <summary>
        /// The contents of the cluster.
        /// </summary>
        private readonly IList<T> _contents = new List<T>();

        /// <summary>
        /// The centroid of this cluster.
        /// </summary>
        private ICentroid<T> _centroid;

        /// <summary>
        /// Create an empty cluster.
        /// </summary>
        public Cluster()
        {
        }

        /// <summary>
        /// Create a cluster with one initial data point. 
        /// </summary>
        /// <param name="d">The initial data point.</param>
        public Cluster(T d)
        {
            _contents.Add(d);
            _centroid = ((ICentroidFactory<T>) d).CreateCentroid();
        }

        /// <summary>
        /// The contents of this cluster.
        /// </summary>
        public IList<T> Contents
        {
            get { return _contents as List<T>; }
        }

        /// <summary>
        /// Add a element to the cluster.
        /// </summary>
        /// <param name="e">The element to add.</param>
        public void Add(T e)
        {
            if (_centroid == null)
                _centroid = ((ICentroidFactory<T>) e) as ICentroid<T>;
            else
                _centroid.Add(e);

            _contents.Add(e);
        }

        /// <summary>
        /// Remove the specified index from the cluster. 
        /// </summary>
        /// <param name="i">The index to remove.</param>
        public void Remove(int i)
        {
            _centroid.Remove(_contents[i]);
            _contents.RemoveAt(i);
        }

        /// <summary>
        /// The centroid of this cluster.
        /// </summary>
        /// <returns>The centroid.</returns>
        public ICentroid<T> Centroid()
        {
            return _centroid;
        }
    }
}
