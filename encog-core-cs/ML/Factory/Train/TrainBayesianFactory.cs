using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Training;
using Encog.ML.Bayesian.Training.Estimator;
using Encog.ML.Bayesian.Training.Search;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Util;
using Encog.ML.Bayesian.Training.Search.k2;

namespace Encog.ML.Factory.Train
{
    public class TrainBayesianFactory
    {
        	/**
	 * Create a K2 trainer.
	 * 
	 * @param method
	 *            The method to use.
	 * @param training
	 *            The training data to use.
	 * @param argsStr
	 *            The arguments to use.
	 * @return The newly created trainer.
	 */
	public IMLTrain Create(IMLMethod method,
			IMLDataSet training, String argsStr) {
		IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
	    ParamsHolder holder = new ParamsHolder(args);

		int maxParents = holder.GetInt(
				MLTrainFactory.PropertyMaxParents, false, 1);
		String searchStr = holder.GetString("SEARCH", false, "k2");
		String estimatorStr = holder.GetString("ESTIMATOR", false, "simple");
		String initStr = holder.GetString("INIT", false, "naive");
		
		IBayesSearch search;
		IBayesEstimator estimator;
		BayesianInit init;
		
		if( string.Compare(searchStr,"k2",true)==0) {
			search = new SearchK2();
		} else if( string.Compare(searchStr,"none",true)==0) {
			search = new SearchNone();
		}
		else {
			throw new BayesianError("Invalid search type: " + searchStr);
		}
		
		if( string.Compare(estimatorStr,"simple",true)==0) {
			estimator = new SimpleEstimator();
		} else if( string.Compare(estimatorStr, "none",true)==0) {
			estimator = new EstimatorNone();
		}
		else {
			throw new BayesianError("Invalid estimator type: " + estimatorStr);
		}
		
		if( string.Compare(initStr, "simple") ==0) {
			init = BayesianInit.InitEmpty;
		} else if( string.Compare(initStr, "naive") ==0) {
			init = BayesianInit.InitNaiveBayes;
		} else if( string.Compare(initStr, "none") ==0) {
			init = BayesianInit.InitNoChange;
		}
		else {
			throw new BayesianError("Invalid init type: " + initStr);
		}
		
		return new TrainBayesian((BayesianNetwork) method, training, maxParents, init, search, estimator);
	}
    }
}
