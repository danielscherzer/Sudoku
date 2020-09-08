using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace WpfSudoku.Model.Tests
{
	[TestClass()]
	public class SudokuCreatorTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
		public void InfluencedCoordinatesTest(int x, int y, int blockSize, List<(int, int)> expected)
		{
			var actual = SudokuCreator.InfluencedCoordinates(x, y, blockSize);
			CollectionAssert.AreEquivalent(expected.Distinct().ToArray(), actual.Distinct().ToArray());
		}

		public static IEnumerable<object[]> GetData()
		{
			yield return new object[] { 8, 2, 3, new List<(int, int)> { (8, 0), (8, 1),         (8, 3), (8, 4), (8, 5), (8, 6), (8, 7), (8, 8),
																		(0, 2), (1, 2), (2, 2), (3, 2), (4, 2), (5, 2), (6, 2), (7, 2),
																		(6, 0), (7, 0), (8, 0), (6, 1), (7, 1), (8, 1), (6, 2), (7, 2) } };
			yield return new object[] { 0, 0, 2, new List<(int, int)> { (0, 1), (0, 2), (0, 3), 
																		(1, 0), (2, 0), (3, 0), 
																		(1, 1) } };
			yield return new object[] { 1, 1, 2, new List<(int, int)> { (1, 0), (1, 2), (1, 3), 
																		(0, 1), (2, 1), (3, 1), 
																		(0, 0) } };
		}
	}
}