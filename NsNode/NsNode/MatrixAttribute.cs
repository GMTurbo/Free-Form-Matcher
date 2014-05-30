using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class MatrixNode : NsNode 
	{
		public MatrixNode(NsNode parent, System.Xml.XmlNode xml)
			: base(parent, xml)
		{
			//if (!FromXml(xml))
			//     throw new AttributeXmlFormatException(null, xml, "Failed to read xml");
		}

		public MatrixNode(NsNode parent, string Label, int rows, int cols)
		{
			m_parent = parent;
			Add(new IntAttribute(this, "Rows", rows));
			Add(new IntAttribute(this, "Columns", cols));
			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++)
					Add(new DoubleAttribute(this, "[" + i + "," + j + "]", 0));

		}

		public MatrixNode(NsNode parent, string Label, double[,] vals)
		{
			m_parent = parent;
			Add(new IntAttribute(this, "Rows", vals.GetLength(0)));
			Add(new IntAttribute(this, "Columns", vals.GetLength(1)));
			for (int i = 0; i < vals.GetLength(0); i++)
				for (int j = 0; j < vals.GetLength(1); j++)
					Add(new DoubleAttribute(this, "[" + i + "," + j + "]", vals[i, j]));

		}

		public IAttribute GetMatrixElement(int i, int j)
		{
            //Attributes.Find(delegate(IAttribute atr)
           // {
            //    return atr.Label.Contains("[" + i + "," + j + "]");
            //});
			//foreach (IAttribute atr in Attributes)
			//	if (atr.Label.Contains("[" + i + "," + j + "]"))
			//		return atr;

            return Attributes.Find(delegate(IAttribute atr)
            {
                return atr.Label.Contains("[" + i + "," + j + "]");
            });
		}
		public int Rows
		{
			get
			{
                IAttribute result = Attributes.Find(delegate(IAttribute atr)
                {
                    return atr.Label == "Rows";
                });
				//foreach (IAttribute atr in Attributes)
				//	if (atr.Label == "Rows")
				//		return (int)atr.Value;

                return (int)result.Value;
			}
		}
		public int Cols
		{
			get
			{
                IAttribute result = Attributes.Find(delegate(IAttribute atr)
                {
                    return atr.Label == "Columns";
                });
                //foreach (IAttribute atr in Attributes)
                //    if (atr.Label == "Columns")
                //        return (int)atr.Value;

                return (int)result.Value;
			}
		}
		public bool SetMatrixElement(int i, int j, IAttribute value)
		{
			IAttribute atr = GetMatrixElement(i, j);
			if (atr == null)
				return false;
			atr.Value = value.Value;
			
			return true;
		}
		public bool SetMatrix(double[,] vals)
		{
			if (vals.GetLength(0) * vals.GetLength(1) == Rows * Cols && Rows == vals.GetLength(0) && Cols == vals.GetLength(1))
			{
				for (int i = 0; i < vals.GetLength(0); i++)
					for (int j = 0; j < vals.GetLength(1); j++)
						SetMatrixElement(i, j, new DoubleAttribute(this,"["+i+","+j+"]",vals[i,j]));

				Update();
				return true;
			}
			return false;
		}
		public double[] Matrix
		{
			get
			{
				double[] ret = new double[Rows*Cols];
				for (int i = 0; i < Rows; i++)
					for (int j = 0; j < Cols; j++)
						ret[Rows * i + j] = (double)GetMatrixElement(i, j).Value;

				return ret;
			}
		}
		NsNode m_parent;
	}
}
