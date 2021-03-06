using System.Collections.Generic;
using System.Drawing;
using BaseLib.Param;
using BaseLibS.Num;
using BaseLibS.Param;
using BaseLibS.Util;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Basic{
	public class AddNoise : IMatrixProcessing{
		public bool HasButton{
			get { return false; }
		}

		public Bitmap DisplayImage{
			get { return null; }
		}

		public string Description{
			get { return "Modulate the data with Gaussian noise."; }
		}

		public string HelpOutput{
			get { return "Same as input matrix with random noise added to the expression columns."; }
		}

		public string[] HelpSupplTables{
			get { return new string[0]; }
		}

		public int NumSupplTables{
			get { return 0; }
		}

		public string Name{
			get { return "Add noise"; }
		}

		public string Heading{
			get { return "Basic"; }
		}

		public bool IsActive{
			get { return true; }
		}

		public float DisplayRank{
			get { return 200; }
		}

		public string[] HelpDocuments{
			get { return new string[0]; }
		}

		public int NumDocuments{
			get { return 0; }
		}

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public string Url{
			get { return "http://141.61.102.17/perseus_doku/doku.php?id=perseus:activities:MatrixProcessing:Basic:AddNoise"; }
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
		                        ref IDocumentData[] documents, ProcessInfo processInfo){
			Random2 rand = new Random2();
			double std = param.GetParam<double>("Standard deviation").Value;
			int[] inds = param.GetParam<int[]>("Columns").Value;
			List<int> mainInds = new List<int>();
			List<int> numInds = new List<int>();
			foreach (var ind in inds){
				if (ind < mdata.ColumnCount){
					mainInds.Add(ind);
				} else{
					numInds.Add(ind - mdata.ColumnCount);
				}
			}
			foreach (int j in mainInds){
				for (int i = 0; i < mdata.RowCount; i++){
					mdata.Values[i, j] += (float) rand.NextGaussian(0, std);
				}
			}
			foreach (int j in numInds){
				for (int i = 0; i < mdata.RowCount; i++){
					mdata.NumericColumns[j][i] += (float) rand.NextGaussian(0, std);
				}
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new Parameter[]{
					new DoubleParam("Standard deviation", 0.1){Help = "Standard deviation of the noise distribution."},
					new MultiChoiceParam("Columns", ArrayUtils.ConsecutiveInts(mdata.ColumnCount)){
						Values = ArrayUtils.Concat(mdata.ColumnNames, mdata.NumericColumnNames)
					}
				});
		}
	}
}