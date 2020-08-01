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
            DbFileNames["Mes"] = Path.Combine(DbHome, "mes.db3");
            DbFileNames[""] = ":memory:";
            Connections = new Dictionary<string, SQLiteConnection>();            
            Connections["Main"] = GetConnection("Main");
            Connections["Data"] = GetConnection("Data");
            Connections["Mes"] = GetConnection("Mes");
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
           db.CreateTable<CcdSettings>();
           db.CreateTable<UiParams>();
           db.CreateTable<CcdVpp>();
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
            db.InsertOrIgnore(new UiParams() { Name = "CcdCount", Data = 3 });
            db.InsertOrIgnore(new UiParams() { Name = "BrandCount", Data = 2 });
            db.InsertOrIgnore(new UiParams() { Name = "PartCountCcd1", Data = 1 });
            db.InsertOrIgnore(new UiParams() { Name = "PartCountCcd2", Data = 1 });
            db.InsertOrIgnore(new UiParams() { Name = "PartCountCcd3", Data = 1 });

            string[] namesCn = new string[] { "内针位置度", "同轴度", "激光引导" };
            string[] namesEn = new string[] { "Ccd1", "Ccd2", "Ccd3"};
            string[] names = { "Ccd1", "Ccd2", "Ccd3" };

            int ccdCount = names.Count();
            for (int i = 0; i < ccdCount; i++)
            {
                var ccdInfo = new CcdInfo() { CcdId = i, Name = names[i], NameEn = namesEn[i], NameCn = namesCn[i] };
                db.InsertOrIgnore(ccdInfo);
            }

            string[] brands = new string[] { "CGQ-001", "CGQ-002" };
            string[] brandsAlias = new string[] { "CGQ-001", "CGQ-002" };

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
                    var ccdTerminal = new CcdTerminal() { CcdId = i, PartId = j, Image = 1, Row = 1, Column = 1 };
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
