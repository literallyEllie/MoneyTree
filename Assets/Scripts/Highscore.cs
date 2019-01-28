using System;

public class Highscore
{
   
    public int Position { get; set; }
    public int TimeCompleted { get; set; }
    public int CoinsCollected { get; set; }
    public int Attempts { get; set; }

    // A highscore object which will not make it to production.
    public Highscore(string data)
    {
        var d = data.Split(';');

        if (String.IsNullOrEmpty(data) || d.Length < 3)
            throw new ArgumentException("Invalid high score string", "data");

        int timeInt;
        if (int.TryParse(d[1], out timeInt))
        {
            this.TimeCompleted = timeInt;
        }
        else
        {
            throw new ArgumentException("Invalid time integer", "data");
        }

        int coinsInt;
        if (int.TryParse(d[2], out coinsInt))
        {
            this.CoinsCollected = timeInt;
        }
        else
        {
            throw new ArgumentException("Invalid collected integer", "data");
        }

        int attemptsInt;
        if (int.TryParse(d[3], out attemptsInt))
        {
            this.Attempts = attemptsInt;
        }
        else
        {
            throw new ArgumentException("Invalid attempts integer", "data");
        }

    }

    // Formats into serializable format.
    public override string ToString()
    {
        return String.Format("{0};{1};{2};{3}", this.Position, this.TimeCompleted, this.CoinsCollected, this.Attempts);
    }

}
