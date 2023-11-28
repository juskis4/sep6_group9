using Microsoft.EntityFrameworkCore;
using webApplication.Models;

namespace webApplication.Data
{
    public interface IMovieDataContext
    {
        DbSet<Movie> Movies { get; set; }
        DbSet<Star> Stars { get; set; }
        DbSet<Director> Directors { get; set; }
        DbSet<Person> People { get; set; }
    }
}