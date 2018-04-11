using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
	static public class EveOnline
	{
		/// <summary>
		/// returns the Root of the UITree.
		/// </summary>
		/// <param name="MemoryReader"></param>
		/// <returns></returns>
		static public UITreeNode UIRoot(
			IPythonMemoryReader MemoryReader)
		{
			var CandidateAddresses = PyTypeObject.EnumeratePossibleAddressesOfInstancesOfPythonTypeFilteredByObType(MemoryReader, "UIRoot");

			var CandidatesWithChildrenCount = new List<KeyValuePair<UITreeNode, int>>();

			foreach (var CandidateAddress in CandidateAddresses)
			{
				var Candidate = new UITreeNode(CandidateAddress, MemoryReader);

				int CandidateChildrenCount = 0;

				{
					var CandidateChildren = Candidate.EnumerateChildrenTransitive(MemoryReader);

					if (null != CandidateChildren)
					{
						CandidateChildrenCount = CandidateChildren.Count();
					}
				}

				CandidatesWithChildrenCount.Add(new KeyValuePair<UITreeNode, int>(Candidate, CandidateChildrenCount));
			}

			//	return the candidate the most children nodes where found in.
			return
				CandidatesWithChildrenCount
				.OrderByDescending((CandidateWithChildrenCount) => CandidateWithChildrenCount.Value)
				.FirstOrDefault()
				.Key;
		}

	}
}
