using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Database
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 配置主键、表名
            // 配置Province主键、表名
            modelBuilder.Entity<Province>()
                .ToTable("Province")
                .HasKey(o => o.Id);

            // 配置City主键、表名
            modelBuilder.Entity<City>()
                .ToTable("City")
                .HasKey(o => o.Id);

            // 配置Mayor主键、表名
            modelBuilder.Entity<Mayor>()
                .ToTable("Mayor")
                .HasKey(o => o.Id);

            // 配置Company主键、表名
            modelBuilder.Entity<Company>()
                .ToTable("Company")
                .HasKey(o => o.Id);

            // 配置CityCompany主键
            modelBuilder.Entity<CityCompany>()
                .ToTable("CityCompany")
                .HasKey(o => o.Id);
            #endregion

            #region 配置关系
            // 配置City与Province一对多关系
            modelBuilder.Entity<City>()
                .HasOne(o => o.Province)
                .WithMany(o => o.Cities)
                .HasForeignKey(o => o.ProvinceId);

            // 配置Mayor与City一对一关系
            // 注意：一对一关系时，只能在一个表上设置外键。比如这里在Mayor表上设置外键，则Mayor必须要有City，但City可以没有Mayor
            // 实际上就是1:1or0的关系
            modelBuilder.Entity<Mayor>()
                .HasOne(o => o.City)
                .WithOne(o => o.Mayor)
                .HasForeignKey<Mayor>(o => o.CityId);

            // 配置City与Company多对多的关系
            modelBuilder.Entity<City>()
                .HasMany(o => o.CityCompanies)
                .WithOne(o => o.City)
                .HasForeignKey(o => o.CityId);
            modelBuilder.Entity<Company>()
                .HasMany(o => o.CityCompanies)
                .WithOne(o => o.Company)
                .HasForeignKey(o => o.CompanyId);
            #endregion

            #region 种子数据
            // 1、ID必须写死，一旦ID改变了会删除旧的数据，创建新的数据
            modelBuilder.Entity<Province>()
                .HasData(new List<Province> { 
                    new Province{Id=1,Name="广东省",Population=8000_0000 },
                    new Province{Id=2,Name="湖北省",Population=5000_0000 },
                    new Province{Id=3,Name="四川省",Population=6000_0000 }
                });
            modelBuilder.Entity<City>()
                .HasData(new List<dynamic> {
                    new {Id=1,ProvinceId=1,Name="广州市" },
                    new {Id=2,ProvinceId=1,Name="深圳市" },
                    new {Id=3,ProvinceId=1,Name="佛山市" },
                    new {Id=4,ProvinceId=2,Name="武汉市" },
                    new {Id=5,ProvinceId=2,Name="襄阳市" },
                    new {Id=6,ProvinceId=2,Name="天门市" },
                    new {Id=7,ProvinceId=3,Name="成都市" },
                    new { Id=8,ProvinceId=3,Name="宜宾市"}
                });
            #endregion

            // 使用EntityTypeConfiguration
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Mayor> Mayors { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<CityCompany> CityCompanies { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
