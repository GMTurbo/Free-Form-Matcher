using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using devDept.Eyeshot.Standard;

namespace NsNodeView
{
	public interface IEntityGroup
	{
		Entity[] Entity { get; }
		devDept.Eyeshot.Labels.LabelBase devpDeptLabel { get; }
	}
}
