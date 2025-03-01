namespace FigureStore.Models
{
    public interface IHasTimestamps
    {
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }
    }
}
