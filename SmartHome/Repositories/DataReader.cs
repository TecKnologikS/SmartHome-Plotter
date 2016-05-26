﻿using SmartHome.Converters;
using SmartHome.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System;
using System.Linq;

namespace SmartHome
{
    public class DataReader
    {
        private XDocument _doc;
        private List<string> _netatmoDatas;
        private readonly string _basePath;

        public DataReader()
        {
            _basePath = "../../../";

            _doc = XDocument.Load(FileSearch("*.xtim").First());
            _netatmoDatas = FileSearch("*.dt").ToList();
        }

        private IEnumerable<string> FileSearch(string filename)
        {
            return Directory.GetFiles(
                _basePath,
                filename,
                SearchOption.AllDirectories
            );
        }

        public List<Capteur> read()
        {
            var capteurs = readCapteurs();

            capteurs.ForEach(d =>
                d.Datas = readDatas(d.Id)
            );

            return capteurs;
        }

        private List<Capteur> readCapteurs()
        {
            var capteurs = new List<Capteur>();

            if (_doc.Descendants("capteurs") != null)
            {
                foreach (XElement node in _doc.Descendants("capteurs").Nodes())
                {
                    var capteur = new Capteur();

                    if (node.Attribute("type") != null)
                    {
                        capteur.Type = TypeCapteurConverter.convert(node.Attribute("type").Value);
                    }

                    if (node.Element("id") != null)
                    {
                        capteur.Id = node.Element("id").Value;
                    }

                    if (node.Element("description") != null)
                    {
                        capteur.Description = node.Element("description").Value;
                    }

                    if (node.Element("grandeur") != null)
                    {
                        capteur.Grandeur = new GrandeurCapteur();

                        if (node.Element("grandeur").Attribute("nom") != null)
                        {
                            capteur.Grandeur.Nom = node.Element("grandeur").Attribute("nom").Value;
                        }

                        if (node.Element("grandeur").Attribute("unite") != null)
                        {
                            capteur.Grandeur.Unite = node.Element("grandeur").Attribute("unite").Value;
                        }

                        if (node.Element("grandeur").Attribute("abreviation") != null)
                        {
                            capteur.Grandeur.Abreviation = node.Element("grandeur").Attribute("abreviation").Value;
                        }
                    }

                    if (node.Element("valeur") != null)
                    {
                        capteur.Valeur = new ValeurCapteur();

                        if (node.Element("valeur").Attribute("type") != null)
                        {
                            capteur.Valeur.Type = node.Element("valeur").Attribute("type").Value;
                        }

                        if (node.Element("valeur").Attribute("min") != null)
                        {
                            capteur.Valeur.Min = double.Parse(node.Element("valeur").Attribute("min").Value);
                        }

                        if (node.Element("valeur").Attribute("max") != null)
                        {
                            capteur.Valeur.Max = double.Parse(node.Element("valeur").Attribute("max").Value);
                        }
                    }

                    if (node.Element("box") != null)
                    {
                        capteur.Box = node.Element("box").Value;
                    }

                    if (node.Element("lieu") != null)
                    {
                        capteur.Lieu = node.Element("lieu").Value;
                    }

                    if (node.Descendants("seuils") != null)
                    {
                        foreach (XElement nodeSeuil in node.Descendants("seuils").Nodes())
                        {
                            if (nodeSeuil.Document.Element("seuil") != null)
                            {
                                var seuil = new SeuilCapteur();

                                if (nodeSeuil.Document.Element("seuil").Attribute("description") != null)
                                {
                                    seuil.Description = nodeSeuil.Document.Element("seuil").Attribute("description").Value;
                                }

                                if (nodeSeuil.Document.Element("seuil").Attribute("valeur") != null)
                                {
                                    seuil.Valeur = double.Parse(nodeSeuil.Document.Element("seuil").Attribute("valeur").Value);
                                }

                                capteur.Seuils.Add(seuil);
                            }
                        }
                    }

                    capteurs.Add(capteur);
                }
            }
            
            return capteurs;
        }

        private IEnumerable<SmartData> readDatas(string id)
        {
            foreach (var filePath in _netatmoDatas)
            {
                foreach (var line in File.ReadLines("@" + Path.Combine(Directory.GetCurrentDirectory(), "\\../" + filePath)))
                {
                    var elements = line.Split(' ');

                    if (elements[2].Equals(id))
                    {
                        yield return new SmartData()
                        {
                            Valeur = double.Parse(elements[3]),
                            Date = DateTime.Parse(
                                elements[0].Substring(1)
                                + " "
                                + elements[1].Substring(0, elements[1].Length - 1)
                            )
                        };
                    }
                }
            }

            //return datas;
        }
    }
}
