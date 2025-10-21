using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Todo_api.Models
{
    public class TodoTask
    {
        //Numer Idenyfikacyjny zadania
        public int Id { get; set; }

        //Tytuł zadania, wartość wymagana.
        public string? Title { get; set; }

        //Opis zadania, wartość opcjonalna.
        public string? Desc { get; set; }

        //Wartość, która określa godzinę oraz datę końcową zadania.
        public DateTime ExpirationDate { get; set; }

        //Wartość określająca procent ukończenia zadania
        public int PercentageComplete { get; set; }

        //Wartość określająca czy zadanie zostało ukończone, przyjmuje true(ukończone) lub false(nieukończone).
        public bool Done { get; set; }
    }
}