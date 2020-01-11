using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOpenGl
{
	enum TypeOfLandscape        //Тип текстуры
	{
		Snow,       //Снег
		Ground,     //Замля
		Water       //Вода
	}

	class MapHeight
	{
		class Hill
		{
			private double r;
			private int iCentre;
			private int jCentre;
			private double deltaR = 0.5;
			private int deltaI = 1;
			private int deltaJ = 1;

			public Hill(int r, int i, int j)
			{
				this.r = r;
				iCentre = i;
				jCentre = j;
			}

			public double R { get => r; }
			public int ICentre { get => iCentre; }
			public int JCentre { get => jCentre; }

			public double update_radius ()
			{
				if (r + deltaR > maxR || r + deltaR < 1)
					deltaR = -deltaR;
				return (r += deltaR);
			}

			public int update_i(int mapSize)
			{
				if (ICentre + deltaI >= mapSize || ICentre + deltaI < 0)
					deltaI = -deltaI;
				return iCentre += deltaI;
			}
			public int update_j(int mapSize)
			{
				if (jCentre + deltaJ >= mapSize || jCentre + deltaJ < 0)
					deltaJ = -deltaJ;
				return jCentre += deltaJ;
			}
		}

		

		private int mapSize;
		private double[,] map;
		private TypeOfLandscape[,] typeLandscape;
		private double snowLevel;
		private double waterLevel;
		private Hill[] hills;

		private static int countHills = 150;
		private static int maxR = 60;

		public MapHeight(int size, double snowLevel, double waterLevel)
		{
			mapSize = size;
			this.snowLevel = snowLevel;
			this.waterLevel = waterLevel;
			map = new double[size, size];
			typeLandscape = new TypeOfLandscape[size, size];
			hills = new Hill[countHills];

			//init_map_random();
			init_map_hills();
		}

		public int MapSize { get => mapSize; }

		public double get_height(int i, int j)
		{
			return map[i, j];
		}

		public TypeOfLandscape get_type(int i, int j)
		{
			return typeLandscape[i, j];
		}
		

		public void update_map ()
		{
			clear_map();	//Очистить карту высот
			for (int k = 0; k < countHills; k++)
			{
				//Получаем параметры k-го холма
				int ii = hills[k].ICentre, jj = hills[k].JCentre;
				double r = hills[k].update_radius();
				int rInt = (int)r;

				//Пройти по квадрату, описыввающему окружность, и поднять холм
				for (int i = ii - rInt; i < ii + rInt; i++)
					for (int j = jj - rInt; j < jj + rInt; j++)
						if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
							map[i, j] += Math.Max(0, r * r - ((j - jj) * (j - jj) + (i - ii) * (i - ii)));
			}
			normalization_map();		//Нормализовать карту высот
		}

		private void init_map_hills()
		{
			Random rand = new Random();

			clear_map();		//Очистить карту высот

			for (int k = 0; k < countHills; k++)
			{
				//Выбор случайного центра и радиуса холма
				int ii = rand.Next(0, mapSize), jj = rand.Next(0, mapSize);
				int r = rand.Next(1, maxR);

				//Сохранить параметры холма для последующего его обновления
				hills[k] = new Hill(r, ii, jj);

				//Пройти по квадрату, описыввающему окружность, и поднять холм
				for (int i = ii - r; i < ii + r; i++)
					for (int j = jj - r; j < jj + r; j++)
						if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
							map[i, j] += Math.Max(0, r * r - ((j - jj) * (j - jj) + (i - ii) * (i - ii)));
							//map[i, j] += r * r - ((j - jj) * (j - jj) + (i - ii) * (i - ii));
			}

			normalization_map();        //Нормализовать карту высот
		}


		//Очистка карты
		private void clear_map()
		{
			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
					map[i, j] = 0;
			for (int i = 0; i < MapSize; i++)
				for (int j = 0; j < MapSize; j++)
					typeLandscape[i, j] = TypeOfLandscape.Ground;
		}

		//Нормализация
		private void normalization_map ()
		{
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
				{
					map[i, j] = (map[i, j] - minHeight) / delta;
					if (map[i, j] > snowLevel)
						typeLandscape[i, j] = TypeOfLandscape.Snow;
					else if (map[i,j] < waterLevel)
						typeLandscape[i, j] = TypeOfLandscape.Water;

				}
		}
	}
}
