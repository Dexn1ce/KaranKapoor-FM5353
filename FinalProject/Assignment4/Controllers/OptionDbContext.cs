
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;
using Assignment4 ;
using Assignment4.Services;


using Npgsql;
using System.Linq;

namespace  Assignment4.Data
{
	public class optionContext : DbContext
	{
		//collection of option object translated as table by entity
		//
		public DbSet<Exchange> exchanges {get; set;}

		public DbSet<Market> markets {get; set;}

		public DbSet<Trade> trades {get; set;}

//		public DbSet<TradeEvalation> tradeeval {get; set;}

//		public DbSet<UnderlyingMarket> um {get; set;}

		public DbSet<underlying> underlying {get; set;}
		public DbSet<historicalPrice> price_series {get; set;}

		public DbSet<rateCurve> ratecurve {get; set;}
		public DbSet<ratePoint> ratepoint {get; set;}

		public DbSet<Option> Options {get; set;}
		public DbSet<Asian_option> Asian_options{get; set;}


		public DbSet<Digital_option> Digital_options{get; set;}
		public DbSet<Lookback_option> Lookback_options{get; set;}
		public DbSet<Range_option> range_options{get; set;}
		public DbSet<Barrier_option> barrier_option{get; set;}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseNpgsql("Host=localhost;Username=karankapoor;Password=finalproject123;Database=options");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Exchange>()
				    .HasKey( a => a.id);

			modelBuilder.Entity<Exchange>()
				    .HasMany( a => a.Markets)
				    .WithOne( b => b.exchange)
				    .HasForeignKey( a=> a.Exchangeid);

			modelBuilder.Entity<Market>()
				    .HasKey(a => a.id);

			modelBuilder.Entity<Market>()
				    .HasOne( a => a.exchange)
				    .WithMany( b => b.Markets)
				    .HasForeignKey( a=> a.Exchangeid);

			modelBuilder.Entity<Market>()
				    .HasMany( b => b.Trades)
				    .WithOne( a => a.Market)
				    .HasForeignKey ( o => o.MarketId);


			modelBuilder.Entity<underlying> ()
					.HasKey( a => a.underlyingid);
			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.prices)
				    .WithOne(p => p.Underlying)
				    .HasForeignKey(o => o.underlyingid);



			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.options)
				    .WithOne(p => p.underlying)
				    .HasForeignKey(o => o.underlyingid);
			
			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.asian_option)
				    .WithOne(p => p.Underlying)
				    .HasForeignKey(o => o.underlyingid);


			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.barrier_option)
				    .WithOne(p => p.underlying)
				    .HasForeignKey(o => o.underlyingid);



			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.range_option)
				    .WithOne(p => p.underlying)
				    .HasForeignKey(o => o.underlyingid);



			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.lookback_option)
				    .WithOne(p => p.underlying)
				    .HasForeignKey(o => o.underlyingid);


			modelBuilder.Entity<underlying>()
				    .HasMany(a => a.digital_option)
				    .WithOne(p => p.underlying)
				    .HasForeignKey(o => o.underlyingid);

			modelBuilder.Entity<underlying>()
				    .HasMany( a => a.Trades)
				    .WithOne(a => a.Underlying)
				    .HasForeignKey( o => o.UnderlyingId);
			
				    
			modelBuilder.Entity<historicalPrice> (s => {
					s.HasKey(a => a.historicalpriceid);
					s.HasOne(b => b.Underlying)
					 .WithMany( u => u.prices)
					 .HasForeignKey(b => b.underlyingid);

					s.ToTable("historicalprice");
					});
		
		
		
			modelBuilder.Entity<Option>( o => {
					o.HasKey(p => p.id);
					o.HasOne(o => o.underlying)
					 .WithMany(u => u.options)
					 .HasForeignKey(o => o.underlyingid);
				
					o.ToTable("option");	
					});

			modelBuilder.Entity<Option>()
				    .HasOne( p => p.Ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);


			modelBuilder.Entity<Asian_option>(o => {
					o.HasKey(p => p.id);
					o.HasOne(o => o.Underlying)
					 .WithMany( u=> u.asian_option)
					 .HasForeignKey(o => o.underlyingid);
				
					o.ToTable("asian_option");});	


			modelBuilder.Entity<Asian_option>()
				    .HasOne( p => p.ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);


			modelBuilder.Entity<Digital_option>().ToTable("digital_option");



			modelBuilder.Entity<Digital_option>(o => { o.HasKey( b => b.id);   });

			modelBuilder.Entity<Digital_option>()
				    .HasOne(b => b.underlying)
				    .WithMany(c => c.digital_option)
				    .HasForeignKey( b => b.underlyingid);


			modelBuilder.Entity<Digital_option>()
				    .HasOne( p => p.Ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);


			modelBuilder.Entity<Lookback_option>().ToTable("lookback_option");
			
			modelBuilder.Entity<Lookback_option>(o => { o.HasKey( b => b.id);   });

			modelBuilder.Entity<Lookback_option>()
				    .HasOne(b => b.underlying)
				    .WithMany(c => c.lookback_option)
				    .HasForeignKey( b => b.underlyingid);

			modelBuilder.Entity<Range_option>()
				    .HasOne( p => p.Ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);



			modelBuilder.Entity<Range_option>(entity => 
					{
						entity.ToTable("range_option");
						entity.Ignore(e => e.K);
						entity.Ignore(e => e.otyp);
					}
			);

			modelBuilder.Entity<Range_option>(o => { o.HasKey( b => b.id);   });

			modelBuilder.Entity<Range_option>()
				    .HasOne(b => b.underlying)
				    .WithMany(c => c.range_option)
				    .HasForeignKey( b => b.underlyingid);

			modelBuilder.Entity<Range_option>()
				    .HasOne( p => p.Ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);

			modelBuilder.Entity<Barrier_option>().ToTable("barrier_option");

			modelBuilder.Entity<Barrier_option>(o => { o.HasKey( b => b.id);   });

				
			modelBuilder.Entity<Barrier_option>()
				    .HasOne(b => b.underlying)
				    .WithMany(c => c.barrier_option)
				    .HasForeignKey( b => b.underlyingid);

			modelBuilder.Entity<Barrier_option>()
				    .HasOne( p => p.Ratecurve)
				    .WithMany()
				    .HasForeignKey( o => o.ratecurveid);

			modelBuilder.Entity<Trade>()
				    .HasKey( o => o.TradeId);

			modelBuilder.Entity<Trade>()
				    .HasOne( b => b.Underlying)
				    .WithMany( a => a.Trades)
				    .HasForeignKey ( o => o.UnderlyingId);


			modelBuilder.Entity<Trade>()
				    .HasOne( b => b.Market)
				    .WithMany( a => a.Trades)
				    .HasForeignKey ( o => o.MarketId);




/*			modelBuilder.Entity<TradeEvaluation>()
				    .HasKey( a => a.TradeEvaluationId);

			modelBuilder.Entity<TradeEvaluation>()
				    .HasOne( b => b.Trade)
				    .WithMany()
				    .HasForeignKey( a => a.TradeId);*/

			modelBuilder.Entity<rateCurve>()
				    .HasKey (a => a.RateCurveId);

			modelBuilder.Entity<rateCurve>()
				    .HasMany(a =>a.RatePoints)
				    .WithOne( p => p.RateCurve)
				    .HasForeignKey( o => o.ratecurveid);

			modelBuilder.Entity<ratePoint>()
				    .HasKey( a => a.RatePointId);

			modelBuilder.Entity<ratePoint>()
				    .HasOne(a => a.RateCurve)
				    .WithMany(r => r.RatePoints)
				    .HasForeignKey( b => b.ratecurveid);


		}

	}
}
