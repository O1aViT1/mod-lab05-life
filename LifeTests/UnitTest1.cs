using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.IO;

namespace LifeTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test1() {
            var c = new Cell { IsAlive = true };
            c.DetermineNextLiveState();
            c.Advance();
            Assert.IsFalse(c.IsAlive);
        }

        [TestMethod]
        public void Test2() {
            var c = new Cell { IsAlive = true };
            for(int i=0; i<2; i++) c.neighbors.Add(new Cell { IsAlive = true });
            c.DetermineNextLiveState();
            c.Advance();
            Assert.IsTrue(c.IsAlive);
        }

        [TestMethod]
        public void Test3() {
            var c = new Cell { IsAlive = false };
            for(int i=0; i<3; i++) c.neighbors.Add(new Cell { IsAlive = true });
            c.DetermineNextLiveState();
            c.Advance();
            Assert.IsTrue(c.IsAlive);
        }

        [TestMethod]
        public void Test4() {
            var b = new Board(10, 10, 1, 0);
            Assert.AreEqual(10, b.Columns);
        }

        [TestMethod]
        public void Test5() {
            var b = new Board(10, 10, 1, 1);
            Assert.IsTrue(b.Cells[0,0].IsAlive);
        }

        [TestMethod]
        public void Test6() {
            var b = new Board(5, 5, 1, 0);
            b.Cells[0,0].IsAlive = true;
            b.Save("test_save.txt");
            Assert.IsTrue(File.Exists("test_save.txt"));
        }

        [TestMethod]
        public void Test7() {
            var b = new Board(5, 5, 1, 0);
            File.WriteAllText("test_load.txt", "* \n     ");
            b.Load("test_load.txt");
            Assert.IsTrue(b.Cells[0,0].IsAlive);
        }

        [TestMethod]
        public void Test8() {
            var b = new Board(10, 10, 1, 0);
            b.Cells[1,1].IsAlive = true;
            b.Cells[1,2].IsAlive = true;
            Assert.AreEqual(1, b.CountClusters());
        }

        [TestMethod]
        public void Test9() {
            var b = new Board(10, 10, 1, 0);
            b.Cells[1,1].IsAlive = true;
            b.Cells[5,5].IsAlive = true;
            Assert.AreEqual(2, b.CountClusters());
        }

        [TestMethod]
        public void Test10() {
            var c = new Cell { IsAlive = true };
            for(int i=0; i<4; i++) c.neighbors.Add(new Cell { IsAlive = true });
            c.DetermineNextLiveState();
            c.Advance();
            Assert.IsFalse(c.IsAlive);
        }

        [TestMethod]
        public void Test11() {
            var b = new Board(5, 5, 1, 0.5);
            b.Advance();
            Assert.IsNotNull(b.Cells);
        }

        [TestMethod]
        public void Test12() {
            var b = new Board(10, 10, 1, 0);
            Assert.AreEqual(0, b.CountClusters());
        }

        [TestMethod]
        public void Test13() {
            var settings = new Settings { Width = 10 };
            Assert.AreEqual(10, settings.Width);
        }

        [TestMethod]
        public void Test14() {
            var b = new Board(10, 10, 1, 0);
            b.Cells[0,0].IsAlive = true;
            b.Cells[0,1].IsAlive = true;
            b.Cells[1,0].IsAlive = true;
            b.Cells[1,1].IsAlive = true;
            Assert.AreEqual(1, b.CountClusters());
        }

        [TestMethod]
        public void Test15() {
            var b = new Board(2, 2, 1, 1);
            Assert.AreEqual(4, b.Cells.Length);
        }

        [TestMethod]
        public void Test16() {
            var c = new Cell();
            Assert.IsNotNull(c.neighbors);
        }

        [TestMethod]
        public void Test17() {
            var b = new Board(10, 10, 1, 0);
            b.Randomize(1.0);
            Assert.IsTrue(b.Cells[5,5].IsAlive);
        }
    }
}
