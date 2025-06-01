using System;
using System.IO;


public interface IMediator 
{
    void RealizujOperacje(IOperacjaFinansowa operacja); //every mediator requires action (must-have-thing)
}
public interface IOperacjaFinansowa //every operation needs to know its own mediator and have the function Realizuj() (must-have-thing)
{
    IMediator Mediator { get; set; }
    void Realizuj();
}
public interface IWyplacalne { } //allows to withdraw money 
public interface IWplacalne { } //allows to put money into the bank account 

public class Bank : IMediator
{
    public void RealizujOperacje(IOperacjaFinansowa operacja)
    {
        operacja.Realizuj(); //makes the operation work
        ZapiszDoPliku(operacja.ToString()); //saves operation to the txt file
    }

    public void ZapiszDoPliku(string operacja)
    {
        string sciezka = "operacje.txt"; //names specific file that we want to overwrite 
        File.AppendAllText(sciezka, operacja + "\n"); //adds our text to the file 
    }
}

public class Wplata : IOperacjaFinansowa, IWplacalne //here it gets clear and everything knows what to do about putting money into the bank account
{
    public IMediator Mediator { get; set; } 

    public void Realizuj() //action
    {
        Console.WriteLine("Wykonano operację wpłaty."); //gives the output to the user
    }

    public override string ToString() => "Operacja: Wplata"; //overwrites the file
}

public class Wyplata : IOperacjaFinansowa, IWyplacalne //here it gets clear and everything knows what to do about withdrawing money from the bank account
{
    public IMediator Mediator { get; set; }

    public void Realizuj() //action
    {
        Console.WriteLine("Wykonano operację wypłaty."); //gives the output to the user
    }

    public override string ToString() => "Operacja: Wyplata"; //overwrites the file
}

public interface IPodatekStrategia //every strategy needs the method to calculate the tax
{
    decimal ObliczPodatek(decimal kwota);
}

public class PodatekPolska : IPodatekStrategia
{
    public decimal ObliczPodatek(decimal kwota)
    {
        return kwota * 0.23m; //for Poland the tax is supposed to be 23%
    }
}

public class PodatekIslandia : IPodatekStrategia
{
    public decimal ObliczPodatek(decimal kwota)
    {
        return kwota * 0.31m; //for Iceland the tax is supposed to be 31% 
    }
}

public class PodatekNiemcy : IPodatekStrategia
{
    public decimal ObliczPodatek(decimal kwota)
    {
        return kwota * 0.19m; //for Germany the tax is supposed to be 19% 
    }
}

public class PodatekSzwecja : IPodatekStrategia
{
    public decimal ObliczPodatek(decimal kwota)
    {
        return kwota * 0.30m;
    }
}

public class KalkulatorPodatku //calculator that allows us to count the tax
{
    private IPodatekStrategia _strategia;

    public KalkulatorPodatku(IPodatekStrategia strategia)
    {
        _strategia = strategia;
    }

    public decimal Oblicz(decimal kwota)
    {
        return _strategia.ObliczPodatek(kwota);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank(); //our mediator

            Wplata wplata = new Wplata { Mediator = bank };
            Wyplata wyplata = new Wyplata { Mediator = bank };

            bank.RealizujOperacje(wplata); //bank as a mediator put money to the bank account
            bank.RealizujOperacje(wyplata); //bank as a mediator withdraws money from the bank account

            KalkulatorPodatku kalkPL = new KalkulatorPodatku(new PodatekPolska());
            KalkulatorPodatku kalkISL = new KalkulatorPodatku(new PodatekIslandia());
            KalkulatorPodatku kalkDE = new KalkulatorPodatku(new PodatekNiemcy());
            KalkulatorPodatku kalkSE = new KalkulatorPodatku(new PodatekSzwecja());

            Console.WriteLine("Podatek Polska od 2507 zł: " + kalkPL.Oblicz(2507)); //gives output to the user
            Console.WriteLine("Podatek Islandia od 2507 zł: " + kalkISL.Oblicz(2507)); //gives output to the user
            Console.WriteLine("Podatek Niemcy od 2507 zł: " + kalkDE.Oblicz(2507)); //gives output to the user 
            Console.WriteLine("Podatek Szwecja od 2507 zł: " + kalkSE.Oblicz(2507)); //gives output to the user 

            Console.ReadKey(); //waiting for the user to press the key
        }
    }
}