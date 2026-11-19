using System;
using System.Globalization;
using System.Text;

namespace LabOverloadingAndOperators
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            RunTask1();
            Console.WriteLine(new string('-', 60));
            RunTask2();
            Console.WriteLine(new string('-', 60));
            RunTask3();
            Console.WriteLine(new string('-', 60));
            RunTask4();

        }

        private static void RunTask1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 1: Перевантаження методів та params ===");
            Console.WriteLine($"Peak(15, 42) -> {NumericAggregator.Peak(15, 42)}");
            Console.WriteLine($"Peak(3.5, 9.25, 4.1) -> {NumericAggregator.Peak(3.5, 9.25, 4.1)}");
            Console.WriteLine($"Peak(10, -4, 88, 23, 7) -> {NumericAggregator.Peak(10, -4, 88, 23, 7)}");

            int[] sampleArray = { -12, -3, -25, -1 };
            Console.WriteLine($"Peak(int[]) -> {NumericAggregator.Peak(sampleArray)}");
            Console.WriteLine($"Peak(\"alpha\", \"omega\") -> {NumericAggregator.Peak("alpha", "omega")}");

            Console.WriteLine($"Select((int)10, (double)20) -> {OverloadResolver.Select((int)10, (double)20)}");
            Console.WriteLine($"Select((double)10, (int)20) -> {OverloadResolver.Select((double)10, (int)20)}");
        }
        private static void RunTask2()
        {
            Console.WriteLine("=== ЗАВДАННЯ 2: Двовимірний вектор Vec2 ===");
            Vec2 a = new Vec2(3.0, 4.0);
            Vec2 b = Vec2.FromPolarOrCartesian(1.0, 2.0);

            Console.WriteLine($"a = {a}, |a| = {a.Magnitude:F4}");
            Console.WriteLine($"b = {b}, |b| = {b.Magnitude:F4}");
            Console.WriteLine($"a + b = {a + b}");
            Console.WriteLine($"a - b = {a - b}");
            Console.WriteLine($"a * 2.5 = {a * 2.5}");
            Console.WriteLine($"0.5 * a = {0.5 * a}");
            Console.WriteLine($"a / 2.0 = {a / 2.0}");
            Console.WriteLine($"-a = {-a}");

            Vec2 nearA = new Vec2(3.0000001, 4.0000001);
            Console.WriteLine($"a == nearA: {a == nearA}, Equals: {a.Equals(nearA)}");
            Console.WriteLine($"a != b: {a != b}");
            Console.WriteLine($"a > b: {a > b}, b < a: {b < a}, Zero < a: {Vec2.Zero < a}");

            try { _ = a / 0.0; }
            catch (DivideByZeroException ex) { Console.WriteLine($"Очікуваний виняток: {ex.Message}"); }
            
        }
        private static void RunTask3()
        {
            Console.WriteLine("=== ЗАВДАННЯ 3: Матриця RealMatrix ===");
            var m1 = new RealMatrix(2, 3);
            m1[0, 0] = 1; m1[0, 1] = 2; m1[0, 2] = 3;
            m1[1, 0] = 4; m1[1, 1] = 5; m1[1, 2] = 6;

            var m2 = new RealMatrix(2, 3);
            for (int i = 0; i < m2.Length; i++) m2.SetLinear(i, (i + 1) * 10);

            Console.WriteLine($"Елемент [1, 2]: {m1[1, 2]}, через лінійний індекс 5: {m1.GetLinear(5)}");
            Console.WriteLine("Матриця M1:\n" + m1.ToFormattedString());
            Console.WriteLine("Матриця M2:\n" + m2.ToFormattedString());
            Console.WriteLine("Сума M1 + M2:\n" + (m1 + m2).ToFormattedString());
            Console.WriteLine("Масштабована M1 * 0.5:\n" + (m1 * 0.5).ToFormattedString());

            try { _ = m1[2, 0]; }
            catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"Виняток індексу: {ex.Message}"); }

            try { _ = m1 + new RealMatrix(3, 3); }
            catch (ArgumentException ex) { Console.WriteLine($"Виняток розміру: {ex.Message}"); }
        }
        private static void RunTask4()
        {
            Console.WriteLine("=== ЗАВДАННЯ 4: implicit / explicit (StorageMb <-> StorageBytes) ===");
            StorageMb mb = new StorageMb(2);
            StorageBytes bytes = mb; // implicit перетворення
            Console.WriteLine($"Implicit: {mb} -> {bytes}");

            StorageBytes raw = new StorageBytes(2_621_440);
            StorageMb back = (StorageMb)raw; // explicit перетворення
            Console.WriteLine($"Explicit: {raw} -> {back}");

            try { _ = new StorageMb(-5); }
            catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"Валідація: {ex.Message}"); }
        }
    }
        

    // ================= ЗАВДАННЯ 1 =================
    public static class NumericAggregator
    {
        public static int Peak(int a, int b) => a >= b ? a : b;

        public static double Peak(double a, double b, double c)
            => Math.Max(a, Math.Max(b, c));

        public static int Peak(params int[] values)
        {
            if (values is null || values.Length == 0)
                throw new ArgumentException("Потрібен принаймні один елемент у масиві params.", nameof(values));

            int currentMax = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > currentMax)
                    currentMax = values[i];
            }
            return currentMax;
        }

        public static T Peak<T>(T first, T second) where T : IComparable<T>
        {
            if (first is null) return second;
            if (second is null) return first;
            return first.CompareTo(second) >= 0 ? first : second;
        }
    }

    public static class OverloadResolver
    {
        public static int Select(int count, double weight) => count + (int)Math.Round(weight);
        public static double Select(double weight, int count) => weight * count;
    }
    // ================= ЗАВДАННЯ 2 =================
    public readonly struct Vec2 : IEquatable<Vec2>
    {
        public const double Epsilon = 1e-6;
        public double X { get; }
        public double Y { get; }

        public Vec2(double x, double y)
        {
            ValidateFinite(x, nameof(x));
            ValidateFinite(y, nameof(y));
            X = x;
            Y = y;
        }

        public static Vec2 Zero => new Vec2(0.0, 0.0);
        public static Vec2 FromPolarOrCartesian(double x, double y) => new Vec2(x, y);
        public double Magnitude => Math.Sqrt(X * X + Y * Y);

        public static Vec2 operator +(Vec2 left, Vec2 right) => new Vec2(left.X + right.X, left.Y + right.Y);
        public static Vec2 operator -(Vec2 left, Vec2 right) => new Vec2(left.X - right.X, left.Y - right.Y);
        public static Vec2 operator *(Vec2 vector, double scalar)
        {
            ValidateFinite(scalar, nameof(scalar));
            return new Vec2(vector.X * scalar, vector.Y * scalar);
        }
        public static Vec2 operator *(double scalar, Vec2 vector) => vector * scalar;

        public static Vec2 operator /(Vec2 vector, double divisor)
        {
            ValidateFinite(divisor, nameof(divisor));
            if (Math.Abs(divisor) <= Epsilon)
                throw new DivideByZeroException($"Дільник {divisor} не перевищує допустимий поріг {Epsilon}.");
            return new Vec2(vector.X / divisor, vector.Y / divisor);
        }

        public static Vec2 operator -(Vec2 vector) => new Vec2(-vector.X, -vector.Y);
        public static bool operator ==(Vec2 left, Vec2 right) => left.Equals(right);
        public static bool operator !=(Vec2 left, Vec2 right) => !left.Equals(right);
        public static bool operator <(Vec2 left, Vec2 right) => !left.Equals(right) && left.Magnitude < right.Magnitude - Epsilon;
        public static bool operator >(Vec2 left, Vec2 right) => !left.Equals(right) && left.Magnitude > right.Magnitude + Epsilon;

        public bool Equals(Vec2 other) => Math.Abs(X - other.X) <= Epsilon && Math.Abs(Y - other.Y) <= Epsilon;
        public override bool Equals(object? obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine((long)Math.Round(X / Epsilon), (long)Math.Round(Y / Epsilon));
        public override string ToString() => $"({X:G6}; {Y:G6})";

        private static void ValidateFinite(double value, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException($"Параметр '{paramName}' має бути скінченним числом.", paramName);
        }
    }
    // ================= ЗАВДАННЯ 3 =================
    public sealed class RealMatrix
    {
        private readonly double[,] _data;
        public int Rows { get; }
        public int Cols { get; }
        public int Length => Rows * Cols;

        public RealMatrix(int rows, int cols)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows), rows, "Кількість рядків має бути > 0.");
            if (cols <= 0) throw new ArgumentOutOfRangeException(nameof(cols), cols, "Кількість стовпців має бути > 0.");
            Rows = rows;
            Cols = cols;
            _data = new double[rows, cols];
        }

        public double this[int row, int col]
        {
            get { ValidateIndices(row, col); return _data[row, col]; }
            set { ValidateIndices(row, col); _data[row, col] = value; }
        }

        public double GetLinear(int index) { ValidateLinearIndex(index); return _data[index / Cols, index % Cols]; }
        public void SetLinear(int index, double value) { ValidateLinearIndex(index); _data[index / Cols, index % Cols] = value; }

        public static RealMatrix operator +(RealMatrix left, RealMatrix right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            if (left.Rows != right.Rows || left.Cols != right.Cols)
                throw new ArgumentException($"Неспівпадіння розмірів: ({left.Rows}x{left.Cols}) та ({right.Rows}x{right.Cols}).");

            var result = new RealMatrix(left.Rows, left.Cols);
            for (int r = 0; r < left.Rows; r++)
                for (int c = 0; c < left.Cols; c++)
                    result._data[r, c] = left._data[r, c] + right._data[r, c];
            return result;
        }

        public static RealMatrix operator *(RealMatrix matrix, double scalar)
        {
            ArgumentNullException.ThrowIfNull(matrix);
            var result = new RealMatrix(matrix.Rows, matrix.Cols);
            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Cols; c++)
                    result._data[r, c] = matrix._data[r, c] * scalar;
            return result;
        }

        public string ToFormattedString()
        {
            var sb = new StringBuilder();
            for (int r = 0; r < Rows; r++)
            {
                sb.Append("[ ");
                for (int c = 0; c < Cols; c++)
                {
                    if (c > 0) sb.Append("  ");
                    sb.Append($"{_data[r, c],8:F2}");
                }
                sb.AppendLine(" ]");
            }
            return sb.ToString().TrimEnd();
        }

        private void ValidateIndices(int row, int col)
        {
            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row), row, $"Індекс рядка поза діапазоном [0..{Rows - 1}], отримано {row}.");
            if (col < 0 || col >= Cols)
                throw new ArgumentOutOfRangeException(nameof(col), col, $"Індекс стовпця поза діапазоном [0..{Cols - 1}], отримано {col}.");
        }

        private void ValidateLinearIndex(int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Лінійний індекс поза діапазоном [0..{Length - 1}], отримано {index}.");
        }
    }
    // ================= ЗАВДАННЯ 4 =================
    public readonly struct StorageMb : IEquatable<StorageMb>
    {
        public const long BytesPerMb = 1024L * 1024L;
        public long Megabytes { get; }

        public StorageMb(long megabytes)
        {
            if (megabytes < 0)
                throw new ArgumentOutOfRangeException(nameof(megabytes), megabytes, "Обсяг у МБ не може бути від'ємним.");
            Megabytes = megabytes;
        }

        public static implicit operator StorageBytes(StorageMb mb)
            => new StorageBytes(checked(mb.Megabytes * BytesPerMb));

        public static explicit operator StorageMb(StorageBytes bytes)
            => new StorageMb(bytes.Bytes / BytesPerMb);

        public bool Equals(StorageMb other) => Megabytes == other.Megabytes;
        public override bool Equals(object? obj) => obj is StorageMb other && Equals(other);
        public override int GetHashCode() => Megabytes.GetHashCode();
        public override string ToString() => $"{Megabytes} MB";
    }

    public readonly struct StorageBytes : IEquatable<StorageBytes>
    {
        public long Bytes { get; }

        public StorageBytes(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes, "Кількість байтів не може бути від'ємною.");
            Bytes = bytes;
        }

        public bool Equals(StorageBytes other) => Bytes == other.Bytes;
        public override bool Equals(object? obj) => obj is StorageBytes other && Equals(other);
        public override int GetHashCode() => Bytes.GetHashCode();
        public override string ToString() => $"{Bytes} B";
    }
}
    