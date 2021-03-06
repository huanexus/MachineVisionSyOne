﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.Generic;

using Newtonsoft.Json;


namespace Hu.MachineVision.Database
{
    public static class DbScheme
    {
        public static Dictionary<string, string> DbFileNames { get; set; }
        public static Dictionary<string, SQLiteConnection> Connections { get; set; }
        public static string DbHome { get; set; }

        static DbScheme()
        {
            DbFileNames = new Dictionary<string, string>();
            DbHome = "config";
            if (!File.Exists(DbHome))
            {
                Directory.CreateDirectory(DbHome);
            }
            DbFileNames["Main"] = Path.Combine(DbHome, "main.db3");
            DbFileNames["Data"] = Path.Combine(DbHome, "data.db3");
            DbFileNames[""] = ":memory:";
            Connections = new Dictionary<string, SQLiteConnection>();            
            Connections["Main"] = GetConnection("Main");
            Connections["Data"] = GetConnection("Data");
        }

        public static SQLiteConnection GetConnection(string name)
        {
            ISQLitePlatform platform = new SQLitePlatformGeneric();
            if (!Connections.ContainsKey(name))
            {
                Connections[name] =  new SQLiteConnection(platform, DbFileNames[name]);    
            }

            return Connections[name];      
        }

        public static void Create()
        {
            CreateDatabaseMain();
            CreateDatabaseData();
            InitializeDatabaseMain();
            InitializeDatabaseData();
        }

        public static void CreateDatabaseMain()
        {
            var db = Connections["Main"];
            db.CreateTable<CcdBrand>();
            db.CreateTable<CcdInfo>();
            db.CreateIndex("CcdInfo", "ccdId", true);
            
           db.CreateTable<CcdTerminal>();
           db.CreateIndex("CcdTerminal", new string[] { "ccdId", "partId" }, true);           
           db.CreateTable<UiParams>();
           db.CreateTable<CcdVpp>();

           db.CreateTable<CcdDi>();
           db.CreateTable<CcdDo>();
           db.CreateTable<CcdSerial>();
           db.CreateTable<CcdParams>();
           db.CreateTable<CcdRoi>();
           db.CreateTable<CcdCompValue>();
           db.CreateTable<UiGlossary>();

           db.CreateTable<ViewScheme>();

           db.CreateIndex("CcdDi", new string[] { "ccdId", "name", "port" }, true);
           db.CreateIndex("CcdDo", new string[] { "ccdId", "name", "port" }, true);
           db.CreateIndex("CcdParams", new string[] { "ccdId", "brandId", "paramId", "name" }, true);
           db.CreateIndex("CcdRoi", new string[] { "ccdId", "brandId", "imageIndex" }, true);
           db.CreateIndex("CcdCompValue", new string[] { "ccdId", "brandId", "item" }, true);
           db.CreateIndex("ViewScheme", new string[] {"ccdId", "brandId", "viewId", "recordId"}, true);
        }

        public static void CreateDatabaseData()
        {
            var db = Connections["Data"];
            db.CreateTable<CcdCycle>();
            db.CreateIndex("CcdCycle", new string[] { "ccdId", "brandId" }, true);
            db.CreateTable<RunStatus>();
        }

        public static void InitializeDatabaseMain()
        {
            var db = Connections["Main"];

            db.InsertOrIgnore(new UiParams() { Name = "ScreenWidth", Data = 1366 });
            db.InsertOrIgnore(new UiParams() { Name = "ScreenHeight", Data = 768 });
            db.InsertOrIgnore(new UiParams() { Name = "CcdCount", Data = 2 });
            db.InsertOrIgnore(new UiParams() { Name = "BrandCount", Data = 1 });
            db.InsertOrIgnore(new UiParams() { Name = "PartCountCcd1", Data = 1 });
            db.InsertOrIgnore(new UiParams() { Name = "PartCountCcd2", Data = 1 });
            db.InsertOrIgnore(new UiParams() { Name = "RunMode", Data = 0 });

            db.InsertOrIgnore(new UiGlossary() { Name = "k", TextEn = "K", TextCn = "补偿系数" });
            db.InsertOrIgnore(new UiGlossary() {Name = "b", TextEn = "B", TextCn = "补偿常数" });
            db.InsertOrIgnore(new UiGlossary() { Name = "r0", TextEn = "R0", TextCn = "标准值" });
            db.InsertOrIgnore(new UiGlossary() { Name = "r1", TextEn = "R1", TextCn = "下限" });
            db.InsertOrIgnore(new UiGlossary() { Name = "r2", TextEn = "R2", TextCn = "上限" });
            db.InsertOrIgnore(new UiGlossary() { Name = "Data", TextEn = "Data", TextCn = "测量值" });
            db.InsertOrIgnore(new UiGlossary() { Name = "RawData", TextEn = "RawData", TextCn = "原始值" });           

            string[] namesCn = new string[] { "针检1", "针检2"};
            string[] namesEn = new string[] { "Ccd1", "Ccd2"};
            string[] names = { "Ccd1", "Ccd2"};

            int ccdCount = names.Count();
            for (int i = 0; i < ccdCount; i++)
            {
                var ccdInfo = new CcdInfo() { CcdId = i, Name = names[i], NameEn = namesEn[i], NameCn = namesCn[i] };
                db.InsertOrIgnore(ccdInfo);
            }

            string[] brands = new string[] { "TJC8" };
            string[] brandsAlias = new string[] { "8"};

            int brandCount = brands.Count();

            for (int i = 0; i < brandCount; i++)
            {
                var ccdBrand = new CcdBrand() { BrandId = i, Brand = brands[i], BrandAlias = brandsAlias[i] };
                db.InsertOrIgnore(ccdBrand);
            }

            int[] parts = Enumerable.Repeat(1, ccdCount).ToArray();

            for (int i = 0; i < ccdCount; i++)
            {
                for (int j = 0; j < parts[i]; j++)
                {
                    var ccdTerminal = new CcdTerminal() { CcdId = i, PartId = j, Image = 1, Row = 2, Column = 20 };
                    db.InsertOrIgnore(ccdTerminal);
                }

            }

            for(int i = 0; i < ccdCount; i++)
            {
                string vppHome = "vpp";
                string namePattern = string.Format("Ccd{0}", i + 1);
                string partPattern = "[A-Za-z]*";
                var patternVpp = new CcdVpp() { CcdId = i, VppHome = vppHome, NamePattern = namePattern, PartPattern = partPattern };
                db.InsertOrIgnore(patternVpp);
            }

            for(int i = 0; i < ccdCount; i++)
            {
                db.InsertOrIgnore(new CcdDi() { CcdId = i, Name = "Trig", Port = 0 });
                db.InsertOrIgnore(new CcdDi() { CcdId = i, Name = "Reset", Port = 4 });
                db.InsertOrIgnore(new CcdDo() { CcdId = i, Name = "Done", Port = 0 });
                db.InsertOrIgnore(new CcdDo() { CcdId = i, Name = "Ok", Port = 1 });
                db.InsertOrIgnore(new CcdDo() { CcdId = i, Name = "Ng", Port = 2 });
                db.InsertOrIgnore(new CcdSerial() { CcdId = i, ComPort = i + 1 });
            }

            for(int i = 0; i < ccdCount; i++)
            {
                for(int j = 0; j < brandCount; j++)
                {
                    db.InsertOrIgnore(new CcdParams() { CcdId = i, BrandId = j, ParamId = 0, Name = "Exposure", Data = 35 });
                }
            }

            for(int i = 0; i < ccdCount; i++)
            {
                for(int j = 0; j < brandCount; j++)
                {
                    db.InsertOrIgnore(new CcdRoi(){CcdId = i, BrandId = j, ImageIndex = 0, X0 = 0, Y0 = 950, Width = 2500, Height = 600 });
                }
            }

            for(int i = 0; i < ccdCount; i++)
            {
                for(int j = 0; j < brandCount; j++)
                {
                    int rowCount = db.ExecuteScalar<int>("select row from CcdTerminal where ccdId = ?", i);
                    int columnCount = db.ExecuteScalar<int>("select column from CcdTerminal where ccdId = ?", i);

                    for (int m = 0; m < rowCount; m++)
                    {
                        for(int n = 0; n < columnCount; n++)
                        {
                            int k = m * columnCount + n;
                            db.InsertOrIgnore(new CcdCompValue() { CcdId = i, BrandId = j, Item = k, K = 1.0, B = 0.0, Label = string.Format("PIN{0}", n + 1), R0 = 1.0, R1 = 0.999, R2 = 1.001 });

                        }
                    }
                   
                }
            }


            for(int i = 0; i < ccdCount; i++)
            {
                for(int j = 0; j < brandCount; j++)
                {
                    db.InsertOrIgnore(new ViewScheme() { CcdId = i, BrandId = j, Category = 0, ViewId = 0, RecordId = 0, TextEn = "", TextCn = "针有无" });
                    db.InsertOrIgnore(new ViewScheme() { CcdId = i, BrandId = j, Category = 0, ViewId = 0, RecordId = 1, TextEn = "", TextCn = "针长短" });
                }
            }
        }

        public static double GetCcdParams(int ccdId, int brandId, int paramId, string name)
        {
            var db = Connections["Main"];
            string sql = "select data from CcdParams where ccdId = ? and brandId = ? and paramId = ? and name = ?";
            double data = db.ExecuteScalar<double>(sql, ccdId, brandId, paramId, name);
            return data;
        }

        public static int GetUiParams(string name)
        {
            var db = Connections["Main"];
            int data = db.ExecuteScalar<int>("select data from UiParams where name = ?", name);

            return data;
        }

        public static int GetRunStatus(string name)
        {
            var db = Connections["Data"];
            int data = db.ExecuteScalar<int>("select data from RunStatus where name = ?", name);

            return data;
        }

        public static void InitializeDatabaseData()
        {
            var db = Connections["Data"];
            db.InsertOrIgnore(new RunStatus() { Name = "BrandId", Data = 0 });
            db.InsertOrIgnore(new RunStatus() { Name = "RunMode", Data = 0 });
            db.InsertOrIgnore(new RunStatus() { Name = "IsLogin", Data = 0 });


            int ccdCount = GetUiParams("CcdCount");
            int brandCount = GetUiParams("BrandCount");

            for (int i = 0; i < ccdCount; i++)
            {
                for (int j = 0; j < brandCount; j++)
                {
                    var cycleCcd = new CcdCycle() { CcdId = i, BrandId = j, All = 0, Ok = 0, Ng = 0 };
                    db.InsertOrIgnore(cycleCcd);
                }
            }
        }
    }
}
