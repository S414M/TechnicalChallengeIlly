using System;
using System.Collections.Generic;

class Table
{
    // Stores the name of the table
    public string Name { get; }

    // Creates a Table object with a name
    public Table(string name)
    {
        Name = name;
    }
}

class Column
{
    // The table that this column belongs to
    public Table Table { get; }

    // The name of the column, e.g. "Name"
    public string Name { get; }

    // Creates a Column object
    public Column(Table table, string name)
    {
        Table = table;
        Name = name;
    }
}

class Condition
{
    // The column that we are checking
    public Column Column { get; }

    // The comparison operator
    public string Operator { get; }


    public object Value { get; }

    // Creates a condition
    public Condition(Column column, string operatorSymbol, object value)
    {
        Column = column;
        Operator = operatorSymbol;
        Value = value;
    }

    // Converts the condition into SQL
    public string Build()
    {
        string value;

        // Put quotation marks around text values
        if (Value is string)
        {
            value = $"'{Value}'";
        }
        else
        {
            value = Value.ToString();
        }

        return $"[{Column.Table.Name}].[{Column.Name}] {Operator} {value}";
    }
}

class Query
{
    // The table we are selecting data FROM
    private Table fromTable;

    // A list of columns that we want to SELECT
    private List<Column> columns = new List<Column>();

    // The WHERE condition
    private Condition whereCondition;

    // Sets the table that the query is selecting from
    public Query From(Table table)
    {
        fromTable = table;
        return this;
    }

    // Adds a column to the SELECT list
    public Query Select(Column column)
    {
        columns.Add(column);
        return this;
    }

    // Adds a WHERE condition to the query
    public Query Where(Condition condition)
    {
        whereCondition = condition;
        return this;
    }

    // Converts the Query object into a SQL string
    public string Build()
    {
        string sql = "SELECT ";

        // Add all selected columns
        foreach (Column column in columns)
        {
            sql += $"[{column.Table.Name}].[{column.Name}], ";
        }

        // Remove the final comma
        sql = sql.TrimEnd(',', ' ');

        // Add the FROM table
        sql += $"\nFROM [{fromTable.Name}]";

        // Add WHERE if a condition exists
        if (whereCondition != null)
        {
            sql += $"\nWHERE {whereCondition.Build()}";
        }

        return sql;
    }
}

class Program
{
    static void Main()
    {
        // Create a table
        Table events = new Table("Events");

        // Create columns
        Column EventId = new Column(events, "EventId");
        Column EventName = new Column(events, "EventName");

        // Create a WHERE condition
        Condition eventNameIsBirthday =
            new Condition(EventName, "=", "Birthday");

        // Build the query
        Query query = new Query()
            .From(events)
            .Select(EventId)
            .Select(EventName)
            .Where(eventNameIsBirthday);

        // Print the SQL
        Console.WriteLine(query.Build());
    }
}