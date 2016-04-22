﻿using System.Collections.Generic;
using OxyPlot;
using System;

namespace SmartHome.Models
{
    public class Humidité
    {
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }

        public Humidité()
        {
            Title = "Humidité";
            Points = new List<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };

            var dataReader = new DataReader();
            var datas = dataReader.read();
            Console.Write(datas);
        }
    }
}
