using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo_api.Models;

namespace Todo_api.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<TodoTask> TodoTasks { get; set; }

        //Zastosowałem podejście Code First, korzystam z Fluent API w celu utworzenia kolumn w bazie danych z odpowiednimi włsnościami i ograniczeniami.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTask>(entity =>
            {
                //Kolumna bazy zawierająca numer identyfikacyjny zadania.
                entity.HasKey(x => x.Id); //Nałożenie własności klucza głownego na kolumnę

                //Kolumna bazy zawierająca tytuł zadania.
                entity.Property(e => e.Title)
                    .IsRequired(true) //Ustawienie aby tytuł był wmagany przy dodawaniu wartości do bazy danych
                    .HasMaxLength(150); //Ustawienie maksymalnej długości tytułu to 150 znaków

                //Kolumna bazy danych zawierająca informacje o dacie zakończenia zadania
                entity.Property(e => e.ExpirationDate)
                    .IsRequired(true) //Ustawienie aby data zakończenia była wmagana przy dodawaniu wartości do bazy danych
                    .HasDefaultValue(DateTime.Now); //Ustawienie aby data zakończenia domyślnie była ustawiana na aktualną date czasu lokalnego.
                
                //Kolumna bazy danych zawierający opis zadania.
                entity.Property(e => e.Desc)
                    .HasMaxLength(500); //Ustawienie maksymalnej długości opisu do 500 znaków

                //Kolumna bazy danych zawierająca informację czy zadanie zostało ukończone.
                entity.Property(e => e.Done)
                    .HasDefaultValue(false); //Ustawienie domyślnej wartości na false.

                //Kolumna bazy zawierająca procent ukończenia zadania.
                entity.ToTable(tb => tb.HasCheckConstraint("CK_PERCENTAGE_RANGE", "PercentageComplete BETWEEN 0 AND 100")); //Warunek sprawdzający czy procent ukończenia jest pomiędzy 0% a 100%
                entity.Property(e => e.PercentageComplete)
                    .HasDefaultValue(0); //Ustawienie domyślnej wartości na 0.
            });
        }
    }
}
    
