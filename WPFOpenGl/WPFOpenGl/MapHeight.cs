using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOpenGl
{
	class MapHeight
	{
		private int mapSize;
		private double[,] map;
		private double zoom;

		public MapHeight(int size, double zoom)
		{
			mapSize = size;
			this.zoom = zoom;
			map = new double[size, size];

			//init_map_random();
			init_map_hills();
		}

		public int MapSize { get => mapSize; }

		public double get_height(int i, int j)
		{
			return map[i, j];
		}
		private void init_map_random()
		{
			Random rand = new Random();

			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
					map[i, j] = rand.NextDouble() / 2;
		}

		private void init_map_hills()
		{
			Random rand = new Random();

			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
					map[i, j] = 0;

			int K = 100;
			int R = 40;

			for (int k = 0; k < K; k++)
			{
				int ii = rand.Next(0, mapSize), jj = rand.Next(0, mapSize);
				int r = rand.Next(1, R);


				for (int i = ii - r; i < ii + r; i++)
					for (int j = jj - r; j < jj + r; j++)
						if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
						{
							//map[i, j] += Math.Max(0, Math.Sqrt(Math.Pow(r * r - ((j - jj) * (j - jj) + (i - ii) * (i - ii)), 2)));
							map[i, j] += Math.Max(0, r * r - ((j - jj) * (j - jj) + (i - ii) * (i - ii)));
						}
			}
			//Нормализация
			double minHeight = 1000, maxHeight = 0;
			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
				{
					minHeight = Math.Min(minHeight, map[i, j]);
					maxHeight = Math.Max(maxHeight, map[i, j]);
				}

			double delta = maxHeight - minHeight;
			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
					map[i, j] = (map[i, j] - minHeight) / delta;
		}
	}
}
