using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;

namespace RBFBasis
{
	public interface IBasisFunction : IAttribute
	{
		double val(double r);
		/* returns the value of the first derivative of this function with respect to the radius */
		double dr(double r);
		/* returns the value of the second derivative of this function with respect to the radius */
		double ddr(double r);
	}

}
