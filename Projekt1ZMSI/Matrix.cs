using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMU.Math
{
    public class Matrix
    {

        #region Declarations

        protected double[] _matrix = null;
        private int _rowCount = 0;
        private int _colCount = 0;
        private List<double> _matrixList = null;
        private Random _rng = new Random();

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the matrix based on the specified array. The matrix created 
        /// will contain a single row.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="columnMatrix"></param>
        public Matrix(double[] array)
        {
            int rows = 1;
            int cols = array.Length;

            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            _matrix = array;

        }
        /// <summary>
        /// Initialises the matrix based on the specified array. The matrix created 
        /// will contain a single row.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="columnMatrix"></param>
        public Matrix(bool[] array)
        {
            int rows = 1;
            int cols = array.Length;

            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            _matrix = ConvertToDoubleBool(array);

        }
        /// <summary>
        /// Initialises the matrix based on the specified array. If the columnMatrix 
        /// parameter is set to false, the matrix created will contain a single row. If the
        /// coulmnMatrix parameter is set to true, the matrix created will contain a single column.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="columnMatrix"></param>
        public Matrix(double[] array, bool columnMatrix)
        {
            int cols = 1;
            int rows = 1;

            if (columnMatrix)
            {
                rows = array.Length;
                if (rows < 1)
                    throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            }
            else
            {
                cols = array.Length;
                if (cols < 1)
                    throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);
            }


            //set the properties
            _rowCount = rows;
            _colCount = cols;

            _matrix = array;
        }
        /// <summary>
        /// Initialises the matrix based on the specified array. If the columnMatrix 
        /// parameter is set to false, the matrix created will contain a single row. If the
        /// coulmnMatrix parameter is set to true, the matrix created will contain a single column.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="columnMatrix"></param>
        public Matrix(bool[] array, bool columnMatrix)
        {
            int cols = 1;
            int rows = 1;

            if (columnMatrix)
            {
                rows = array.Length;
                if (rows < 1)
                    throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            }
            else
            {
                cols = array.Length;
                if (cols < 1)
                    throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);
            }


            //set the properties
            _rowCount = rows;
            _colCount = cols;

            _matrix = ConvertToDoubleBool(array);
        }
        /// <summary>
        /// Initialises the matrix based on the specified array. Dimension 0 of the array will 
        /// be represented as Rows and dimension 1 of the array will be represented as Columns.
        /// </summary>
        /// <param name="array"></param>
        public Matrix(double[,] array)
       {
           int rows = array.GetLength(0);
           int cols = array.GetLength(1);

           if (rows < 1)
               throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
           if (cols < 1)
               throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

           //set the properties
           _rowCount = rows;
           _colCount = cols;

           //populate the matrix (cant use array copy as the arrays are different dimensions)
           _matrix = new double[rows * cols];
           for (int row = 0; row < rows; row++)
           {
               for (int col = 0; col < cols; col++)
               {
                   _matrix[row * cols + col] = array[row, col];
               }
           }
        }
        /// <summary>
        /// Initialises the matrix based on the specified array. Dimension 0 of the array will 
        /// be represented as Rows and dimension 1 of the array will be represented as Columns.
        /// </summary>
        /// <param name="array"></param>
        public Matrix(bool[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            if (rows < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            //populate the matrix (cant use array copy as the arrays are different dimensions)
            _matrix = new double[rows * cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if(array[row, col])
                        _matrix[row * cols + col] = 1.0;
                    else
                        _matrix[row * cols + col] = 0;
                }
            }
        }
        /// <summary>
        /// Initialises the matrix based on the specified array, column and row parameters.
        /// </summary>
        /// <param name="array"></param>
        public Matrix(double[] array, int rows, int cols)
        {

            if (rows < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            _matrix = new double[rows * cols];

            if (_matrix.Length == array.Length)
            {
                _matrix = array;
            }
            else
            {
                throw new ArgumentException(Resources.ErrorMessages.ArgumentVectorNotValid);
            }

        }

        /// <summary>
        /// Initialises the matrix to the specified size. All elements are set to zero.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Matrix(int rows, int cols)
        {
            if (rows < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            //store the whole thing in a single array
            //one row at a time for example the matrix
            //
            // 1 2 3
            // 6 7 8
            //
            //would be stored as follows
            //
            // 1 2 3 6 7 8

            _matrix = new double[rows * cols];

            for (int index = 0; index < _matrix.Length; index++)
            {
                _matrix[index] = 0.0;
            }

        }
        /// <summary>
        /// Initialises the matrix to the specified size, setting all cells to the 
        /// specified value.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="addRandomValues"></param>
        public Matrix(int rows, int cols, double value)
        {
            if (rows < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            //store the whole thing in a single array
            //one row at a time for example the matrix
            //
            // 1 2 3
            // 6 7 8
            //
            //would be stored as follows
            //
            // 1 2 3 6 7 8

            _matrix = new double[rows * cols];


            for (int index = 0; index < _matrix.Length; index++)
            {
                _matrix[index] = value;
            }


        }
        /// <summary>
        /// Initialises the matrix to the specified size. If the addRandomValues parameter 
        /// is set to true, all elements are set to a random value between 0 and 1.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="addRandomValues"></param>
        public Matrix(int rows, int cols, bool addRandomValues)
        {
            if (rows < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentRowLength);
            if (cols < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentColumnLength);

            //set the properties
            _rowCount = rows;
            _colCount = cols;

            //store the whole thing in a single array
            //one row at a time for example the matrix
            //
            // 1 2 3
            // 6 7 8
            //
            //would be stored as follows
            //
            // 1 2 3 6 7 8

            _matrix = new double[rows * cols];

            if (addRandomValues)
            {
                for (int index = 0; index < _matrix.Length; index++)
                {
                    _matrix[index] = _rng.NextDouble();
                }
            }
            else
            {
                for (int index = 0; index < _matrix.Length; index++)
                {
                    _matrix[index] = 0.0;
                }
            }

        }
        /// <summary>
        /// Create a square diagonal matrix. Cells are initialised to zero with the exception of those 
        /// on the diagonal (Top Left to Bottom Right), which are set to the value specified.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="unitMatrix"></param>
        public Matrix(int diagonalMatrixSize, double diagonalValue)
        {

            if (diagonalMatrixSize < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentSizeZero);

            _matrix = new double[diagonalMatrixSize * diagonalMatrixSize];

            _rowCount = diagonalMatrixSize;
            _colCount = diagonalMatrixSize;

            int row = -1;

            for (int index = 0; index < _matrix.Length; index++)
            {

                if (index % this.ColumnCount == 0) row++;

                if (index == (row * this.ColumnCount) + row)
                    _matrix[index] = diagonalValue;
                else
                    _matrix[index] = 0.0;
            }
        }
        /// <summary>
        /// Create a square unit matrix. Cells are initialised to zero with the exception of those 
        /// on the diagonal (Top Left to Bottom Right), which are set to 1.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="unitMatrix"></param>
        public Matrix(int unitMatrixSize)
        {

            if (unitMatrixSize < 1)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentSizeZero);

            _matrix = new double[unitMatrixSize * unitMatrixSize];

            _rowCount = unitMatrixSize;
            _colCount = unitMatrixSize;

            int row = -1;

            for (int index = 0; index < _matrix.Length; index++)
            {

                if (index % this.ColumnCount == 0) row++;

                if (index == (row * this.ColumnCount) + row)
                    _matrix[index] = 1.0;
                else
                    _matrix[index] = 0.0;
            }
        }
    
        #endregion

        #region Properties

        /// <summary>
        /// Returns an integer representing the number of rows in the matrix.
        /// </summary>
        public int RowCount
        {
            get { return _rowCount; }
        }
        /// <summary>
        /// Returns an integer representing the number of columns in the matrix.
        /// </summary>
        public int ColumnCount
        {
            get { return _colCount; }
        }

        /// <summary>
        /// Returns true if the matrix consists of a single row or a single column.
        /// </summary>
        public bool IsVector
        {
            get
            {
                return this.ColumnCount == 1 ||  this.RowCount == 1;
            }
        }
        /// <summary>
        /// Returns true if the matrix consists of a single row.
        /// </summary>
        public bool IsRowVector
        {
            get
            {
                return this.RowCount == 1;
            }
        }
        /// <summary>
        /// Returns true if the matrix consists of a single column.
        /// </summary>
        public bool IsColumnVector
        {
            get
            {
                return this.ColumnCount == 1;
            }
        }
        /// <summary>
        /// Returns the number of cells in the matrix.
        /// </summary>
        public int Length
        {
            get
            {
                return _matrix.Length;
            }
        }
        /// <summary>
        /// returns the total of the values of all elements of the matrix.
        /// </summary>
        /// <returns></returns>
        public double Sum
        {
            get
            {
                double result = 0.0;
                for (int index = 0; index < _matrix.Length; index++)
                {
                    result += _matrix[index];
                }
                return result;
            }
        }
        /// <summary>
        /// Returns the square root of the sum of the squared elements.
        /// </summary>
        public double VectorLength
        {
            get
            {
                if (!this.IsVector)
                    throw new NotSupportedException(Resources.ErrorMessages.ArgumentMatrixIsNotVector);

                double result = 0.0;

                for (int index = 0; index < _matrix.Length; index++)
                {
                    result += System.Math.Pow(_matrix[index], 2);
                }
                return System.Math.Sqrt(result);
            }
        }
        #endregion

        #region Instance Methods

        /// <summary>
        /// Returns a Matrix containing the specified rows.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        public Matrix GetRows(int startRow, int endRow)
        {

            //validate arguments
            if (startRow >= endRow)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentStartRowGreaterThanEndRow);
            if (startRow >= this.RowCount || endRow >= this.RowCount || startRow < 0 || endRow < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidRow);

            //get rows to return and number of columns in a row
            int rowsToReturn = endRow - startRow + 1;

            //create a matrix for the result
            Matrix result = new Matrix(rowsToReturn, this.ColumnCount);

            //populate the result
            Array.Copy(this._matrix, startRow * this.ColumnCount, result._matrix, 0, rowsToReturn * this.ColumnCount);

            //create the Matrix to return
            return result;

        }
        /// <summary>
        /// Returns a RowMatrix containing the specified row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Matrix GetRow(int row)
        {
            //validate arguments
            if (row >= this.RowCount || row < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidRow);

            double[] selectedRow = new double[this.ColumnCount];
            Array.Copy(_matrix, row * this.ColumnCount, selectedRow, 0, this.ColumnCount);

            //get number of elements in each Row (i.e. how many columns it has)
            Matrix result = new Matrix(1, this.ColumnCount);
            result._matrix = selectedRow;

            return result;
        }
        /// <summary>
        /// Returns a Matrix containing the specified columns.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        public Matrix GetColumns(int startColumn, int endColumn)
        {
            //validate arguments
            if (startColumn >= endColumn)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentStartColumnGreaterThanEndRow);
            if (startColumn >= this.ColumnCount || endColumn >= this.ColumnCount || startColumn < 0 || endColumn < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidColumn);

            //get rows to return and number of columns in a row
            int colsToReturn = endColumn - startColumn + 1;

            //create a matrix for the result
            Matrix result = new Matrix(this.RowCount, colsToReturn);

            //populate the result
            for (int row = 0; row < this.RowCount; row++)
            {
                Array.Copy(_matrix, (row * this.ColumnCount) + startColumn, result._matrix, row * colsToReturn, colsToReturn);
            }
            return result;
        }

        /// <summary>
        /// Returns a ColumnMatrix containing the specified coulmn.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Matrix GetColumn(int column)
        {

            //validate arguments
            if (column >= this.ColumnCount || column < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidColumn);

            double[] selectedColumn = new double[this.RowCount];
            for (int index = 0; index < this.RowCount; index++)
            {
                selectedColumn[index] = _matrix[(this.ColumnCount * index) + column];

            }

            //get number of elements in each Row (i.e. how many columns it has)
            Matrix result = new Matrix(this.RowCount, 1);
            result._matrix = selectedColumn;

            return result;

        }
        /// <summary>
        /// Returns the contents of the specified matrix element.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public double GetElement(int row, int column)
        {
            //validate arguments
            if (row >= this.RowCount || row < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidColumn);
            if (column >= this.ColumnCount || column < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidColumn);

            return _matrix[(row * this.ColumnCount) + column];
        }

        /// <summary>
        /// Returns the contents of the specified matrix element.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public void SetElement(int row, int column, double value)
        {
            //validate arguments
            if (row >= this.RowCount || row < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidRow);
            if (column >= this.ColumnCount || column < 0)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentInvalidColumn);

            _matrix[(row * this.ColumnCount) + column] = value;
        }
        /// <summary>
        /// Returns the matrix as a one dimension generic list.
        /// </summary>
        /// <returns></returns>
        public List<double> ToList()
        {
            _matrixList.AddRange(_matrix);
            return _matrixList;
        }
        /// <summary>
        /// Returns the matrix as a one dimension array. Returns Row 1 followed by Row 2 etc.
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return _matrix;
        }

        /// <summary>
        /// Returns a BiPolar copy of the Matrix as a new Matrix. Matrix returned will contain values or only 1 and -1 and will relflect the values of instance of the Matrix as follows:
        /// Values greater than 0 will be represented by -1, values less than or equal to 0 will be represented by values of -1.
        /// </summary>
        /// <returns></returns>
        public Matrix ToBiPolar()
        {
            Matrix result = new Matrix(this.RowCount, this.ColumnCount);
            double[] biPolarMatrix = new double[_matrix.Length];

            for (int index = 0; index < biPolarMatrix.Length; index++)
            {
                if (_matrix[index] > 0)
                    biPolarMatrix[index] = 1;
                else
                    biPolarMatrix[index] = -1;
            }

            result._matrix = biPolarMatrix;

            return result;
        }
        /// <summary>
        /// Returns a Binary copy of the Matrix as a new Matrix. Matrix returned will contain values or only 1 and 0 and will relflect the values of this instance of the Matrix as follows:
        /// Values greater than 0 will be represented by 0, values less than or eqaul to 0 will be represented by values of 0.
        /// </summary>
        /// <returns></returns>
        public Matrix ToBinary()
        {
            Matrix result = new Matrix(this.RowCount, this.ColumnCount);
            double[] binaryMatrix = new double[_matrix.Length];

            for (int index = 0; index < binaryMatrix.Length; index++)
            {
                if (_matrix[index] > 0)
                    binaryMatrix[index] = 1;
                else
                    binaryMatrix[index] = 0;
            }

            result._matrix = binaryMatrix;

            return result;
        }
        /// <summary>
        /// Returns the matrix as a comma delimited string. Uses the default format \"F4\" and the default row seperator CrLf.
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return this.ToString("F4", ",", ";");
        }
        /// <summary>
        /// Returns the matrix with format applied to the numeric values. Uses the default format \"F4\" and the default row seperator CrLf.
        /// </summary>
        /// <returns></returns>
        public string ToString(string format)
        {
            return this.ToString(format, ",", ";");
        }
        /// <summary>
        /// Returns the matrix with format applied to the numeric values and rowDelimiter
        /// </summary>
        /// <returns></returns>
        public string ToString(string format, string columnDelimiter, string rowDelimiter)
        {
            StringBuilder toString = new StringBuilder();
            for (int index = 0; index < _matrix.Length; index++)
            {
                toString.Append(_matrix[index].ToString(format));
                if ((index + 1) % this.ColumnCount == 0)
                {
                    toString.Append(rowDelimiter);
                }
                else
                {
                    toString.Append(columnDelimiter);
                }
            }
            string result = toString.ToString();

            //tidy up spaces at the end, any other character is left in place
            if (result.EndsWith(" "))
                return result.Trim();
            else
                return result;
        }
        /// <summary>
        /// Returns true if the specified matrix is equal in value to this instance.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool Equals(Matrix matrix)
        {
            //compare rows columns and matrix values
            bool result = true;

            if (this.RowCount == matrix.RowCount && this.ColumnCount == matrix.ColumnCount)
            {
                //check lengths
                if (this._matrix.Length == matrix._matrix.Length)
                {
                    //compare values
                    for (int index = 0; index < this._matrix.Length; index++)
                    {
                        if (this._matrix[index] != matrix._matrix[index])
                            result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;

        }
        /// <summary>
        /// Returns a new Matrix that is an exact copy of this instance.
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            Matrix clone = new Matrix(this.RowCount, this.ColumnCount);
            clone._matrix = this._matrix;
            return clone;
        }
        /// <summary>
        /// Sets all elements of the matrix to 0.
        /// </summary>
        public void Clear()
        {
            for (int index = 0; index < _matrix.Length; index++)
            {
                _matrix[index] = 0.0;
            }
        }
        #endregion

        #region Local Methods

        /// <summary>
        /// Converts an array of boolean values to an array of doubles in the range 0 to 1.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private double[] ConvertToDoubleBool(bool[] array)
        {
            double[] result = new double[array.Length];

            for(int index = 0;index < array.Length; index++)
            {
                if (array[index])
                    result[index] = 1.0;
                else
                    result[index] = 0;
            }
            return result;
        }
        /// <summary>
        /// Converts an array of boolean values to an array of doubles in the range -1 to 1.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private double[] ConvertToDoubleBiPolar(bool[] array)
        {
            double[] result = new double[array.Length];

            for (int index = 0; index < array.Length; index++)
            {
                if (array[index])
                    result[index] = 1.0;
                else
                    result[index] = -1.0;
            }
            return result;
        }
        
        #endregion

        #region Static Methods

        /// <summary>
        /// Multiplies the specified matrix by the scalar value.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix MultiplyScalar(Matrix matrix, double scalarValue)
        {

            int index = 0;
            Matrix result = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < matrix.RowCount; row++)
            {

                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    index = (matrix.ColumnCount * row) + col;
                    result._matrix[index] = matrix._matrix[index] * scalarValue;
                }
            }

            return result;
        }
        /// <summary>
        /// Divides the specified matrix by the scalar value.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix DivideScalar(Matrix matrix, double scalarValue)
        {

            int index = 0;
            Matrix result = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < matrix.RowCount; row++)
            {

                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    index = (matrix.ColumnCount * row) + col;
                    result._matrix[index] = matrix._matrix[index] / scalarValue;
                }
            }

            return result;
        }
        /// <summary>
        /// Adds the the scalar value to the specified matrix.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix AddScalar(Matrix matrix, double scalarValue)
        {

            int index = 0;
            Matrix result = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < matrix.RowCount; row++)
            {

                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    index = (matrix.ColumnCount * row) + col;
                    result._matrix[index] = matrix._matrix[index] + scalarValue;
                }
            }

            return result;
        }
        /// <summary>
        /// Subtracts the the scalar value to the specified matrix.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix SubtractScalar(Matrix matrix, double scalarValue)
        {

            int index = 0;
            Matrix result = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < matrix.RowCount; row++)
            {

                for (int col = 0; col < matrix.ColumnCount; col++)
                {
                    index = (matrix.ColumnCount * row) + col;
                    result._matrix[index] = matrix._matrix[index] - scalarValue;
                }
            }

            return result;
        }
        /// <summary>
        /// Adds two matrices and returns the resultant matrix.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix matrix1, Matrix matrix2)
        {

            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatricesIncompatible);

            int index = 0;
            Matrix result = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

            for (int row = 0; row < matrix1.RowCount; row++)
            {

                for (int col = 0; col < matrix1.ColumnCount; col++)
                {
                    index = (matrix1.ColumnCount * row) + col;
                    result._matrix[index] = matrix1._matrix[index] + matrix2._matrix[index];
                }
            }

            return result;
        }
        /// <summary>
        /// Subtracts matrix m2 from matrix m1 and returns the result.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
        {

            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatricesIncompatible);

            int index = 0;
            Matrix result = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

            for (int row = 0; row < matrix1.RowCount; row++)
            {

                for (int col = 0; col < matrix1.ColumnCount; col++)
                {
                    index = (matrix1.ColumnCount * row) + col;
                    result._matrix[index] = matrix1._matrix[index] - matrix2._matrix[index];
                }
            }

            return result;
        }
        /// <summary>
        /// Returns the Dot Product of two vectors. The respective elements of each vector are multiplied together and summed. 
        /// Note that the vectors must be the same length.
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static double DotProduct(Matrix matrix1, Matrix matrix2)
        {
            //both must be a vector
            if(!(matrix1.IsVector && matrix2.IsVector))
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatricesIncompatible);
            if(matrix1.Length != matrix2.Length)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatricesIncompatible);


            double result = 0.0;
            for (int index = 0; index < matrix1.Length; index++)
            {
                result += matrix1._matrix[index] * matrix2._matrix[index];
            }

            return result;
        }

        /// <summary>
        /// Multiplies the two specified matrices. Note that matrix1 must have the same number of columns as matrix2 has rows.
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.ColumnCount != matrix2.RowCount)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatricesIncompatible);

            Matrix result = new Matrix(matrix1.RowCount, matrix2.ColumnCount);

            for (int rowIndex = 0; rowIndex < result.RowCount; rowIndex++)
            {

                for (int colIndex = 0; colIndex < result.ColumnCount; colIndex++)
                {
                    double[] row = matrix1.GetRow(rowIndex).ToArray();
                    double[] col = matrix2.GetColumn(colIndex).ToArray();

                    //loop here to perform the multiplication
                    for (int index = 0; index < row.Length; index++)
                    {
                        result._matrix[(rowIndex * result.ColumnCount) + colIndex] += row[index] * col[index];
                    }
                }

            }

            return result;
        }
        /// <summary>
        /// Method to transpose the matrix. All rows become columns and all columns become rows.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix Transpose(Matrix matrix)
        {
            //create a result matrix that is the reverse of the passed in matrix
            Matrix result = new Matrix(matrix.ColumnCount, matrix.RowCount);

            for (int colIndex = 0; colIndex < matrix.ColumnCount; colIndex++)
            {
                Array.Copy(matrix.GetColumn(colIndex).ToArray(), 0, result._matrix, colIndex * matrix.RowCount, matrix.RowCount);
            }
            return result;
        }
        
        #endregion
        

    }
}
