using System.Collections.Generic;
using System.Drawing;
using BaseLib.Param;
using BaseLibS.Param;
using BaseLibS.Util;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Rearrange{
	public delegate double Conversion(double[] x);

	public class ConvertMultiNumeric : IMatrixProcessing{
		private static readonly string[] names = new[]{"Count", "Sum", "Product", "Average", "Median"};
		private static readonly Conversion[] operations = new Conversion[]{
			x => x.Length, x => ArrayUtils.Sum(x), x => ArrayUtils.Product(x), x => ArrayUtils.Mean(x), x => ArrayUtils.Median(x)
		};
		public bool HasButton { get { return false; } }
		public Bitmap DisplayImage { get { return null; } }
		public string Description{
			get{
				return
					"Creates for the specified multi-numeric columns a numeric column containing the result of the specified operation " +
						"applied to the items in each cell of each selected multi-numeric column.";
			}
		}
		public string Name { get { return "Convert multi-numeric column"; } }
		public string Heading { get { return "Rearrange"; } }
		public bool IsActive { get { return true; } }
		public float DisplayRank { get { return 17; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string HelpOutput { get { return "If n multi-numeric columns are selected, n numeric columns will be added to the matrix."; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public int NumDocuments { get { return 0; } }
		public string Url { get { return "http://141.61.102.17/perseus_doku/doku.php?id=perseus:activities:MatrixProcessing:Rearrange:ConvertMultiNumeric"; } }

		public int GetMaxThreads(Parameters parameters) {
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param1, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
				int[] cols = param1.GetParam<int[]>("Columns").Value;
			int[] ops = param1.GetParam<int[]>("Operation").Value;
			foreach (int t in ops){
				double[][] vals = new double[cols.Length][];
				for (int i = 0; i < cols.Length; i++){
					double[][] x = mdata.MultiNumericColumns[cols[i]];
					vals[i] = new double[x.Length];
					for (int j = 0; j < vals[i].Length; j++){
						vals[i][j] = operations[t](x[j]);
					}
				}
				for (int i = 0; i < cols.Length; i++){
					mdata.AddNumericColumn(mdata.MultiNumericColumnNames[cols[i]] + "_" + names[t], "", vals[i]);
				}
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString) {
			IList<string> values = mdata.MultiNumericColumnNames;
			int[] sel = ArrayUtils.ConsecutiveInts(values.Count);
			return
				new Parameters(new Parameter[]{
					new MultiChoiceParam("Operation"){
						Values = names,
						Help = "How should the numbers in a cell of the multi-numeric columns be transformed to a single number?"
					},
					new MultiChoiceParam("Columns"){
						Values = values, Value = sel,
						Help = "Select here the multi-numeric colums that should be converted to numeric columns."
					}
				});
		}
	}
}