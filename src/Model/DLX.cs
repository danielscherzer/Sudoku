using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfSudoku.Model
{
	internal class DLX
	{
		private readonly Header root = new(null, null) { Size = int.MaxValue };
		private readonly List<Header> columns;
		private readonly List<Node> rows;
		private readonly Stack<Node> solutionNodes = new();
		private int initial = 0;

		public DLX(int columnCapacity, int rowCapacity)
		{
			columns = new List<Header>(columnCapacity);
			rows = new List<Node>(rowCapacity);
			for (int i = 0; i < columnCapacity; i++) AddHeader();
		}

		private void AddHeader()
		{
			Header h = new(root.Left, root);
			h.AttachLeftRight();
			columns.Add(h);
		}

		public void AddRow(params int[] newRow)
		{
			Node AddFirstNode(int column, int row)
			{
				Node n = new(null, null, columns[column].Up, columns[column], columns[column], row);
				n.AttachUpDown();
				return n;
			}

			void AddNode(int column, Node firstNode)
			{
				Node n = new(firstNode.Left, firstNode, columns[column].Up, columns[column], columns[column], firstNode.Row);
				n.AttachLeftRight();
				n.AttachUpDown();
			}

			var count = newRow.Length;
			if(count > 0)
			{
				var first = AddFirstNode(column:newRow[0], row:rows.Count);
				rows.Add(first);
				for (int i = 1; i < count; ++i)
				{
					AddNode(newRow[i], first);
				}
			}
		}

		public void Give(int row)
		{
			solutionNodes.Push(rows[row]);
			CoverMatrix(rows[row]);
			initial++;
		}

		public IEnumerable<int[]> Solutions()
		{
			try
			{
				Node node = ChooseSmallestColumn().Down;
				do
				{
					if (node == node.Head)
					{
						if (node == root)
						{
							yield return solutionNodes.Select(n => n.Row).ToArray();
						}
						if (solutionNodes.Count > initial)
						{
							node = solutionNodes.Pop();
							UncoverMatrix(node);
							node = node.Down;
						}
					}
					else
					{
						solutionNodes.Push(node);
						CoverMatrix(node);
						node = ChooseSmallestColumn().Down;
					}
				} while (solutionNodes.Count > initial || node != node.Head);
			}
			finally
			{
				void Restore()
				{
					while (solutionNodes.Count > 0) UncoverMatrix(solutionNodes.Pop());
					initial = 0;
				}
				Restore();
			}
		}

		private Header ChooseSmallestColumn()
		{
			Header traveller = root, choice = root;
			do
			{
				traveller = (Header)traveller.Right;
				if (traveller.Size < choice.Size) choice = traveller;
			} while (traveller != root && choice.Size > 0);
			return choice;
		}

		private static void CoverMatrix(Node node)
		{
			static void CoverRow(Node row)
			{
				for (Node traveller = row.Right; traveller != row; traveller = traveller.Right)
				{
					traveller.DetachUpDown();
				}
			}

			static void CoverColumn(Header column)
			{
				column.DetachLeftRight();
				for (Node traveller = column.Down; traveller != column; traveller = traveller.Down)
				{
					CoverRow(traveller);
				}
			}
			Node traveller = node;
			do
			{
				CoverColumn(traveller.Head);
				traveller = traveller.Right;
			} while (traveller != node);
		}

		private static void UncoverMatrix(Node node)
		{
			static void UncoverRow(Node row)
			{
				for (Node traveller = row.Left; traveller != row; traveller = traveller.Left)
				{
					traveller.AttachUpDown();
				}
			}

			static void UncoverColumn(Header column)
			{
				Node traveller = column.Up;
				while (traveller != column)
				{
					UncoverRow(traveller);
					traveller = traveller.Up;
				}
				column.AttachLeftRight();
			}
			Node traveller = node;
			do
			{
				traveller = traveller.Left;
				UncoverColumn(traveller.Head);
			} while (traveller != node);
		}

		private class Node
		{
			public Node(Node? left, Node? right, Node? up, Node? down, Header? head, int row)
			{
				Left = left ?? this;
				Right = right ?? this;
				Up = up ?? this;
				Down = down ?? this;
				Head = head ?? this as Header ?? throw new ApplicationException("Node instance is not a header");
				Row = row;
			}

			public Node Left { get; private set; }
			public Node Right { get; private set; }
			public Node Up { get; private set; }
			public Node Down { get; private set; }
			public Header Head { get; }
			
			public int Row { get; }

			public void AttachLeftRight()
			{
				Left.Right = this;
				Right.Left = this;
			}

			public void AttachUpDown()
			{
				Up.Down = this;
				Down.Up = this;
				Head.Size++;
			}

			public void DetachLeftRight()
			{
				Left.Right = Right;
				Right.Left = Left;
			}

			public void DetachUpDown()
			{
				Up.Down = Down;
				Down.Up = Up;
				--Head.Size;
			}
		}

		private class Header : Node
		{
			public Header(Node? left, Node? right) : base(left, right, null, null, null, -1) { }

			public int Size { get; set; }
		}
	}
}
