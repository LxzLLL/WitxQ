// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

DBMetadata db = new DBMetadata
{
    DBName = "sqlite",
    DBVersion = "1.2.0"
};

Person p = new Person
{
    Name = "tony",
    Address = "city"
};


var p1 = p with { Name = "wang" };
Console.WriteLine(p);
Console.WriteLine(DateTime.UtcNow);
Console.WriteLine(GetName(p1));


string GetName(Person @person)
{
    /*if (person is null)
    {
        throw new ArgumentNullException(nameof(person));
    }*/
    return @person.Name; 
}

public class DBMetadata
{
    public string DBName { get; init; }
    public string DBVersion { get; init; }
}




public record class Person
{
    public string Name { get; init; }
    public string Address { get; init; }
}


