using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace WpfSudoku.Model.Tests
{
	[TestClass()]
	public class SudokuTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
		public void InfluencedCoordinatesTest(int x, int y, int blockSize, List<(int, int)> expected)
		{
			var actual = Sudoku.InterdependentFields(x, y, blockSize);
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

		private const int createTestCount = 500;
		[TestMethod()]
		[Timeout(100 * createTestCount)]
		public void CreateTest()
		{
			for (int i = 0; i < createTestCount; ++i)
			{
				var field = Sudoku.Create();
				Assert.IsTrue(ValidityChecks.All(field), "Created field not valid");
				Assert.IsFalse(field.Array.Any(value => value == 0), "Created field has 0 cells");
			}
		}

		private const int solveTestCount = 5;
		[DataTestMethod()]
		[Timeout(5000 * solveTestCount)]
		[DataRow(0.0)]
		[DataRow(1.0)]
		[DataRow(0.1)]
		[DataRow(0.2)]
		[DataRow(0.5)]
		[DataRow(0.8)]
		[DataRow(0.9)]
		public void SolveTest(double emptyFieldPropability)
		{
			for (int i = 0; i < solveTestCount; ++i)
			{
				var field = Sudoku.Create();
				Sudoku.RemoveSome(field, emptyFieldPropability);
				Sudoku.Solve(field);
				Assert.IsTrue(ValidityChecks.All(field), "Solved field not valid.");
				Assert.IsFalse(field.Array.Any(value => value == 0), "Solved field has 0 cells.");
			}
		}
	}
}