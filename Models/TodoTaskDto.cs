using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Todo_api.Models
{
    //Klasa DTO, utworzona na potrzeby dodawania oraz modyfikacji tasków.
    //Brak ustawiania wartości Id, jest to wykonywane automatycznie przez bazę danych i musi pozostać unikalne bez wpływu użytkownika.
    //Brak ustawiania procentu ukończenia oraz statusu ukończenia, realizować będzie to inny endpoint.
    public class TodoTaskDto
    {
        //Walidacja danych sprawdzająca czy użytkownik podał tytuł oraz czy tytuł nie jest dłuższy niż 150 znaków.
        [Required(ErrorMessage = "Tytuł jest wymagany!")]
        [MaxLength(150, ErrorMessage = "Długość tytułu nie może być dłuższa niż 150 znaków!")]
        public required string Title { get; set; }
        
        //Walidacja danych sprawdzająca czy tytuł nie przekracza 500 znaków, uzupełnienie opisu jest opcjonalne.
        [MaxLength(500, ErrorMessage = "Długość opisu nie może być dłuższa niż 500 znaków!")]
        public string? Desc { get; set; }

        public DateTime ExpirationDate { get; set; } = DateTime.Now;

    }
}