using WpfSudoku.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Zenseless.Spatial;
using System.Diagnostics;

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

		public static void PrintGrid(IReadOnlyGrid<int> grid)
		{
			for (int row = 0; row < grid.Rows; ++row)
			{
				Debug.Write('|');
				for (int column = 0; column < grid.Columns; ++column)
				{
					Debug.Write(grid[column, row]);
					Debug.Write('|');
				}
				Debug.WriteLine("");
			}
		}

		[TestMethod()]
		public void ValidValuesTest()
		{
			for (int i = 0; i < createTestCount; ++i)
			{
				var grid = Sudoku.Create();
				Sudoku.RemoveSome(grid, 0.5);
				//find empty cell
				for (int cell = 0; cell < grid.Cells.Length; ++cell)
				{
					var (column, row) = grid.GetColRow(cell);
					var validValues = ValidityChecks.ValidValues(grid, column, row);
					if (0 == grid.Cells[cell])
					{
						foreach (var n in validValues)
						{
							grid.Cells[cell] = n; // fill this cell
							Assert.IsTrue(ValidityChecks.All(grid), "Created field not valid");
						}
					}
					else
					{
						Assert.AreEqual(0, validValues.Count);
					}
				}
			}
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
				Assert.IsFalse(field.Cells.Any(value => value == 0), "Created field has 0 cells");
			}
		}

		private const int solveTestCount = 100;
		[DataTestMethod()]
		[Timeout(500 * solveTestCount)]
		[DataRow(0.0)]
		[DataRow(1.0)]
		[DataRow(0.1)]
		[DataRow(0.2)]
		[DataRow(0.5)]
		[DataRow(0.8)]
		[DataRow(0.9)]
		public void SolveBackTrackTest(double emptyFieldPropability)
		{
			for (int i = 0; i < solveTestCount; ++i)
			{
				var grid = Sudoku.Create();
				Sudoku.RemoveSome(grid, emptyFieldPropability);
				Sudoku.SolveBacktrack(grid);
				PrintGrid(grid);
				Assert.IsTrue(ValidityChecks.All(grid), "Solved field not valid.");
				Assert.IsFalse(grid.Cells.Any(value => value == 0), "Solved field has 0 cells.");
			}
		}

		[DataTestMethod()]
		[Timeout(500 * solveTestCount)]
		[DataRow(0.1)]
		[DataRow(0.0)]
		[DataRow(1.0)]
		[DataRow(0.2)]
		[DataRow(0.5)]
		[DataRow(0.8)]
		[DataRow(0.9)]
		public void SolveBestFirstTest(double emptyFieldPropability)
		{
			for (int i = 0; i < solveTestCount; ++i)
			{
				var grid = Sudoku.Create();
				Sudoku.RemoveSome(grid, emptyFieldPropability);
				PrintGrid(grid);
				Sudoku.SolveBestFirst(grid);
				PrintGrid(grid);
				Assert.IsTrue(ValidityChecks.All(grid), "Solved field not valid.");
				Assert.IsFalse(grid.Cells.Any(value => value == 0), "Solved field has 0 cells.");
			}
		}
	}
}