namespace min_receptor {
    internal class Program {
        static void Main(string[] args) {
            System.Console.WriteLine("Hello, World!");
            var a = "";
            var b = a + "";
            var c = b + "";
            var d = c + "";
            System.Console.WriteLine($"{d}!");
            System.Console.WriteLine("Hello, World!");
        }

        private static void Test1() {
            var a = "a";
            var b = a + "b";
            var c = b + "c";
            var d = c + "d";
            System.Console.WriteLine($"{d}!");
        }
    }
}
