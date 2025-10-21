using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Todo_api.Context;
using Todo_api.Models;

//Z uwagi na to że jest to proste API, pozowliłem sobie na pominięcie warstwy serwisu i umieszczeniu logiki w kontrolerze
namespace Todo_api.Controllers
{
    [ApiController]
    [Route("api/todo/tasks")]
    public class TodoTaskController : ControllerBase
    {
        private readonly AppDbContext context;
        public TodoTaskController(AppDbContext _context)
        {
            context = _context;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateTask([FromBody] TodoTaskDto TaskData)
        {
            try
            {
                //Walidacja danych, ModelState.IsValid sprawdza warunki opisane w modelu TodoTaskDto.cs, jeżeli jest jakiś błąd w danych zwraca błąd 400 z odpowiednimi informacjami.
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values.SelectMany(e => e.Errors).ToList());
                }

                //Gdy dane są poprawne następuje dodanie danych do bazy.
                await context.AddAsync(new TodoTask
                {
                    Title = TaskData.Title,
                    Desc = TaskData.Desc,
                    ExpirationDate = TaskData.ExpirationDate,
                });
                await context.SaveChangesAsync();
                return Ok("Zadanie zostało utworzone pomyślnie!");
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        //Modfikacja wyłącznie tytułu, opisu oraz daty zakończenia zadania. Status oraz procent ukończenia są realizowane w innym endpointcie.
        [HttpPatch]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateTask([FromBody] TodoTaskDto TaskData, [FromRoute] int id)
        {
            try
            {
                //Pojedyńcze zadanie pobrane z bazy danych wg. konktretnego Id.
                var task = await context.TodoTasks.FirstOrDefaultAsync(x => x.Id == id);
                if (task != null)
                {
                    //Walidacja danych, ModelState.IsValid sprawdza warunki opisane w modelu TodoTaskDto.cs i zwraca błąd jeżeli dane są nie poprawne.
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState.Values.SelectMany(e => e.Errors).ToList());
                    }

                    //Aktualizacja tytułu, opisu oraz daty zakończenia w momencie poprawnej walidacji.
                    task.Title = TaskData.Title;
                    task.Desc = TaskData.Desc;
                    task.ExpirationDate = TaskData.ExpirationDate;
                    await context.SaveChangesAsync();

                    //Zwrócenie informacji o poprawnej aktualizacji zadania.
                    return Ok("Zadanie zostało zaktualizowane pomyślnie!");
                }
                else
                {
                    //Zwrócenie błędu 404 z odpowiednią informacją jeżeli task o zadanym id nie istnieje.
                    return NotFound("Operacja niemożliwa, task o podanym id nie istnieje!");
                }
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpPatch]
        [Route("change-percentage/{id}")]
        public async Task<IActionResult> UpdateTaskPercentageComplete([FromBody] int PercentageComplete, [FromRoute] int id)
        {
            try
            {
                //Pojedyńcze zadanie pobrane z bazy danych wg. konktretnego Id.
                var task = await context.TodoTasks.FirstOrDefaultAsync(x => x.Id == id);
                if (task != null)
                {

                    //Sprawdzenie czy podana wartość procentowa mieści się w przedziale od 0 do 100, jeżeli tak to zadanie zostaje zaktualizowane.
                    if (PercentageComplete >= 0 && PercentageComplete <= 100)
                    {
                        task.PercentageComplete = PercentageComplete;
                        await context.SaveChangesAsync();
                        return Ok("Procent ukończenia zadania został zaktualizowany pomyślnie!");
                    }
                    //Zwrócenie błędu jeżeli została podana błędna wartosć procentowa, tj. nie mieszcząca się w przedziale 0-100
                    else
                    {
                        return BadRequest("Wartość procentu ukończenia zadania musi mieścić się w przedziale od 0% do 100%");
                    }
                }
                else
                {
                    //Zwrócenie błędu 404 z odpowiednią informacją jeżeli task o zadanym id nie istnieje.
                    return NotFound("Operacja niemożliwa, zadanie o podanym id nie istnieje!");
                }
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpPatch]
        [Route("change-status/{id}")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] bool TaskStatus, [FromRoute] int id)
        {
            try
            {
                //Pojedyńcze zadanie pobrane z bazy danych wg. konktretnego Id.
                var task = await context.TodoTasks.FirstOrDefaultAsync(x => x.Id == id);
                if (task != null)
                {
                    task.Done = TaskStatus;
                    //Dodatkowy warunek, który sprawdza czy status ukończenia jest ustawiany na true(ukończony), uwzględniająć tą informację procent ukończenia zostaje zmieniony na 100%
                    if (TaskStatus == true)
                    {
                        task.PercentageComplete = 100;

                    }

                    await context.SaveChangesAsync();
                    return Ok("Status ukończenia zadania został zaktualizowany pomyślnie!");
                }
                else
                {
                    //Zwrócenie błędu 404 z odpowiednią informacją jeżeli task o podanym id nie istnieje.
                    return NotFound("Operacja niemożliwa, zadanie o podanym id nie istnieje!");
                }
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSingleTask([FromRoute] int id)
        {
            try
            {
                //Pojedyńcze zadanie pobrane z bazy danych wg. konktretnego Id.
                var task = await context.TodoTasks.FirstOrDefaultAsync(x => x.Id == id);

                //Zwrócenie zadania jeżeli zostało znalezione, w przeciwnym wypadku zwrócenie błędu 404 o braku żądanego zasobu.
                if (task != null)
                    return Ok(task);
                else
                    return NotFound("Nie znaleziono zadania o podanym id!");

            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpGet]
        [Route("today")]
        public async Task<IActionResult> GetTodayTasks()
        {
            try
            {
                //Lista zadań, których data zakończenia jest równa dacie dnia dzisiejszego, uzyskana przez porównanie dat przez wykorzystanie metody Date z obiektu DataTime, która umożliwiła porównanie samej daty bez godziny.
                //Posortowana wg. daty zakończenia tak aby na początku listy znajdowały się zadania, które kończą się najszybciej
                var tasks = await context.TodoTasks.Where(x => x.ExpirationDate.Date == DateTime.Now.Date).OrderBy(x => x.ExpirationDate).ToListAsync();

                //Sprawdzenie czy znajdują się jakieś zadania na dzisaj, jeżeli nie to zwracana jest krótka informacja, w przeciwnym razie zwracana jest lista zadań z datą zakończenia na dzień dzisiejszy.
                if (!tasks.IsNullOrEmpty())
                    return Ok(tasks);
                else
                    return Ok("Brak zadań na dzisiaj!");

            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpGet]
        [Route("tomorrow")]
        public async Task<IActionResult> GetTomorrowTasks()
        {
            try
            {
                //Data dnia następnego uzyskana przez dodanie jednego dnia do dzisiejszej daty.
                var tomorrowDate = DateTime.Now.AddDays(1);

                //Lista zadań, których data zakończenia jest równa dacie z dodanym jednym dniem, uzyskana przez porównanie dat przez wykorzystanie metody Date z obiektu DataTime, która umożliwiła porównanie samej daty bez godziny.
                //Sortowanie wg. daty zakończenia tak aby na początku listy znajdowały się zadania, które kończą się najszybciej
                var tasks = await context.TodoTasks.Where(x => x.ExpirationDate.Date == tomorrowDate.Date).OrderBy(x => x.ExpirationDate).ToListAsync();

                //Sprawdzenie czy znajdują się jakieś zadania na jutro, jeżeli nie to zwracana jest krótka informacja, w przeciwnym razie zwracana jest lista zadań z datą zakończenia na dzień jutrzejszy.
                if (!tasks.IsNullOrEmpty())
                    return Ok(tasks);
                else
                    return Ok("Brak zadań na jutro!");

            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpGet]
        [Route("current-week")]
        public async Task<IActionResult> GetCurrentWeekTasks()
        {
            try
            {
                //Liczba dni jaką trzeba odjąć od aktualnej daty aby wyznaczyć datę początku tygodnia.
                var daysDiff = 0;

                //Warunek wyznaczający liczbę dni, jaką należy odjąć aby uzyskać datę początkową aktualnego tygodnia tj. poniedziałku.
                //Sprawdzenie czy aktualnym dniem jest niedziela, ponieważ DateTime traktuje niedzielę jako początek tygodnia.
                if (DateTime.Now.DayOfWeek == 0)
                {
                    //Wyznaczenie liczby dni do odjęcia na pełny tydzień.
                    daysDiff = 6;
                }
                else
                {
                    //Wyznaczenie liczby dni do odjęcia na podstawie aktualnego dnia tygodnia minus jeden.
                    daysDiff = (int)DateTime.Now.DayOfWeek - 1;
                }

                //Data oznaczająca początek aktualnego tygodnia. Wyznaczona poprzez odjęcie określonej liczby dni.
                var startDate = DateTime.Now.AddDays(-daysDiff);

                //Wyznaczenie końcowej daty poprzez dodanie sześciu dni do daty początkowej tak aby data zakończenia była ustawiona na niedzielę aktualnego tygodnia
                var endDate = startDate.AddDays(6);

                //Lista zadań pobrana z bazy, których data zakończenia mieści się w wyznaczonych datach początkowych i końcowych bierzącego tygodnia.
                //Portowana po dacie tak aby na początku listy znajdowały się zadania, które skończą się najszybciej
                var tasks = await context.TodoTasks.Where(x => x.ExpirationDate.Date >= startDate.Date && x.ExpirationDate.Date <= endDate.Date).OrderBy(x => x.ExpirationDate)
                            .OrderBy(x => x.ExpirationDate  ).ToListAsync();

                //Sprawdzenie czy znajdują się jakieś zadania na bierzący tydzień, jeżeli nie to zwracana jest krótka informacja, w przeciwnym razie zwracana jest lista zadań z datą zakończenia w bierzącym tygodniu.
                if (!tasks.IsNullOrEmpty())
                    return Ok(tasks);
                else
                    return Ok("Brak zadań na bieżący tydzień!");

            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                //Lista wszystkich zadań jakie znajduja się bazie danych. Posortowana wg. daty zakończenia od najwcześniejszej 
                var tasks = await context.TodoTasks.OrderBy(x => x.ExpirationDate).ToListAsync();
                return Ok(tasks);
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            try
            {
                //Pojedyńcze zadanie pobrane z bazy danych wg. konktretnego Id.
                var task = await context.TodoTasks.FirstOrDefaultAsync(x => x.Id == id);
                if (task != null)
                {
                    //Usunięcie znalezionego w bazie zadania wg. konkretnego Id.
                    context.Remove(task);
                    await context.SaveChangesAsync();
                    return Ok("Zadanie zostało usunięte pomyślnie!");
                }
                else
                {
                    //Zwrócenie błędu 404 z odpowiednią informacją jeżeli zadanie o wskazanym id nie istnieje.
                    return NotFound("Operacja niemożliwa, zadanie o podanym id nie istnieje!");
                }
            }
            catch (Exception err)
            {
                return Problem(statusCode: 500, title: "Błąd serwera!", detail: err.Message);
            }
        }

    }
}