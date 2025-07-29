using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateSystem.Models;

namespace TemplateSystem.Util
{
    internal class SqlAccess
    {
        /// <summary>
        /// 系统数据访问
        /// </summary>
        public SqlSugarClient SystemDataAccess;



        public SqlAccess()
        {
            MySqlLink();
        }



        public void MySqlLink()
        {
            //用于存储系统数据的数据库
            SystemDataAccess = new SqlSugarClient(new ConnectionConfig()
            {
                //ConnectionString = "server=127.0.0.1;uid=root;pwd=123456;database=csdk_zj",
                ConnectionString = "server=127.0.0.1;uid=root;pwd=Csdk@2025;database=csdk_zj",
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            SystemDataAccess.DbMaintenance.CreateDatabase();





        }

        /// <summary>
        /// 系统数据修改
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <param name="dataValue">数据值</param>
        public static void SystemDatasUpdateable(string name, string value)
        {
            sys_bd_systemsettingsdatamodel data = new sys_bd_systemsettingsdatamodel();
            data.Name = name;
            data.Value = value;

            using (var sDB = new SqlAccess().SystemDataAccess)
            {
                // 更新操作
                sDB.Updateable<sys_bd_systemsettingsdatamodel>()
                   .SetColumns(x => x.Value == value)
                   .Where(x => x.Name == name)
                   .ExecuteCommand();

            } // 自动释放连接
        }

        /// <summary>
        /// 系统数据插入
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SystemDatasInsertable(string name, string value)
        {
            sys_bd_systemsettingsdatamodel data = new sys_bd_systemsettingsdatamodel();
            data.Name = name;
            data.Value = value;


            using (var sDB = new SqlAccess().SystemDataAccess)
            {
                // 新增操作
                sys_bd_systemsettingsdatamodel newData = new sys_bd_systemsettingsdatamodel
                {
                    Name = name,
                    Value = value
                };
                sDB.Insertable(newData).ExecuteCommand();

            } // 自动释放连接
        }
        /// <summary>
        /// 初始化表格
        /// </summary>
        public void InitializeTable()
        {
            SystemDataAccess.CodeFirst.InitTables(typeof(sys_bd_Templatedatamodel)); //*
        }
    }
}
