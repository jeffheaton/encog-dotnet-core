using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.RBF;

namespace Encog.MathUtil.RBF
{
    public abstract class BasicRBF : IRadialBasisFunction
    {
        	/**
	 * The center of the RBF.
	 */
	private double[] center;

	/**
	 * The peak of the RBF.
	 */
	private double peak;

	/**
	 * The width of the RBF.
	 */
	private double width;

	/**
	 * {@inheritDoc}
	 */
	public double GetCenter(int dimension) {
		return this.center[dimension];
	}

	/**
	 * {@inheritDoc}
	 */
	public double[] Centers {
        get
        {
		return this.center;
        }
        set
        {
            this.center = value;
        }
	}

	/**
	 * {@inheritDoc}
	 */
	public int Dimensions {
        get
        {
		return this.center.Length;
        }
	}

	/**
	 * {@inheritDoc}
	 */
	public double Peak {
        get
        {
		return this.peak;
        }
        set
        {
            this.peak = value;
        }
	}

	/**
	 * {@inheritDoc}
	 */
	public double Width {
        get
        {
		return this.width;
        }
        set
        {
            this.width = value;
        }
	}

    

    public abstract double Calculate(double[] x);
    
    }
}
