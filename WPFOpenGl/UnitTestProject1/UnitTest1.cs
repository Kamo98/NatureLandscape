using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFOpenGl;

namespace UnitTestProject1
{
	[TestClass]
	public class MapHeightTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			int mapSize = 256;
			double snowLevel = 0.66, waterLevel = 0.25;
			MapHeight mapHeight = new MapHeight(mapSize, snowLevel, waterLevel);

			Assert.AreEqual(mapHeight.MapSize, mapSize);
		}
	}
}
