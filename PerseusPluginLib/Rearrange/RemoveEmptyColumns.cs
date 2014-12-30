﻿using System.Collections.Generic;
using System.Drawing;
using BaseLibS.Param;
using BaseLibS.Util;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;
using PerseusPluginLib.Utils;

namespace PerseusPluginLib.Rearrange{
	public class RemoveEmptyColumns : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Bitmap DisplayImage { get { return null; } }
		public string HelpOutput { get { return "Same matrix but with empty columns removed."; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Heading { get { return "Rearrange"; } }
		public string Name { get { return "Remove empty columns"; } }
		public bool IsActive { get { return true; } }
		public float DisplayRank { get { return 3.5f; } }
		public string Description { get { return "Columns containing no values or only invalid values will be removed."; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public int NumDocuments { get { return 0; } }
		public string Url { get { return "http://141.61.102.17/perseus_doku/doku.php?id=perseus:activities:MatrixProcessing:Rearrange:RemoveEmptyColumns"; } }

		public int GetMaxThreads(Parameters parameters) {
			return 1;
		}

		public void ProcessData(IMatrixData data, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			int[] exColInds = GetValidExCols(data);
			int[] numColInds = GetValidNumCols(data);
			int[] multiNumColInds = GetValidMultiNumCols(data);
			int[] catColInds = GetValidCatCols(data);
			int[] textColInds = GetValidTextCols(data);
			if (exColInds.Length < data.ColumnCount){
				data.ExtractColumns(exColInds);
			}
			if (numColInds.Length < data.NumericColumnCount){
				data.NumericColumns = ArrayUtils.SubList(data.NumericColumns, numColInds);
				data.NumericColumnNames = ArrayUtils.SubList(data.NumericColumnNames, numColInds);
				data.NumericColumnDescriptions = ArrayUtils.SubList(data.NumericColumnDescriptions, numColInds);
			}
			if (multiNumColInds.Length < data.MultiNumericColumnCount){
				data.MultiNumericColumns = ArrayUtils.SubList(data.MultiNumericColumns, multiNumColInds);
				data.MultiNumericColumnNames = ArrayUtils.SubList(data.MultiNumericColumnNames, multiNumColInds);
				data.MultiNumericColumnDescriptions = ArrayUtils.SubList(data.MultiNumericColumnDescriptions, multiNumColInds);
			}
			if (catColInds.Length < data.CategoryColumnCount){
				data.CategoryColumns = PerseusPluginUtils.GetCategoryColumns(data, catColInds);
				data.CategoryColumnNames = ArrayUtils.SubList(data.CategoryColumnNames, catColInds);
				data.CategoryColumnDescriptions = ArrayUtils.SubList(data.CategoryColumnDescriptions, catColInds);
			}
			if (textColInds.Length < data.StringColumnCount){
				data.StringColumns = ArrayUtils.SubList(data.StringColumns, textColInds);
				data.StringColumnNames = ArrayUtils.SubList(data.StringColumnNames, textColInds);
				data.StringColumnDescriptions = ArrayUtils.SubList(data.StringColumnDescriptions, textColInds);
			}
		}

		private static int[] GetValidTextCols(IMatrixData data){
			List<int> valids = new List<int>();
			for (int i = 0; i < data.StringColumnCount; i++){
				if (!IsInvalidStringColumn(data.StringColumns[i])){
					valids.Add(i);
				}
			}
			return valids.ToArray();
		}

		private static int[] GetValidCatCols(IMatrixData data){
			List<int> valids = new List<int>();
			for (int i = 0; i < data.CategoryColumnCount; i++){
				if (!IsInvalidCatColumn(data.GetCategoryColumnAt(i))){
					valids.Add(i);
				}
			}
			return valids.ToArray();
		}

		private static int[] GetValidMultiNumCols(IMatrixData data){
			List<int> valids = new List<int>();
			for (int i = 0; i < data.MultiNumericColumnCount; i++){
				if (!IsInvalidMultiNumColumn(data.MultiNumericColumns[i])){
					valids.Add(i);
				}
			}
			return valids.ToArray();
		}

		private static int[] GetValidNumCols(IMatrixData data){
			List<int> valids = new List<int>();
			for (int i = 0; i < data.NumericColumnCount; i++){
				if (!IsInvalidNumColumn(data.NumericColumns[i])){
					valids.Add(i);
				}
			}
			return valids.ToArray();
		}

		private static int[] GetValidExCols(IMatrixData data){
			List<int> valids = new List<int>();
			for (int i = 0; i < data.ColumnCount; i++){
				if (!IsInvalidExColumn(data.GetColumn(i))){
					valids.Add(i);
				}
			}
			return valids.ToArray();
		}

		private static bool IsInvalidStringColumn(IEnumerable<string> stringColumn){
			foreach (string s in stringColumn){
				if (!string.IsNullOrEmpty(s)){
					return false;
				}
			}
			return true;
		}

		private static bool IsInvalidCatColumn(IEnumerable<string[]> categoryColumn){
			foreach (string[] s in categoryColumn){
				if (s != null && s.Length > 0){
					return false;
				}
			}
			return true;
		}

		private static bool IsInvalidMultiNumColumn(IEnumerable<double[]> multiNumericColumn){
			foreach (double[] s in multiNumericColumn){
				if (s != null && s.Length > 0){
					return false;
				}
			}
			return true;
		}

		private static bool IsInvalidNumColumn(IEnumerable<double> numericColumn){
			foreach (double d in numericColumn){
				if (!double.IsNaN(d) && !double.IsInfinity(d)){
					return false;
				}
			}
			return true;
		}

		private static bool IsInvalidExColumn(IEnumerable<float> expressionColumn){
			foreach (float d in expressionColumn){
				if (!float.IsNaN(d) && !float.IsInfinity(d)){
					return false;
				}
			}
			return true;
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString) {
			return new Parameters();
		}
	}
}