
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Db.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Db.Dbc>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}